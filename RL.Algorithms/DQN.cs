using System;
using System.Collections.Generic;
using RL.Core;
using RL.Environments;
using RL.Environments.Spaces;
using RL.Generators;
using RL.MDArrays;
using RL.Random;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;
using static TorchSharp.torch.nn.utils;
using static TorchSharp.torch.optim;
using Generator = RL.Generators.Generator;

namespace RL.Algorithms;

public class DQN(double lr = 1e-4)
{
    private const int BatchSize = 128;
    private const int FullMemoryCapacity = 10000;
    private const double Tau = 0.005;
    private readonly List<Tensor> _actionList = new(BatchSize);
    private readonly List<Tensor> _nonFinalNextStateList = new(BatchSize);

    private readonly IRandomGenerator _random = new RandomGenerator();
    private readonly List<Tensor> _rewardList = new(BatchSize);
    private readonly List<Tensor> _stateList = new(BatchSize);

    public (Array1D<double> rewards, int steps, int terminatedSteps) Train<TG>(
        TG episodeGenerator,
        Module<Tensor, Tensor> model,
        Module<Tensor, Tensor> policyModel,
        IEnvironment<Box<Array1D<float>>, Discrete, Array1D<float>, int> environment
    ) where TG : IGenerator<int>
    {
        policyModel.load_state_dict(model.state_dict());
        using var optimizer = AdamW(policyModel.parameters(), lr: lr, amsgrad: true);
        var memory = new ReplayMemory(FullMemoryCapacity);

        var count = episodeGenerator.Count();
        var totalRewards = count.Zeroes<double>();
        var steps = 0;
        var terminatedSteps = 0;
        foreach (var episode in episodeGenerator.AsGeneratorEnumerable())
        {
            var episodeReward = 0.0;

            var state = environment.Reset();

            foreach (var _ in Generator.Sequence<int>())
            {
                var action = SelectAction(episode, policyModel, environment, state, _random);

                var t = environment.Step(action);
                episodeReward += t.Reward;

                var done = t.Terminated || t.Truncated;

                if (t.Terminated)
                    terminatedSteps++;

                memory.Push(
                    as_tensor(state).unsqueeze(0),
                    full(1, (long)action).unsqueeze(0),
                    !t.Terminated ? as_tensor(t.NextState).unsqueeze(0) : null,
                    full(1, t.Reward)
                );
                state = t.NextState;

                if (OptimizeModel(batchSize: BatchSize, memory, policyModel, model, optimizer))
                    steps++;

                var targetModelStateDict = model.state_dict();
                var policyModelStateDict = policyModel.state_dict();

                foreach (var pair in policyModelStateDict)
                    targetModelStateDict[pair.Key] = pair.Value * Tau + targetModelStateDict[pair.Key] * (1 - Tau);
                model.load_state_dict(targetModelStateDict);

                if (done)
                    break;
            }

            totalRewards[episode] = episodeReward;
        }

        return (totalRewards, steps, terminatedSteps);
    }

    private bool OptimizeModel(
        int batchSize,
        ReplayMemory memory,
        Module<Tensor, Tensor> policyModel,
        Module<Tensor, Tensor> targetModel,
        Optimizer optimizer
    )
    {
        const double gamma = 0.99;
        if (memory.Count < batchSize)
            return false;

        var transitions = memory.Sample(batchSize, _random);

        var nonFinalMask = empty(batchSize, ScalarType.Bool);

        foreach (var (transition, index) in transitions.Index())
        {
            _stateList.Add(transition.state);
            _actionList.Add(transition.action);
            _rewardList.Add(transition.reward);
            var terminated = (object?)transition.nextState == null;
            nonFinalMask[index] = !terminated;
            if (!terminated)
                _nonFinalNextStateList.Add(transition.nextState!);
        }

        var stateBatch = cat(_stateList);
        var actionBatch = cat(_actionList);
        var nonFinalNextStates = cat(_nonFinalNextStateList);
        var rewardBatch = cat(_rewardList);
        _stateList.Clear();
        _actionList.Clear();
        _nonFinalNextStateList.Clear();
        _rewardList.Clear();

        var predict = policyModel.call(stateBatch);
        var stateActionValues = predict.gather(1, actionBatch);

        var nextStateValues = zeros(batchSize);
        using (no_grad())
            nextStateValues[nonFinalMask] = targetModel.call(nonFinalNextStates).max(1).values;

        var expectedStateActionValues = nextStateValues * gamma + rewardBatch;

        var output = SmoothL1Loss().call(stateActionValues, expectedStateActionValues.unsqueeze(1));

        optimizer.zero_grad();
        output.backward();
        clip_grad_value_(policyModel.parameters(), 100);
        optimizer.step();
        return true;
    }

    private static int SelectAction<TOSpace, TO>(
        int step,
        Module<Tensor, Tensor> policyModel,
        IEnvironment<TOSpace, Discrete, TO, int> environment,
        Array1D<float> state,
        IRandomGenerator random
    ) where TOSpace : ISpace<TO>
    {
        const double epsilonStart = 0.9;
        const double epsilonEnd = 0.05;
        const double epsilonDecay = 100;

        var sample = random.Random(0.0, 1.0);
        var epsilonThreshold = epsilonEnd + (epsilonStart - epsilonEnd) * Math.Exp(-1.0 * step / epsilonDecay);

        if (sample <= epsilonThreshold)
            return environment.ActionSpace.Sample(random);

        using (no_grad())
        {
            var s = as_tensor(state);
            var predict = policyModel.call(s);
            return (int)predict.max(-1).indexes.ToInt64();
        }
    }
}
using System.Collections.Generic;
using RL.Core;
using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;
using RL.Random;
using TorchSharp;
using static RL.Generators.Generator;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;
using static TorchSharp.torch.optim;

namespace RL.Algorithms;

public class CrossEntropy(
    int trajectoryCount,
    int trajectoryLength,
    float q = 0.8f
)
{
    private readonly IRandomGenerator _random = new RandomGenerator();

    public (Array1D<float>, long) Train<TG>(
        TG episodeGenerator,
        Module<Tensor, Tensor> model,
        IEnvironment<Box<Array1D<float>>, Discrete, Array1D<float>, int> environment,
        Device device
    ) where TG : IGenerator<int>
    {
        model.train();

        using var optimizer = Adam(model.parameters(), lr: 0.01);

        var totalRewards = episodeGenerator.Count().Zeroes<float>();

        List<Tensor> states = [];
        List<Tensor> actions = [];

        var step = 0;

        foreach (var episode in episodeGenerator.AsGeneratorEnumerable())
        {
            var trajectories = Range<int>(trajectoryCount)
                .Select(
                    (model, environment, device, length: trajectoryLength, random: _random),
                    static (t, _) => Trajectory(t.model, t.environment, t.device, t.length, t.random)
                )
                .ArrayCache();

            var rewards = trajectories.Select(t1 => t1.totalReward).ToMDArray();
            totalRewards[episode] = rewards.Average();

            var quantile = as_tensor(rewards).quantile(q).ToDouble();
            foreach (var (s, a, reward) in trajectories)
            {
                if (reward <= quantile)
                    continue;

                states.Add(s);
                actions.Add(a);
            }

            if (states.Count > 0)
            {
                TrainStep(model, optimizer, CrossEntropyLoss(), cat(states), cat(actions));
                step++;
            }

            states.Clear();
            actions.Clear();
        }

        return (totalRewards, step);

        static void TrainStep(
            Module<Tensor, Tensor> model,
            Optimizer optimizer, Module<Tensor, Tensor, Tensor> loss,
            Tensor states,
            Tensor actions
        )
        {
            using var _ = NewDisposeScope();
            var prediction = model.forward(states);
            var output = loss.forward(prediction, actions);
            optimizer.zero_grad();
            output.backward();
            optimizer.step();
        }
    }

    private static (Tensor states, Tensor actions, float totalReward) Trajectory(
        Module<Tensor, Tensor> model,
        IEnvironment<Box<Array1D<float>>, Discrete, Array1D<float>, int> environment,
        Device device,
        int length,
        IRandomGenerator random
    )
    {
        var states = new List<Tensor>(length);
        var actions = new List<long>(length);
        var totalReward = 0.0f;
        var state = as_tensor(environment.Reset(), device: device);

        foreach (var _ in Range<int>(length))
        {
            states.Add(state);

            var action = ChoiceAction(model, state, random);
            actions.Add(action);

            var t = environment.Step(action);
            state = as_tensor(t.NextState, device: device);

            totalReward += t.Reward;

            if (t.Terminated || t.Truncated)
                break;
        }

        return (stack(states), as_tensor(actions, device: device), totalReward);

        static int ChoiceAction(
            Module<Tensor, Tensor> model,
            Tensor state,
            IRandomGenerator random
        )
        {
            using (NewDisposeScope())
            {
                var predict = model.forward(state);
                var probabilities = softmax(predict, -1).AsGenerator<float>();
                return probabilities.ChoiceIndex(random);
            }
        }
    }
}
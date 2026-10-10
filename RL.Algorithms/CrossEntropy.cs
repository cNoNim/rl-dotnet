using System.Collections.Generic;
using RL.Core;
using RL.Environments.Spaces;
using RL.MDArrays;
using RL.Random;
using TorchSharp;
using static RL.Generators.Generator;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;
using static TorchSharp.torch.optim;

namespace RL.Algorithms;

public class CrossEntropy<TO>(
    int trajectoryCount,
    int trajectoryLength,
    float q = 0.8f
)
{
    private readonly IRandomGenerator _random = new RandomGenerator();

    public (Array1D<float> rewards, int terminatedCount) Train<TOSpace>(
        Module<Tensor, Tensor> model,
        IPolicyEnvironment<TOSpace, Discrete, TO, int> environment,
        IGenerator<int> episodeGenerator
    )
        where TOSpace : ISpace<TO>
    {
        model.train();

        using var optimizer = Adam(model.parameters(), lr: 0.01);

        var totalRewards = episodeGenerator.Count().Zeroes<float>();

        List<Tensor> states = [];
        List<Tensor> actions = [];

        var terminatedCount = 0;

        foreach (var episode in episodeGenerator.AsGeneratorEnumerable())
        {
            var trajectories = Range<int>(trajectoryCount)
                .Select(
                    (model, environment, length: trajectoryLength, random: _random),
                    static (t, _) => Trajectory(t.model, t.environment, t.length, t.random)
                )
                .ArrayCache();

            var rewards = trajectories.Select(t1 => t1.totalReward).ToMDArray();
            totalRewards[episode] = rewards.Average();

            var quantile = as_tensor(rewards).quantile(q).ToDouble();
            foreach (var (s, a, reward, terminated) in trajectories)
            {
                if (reward <= quantile)
                    continue;

                if (terminated)
                    terminatedCount++;

                states.Add(s);
                actions.Add(a);
            }

            if (states.Count > 0)
                TrainStep(model, optimizer, CrossEntropyLoss(), cat(states), cat(actions));

            states.Clear();
            actions.Clear();
        }

        return (totalRewards, terminatedCount);

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

    private static (Tensor states, Tensor actions, float totalReward, bool terminated) Trajectory<TOSpace>(
        Module<Tensor, Tensor> model,
        IPolicyEnvironment<TOSpace, Discrete, TO, int> environment,
        int length,
        IRandomGenerator random
    )
        where TOSpace : ISpace<TO>
    {
        var states = new List<Tensor>(length);
        var actions = new List<long>(length);
        var totalReward = 0.0f;
        var observation = environment.Reset();

        var terminated = false;
        foreach (var _ in Range<int>(length))
        {
            var tensor = environment.AsTensor(observation);
            states.Add(tensor);

            var action = SelectAction(model, tensor, random);
            actions.Add(action);

            var t = environment.Step(action);
            observation = t.NextObservation ;

            totalReward += t.Reward;

            if (t.Terminated || t.Truncated)
            {
                terminated = t.Terminated;
                break;
            }
        }

        return (stack(states), as_tensor(actions), totalReward, terminated);

        static int SelectAction(
            Module<Tensor, Tensor> model,
            Tensor state,
            IRandomGenerator random
        )
        {
            model.eval();
            using (NewDisposeScope())
            {
                var predict = model.forward(state);
                var probabilities = softmax(predict, -1).AsGenerator<float>();
                return probabilities.ChoiceIndex(random);
            }
        }
    }
}

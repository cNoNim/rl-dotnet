using System;
using System.Collections.Generic;
using System.Linq;
using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;
using RL.Random;
using static RL.Generators.Generator;

namespace RL.Algorithms;

public readonly struct MonteCarlo(
    int episodeCount,
    int stepCount,
    double gamma = 0.99
) : IDiscreteAlgorithm
{
    public string Name => nameof(MonteCarlo);

    public Array2D<double> CreateQ(IEnvironment<Discrete, Discrete, int, int> environment) =>
        (environment.ObservationSpace.Size, environment.ActionSpace.Size).Zeroes<double>();

    public (Array1D<double> rewards, Array2D<double> qTable) Train(
        IEnvironment<Discrete, Discrete, int, int> environment,
        Array2D<double>? qTable = default
    )
    {
        var totalRewards = episodeCount.Zeroes<double>();
        var q = qTable ?? CreateQ(environment);
        var counter = (environment.ObservationSpace.Size, environment.ActionSpace.Size).Zeroes<int>();

        var steps = new List<Transition<int, int, float>>(stepCount);
        var returns = new double[stepCount + 1];

        foreach (var episode in Range<int>(episodeCount))
        {
            var epsilon = 1.0 / (episode + 1);

            var state = environment.Reset();
            foreach (var _ in Range<int>(stepCount))
            {
                var action = q[state].EpsilonGreedy(epsilon).ChoiceIndex(environment.Random);
                var t = environment.Step(action);
                steps.Add(t);
                state = t.NextState;

                if (t.Terminated || t.Truncated)
                    break;
            }

            totalRewards[episode] = steps.Select(tuple => tuple.Reward).Sum();

            var count = steps.Count;

            foreach (var i in Range(count - 1, 0, -1))
                returns[i] = steps[i].Reward + gamma * returns[i + 1];

            foreach (var i in Range<int>(count))
            {
                var (s, a, _, _, _, _) = steps[i];
                q[s][a] += (returns[i] - q[s][a]) / (1 + counter[s][a]);
                counter[s][a] += 1;
            }

            Array.Clear(returns);
            steps.Clear();
        }

        return (totalRewards, q);
    }
}
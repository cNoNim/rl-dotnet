using RL.Core;
using RL.Generators;

namespace RL.Random;

public static class Random
{
    public static int ChoiceIndex<TG>(this TG probabilities, IRandomGenerator generator)
        where TG : IGenerator<double>
    {
        var sum = probabilities.Sum<TG, double>();
        var r = generator.Random(0.0, sum);
        var cumSum = 0.0;
        var sample = -1;
        var index = 0;

        foreach (var value in probabilities.AsGeneratorEnumerable<TG, double>())
        {
            cumSum += value;

            if (cumSum > r)
            {
                sample = index;
                break;
            }

            index++;
        }

        return sample;
    }
}
using RL.Core;

namespace RL.Random;

public static class Random
{
    public static int ChoiceIndex<TPList>(this TPList probabilities, IRandomGenerator generator)
        where TPList : IReadOnlyList<double>
    {
        var sum = probabilities.Sum<TPList, double>();
        var r = generator.Random(0.0, sum);
        var cumSum = 0.0;
        var sample = -1;
        var index = 0;

        foreach (var value in probabilities.AsStructEnumerable<TPList, double>())
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

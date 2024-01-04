using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;
using RL.Generators;

namespace RL.Random;

public static class Random
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ChoiceIndex<TG, T, TTo>(this SelectGenerator<TG, T, TTo> probabilities, IRandomGenerator random)
        where TG : IGenerator<T>
        where TTo : INumber<TTo> =>
        ChoiceIndex<SelectGenerator<TG, T, TTo>, TTo>(probabilities, random);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ChoiceIndex<T>(this IGenerator<T> probabilities, IRandomGenerator random)
        where T : INumber<T> =>
        ChoiceIndex<IGenerator<T>, T>(probabilities, random);

    public static int ChoiceIndex<TG, T>(this TG probabilities, IRandomGenerator random)
        where TG : IGenerator<T>
        where T : INumber<T>
    {
        var sum = probabilities.Sum<TG, T>();
        var r = random.Random(T.Zero, sum);
        var cumSum = T.Zero;
        var sample = -1;
        var index = 0;

        foreach (var value in probabilities.AsGeneratorEnumerable<TG, T>())
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<SequenceGenerator<int>, int, T, (IRandomGenerator random, T min, T max)>
        Sequence<T>(this IRandomGenerator random, T min, T max)
        where T : INumber<T> =>
        Generator.Sequence<int>().Select<SequenceGenerator<int>, int, T, (IRandomGenerator random, T min, T max)>(
            (random, min, max),
            static (t, _) => t.random.Random(t.min, t.max)
        );
}
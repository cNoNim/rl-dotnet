using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<T>(this SequenceGenerator<T> generator)
        where T : INumberBase<T> =>
        Count<SequenceGenerator<T>, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<T>(this IGenerator<T> generator) =>
        Count<IGenerator<T>, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<TG>(this TG generator)
        where TG : IGenerator<int> =>
        generator.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<TG, T>(this TG generator)
        where TG : IGenerator<T> =>
        generator.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<TG, T>(this TG generator, Func<T, bool> predicate)
        where TG : IGenerator<T>
    {
        var count = 0;
        foreach (var value in generator.AsGeneratorEnumerable<TG, T>())
        {
            if (predicate(value))
                count++;
        }

        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count<TG, T, TContext>(
        this TG generator,
        TContext context,
        Func<TContext, T, bool> predicate
    )
        where TG : IGenerator<T>
    {
        var count = 0;
        foreach (var value in generator.AsGeneratorEnumerable<TG, T>())
        {
            if (predicate(context, value))
                count++;
        }

        return count;
    }
}
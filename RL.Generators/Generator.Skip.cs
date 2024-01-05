using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SkipGenerator<SequenceGenerator<T>, T> Skip<T>(
        this SequenceGenerator<T> generator,
        int count
    ) where T : INumberBase<T> =>
        Skip<SequenceGenerator<T>, T>(generator, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SkipGenerator<IGenerator<T>, T> Skip<T>(this IGenerator<T> generator, int count) =>
        Skip<IGenerator<T>, T>(generator, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SkipGenerator<TG, T> Skip<TG, T>(this TG generator, int count)
        where TG : IGenerator<T> =>
        new(generator, count);
}
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
    public static int Count<TG, T>(this TG generator)
        where TG : IGenerator<T> =>
        generator.Count;
}
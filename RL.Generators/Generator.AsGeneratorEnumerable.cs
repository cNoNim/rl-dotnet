using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GeneratorEnumerable<IGenerator<T>, T> AsGeneratorEnumerable<T>(
        this IGenerator<T> generator
    ) => AsGeneratorEnumerable<IGenerator<T>, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GeneratorEnumerable<TG, T> AsGeneratorEnumerable<TG, T>(
        this TG generator
    ) where TG : IGenerator<T> =>
        new(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> AsEnumerable<T>(
        this IGenerator<T> generator
    ) => AsEnumerable<IGenerator<T>, T>(generator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> AsEnumerable<TG, T>(
        this TG generator
    ) where TG : IGenerator<T> =>
        new DisposableGeneratorEnumerable<TG, T>(generator);
}
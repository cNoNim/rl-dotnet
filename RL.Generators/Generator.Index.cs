using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZipGenerator<IGenerator<T>, SequenceGenerator<int>, T, int> Index<T>(
        this IGenerator<T> generator
    ) => Index<IGenerator<T>, T>(generator);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZipGenerator<TG, SequenceGenerator<int>, T, int> Index<TG, T>(
        this TG generator
    ) where TG : IGenerator<T> =>
        Zip<TG, SequenceGenerator<int>, T, int>(generator, Sequence<int>());
}
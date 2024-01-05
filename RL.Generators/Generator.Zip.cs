using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZipGenerator<IGenerator<T1>, IGenerator<T2>, T1, T2>
        Zip<T1, T2>(this IGenerator<T1> generator1, IGenerator<T2> generator2) =>
        Zip<IGenerator<T1>, IGenerator<T2>, T1, T2>(generator1, generator2);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZipGenerator<TG1, TG2, T1, T2>
        Zip<TG1, TG2, T1, T2>(this TG1 generator1, TG2 generator2)
        where TG1 : IGenerator<T1>
        where TG2 : IGenerator<T2> => new(generator1, generator2);
}
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZipGenerator<IGenerator<T1>, IGenerator<T2>, T1, T2>
        Zip<T1, T2>(this IGenerator<T1> generator, IGenerator<T2> other) =>
        Zip<IGenerator<T1>, IGenerator<T2>, T1, T2>(generator, other);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZipGenerator<TList1, TList2, T1, T2>
        Zip<TList1, TList2, T1, T2>(this TList1 list, TList2 other)
        where TList1 : IGenerator<T1>
        where TList2 : IGenerator<T2> => new(list, other);
}
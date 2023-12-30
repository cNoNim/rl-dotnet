using System.Numerics;
using System.Runtime.CompilerServices;

namespace RL.Core;

public static partial class List
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RangeList<T> Range<T>(T count) where T : INumberBase<T> =>
        Range(T.Zero, count - T.One);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RangeList<T> Range<T>(T from, T to) where T : INumberBase<T> =>
        Range(from, to, T.One);

    public static RangeList<T> Range<T>(T from, T to, T step) where T : INumberBase<T> =>
        new(from, to, step);
}

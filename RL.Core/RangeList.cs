using System.Numerics;
using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly struct RangeList<T>(T from, T to, T step) :
    IStructList<RangeList<T>, T>
    where T : INumberBase<T>
{
    public int Count => int.CreateChecked((to - from) / step) + 1;
    public T this[int index] => from + step * T.CreateChecked(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<RangeList<T>, T> GetEnumerator() =>
        new(this);
}

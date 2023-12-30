using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly ref struct StructEnumerable<TList, T>(
    TList list,
    int index,
    int count
) where TList : IReadOnlyList<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<TList, T> GetEnumerator() => new(list, index, count);
}

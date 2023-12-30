using System.Runtime.CompilerServices;

namespace RL.Core;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly ref struct ReadOnlyListStructEnumerable<TList, T>(
    TList list,
    int index,
    int count
) where TList : IReadOnlyList<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyListStructEnumerator<TList, T> GetEnumerator() => new(list, index, count);
}

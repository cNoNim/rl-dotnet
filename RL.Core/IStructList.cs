using System.Collections;
using System.Runtime.CompilerServices;

namespace RL.Core;

public interface IStructList<TList, T> : IReadOnlyList<T>
    where TList : IStructList<TList, T>, IReadOnlyList<T>
{
    public new StructEnumerator<TList, T> GetEnumerator();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator().AsEnumerator();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator().AsEnumerator();
}

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RL.Core;

public struct ReadOnlyListStructEnumerator<TList, T> : IStructEnumerator<T>
    where TList : IReadOnlyList<T>
{
    private readonly TList _list;
    private readonly int _count;
    private int _next;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyListStructEnumerator(TList list) :
        this(list, 0, list.Count)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyListStructEnumerator(TList list, int index) :
        this(list, index, list.Count - index)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyListStructEnumerator(TList list, int index, int count)
    {
        Check(index, count, list.Count);

        _list = list;
        _count = index + count;
        return;

        [Conditional("DEBUG")]
        static void Check(int index, int count, int listCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index + count, listCount);
        }
    }

    public readonly T Current => _list[_next - 1];

    public bool MoveNext()
    {
        if (_next >= _count)
        {
            _next = _count + 1;
            return false;
        }

        ++_next;
        return true;
    }

    public readonly DisposableStructEnumerator<ReadOnlyListStructEnumerator<TList, T>, T> AsEnumerator() =>
        new(this);
}

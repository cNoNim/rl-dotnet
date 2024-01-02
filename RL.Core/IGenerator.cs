using System.Collections;
using System.Runtime.CompilerServices;

namespace RL.Core;

public interface IGenerator<out T> : IReadOnlyList<T>
{
    public bool IsFinite => false;

    int IReadOnlyCollection<T>.Count => throw new OverflowException();

    public bool TryGetNext(int current, out int next)
    {
        next = current + 1;
        return true;
    }
}

public interface IGenerator<TSelf, T> : IGenerator<T>
    where TSelf : IGenerator<TSelf, T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator().AsEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator().AsEnumerator();

    public new GeneratorEnumerator<TSelf, T> GetEnumerator();
}

public interface IFiniteGenerator<TSelf, T> : IGenerator<TSelf, T>
    where TSelf : IFiniteGenerator<TSelf, T>
{
    bool IGenerator<T>.IsFinite => true;

    bool IGenerator<T>.TryGetNext(int current, out int next)
    {
        if (current >= Count)
        {
            next = Count + 1;
            return false;
        }

        next = current + 1;
        return true;
    }
}

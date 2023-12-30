using System.Collections;

namespace RL.Core;

public struct DisposableStructEnumerator<TEnumerator, T>(
    TEnumerator enumerator
) : IStructEnumerator<T>, IEnumerator<T>
    where TEnumerator : IStructEnumerator<T>
{
    private readonly TEnumerator _default = enumerator;
    private TEnumerator _enumerator = enumerator;
    public T Current => _enumerator.Current;
    public bool MoveNext() => _enumerator.MoveNext();
    public void Reset() => _enumerator = _default;
    object IEnumerator.Current => Current ?? throw new InvalidOperationException();
    void IDisposable.Dispose()
    {
    }
}

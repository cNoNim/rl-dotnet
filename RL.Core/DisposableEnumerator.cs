using System;
using System.Collections;
using System.Collections.Generic;

namespace RL.Core;

public struct DisposableEnumerator<T>(
    IGeneratorEnumerator<T> enumerator
) : IGeneratorEnumerator<T>, IEnumerator<T>
{
    private readonly IGeneratorEnumerator<T> _default = enumerator;
    private IGeneratorEnumerator<T> _enumerator = enumerator;
    public T Current => _enumerator.Current;
    public bool MoveNext() => _enumerator.MoveNext();
    void IEnumerator.Reset() => _enumerator = _default;
    object IEnumerator.Current => Current ?? throw new InvalidOperationException();

    void IDisposable.Dispose()
    {
    }
}
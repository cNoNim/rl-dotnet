using System;
using System.ComponentModel;
using System.Numerics;
using RL.Core;

namespace RL.Generators;

public readonly struct SequenceGenerator<T> : IGenerator<SequenceGenerator<T>, T>
    where T : INumberBase<T>
{
    public T this[int index] => T.CreateChecked(index);
    public GeneratorEnumerator<SequenceGenerator<T>, T> GetEnumerator() => new(this);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsFinite => false;

    [Obsolete("Not supported", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Count => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetNext(int current, out int next)
    {
        next = current + 1;
        return true;
    }
}
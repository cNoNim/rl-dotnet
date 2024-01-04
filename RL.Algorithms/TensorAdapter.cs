using System.ComponentModel;
using RL.Core;
using TorchSharp.Utils;

namespace RL.Algorithms;

public readonly struct TensorAdapter<T>(TensorAccessor<T> accessor) :
    IGenerator<TensorAdapter<T>, T>
    where T : unmanaged
{
    public T this[int index] => accessor[index];

    public int Count => (int)accessor.Count;

    public GeneratorEnumerator<TensorAdapter<T>, T> GetEnumerator() => new(this);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsFinite => true;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetNext(int current, out int next)
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
using System.Numerics;
using RL.Core;

namespace RL.Generators;

public struct SequenceGenerator<T> : IGenerator<SequenceGenerator<T>, T>
    where T : INumberBase<T>
{
    public T this[int index] => T.CreateChecked(index);

    public GeneratorEnumerator<SequenceGenerator<T>, T> GetEnumerator() => new(this);
}
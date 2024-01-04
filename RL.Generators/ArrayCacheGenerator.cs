using System.ComponentModel;
using RL.Core;

namespace RL.Generators;

public readonly struct ArrayCacheGenerator<TG, T>(TG generator) :
    IGenerator<ArrayCacheGenerator<TG, T>, T>
    where TG : IGenerator<T>
{
    private readonly (T value, bool cached)[] _cache = new (T value, bool cached)[generator.Count];

    public T this[int index]
    {
        get
        {
            var (value, cached) = _cache[index];
            if (cached)
                return value;

            value = generator[int.CreateChecked(index)];
            _cache[index] = (value, true);
            return value;
        }
    }

    public int Count => generator.Count;

    public GeneratorEnumerator<ArrayCacheGenerator<TG, T>, T> GetEnumerator() => new(this);

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
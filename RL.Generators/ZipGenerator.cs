using RL.Core;
using static System.Math;

namespace RL.Generators;

public readonly struct ZipGenerator<TG1, TG2, T1, T2>(TG1 generator1, TG2 generator2) :
    IGenerator<ZipGenerator<TG1, TG2, T1, T2>, (T1, T2)>
    where TG1 : IGenerator<T1>
    where TG2 : IGenerator<T2>
{
    private readonly bool _isFinite = generator1.IsFinite || generator2.IsFinite;

    public (T1, T2) this[int index] => (generator1[index], generator2[index]);

    public int Count { get; } = (generator1.IsFinite, generator2.IsFinite) switch
    {
        (true, true) => Min(generator1.Count, generator2.Count),
        (true, false) => generator1.Count,
        (false, true) => generator2.Count,
        _ => throw new OverflowException()
    };

    public GeneratorEnumerator<ZipGenerator<TG1, TG2, T1, T2>, (T1, T2)> GetEnumerator() => new(this);

    bool IGenerator<(T1, T2)>.IsFinite => _isFinite;

    bool IGenerator<(T1, T2)>.TryGetNext(int current, out int next)
    {
        var isFinite = _isFinite;
        if (isFinite && current >= Count)
        {
            next = Count + 1;
            return false;
        }

        next = current + 1;
        return true;
    }
}

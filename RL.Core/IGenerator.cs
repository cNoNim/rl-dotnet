namespace RL.Core;

public interface IGenerator<out T>
{
    public bool IsFinite => false;

    T this[int index] { get; }
    int Count => throw new OverflowException();

    public bool TryGetNext(int current, out int next)
    {
        next = current + 1;
        return true;
    }
}

public static class GeneratorExtensions
{
    public static GeneratorEnumerator<TG, T> GetEnumerator<TG, T>(this TG generator)
        where TG : IGenerator<T> =>
        new(generator);
}

public interface IGenerator<TSelf, T> : IGenerator<T>
    where TSelf : IGenerator<TSelf, T>
{
    public GeneratorEnumerator<TSelf, T> GetEnumerator();
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

namespace RL.Core;

public interface IGenerator<out T>
{
    public bool IsFinite { get; }

    T this[int index] { get; }
    int Count { get; }

    public bool TryGetNext(int current, out int next);
}

public interface IGenerator<TSelf, T> : IGenerator<T>
    where TSelf : IGenerator<TSelf, T>
{
    public GeneratorEnumerator<TSelf, T> GetEnumerator();
}
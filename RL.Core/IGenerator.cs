namespace RL.Core;

public interface IGenerator
{
    public bool IsFinite { get; }
    int Count { get; }
    public object? this[int index] { get; }
    public bool TryGetNext(int current, out int next);
}

public interface IGenerator<out T> : IGenerator
{
    new T this[int index] { get; }

    object? IGenerator.this[int index] => this[index];
}

public interface IGenerator<TSelf, T> : IGenerator<T>
    where TSelf : IGenerator<TSelf, T>
{
    public GeneratorEnumerator<TSelf, T> GetEnumerator();
}
namespace RL.Core;

public interface IGeneratorEnumerator<out T>
{
    public T Current { get; }
    public bool MoveNext();
}
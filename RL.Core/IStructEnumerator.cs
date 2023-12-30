namespace RL.Core;

public interface IStructEnumerator<out T>
{
    public T Current { get;  }
    public bool MoveNext();
}

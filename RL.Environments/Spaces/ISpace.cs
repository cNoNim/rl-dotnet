namespace RL.Environments.Spaces;

public interface ISpace<in T>
{
    public bool Contains(T value);
}
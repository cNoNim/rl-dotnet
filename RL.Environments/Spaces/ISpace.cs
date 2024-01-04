using RL.Random;

namespace RL.Environments.Spaces;

public interface ISpace<T>
{
    public bool Contains(T value);
    public T Sample(IRandomGenerator random);
}
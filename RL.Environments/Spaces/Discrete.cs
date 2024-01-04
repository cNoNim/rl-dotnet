namespace RL.Environments.Spaces;

public record struct Discrete(int Size) : ISpace<int>
{
    public bool Contains(int value) => value >= 0 && value < Size;
}
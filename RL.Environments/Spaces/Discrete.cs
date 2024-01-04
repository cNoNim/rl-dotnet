using RL.Random;

namespace RL.Environments.Spaces;

public record struct Discrete(int Size) : ISpace<int>
{
    public bool Contains(int value) => value >= 0 && value < Size;

    public int Sample(IRandomGenerator random) =>
        random.Random(0, Size);
}
using System.Numerics;

namespace RL.Environments.Spaces;

public record struct Box<T>(T Low, T High) : ISpace<T>
    where T : IComparisonOperators<T, T, bool>
{
    public bool Contains(T value) => Low <= value && value <= High;
}
namespace RL.Environments.Spaces;

public readonly record struct Discrete(int Size) : ISpace<int>;

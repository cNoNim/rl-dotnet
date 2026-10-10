namespace RL.Environments.Spaces;

public readonly record struct Box<T>(T Low, T High) : ISpace<T>;

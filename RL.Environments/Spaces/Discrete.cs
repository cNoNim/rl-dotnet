namespace RL.Environments.Spaces;

public record Discrete(int Size) : Space<int>(Size);

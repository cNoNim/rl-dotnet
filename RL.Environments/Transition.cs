namespace RL.Environments;

public readonly record struct Transition<TO, TA, TR>(
    TO State,
    TA Action,
    TO NextState,
    TR Reward,
    bool Terminated,
    bool Truncated
);
namespace RL.Environments;

public readonly record struct Transition<TO, TA>(
    TO Observation,
    TA Action,
    TO NextObservation,
    float Reward,
    bool Terminated,
    bool Truncated
);

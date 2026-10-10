using RL.Environments;
using RL.Random;
using static TorchSharp.torch;

namespace RL.Algorithms;

public class ReplayBuffer<TO, TA>(
    IPolicyEnvironment<TO, TA> environment,
    int capacity,
    int batchSize
)
{
    private long _position;
    private bool _full;

    private readonly Tensor _observations = empty(
        [capacity, ..environment.ObservationSize],
        environment.ObservationScalarType
    );

    private readonly Tensor _actions = empty(
        [capacity, ..environment.ActionSize],
        environment.ActionScalarType
    );

    private readonly Tensor _nextObservations = empty(
        [capacity, ..environment.ObservationSize],
        environment.ObservationScalarType
    );

    private readonly Tensor _rewards = empty(capacity, ScalarType.Float32);
    private readonly Tensor _terminations = empty(capacity, ScalarType.Float32);
    private readonly Tensor _truncations = empty(capacity, ScalarType.Float32);

    public void Push(Transition<TO, TA> transition)
    {
        _observations[_position] = environment.AsTensor(transition.Observation);
        _actions[_position] = environment.AsTensor(transition.Action);
        _nextObservations[_position] = environment.AsTensor(transition.NextObservation);
        _rewards[_position] = transition.Reward;
        _terminations[_position] = transition.Terminated ? 1.0f : 0.0f;
        _truncations[_position] = transition.Truncated ? 1.0f : 0.0f;

        _position++;
        if (_position < capacity)
            return;
        _position = 0;
        _full = true;
    }

    public (Tensor observations, Tensor actions, Tensor nextObservations, Tensor rewards, Tensor nonFinals)
        Sample(IRandomGenerator random)
    {
        using var batchIndex = empty(batchSize, ScalarType.Int64);
        var uBound = _full ? capacity : _position;
        for (var i = 0; i < batchSize; i++)
            batchIndex[i] = random.Random(0, uBound);

        var observations = _observations[batchIndex];
        var actions = _actions[batchIndex];
        var nextObservations = _nextObservations[batchIndex];
        var rewards = _rewards[batchIndex];
        var terminations = _terminations[batchIndex];
        var truncations = _truncations[batchIndex];
        return (
            observations,
            actions,
            nextObservations,
            rewards.reshape(-1, 1),
            (1 - terminations * (1 - truncations)).reshape(-1, 1)
        );
    }

    public void Clear()
    {
        _position = 0;
        _full = false;
    }
}

public static class EnvironmentExtensions
{
    public static ReplayBuffer<TO, TA>
        ReplayBuffer<TO, TA>(this IPolicyEnvironment<TO, TA> environment, int capacity, int batchSize) =>
        new(environment, capacity, batchSize);
}

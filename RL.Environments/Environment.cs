using RL.Environments.Spaces;
using RL.Random;

namespace RL.Environments;

public abstract class Environment<TObservation, TAction> : IEnvironment<TObservation, TAction>
{
    private IRandomGenerator? _generator;

    public IRandomGenerator Generator => _generator ??= new RandomGenerator();
    public abstract Space<TAction> ActionSpace { get; }
    public abstract Space<TObservation> ObservationSpace { get; }

    public abstract (TObservation observation, double reward, bool terminated, bool truncated)
        Step(TAction action);

    public TObservation Reset(uint? seed = null)
    {
        _generator = seed != null ? new RandomGenerator(seed) : null;
        return DoReset();
    }

    protected abstract TObservation DoReset();

}

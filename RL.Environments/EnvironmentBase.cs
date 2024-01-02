using RL.Environments.Spaces;
using RL.Random;

namespace RL.Environments;

public abstract class EnvironmentBase<TObservation, TAction>
{
    private IRandomGenerator? _random;
    public IRandomGenerator Random => _random ??= new RandomGenerator();
    public abstract Space<TAction> ActionSpace { get; }
    public abstract Space<TObservation> ObservationSpace { get; }

    public abstract (TObservation observation, double reward, bool terminated)
        Step(TAction action);

    public TObservation Reset(uint? seed = null)
    {
        _random = seed != null ? new RandomGenerator(seed) : null;
        return DoReset();
    }

    protected abstract TObservation DoReset();
}
using RL.Environments.Spaces;
using RL.Random;

namespace RL.Environments;

public interface IEnvironment<TO, TA>
{
    public string Name { get; }
    public IRandomGenerator Random { get; }
    public TO Observation { get; }

    public Transition<TO, TA> Step(TA action);
    public TO Reset(uint? seed = null, object? options = null);

    TO SampleObservation(IRandomGenerator random);
    TA SampleAction(IRandomGenerator random);
}

public interface IEnvironment<out TOSpace, out TASpace, TO, TA> : IEnvironment<TO, TA>
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    TOSpace ObservationSpace { get; }
    TASpace ActionSpace { get; }
}

using RL.Environments.Spaces;
using RL.Random;

namespace RL.Environments;

public interface IEnvironment<TO, TA>
{
    public string Name { get; }
    public IRandomGenerator Random { get; }
    public int Steps { get; }
    public TO State { get; }

    public Transition<TO, TA, float> Step(TA action);
    public TO Reset(uint? seed = null, object? options = null);

    public TA Heuristic(TO state);
}

public interface IEnvironment<out TOSpace, out TASpace, TO, TA> : IEnvironment<TO, TA>
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    TOSpace ObservationSpace { get; }
    TASpace ActionSpace { get; }
}
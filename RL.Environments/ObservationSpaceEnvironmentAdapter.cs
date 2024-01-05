using System;
using RL.Environments.Spaces;
using RL.Random;

namespace RL.Environments;

public class ObservationSpaceEnvironmentAdapterIEnvironment<TOFromSpace, TOSpace, TASpace, TOFrom, TO, TA, TContext>(
    IEnvironment<TOFromSpace, TASpace, TOFrom, TA> environment,
    Func<TContext, TOFromSpace, TOSpace> spaceAdapter,
    Func<TContext, TOFromSpace, TOFrom, TO> stateAdapter,
    TContext context
) : IEnvironment<TOSpace, TASpace, TO, TA>
    where TOFromSpace : ISpace<TOFrom>
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    public string Name => environment.Name;
    public IRandomGenerator Random => environment.Random;
    public TOSpace ObservationSpace { get; } = spaceAdapter(context, environment.ObservationSpace);
    public TASpace ActionSpace { get; } = environment.ActionSpace;

    public (TO observation, double reward, bool terminated) Step(TA action)
    {
        var (state, reward, terminated) = environment.Step(action);
        return (stateAdapter(context, environment.ObservationSpace, state), reward, terminated);
    }

    public TO Reset(uint? seed = null, object? options = null) => 
        stateAdapter(context, environment.ObservationSpace, environment.Reset(seed, options));
}
using System;
using RL.Environments.Spaces;

namespace RL.Environments;

public static class Environment
{
    public static ObservationSpaceEnvironmentAdapterIEnvironment<
        TOFromSpace,
        TOSpace,
        TASpace,
        TOFrom,
        TO,
        TA,
        TContext
    > AdaptObservationSpace<TOFromSpace, TOSpace, TASpace, TOFrom, TO, TA, TContext>(
        this IEnvironment<TOFromSpace, TASpace, TOFrom, TA> environment,
        TContext context,
        Func<TContext, TOFromSpace, TOSpace> spaceAdapter,
        Func<TContext, TOFromSpace, TOFrom, TO> stateAdapter
    )
        where TOFromSpace : ISpace<TOFrom>
        where TOSpace : ISpace<TO>
        where TASpace : ISpace<TA> => new(
        environment,
        spaceAdapter,
        stateAdapter,
        context
    );

    public static ObservationSpaceEnvironmentAdapterIEnvironment<
        TOFromSpace,
        TOSpace,
        TASpace,
        TOFrom,
        TO,
        TA,
        (Func<TOFromSpace, TOSpace> spaceAdapter, Func<TOFromSpace, TOFrom, TO> stateAdapter)
    > AdaptObservationSpace<TOFromSpace, TOSpace, TASpace, TOFrom, TO, TA>(
        this IEnvironment<TOFromSpace, TASpace, TOFrom, TA> environment,
        Func<TOFromSpace, TOSpace> spaceAdapter,
        Func<TOFromSpace, TOFrom, TO> stateAdapter
    )
        where TOFromSpace : ISpace<TOFrom>
        where TOSpace : ISpace<TO>
        where TASpace : ISpace<TA> => new(
        environment,
        static (tuple, s) => tuple.spaceAdapter(s),
        static (tuple, s, st) => tuple.stateAdapter(s, st),
        (spaceAdapter, stateAdapter)
    );
}
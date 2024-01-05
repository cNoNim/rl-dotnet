using System;
using System.Diagnostics;
using RL.Environments.Spaces;
using RL.Random;

namespace RL.Environments;

public abstract class EnvironmentBase<TOSpace, TASpace, TO, TA, TOptions> :
    IEnvironment<TOSpace, TASpace, TO, TA>
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    private IRandomGenerator? _random;

    protected EnvironmentBase(TO? startState = default) =>
        State = startState ?? Reset();

    protected virtual TOptions? DefaultOptions => default;
    protected TOptions? Options { get; private set; }
    protected TO State { get; private set; }

    public virtual string Name => GetType().Name;

    public IRandomGenerator Random => _random ??= new RandomGenerator();
    public abstract TOSpace ObservationSpace { get; }
    public abstract TASpace ActionSpace { get; }

    public (TO observation, double reward, bool terminated)
        Step(TA action)
    {
        Check(action, ActionSpace);

        (State, var reward, var terminated) = DoStep(action);
        return (State, reward, terminated);

        [Conditional("DEBUG")]
        static void Check(TA action, TASpace actionSpace)
        {
            if (!actionSpace.Contains(action))
                throw new ArgumentOutOfRangeException(nameof(action), "Invalid");
        }
    }

    public TO Reset(uint? seed = null, object? options = null)
    {
        _random = seed != null ? new RandomGenerator(seed) : null;
        Options = options is TOptions opts ? opts : DefaultOptions;
        return State = DoReset(Options);
    }

    protected abstract (TO observation, double reward, bool terminated) DoStep(TA action);

    protected abstract TO DoReset(TOptions? options);
}

public abstract class EnvironmentBase<TOSpace, TASpace, TO, TA> :
    EnvironmentBase<TOSpace, TASpace, TO, TA, object>
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    protected sealed override object? DefaultOptions => null;
    protected sealed override TO DoReset(object? options) => DoReset();
    protected abstract TO DoReset();
}
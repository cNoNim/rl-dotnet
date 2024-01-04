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

    protected EnvironmentBase(string? name = null, TO? startState = default)
    {
        Name = name ?? GetType().Name;
        State = startState ?? Reset();
    }

    protected virtual TOptions? DefaultOptions => default;
    protected TOptions? Options { get; private set; }
    public virtual int? MaxStepCount => null;

    public TO State { get; private set; }

    public string Name { get; }

    public int Steps { get; private set; }

    public IRandomGenerator Random => _random ??= new RandomGenerator();
    public abstract TOSpace ObservationSpace { get; }
    public abstract TASpace ActionSpace { get; }

    public Transition<TO, TA, float>
        Step(TA action)
    {
        Check(action, ActionSpace);

        var state = State;
        var step = Steps++;
        if (step >= MaxStepCount)
            return new Transition<TO, TA, float>(state, action, state, 0, false, true);

        var (nextState, reward, terminated) = DoStep(state, action);
        State = nextState;
        return new Transition<TO, TA, float>(state, action, nextState, reward, terminated, Steps >= MaxStepCount);

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
        Steps = 0;
        return State = DoReset(Options);
    }

    public virtual TA Heuristic(TO state) => throw new NotSupportedException();

    protected abstract (TO nextState, float reward, bool terminated) DoStep(TO state, TA action);

    protected abstract TO DoReset(TOptions? options);
}

public abstract class EnvironmentBase<TOSpace, TASpace, TO, TA>(string? name = null, TO? startState = default) :
    EnvironmentBase<TOSpace, TASpace, TO, TA, object>(name, startState)
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    protected sealed override object? DefaultOptions => null;
    protected sealed override TO DoReset(object? options) => DoReset();
    protected abstract TO DoReset();
}
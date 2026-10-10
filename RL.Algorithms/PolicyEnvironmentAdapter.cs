using System;
using RL.Environments;
using RL.Environments.Spaces;
using RL.Random;
using static TorchSharp.torch;

namespace RL.Algorithms;

public abstract class PolicyEnvironmentAdapterBase<TE, TOSpace, TASpace, TO, TA>(
    TE environment
) : IPolicyEnvironment<TOSpace, TASpace, TO, TA>
    where TE : IEnvironment<TOSpace, TASpace, TO, TA>
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    protected TE Environment => environment;

    public string Name => environment.Name;
    public IRandomGenerator Random => environment.Random;
    public TO Observation => environment.Observation;

    public Transition<TO, TA> Step(TA action)
    {
        var transition = environment.Step(action);
        if (transition.Truncated || transition.Terminated)
            Reset(Random.NextState());
        return transition;
    }

    public TO Reset(uint? seed = null, object? options = null) =>
        environment.Reset(seed, options);

    public TO SampleObservation(IRandomGenerator random) =>
        environment.SampleObservation(random);

    public TA SampleAction(IRandomGenerator random) =>
        environment.SampleAction(random);

    public TOSpace ObservationSpace => environment.ObservationSpace;
    public TASpace ActionSpace => environment.ActionSpace;

    public abstract Tensor AsTensor(TO observation);
    public abstract Tensor AsTensor(TA action);
    public abstract TA FromTensor(Tensor tensor);

    public abstract ScalarType ObservationScalarType { get; }
    public abstract ReadOnlySpan<long> ObservationSize { get; }
    public abstract ScalarType ActionScalarType { get; }
    public abstract ReadOnlySpan<long> ActionSize { get; }
    public abstract int InputSize { get; }
    public abstract int OutputSize { get; }
}

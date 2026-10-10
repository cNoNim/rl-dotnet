using System;
using RL.Environments;
using RL.Environments.Spaces;
using static TorchSharp.torch;

namespace RL.Algorithms;

public interface IPolicyEnvironment
{
    public ScalarType ObservationScalarType { get; }
    public ReadOnlySpan<long> ObservationSize { get; }
    public ScalarType ActionScalarType { get; }
    public ReadOnlySpan<long> ActionSize { get; }
}

public interface IPolicyEnvironment<TO, TA> :
    IEnvironment<TO, TA>,
    IPolicyEnvironment
{
    public Tensor AsTensor(TO observation);
    public Tensor AsTensor(TA action);
    public TA FromTensor(Tensor tensor);
}

public interface IPolicyEnvironment<out TOSpace, out TASpace, TO, TA> :
    IEnvironment<TOSpace, TASpace, TO, TA>,
    IPolicyEnvironment<TO, TA>
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>;

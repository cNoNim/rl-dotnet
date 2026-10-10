using System;
using RL.Algorithms;
using RL.Classic;
using RL.Environments.Spaces;
using RL.MDArrays;
using TorchSharp;

namespace RL.CartPole;

public class CartPoleAdapter(CartPoleEnvironment environment) :
    PolicyEnvironmentAdapterBase<CartPoleEnvironment, Box<Array1D<float>>, Discrete, Array1D<float>, int>(environment)
{
    private readonly long[] _observationSize = [environment.ObservationSpace.High.Count];
    private readonly long[] _actionSize = [1];

    public override torch.Tensor AsTensor(Array1D<float> observation) =>
        torch.as_tensor(observation);

    public override torch.Tensor AsTensor(int action) =>
        torch.as_tensor(action).unsqueeze(0);

    public override int FromTensor(torch.Tensor tensor) =>
        tensor.argmax(-1).ToInt32();

    public override torch.ScalarType ObservationScalarType => torch.ScalarType.Float32;
    public override ReadOnlySpan<long> ObservationSize => _observationSize;
    public override torch.ScalarType ActionScalarType => torch.ScalarType.Int32;
    public override ReadOnlySpan<long> ActionSize => _actionSize;

    public override int InputSize => Environment.ObservationSpace.High.Count;

    public override int OutputSize => Environment.ActionSpace.Size;
}

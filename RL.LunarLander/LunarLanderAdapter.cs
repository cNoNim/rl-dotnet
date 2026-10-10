using System;
using RL.Algorithms;
using RL.Box2D;
using RL.Environments.Spaces;
using TorchSharp;
using static TorchSharp.torch;

namespace RL.LunarLander;

public class LunarLanderAdapter(LunarLanderEnvironment environment) :
    PolicyEnvironmentAdapterBase<LunarLanderEnvironment, Box<LunarLanderObservation>, Discrete, LunarLanderObservation, int>(environment)
{
    private readonly long[] _observationSize = [8];
    private readonly long[] _actionSize = [1];

    public override Tensor AsTensor(LunarLanderObservation observation)
    {
        var tensor = empty(8, float32);
        tensor[0] = observation.Delta.X;
        tensor[1] = observation.Delta.Y;
        tensor[2] = observation.Velocity.X;
        tensor[3] = observation.Velocity.Y;
        tensor[4] = observation.Angle;
        tensor[5] = observation.AngularVelocity;
        tensor[6] = observation.LeftContact ? 1.0f : 0.0f;
        tensor[7] = observation.RightContact ? 1.0f : 0.0f;
        return tensor;
    }


    public override Tensor AsTensor(int action) =>
        as_tensor(action).unsqueeze(0);

    public override int FromTensor(Tensor tensor) =>
        tensor.argmax(-1).ToInt32();

    public override ScalarType ObservationScalarType => ScalarType.Float32;
    public override ReadOnlySpan<long> ObservationSize => _observationSize;
    public override ScalarType ActionScalarType => ScalarType.Int32;
    public override ReadOnlySpan<long> ActionSize => _actionSize;

    public override int InputSize => 8;

    public override int OutputSize => Environment.ActionSpace.Size;
}

using System;
using RL.Environments;
using RL.Environments.Spaces;
using RL.Tensors;
using static System.MathF;

namespace RL.Classic;

public class CartPoleEnvironment : EnvironmentBase<
    Box<Tensor1D<float>>,
    Discrete,
    Tensor1D<float>,
    int,
    (float low, float high, CartPoleEnvironment.Integrator integrator)>
{
    public enum Integrator
    {
        Euler,
        SemiImplicitEuler
    }

    private const int Left = 0;
    private const int Right = 1;

    private const float Gravity = 9.8f;
    private const float MassCart = 1.0f;
    private const float MassPole = 0.1f;
    private const float TotalMass = MassCart + MassPole;
    private const float Length = 0.5f;
    private const float PoleMassLength = MassPole * Length;
    private const float ForceMag = 10.0f;
    private const float Tau = 0.02f;

    private const float XThreshold = 2.4f;
    private const float ThetaThreshold = 12 * 2 * PI / 360;

    private int? _stepsBeyondTerminated;

    public CartPoleEnvironment()
    {
        Tensor1D<float> high = [XThreshold * 2, float.MaxValue, ThetaThreshold * 2, float.MaxValue];
        ObservationSpace = new Box<Tensor1D<float>>(-high, high);
    }

    public override Box<Tensor1D<float>> ObservationSpace { get; }

    public override Discrete ActionSpace { get; } = new(2);

    protected override (float low, float high, Integrator integrator) DefaultOptions =>
        (-0.05f, 0.05f, Integrator.Euler);

    protected override (Tensor1D<float> observation, double reward, bool terminated) DoStep(int action)
    {
        var (x, xV, theta, thetaV) = State;

        var force = action == Right ? ForceMag : -ForceMag;
        var cosTheta = Cos(theta);
        var sinTheta = Sin(theta);

        var temp =
            (force + PoleMassLength * Pow(thetaV, 2) * sinTheta) /
            TotalMass;
        var thetaAcc =
            (Gravity * sinTheta - cosTheta * temp) /
            (Length * (4.0f / 3.0f - MassPole * Pow(cosTheta, 2) / TotalMass));
        var xAcc = temp - PoleMassLength * thetaAcc * cosTheta / TotalMass;

        switch (Options.integrator)
        {
            case Integrator.Euler:
                x += Tau * xV;
                xV += Tau * xAcc;
                theta += Tau * thetaV;
                thetaV += Tau * thetaAcc;
                break;
            case Integrator.SemiImplicitEuler:
                xV += Tau * xAcc;
                x += Tau * xV;
                thetaV += Tau * thetaAcc;
                theta += Tau * thetaV;
                break;
            default:
                throw new NotSupportedException();
        }

        State[0] = x;
        State[1] = xV;
        State[2] = theta;
        State[3] = thetaV;

        var terminated =
            x is < -XThreshold or > XThreshold ||
            theta is < -ThetaThreshold or > ThetaThreshold;

        if (!terminated)
            return (State, 1.0, terminated);

        if (!_stepsBeyondTerminated.HasValue)
        {
            _stepsBeyondTerminated = 0;
            return (State, 1.0, terminated);
        }

        _stepsBeyondTerminated++;
        return (State, reward: 0.0, terminated);
    }

    protected override Tensor1D<float> DoReset((float low, float high, Integrator integrator) options) =>
    [
        Random.Random(options.low, options.high),
        Random.Random(options.low, options.high),
        Random.Random(options.low, options.high),
        Random.Random(options.low, options.high)
    ];
}
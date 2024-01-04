using System;
using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;
using static System.MathF;

namespace RL.Classic;

public class CartPoleEnvironment : EnvironmentBase<
    Box<Array1D<float>>,
    Discrete,
    Array1D<float>,
    int,
    (float low, float high, CartPoleEnvironment.Integrator integrator)>
{
    public enum Integrator
    {
        Euler,
        SemiImplicitEuler
    }

    // private const int Left = 0;
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

    public CartPoleEnvironment() : base("CartPole")
    {
        Array1D<float> high = [XThreshold * 2, float.MaxValue, ThetaThreshold * 2, float.MaxValue];
        ObservationSpace = new Box<Array1D<float>>(-high, high);
    }

    public override Box<Array1D<float>> ObservationSpace { get; }

    public override Discrete ActionSpace { get; } = new(2);

    public override int? MaxStepCount => 500;

    protected override (float low, float high, Integrator integrator) DefaultOptions =>
        (-0.05f, 0.05f, Integrator.Euler);

    protected override (Array1D<float> nextState, float reward, bool terminated)
        DoStep(Array1D<float> state, int action)
    {
        var (x, xV, theta, thetaV) = state;

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

        Array1D<float> nextState = [x, xV, theta, thetaV];

        var terminated =
            x is < -XThreshold or > XThreshold ||
            theta is < -ThetaThreshold or > ThetaThreshold;

        if (!terminated)
            return (nextState, 1.0f, terminated);

        if (!_stepsBeyondTerminated.HasValue)
        {
            _stepsBeyondTerminated = 0;
            return (nextState, 1.0f, terminated);
        }

        _stepsBeyondTerminated++;
        return (nextState, reward: 0.0f, terminated);
    }

    protected override Array1D<float> DoReset((float low, float high, Integrator integrator) options)
    {
        _stepsBeyondTerminated = null;
        return
        [
            Random.Random(options.low, options.high),
            Random.Random(options.low, options.high),
            Random.Random(options.low, options.high),
            Random.Random(options.low, options.high)
        ];
    }
}
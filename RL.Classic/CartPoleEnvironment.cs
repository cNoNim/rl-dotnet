using RL.Environments;
using RL.Environments.Spaces;
using RL.Tensors;

namespace RL.Classic;

public class CartPoleEnvironment : EnvironmentBase<
    Box<Tensor1D<double>>,
    Discrete,
    Tensor1D<double>,
    int,
    (double low, double high, CartPoleEnvironment.Integrator integrator)>
{
    public enum Integrator
    {
        Euler,
        SemiImplicitEuler
    }

    private const int Left = 0;
    private const int Right = 1;

    private const double Gravity = 9.8;
    private const double MassCart = 1.0;
    private const double MassPole = 0.1;
    private const double TotalMass = MassCart + MassPole;
    private const double Length = 0.5;
    private const double PoleMassLength = MassPole * Length;
    private const double ForceMag = 10.0;
    private const double Tau = 0.02;

    private const double XThreshold = 2.4;
    private const double ThetaThreshold = 12 * 2 * Math.PI / 360;

    private int? _stepsBeyondTerminated;

    public CartPoleEnvironment()
    {
        Tensor1D<double> high = [XThreshold * 2, double.MaxValue, ThetaThreshold * 2, double.MaxValue];
        ObservationSpace = new Box<Tensor1D<double>>(-high, high);
    }

    public override Box<Tensor1D<double>> ObservationSpace { get; }

    public override Discrete ActionSpace { get; } = new(2);

    protected override (double low, double high, Integrator integrator) DefaultOptions =>
        (-0.05, 0.05, Integrator.Euler);

    protected override (Tensor1D<double> observation, double reward, bool terminated) DoStep(int action)
    {
        var (x, xV, theta, thetaV) = State;

        var force = action == Right ? ForceMag : -ForceMag;
        var cosTheta = Math.Cos(theta);
        var sinTheta = Math.Sin(theta);

        var temp =
            (force + PoleMassLength * Math.Pow(thetaV, 2) * sinTheta) /
            TotalMass;
        var thetaAcc =
            (Gravity * sinTheta - cosTheta * temp) /
            (Length * (4.0 / 3.0 - MassPole * Math.Pow(cosTheta, 2) / TotalMass));
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

        Tensor1D<double> state = [x, xV, theta, thetaV];

        var terminated =
            x is < -XThreshold or > XThreshold ||
            theta is < -ThetaThreshold or > ThetaThreshold;

        if (!terminated)
            return (state, 1.0, terminated);

        if (!_stepsBeyondTerminated.HasValue)
        {
            _stepsBeyondTerminated = 0;
            return (state, 1.0, terminated);
        }

        _stepsBeyondTerminated++;
        return (state, reward: 0.0, terminated);
    }

    protected override Tensor1D<double> DoReset((double low, double high, Integrator integrator) options) =>
    [
        Random.Random(options.low, options.high),
        Random.Random(options.low, options.high),
        Random.Random(options.low, options.high),
        Random.Random(options.low, options.high)
    ];
}
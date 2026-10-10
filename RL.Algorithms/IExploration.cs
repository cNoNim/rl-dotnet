using System;
using RL.Algorithms.Callbacks;

namespace RL.Algorithms;

public interface IExploration
{
    public double Epsilon { get; }
}

public class ConstantExploration(double epsilon) : IExploration
{
    public double Epsilon => epsilon;
}

public class HomoGraphicExploration(IProgress progress) :
    IExploration
{
    public double Epsilon => 1.0 / (progress.Step + 1.0);
}

public class LinearExploration(IProgress progress, int target, double start, double end, double fraction) :
    IExploration,
    ICallbackNode
{
    public double Epsilon
    {
        get
        {
            var p = progress.Step / (double)target;
            return p > fraction ? end : start + p * (end - start) / fraction;
        }
    }

    void ICallbackNode.Register(ICallbackRegistrationContext context) =>
        context.Register(progress);
}

public class ExponentialExploration(IProgress progress, double start, double end, double decay) :
    IExploration
{
    public double Epsilon => end + (start - end) * Math.Exp(-progress.Step / decay);
}

public static class Exploration
{
    public static LinearExploration Linear(IProgress progress, int target, double start, double end, double fraction) =>
        new(progress, target, start, end, fraction);

    public static ExponentialExploration Exponential(IProgress progress, double start, double end, double decay) =>
        new(progress, start, end, decay);
}

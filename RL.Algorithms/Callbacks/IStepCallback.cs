using System;

namespace RL.Algorithms.Callbacks;

public interface IStepCallback<TO, TA> : ICallbackNode
{
    public void OnStep(IAlgorithm<TO, TA> algorithm);
}

public class StepAction<TO, TA, TContext>(Action<TContext, IAlgorithm<TO, TA>> callback, TContext context) :
    IStepCallback<TO, TA>
{
    public void OnStep(IAlgorithm<TO, TA> algorithm) => callback.Invoke(context, algorithm);
}

public class StepFunction<TO, TA, TContext>(Func<TContext, IAlgorithm<TO, TA>, TContext> callback, TContext context) :
    IStepCallback<TO, TA>
{
    public void OnStep(IAlgorithm<TO, TA> algorithm) => context = callback.Invoke(context, algorithm);
}

public static class Step<TO, TA>
{
    public static StepAction<TO, TA, TContext> Action<TContext>(
        Action<TContext, IAlgorithm<TO, TA>> callback,
        TContext context
    ) => new(callback, context);

    public static StepFunction<TO, TA, TContext> Function<TContext>(
        Func<TContext, IAlgorithm<TO, TA>, TContext> callback,
        TContext context
    ) => new(callback, context);
}

using System;

namespace RL.Algorithms.Callbacks;

public interface ITrainStepCallback<TO, TA> : ICallbackNode
{
    public void OnTrainStep(IAlgorithm<TO, TA> algorithm);
}

public class TrainStepAction<TO, TA, TContext>(
    Action<TContext, IAlgorithm<TO, TA>> callback,
    TContext context
) : ITrainStepCallback<TO, TA>
{
    public void OnTrainStep(IAlgorithm<TO, TA> algorithm) => callback.Invoke(context, algorithm);
}

public class TrainStepFunction<TO, TA, TContext>(
    Func<TContext, IAlgorithm<TO, TA>, TContext> callback,
    TContext context
) : ITrainStepCallback<TO, TA>
{
    public void OnTrainStep(IAlgorithm<TO, TA> algorithm) => context = callback.Invoke(context, algorithm);
}

public static class TrainStep<TO, TA>
{
    public static TrainStepAction<TO, TA, TContext> Action<TContext>(
        Action<TContext, IAlgorithm<TO, TA>> callback,
        TContext context
    ) => new(callback, context);

    public static TrainStepFunction<TO, TA, TContext> Function<TContext>(
        Func<TContext, IAlgorithm<TO, TA>, TContext> callback,
        TContext context
    ) => new(callback, context);
}

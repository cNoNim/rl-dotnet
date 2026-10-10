using System;

namespace RL.Algorithms.Callbacks;

public interface IAfterTrainCallback<TO, TA> : ICallbackNode
{
    public void OnAfterTrain(IAlgorithm<TO, TA> algorithm);
}

public class AfterTrainContextAction<TO, TA, TContext>(
    Action<TContext, IAlgorithm<TO, TA>> callback,
    TContext context
) : IAfterTrainCallback<TO, TA>
{
    public void OnAfterTrain(IAlgorithm<TO, TA> algorithm) => callback.Invoke(context, algorithm);
}

public static class AfterTrain<TO, TA>
{
    public static AfterTrainContextAction<TO, TA, TContext> Action<TContext>(
        Action<TContext, IAlgorithm<TO, TA>> callback,
        TContext context
    ) => new(callback, context);
}

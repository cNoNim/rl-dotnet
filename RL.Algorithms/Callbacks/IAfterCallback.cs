using System;

namespace RL.Algorithms.Callbacks;

public interface IAfterCallback<TO, TA> : ICallbackNode
{
    public void OnAfter(IAlgorithm<TO, TA> algorithm);
}

public class AfterContextAction<TO, TA, TContext>(
    Action<TContext, IAlgorithm<TO, TA>> callback,
    TContext context
) : IAfterCallback<TO, TA>
{
    public void OnAfter(IAlgorithm<TO, TA> algorithm) => callback.Invoke(context, algorithm);
}

public static class After<TO, TA>
{
    public static AfterContextAction<TO, TA, TContext> Action<TContext>(
        Action<TContext, IAlgorithm<TO, TA>> callback,
        TContext context
    ) => new(callback, context);
}

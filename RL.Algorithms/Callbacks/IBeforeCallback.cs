using System;

namespace RL.Algorithms.Callbacks;

public interface IBeforeCallback<TO, TA> : ICallbackNode
{
    public void OnBefore(IAlgorithm<TO, TA> algorithm);
}

public class BeforeContextAction<TO, TA, TContext>(
    Action<TContext, IAlgorithm<TO, TA>> callback,
    TContext context
) : IBeforeCallback<TO, TA>
{
    public void OnBefore(IAlgorithm<TO, TA> algorithm) => callback.Invoke(context, algorithm);
}

public static class Before<TO, TA>
{
    public static BeforeContextAction<TO, TA, TContext> Action<TContext>(
        Action<TContext, IAlgorithm<TO, TA>> callback,
        TContext context
    ) => new(callback, context);
}

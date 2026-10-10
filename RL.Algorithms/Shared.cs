using System;
using RL.Algorithms.Callbacks;

namespace RL.Algorithms;

public class Shared<TContext>(TContext context)
{
    public TContext Context { get; set; } = context;
}

public readonly ref struct SharedAction<TO, TA, TContext>(TContext context)
{
    public Action<TContext, IAlgorithm<TO, TA>>? Before { get; init; }
    public Action<TContext, IAlgorithm<TO, TA>>? Step { get; init; }
    public Action<TContext, IAlgorithm<TO, TA>>? TrainStep { get; init; }
    public Action<TContext, IAlgorithm<TO, TA>>? AfterTrain { get; init; }
    public Action<TContext, IAlgorithm<TO, TA>>? After { get; init; }

    public void Register(IAlgorithm<TO, TA> algorithm) =>
        RegisterShared(algorithm);

    private void RegisterShared(IAlgorithm<TO, TA> algorithm)
    {
        var shared = new Shared<TContext>(context);
        if (Before != null)
            algorithm.Register(Before<TO, TA>.Action(Action, (context: shared, action: Before)));
        if (Step != null)
            algorithm.Register(Step<TO, TA>.Action(Action, (context: shared, action: Step)));
        if (TrainStep != null)
            algorithm.Register(TrainStep<TO, TA>.Action(Action, (context: shared, action: TrainStep)));
        if (AfterTrain != null)
            algorithm.Register(AfterTrain<TO, TA>.Action(Action, (context: shared, action: AfterTrain)));
        if (After != null)
            algorithm.Register(After<TO, TA>.Action(Action, (context: shared, action: After)));
        return;

        static void Action(
            (Shared<TContext> context, Action<TContext, IAlgorithm<TO, TA>> action) shared,
            IAlgorithm<TO, TA> algorithm
        ) => shared.action(shared.context.Context, algorithm);
    }
}

public readonly ref struct SharedFunction<TO, TA, TContext>(TContext context)
{
    public Func<TContext, IAlgorithm<TO, TA>, TContext>? Before { get; init; }
    public Func<TContext, IAlgorithm<TO, TA>, TContext>? Step { get; init; }
    public Func<TContext, IAlgorithm<TO, TA>, TContext>? TrainStep { get; init; }
    public Func<TContext, IAlgorithm<TO, TA>, TContext>? AfterTrain { get; init; }
    public Func<TContext, IAlgorithm<TO, TA>, TContext>? After { get; init; }

    public void Register(IAlgorithm<TO, TA> algorithm)
    {
        var shared = new Shared<TContext>(context);
        if (Before != null)
            algorithm.Register(Before<TO, TA>.Action(Action, (context: shared, action: Before)));
        if (Step != null)
            algorithm.Register(Step<TO, TA>.Action(Action, (context: shared, action: Step)));
        if (TrainStep != null)
            algorithm.Register(TrainStep<TO, TA>.Action(Action, (context: shared, action: TrainStep)));
        if (AfterTrain != null)
            algorithm.Register(AfterTrain<TO, TA>.Action(Action, (context: shared, action: AfterTrain)));
        if (After != null)
            algorithm.Register(After<TO, TA>.Action(Action, (context: shared, action: After)));
        return;

        static void Action(
            (Shared<TContext> context, Func<TContext, IAlgorithm<TO, TA>, TContext> function) shared,
            IAlgorithm<TO, TA> algorithm
        )
        {
            var context = shared.function(shared.context.Context, algorithm);
            shared.context.Context = context;
        }
    }
}

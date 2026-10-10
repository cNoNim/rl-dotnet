using System;

namespace RL.Algorithms;

public static class AlgorithmExtensions
{
    public static void Register<TO, TA, TContext>(
        this IAlgorithm<TO, TA> algorithm,
        TContext context,
        Action<TContext, IAlgorithm<TO, TA>>? before = null,
        Action<TContext, IAlgorithm<TO, TA>>? step = null,
        Action<TContext, IAlgorithm<TO, TA>>? trainStep = null,
        Action<TContext, IAlgorithm<TO, TA>>? afterTrain = null,
        Action<TContext, IAlgorithm<TO, TA>>? after = null
    ) => new SharedAction<TO, TA, TContext>(context)
    {
        Before = before,
        Step = step,
        TrainStep = trainStep,
        AfterTrain = afterTrain,
        After = after
    }.Register(algorithm);

    public static void Register<TO, TA, TContext>(
        this IAlgorithm<TO, TA> algorithm,
        TContext context,
        Func<TContext, IAlgorithm<TO, TA>, TContext>? beforeLearn = null,
        Func<TContext, IAlgorithm<TO, TA>, TContext>? step = null,
        Func<TContext, IAlgorithm<TO, TA>, TContext>? trainStep = null,
        Func<TContext, IAlgorithm<TO, TA>, TContext>? afterTrain = null,
        Func<TContext, IAlgorithm<TO, TA>, TContext>? after = null
    ) => new SharedFunction<TO, TA, TContext>(context)
    {
        Before = beforeLearn,
        Step = step,
        TrainStep = trainStep,
        AfterTrain = afterTrain,
        After = after
    }.Register(algorithm);
}

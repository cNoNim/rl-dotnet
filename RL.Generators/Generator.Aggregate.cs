using System;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TAccumulate Aggregate<TSource, TAccumulate>(
        this IGenerator<TSource> generator,
        TAccumulate accumulate,
        Func<TAccumulate, TSource, TAccumulate> func
    ) => Aggregate<IGenerator<TSource>, TSource, TAccumulate>(generator, accumulate, func);

    public static TAccumulate Aggregate<TG, TSource, TAccumulate>(
        this TG generator,
        TAccumulate accumulate,
        Func<TAccumulate, TSource, TAccumulate> func
    )
        where TG : IGenerator<TSource>
    {
        if (!generator.IsFinite)
            throw new OverflowException();

        var result = accumulate;
        foreach (var element in generator.AsGeneratorEnumerable<TG, TSource>())
            result = func(result, element);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<TG, TSource, TAccumulate, TResult>(
        this TakeGenerator<TG, TSource> generator,
        TAccumulate accumulate,
        Func<TAccumulate, TSource, TAccumulate> func,
        Func<TAccumulate, TResult> resultSelector
    ) where TG : IGenerator<TSource> => Aggregate<TakeGenerator<TG, TSource>, TSource, TAccumulate, TResult>(
        generator,
        accumulate,
        func,
        resultSelector
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Aggregate<TSource, TAccumulate, TResult>(
        this IGenerator<TSource> generator,
        TAccumulate accumulate,
        Func<TAccumulate, TSource, TAccumulate> func,
        Func<TAccumulate, TResult> resultSelector
    ) => Aggregate<IGenerator<TSource>, TSource, TAccumulate, TResult>(generator, accumulate, func, resultSelector);

    public static TResult Aggregate<TG, TSource, TAccumulate, TResult>(
        this TG generator,
        TAccumulate accumulate,
        Func<TAccumulate, TSource, TAccumulate> func,
        Func<TAccumulate, TResult> resultSelector
    )
        where TG : IGenerator<TSource>
    {
        if (!generator.IsFinite)
            throw new OverflowException();

        var result = accumulate;
        foreach (var element in generator.AsGeneratorEnumerable<TG, TSource>())
            result = func(result, element);
        return resultSelector(result);
    }
}
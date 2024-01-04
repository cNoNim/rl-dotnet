using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Generators;

public static partial class Generator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImmutableArray<int> WhereIndices<TG, T, TTo, TContext>(
        this TG generator,
        TContext context,
        Func<TContext, T, bool> predicate
    ) where TG : IGenerator<T>
    {
        var builder = ImmutableArray.CreateBuilder<int>();

        var current = -1;
        while (generator.TryGetNext(current, out current))
        {
            var value = generator[current];
            if (predicate(context, value))
                builder.Add(current);
        }

        return builder.ToImmutable();
    }
}
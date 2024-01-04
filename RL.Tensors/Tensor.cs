using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Generators;

namespace RL.Tensors;

public static partial class Tensor
{
    public static Tensor1D<T> Create<T>(ReadOnlySpan<T> span)
        where T :
        IAdditionOperators<T, T, T>,
        IDivisionOperators<T, T, T>,
        IComparisonOperators<T, T, bool>,
        IModulusOperators<T, T, T>,
        IMultiplyOperators<T, T, T>,
        ISubtractionOperators<T, T, T>,
        IUnaryPlusOperators<T, T>,
        IUnaryNegationOperators<T, T>,
        IEquatable<T> =>
        new(span);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Flatten(this (int x, int y) shape) => shape.x * shape.y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Flatten(this (int x, int y) index, (int x, int y) shape) => index.x * shape.y + index.y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Flatten(this (Index x, Index y) index, (int x, int y) shape) =>
        Flatten((index.x.GetOffset(shape.x), index.y.GetOffset(shape.y)), shape);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (int x, int y) Multi(this int index, (int x, int y) shape) => (index / shape.y, index % shape.y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor1D<T> Zeroes<T>(this int shape)
        where T :
        IAdditionOperators<T, T, T>,
        IDivisionOperators<T, T, T>,
        IComparisonOperators<T, T, bool>,
        IModulusOperators<T, T, T>,
        IMultiplyOperators<T, T, T>,
        ISubtractionOperators<T, T, T>,
        IUnaryPlusOperators<T, T>,
        IUnaryNegationOperators<T, T>,
        IEquatable<T> => new(shape);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor2D<T> Zeroes<T>(this (int x, int y) shape)
        where T : INumberBase<T> => new(shape);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tensor2D<T> Zeroes<T>(int x, int y)
        where T : INumberBase<T> => Zeroes<T>((x, y));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<
        TakeGenerator<SequenceGenerator<int>, int>,
        int,
        double,
        (int max, double epsilon, double em)
    > EpsilonGreedy<T>(
        this Tensor2D<T>.Row generator, double epsilon)
        where T : INumberBase<T>, IMinMaxValue<T>, IComparisonOperators<T, T, bool> =>
        generator.EpsilonGreedy<Tensor2D<T>.Row, T>(epsilon);

    public static T Max<T>(this Tensor2D<T>.Row generator)
        where T : INumberBase<T>, IComparisonOperators<T, T, bool>, IMinMaxValue<T> =>
        generator.Max<Tensor2D<T>.Row, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SelectGenerator<Tensor1D<T>, T, TTo, TContext> Select<T, TTo, TContext>(
        this Tensor1D<T> generator,
        TContext context,
        Func<TContext, T, TTo> selector
    ) where T :
        IAdditionOperators<T, T, T>,
        IDivisionOperators<T, T, T>,
        IComparisonOperators<T, T, bool>,
        IModulusOperators<T, T, T>,
        IMultiplyOperators<T, T, T>,
        ISubtractionOperators<T, T, T>,
        IUnaryPlusOperators<T, T>,
        IUnaryNegationOperators<T, T>,
        IEquatable<T> =>
        generator.Select<Tensor1D<T>, T, TTo, TContext>(context, selector);
}
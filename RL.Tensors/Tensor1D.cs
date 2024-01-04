using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Tensors;

[CollectionBuilder(typeof(Tensor), "Create")]
public readonly partial struct Tensor1D<T> :
    IFiniteGenerator<Tensor1D<T>, T>,
    IComparisonOperators<Tensor1D<T>, Tensor1D<T>, bool>
    where T :
    IAdditionOperators<T, T, T>,
    IDivisionOperators<T, T, T>,
    IComparisonOperators<T, T, bool>,
    IModulusOperators<T, T, T>,
    IMultiplyOperators<T, T, T>,
    ISubtractionOperators<T, T, T>,
    IUnaryPlusOperators<T, T>,
    IUnaryNegationOperators<T, T>,
    IEquatable<T>
{
    private readonly T[] _array;

    private Tensor1D(int shape, T[] array)
    {
        _array = array;
        Shape = shape;
    }

    public Tensor1D(ReadOnlySpan<T> span)
        : this(span.Length, span.ToArray())
    {
    }

    public Tensor1D(int shape)
        : this(shape, new T[shape])
    {
    }

    public ref T this[int index] => ref _array[index];
    public int Shape { get; }

    public GeneratorEnumerator<Tensor1D<T>, T> GetEnumerator() => new(this);
    public static implicit operator T[](Tensor1D<T> adapter) => adapter._array;
    T IGenerator<T>.this[int index] => this[index];
    int IGenerator<T>.Count => Shape;

    private static Tensor1D<T> CombineToTensor(Tensor1D<T> left, Tensor1D<T> right, Func<T, T, T> combiner)
    {
        CheckShape(left, right);
        var shape = left.Shape;
        var result = new Tensor1D<T>(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left[i], right[i]);
        return result;
    }

    private static Tensor1D<T> CombineToTensor(Tensor1D<T> left, T right, Func<T, T, T> combiner)
    {
        var shape = left.Shape;
        var result = new Tensor1D<T>(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left[i], right);
        return result;
    }

    private static Tensor1D<T> CombineToTensor(T left, Tensor1D<T> right, Func<T, T, T> combiner)
    {
        var shape = right.Shape;
        var result = new Tensor1D<T>(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left, right[i]);
        return result;
    }

    private static bool CombineToBoolean(Tensor1D<T> left, Tensor1D<T> right, Func<T, T, bool> combiner)
    {
        CheckShape(left, right);
        for (var i = 0; i < left.Shape; i++)
            if (!combiner(left[i], right[i]))
                return false;
        return true;
    }

    private static BooleanTensor1D CombineToBooleanTensor(
        Tensor1D<T> left,
        Tensor1D<T> right,
        Func<T, T, bool> combiner
    )
    {
        CheckShape(left, right);
        var shape = left.Shape;
        var result = new BooleanTensor1D(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left[i], right[i]);
        return result;
    }

    private static BooleanTensor1D CombineToBooleanTensor(
        Tensor1D<T> left,
        T right,
        Func<T, T, bool> combiner
    )
    {
        var shape = left.Shape;
        var result = new BooleanTensor1D(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left[i], right);
        return result;
    }

    private static BooleanTensor1D CombineToBooleanTensor(
        T left,
        Tensor1D<T> right,
        Func<T, T, bool> combiner
    )
    {
        var shape = right.Shape;
        var result = new BooleanTensor1D(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left, right[i]);
        return result;
    }

    [Conditional("DEBUG")]
    private static void CheckShape(Tensor1D<T> left, Tensor1D<T> right) =>
        ArgumentOutOfRangeException.ThrowIfNotEqual(left.Shape, right.Shape);

    [Conditional("DEBUG")]
    private static void CheckShape(Tensor1D<T> left, int shape) =>
        ArgumentOutOfRangeException.ThrowIfNotEqual(left.Shape, shape);
}
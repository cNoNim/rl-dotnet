using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;
using RL.Generators;

namespace RL.MDArrays;

[DebuggerTypeProxy(typeof(GeneratorDebugView))]
[DebuggerDisplay("Count = {Count}")]
[CollectionBuilder(typeof(MDArray), nameof(MDArray.Create))]
public readonly partial struct Array1D<T> :
    IGenerator<Array1D<T>, T>,
    IComparisonOperators<Array1D<T>, Array1D<T>, bool>
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

    private Array1D(int shape, T[] array)
    {
        _array = array;
        Shape = shape;
    }

    public Array1D(ReadOnlySpan<T> span)
        : this(span.Length, span.ToArray())
    {
    }

    public Array1D(int shape)
        : this(shape, new T[shape])
    {
    }

    public ref T this[int index] => ref _array[index];
    public int Shape { get; }

    public GeneratorEnumerator<Array1D<T>, T> GetEnumerator() => new(this);
    public static implicit operator T[](Array1D<T> adapter) => adapter._array;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Count => Shape;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsFinite => true;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetNext(int current, out int next)
    {
        if (current >= Shape)
        {
            next = Shape + 1;
            return false;
        }

        next = current + 1;
        return true;
    }

    T IGenerator<T>.this[int index] => this[index];

    private static Array1D<T> CombineToTensor(Array1D<T> left, Array1D<T> right, Func<T, T, T> combiner)
    {
        CheckShape(left, right);
        var shape = left.Shape;
        var result = new Array1D<T>(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left[i], right[i]);
        return result;
    }

    private static Array1D<T> CombineToTensor(Array1D<T> left, T right, Func<T, T, T> combiner)
    {
        var shape = left.Shape;
        var result = new Array1D<T>(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left[i], right);
        return result;
    }

    private static Array1D<T> CombineToTensor(T left, Array1D<T> right, Func<T, T, T> combiner)
    {
        var shape = right.Shape;
        var result = new Array1D<T>(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left, right[i]);
        return result;
    }

    private static bool CombineToBoolean(Array1D<T> left, Array1D<T> right, Func<T, T, bool> combiner)
    {
        CheckShape(left, right);
        for (var i = 0; i < left.Shape; i++)
            if (!combiner(left[i], right[i]))
                return false;
        return true;
    }

    private static BooleanArray1D CombineToBooleanTensor(
        Array1D<T> left,
        Array1D<T> right,
        Func<T, T, bool> combiner
    )
    {
        CheckShape(left, right);
        var shape = left.Shape;
        var result = new BooleanArray1D(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left[i], right[i]);
        return result;
    }

    private static BooleanArray1D CombineToBooleanTensor(
        Array1D<T> left,
        T right,
        Func<T, T, bool> combiner
    )
    {
        var shape = left.Shape;
        var result = new BooleanArray1D(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left[i], right);
        return result;
    }

    private static BooleanArray1D CombineToBooleanTensor(
        T left,
        Array1D<T> right,
        Func<T, T, bool> combiner
    )
    {
        var shape = right.Shape;
        var result = new BooleanArray1D(shape);
        for (var i = 0; i < shape; i++)
            result[i] = combiner(left, right[i]);
        return result;
    }

    [Conditional("DEBUG")]
    private static void CheckShape(Array1D<T> left, Array1D<T> right) =>
        ArgumentOutOfRangeException.ThrowIfNotEqual(left.Shape, right.Shape);

    [Conditional("DEBUG")]
    private static void CheckShape(Array1D<T> left, int shape) =>
        ArgumentOutOfRangeException.ThrowIfNotEqual(left.Shape, shape);
}
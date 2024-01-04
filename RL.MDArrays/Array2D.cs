using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.MDArrays;

public readonly struct Array2D<T>((int x, int y) shape) :
    IGenerator<Array2D<T>, Array2D<T>.Row>,
    IAdditionOperators<Array2D<T>, Array2D<T>, Array2D<T>>
    where T : INumberBase<T>
{
    private readonly T[] _array = new T[shape.Flatten()];

    public Row this[int row] => new(this, row);

    public ref T this[(int x, int y) index] => ref _array[index.Flatten(shape)];

    public ref T this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref this[(x, y)];
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Count => Shape.x;

    public (int x, int y) Shape => shape;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GeneratorEnumerator<Array2D<T>, Row> GetEnumerator() =>
        new(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> RowSpan(int row) => _array.AsSpan((row, 0).Flatten(Shape), shape.y);

    public static explicit operator T[](Array2D<T> adapter) => adapter._array;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsFinite => true;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetNext(int current, out int next)
    {
        if (current >= Count)
        {
            next = Count + 1;
            return false;
        }

        next = current + 1;
        return true;
    }

    public readonly struct Row(Array2D<T> array, int row) :
        IGenerator<Row, T>
    {
        public ref T this[int index] => ref array[row, index];
        public int Shape => array.Shape.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GeneratorEnumerator<Row, T> GetEnumerator() =>
            new(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start) => array.RowSpan(row)[start..];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start, int length) => array.RowSpan(row).Slice(start, length);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Count => Shape;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsFinite => true;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool TryGetNext(int current, out int next)
        {
            if (current >= Count)
            {
                next = Count + 1;
                return false;
            }

            next = current + 1;
            return true;
        }

        T IGenerator<T>.this[int index] => this[index];
    }

    public static Array2D<T> operator +(Array2D<T> left, Array2D<T> right)
    {
        CheckShape(left, right);

        var shape = left.Shape;
        var result = new Array2D<T>(shape);
        for (var x = 0; x < shape.x; x++)
        for (var y = 0; y < shape.y; y++)
            result[x, y] = left[x, y] + right[x, y];
        return result;
    }

    [Conditional("DEBUG")]
    private static void CheckShape(Array2D<T> left, Array2D<T> right) =>
        ArgumentOutOfRangeException.ThrowIfNotEqual(left.Shape, right.Shape);
}
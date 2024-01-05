using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.Tensors;

public readonly struct Tensor2D<T>((int x, int y) shape) :
    IGenerator<Tensor2D<T>, Tensor2D<T>.Row>,
    IAdditionOperators<Tensor2D<T>, Tensor2D<T>, Tensor2D<T>>
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
    public GeneratorEnumerator<Tensor2D<T>, Row> GetEnumerator() =>
        new(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> RowSpan(int row) => _array.AsSpan((row, 0).Flatten(Shape), shape.y);

    public static explicit operator T[](Tensor2D<T> adapter) => adapter._array;

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

    public readonly struct Row(Tensor2D<T> tensor, int row) :
        IGenerator<Row, T>
    {
        public ref T this[int index] => ref tensor[row, index];
        public int Shape => tensor.Shape.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GeneratorEnumerator<Row, T> GetEnumerator() =>
            new(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start) => tensor.RowSpan(row)[start..];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start, int length) => tensor.RowSpan(row).Slice(start, length);

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

    public static Tensor2D<T> operator +(Tensor2D<T> left, Tensor2D<T> right)
    {
        CheckShape(left, right);

        var shape = left.Shape;
        var result = new Tensor2D<T>(shape);
        for (var x = 0; x < shape.x; x++)
        for (var y = 0; y < shape.y; y++)
            result[x, y] = left[x, y] + right[x, y];
        return result;
    }

    [Conditional("DEBUG")]
    private static void CheckShape(Tensor2D<T> left, Tensor2D<T> right) =>
        ArgumentOutOfRangeException.ThrowIfNotEqual(left.Shape, right.Shape);
}
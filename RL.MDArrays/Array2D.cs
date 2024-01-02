using System.Runtime.CompilerServices;
using RL.Core;

namespace RL.MDArrays;

public readonly struct Array2D<T>((int x, int y) shape) :
    IFiniteGenerator<Array2D<T>, Array2D<T>.Row>
{
    private readonly T[] _array = new T[shape.Flatten()];
    public Row this[int row] => new(this, row);
    public int Count => Shape.x;
    public ref T this[(int x, int y) index] => ref _array[index.Flatten(shape)];
    public ref T this[int x, int y] => ref this[(x, y)];
    public (int x, int y) Shape => shape;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GeneratorEnumerator<Array2D<T>, Row> GetEnumerator() =>
        new(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> RowSpan(int row) => _array.AsSpan((row, 0).Flatten(Shape), shape.y);

    public static explicit operator T[](Array2D<T> adapter) => adapter._array;

    public readonly struct Row(Array2D<T> array, int row) :
        IFiniteGenerator<Row, T>
    {
        public ref T this[int index] => ref array[row, index];
        public int Shape => array.Shape.y;
        public int Count => Shape;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GeneratorEnumerator<Row, T> GetEnumerator() =>
            new(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start) => array.RowSpan(row)[start..];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> Slice(int start, int length) => array.RowSpan(row).Slice(start, length);

        T IReadOnlyList<T>.this[int index] => this[index];
    }
}
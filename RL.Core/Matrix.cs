using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly struct Matrix<T>(T[,] array) :
    IStructList<Matrix<T>, Matrix<T>.Row>
{
    public int Count => array.GetLength(0);
    public Row this[int index] => new(array, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<Matrix<T>, Row> GetEnumerator() =>
        new(this);

    public readonly struct Row(T[,] array, int row) :
        IStructList<Row, T>
    {
        public int Count => array.GetLength(1);
        public ref T this[int index] => ref array[row, index];
        T IReadOnlyList<T>.this[int index] => this[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StructEnumerator<Row, T> GetEnumerator() =>
            new(this);
    }
}

using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly struct MatrixRowList<T>(T[,] array, int row) :
    IStructList<MatrixRowList<T>, T>
{
    public int Count => array.GetLength(1);
    public ref T this[int index] => ref array[row, index];
    T IReadOnlyList<T>.this[int index] => this[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<MatrixRowList<T>, T> GetEnumerator() =>
        new(this);
}

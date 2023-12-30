using System.Collections;

namespace RL.Core;

public readonly struct MatrixRowList<T>(T[,] array, int row) :
    IReadOnlyList<T>
{
    public int Count => array.GetLength(1);

    public ref T this[int index] => ref array[row, index];

    T IReadOnlyList<T>.this[int index] => this[index];

    public ReadOnlyListStructEnumerator<MatrixRowList<T>, T> GetEnumerator() =>
        new(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator().AsEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator().AsEnumerator();
}

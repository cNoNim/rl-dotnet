using System.Collections;

namespace RL.Core;

public readonly struct MatrixList<T>(T[,] array) :
    IReadOnlyList<MatrixRowList<T>>
{
    public int Count => array.GetLength(0);

    public MatrixRowList<T> this[int index] => array.Row(index);

    public ReadOnlyListStructEnumerator<MatrixList<T>, MatrixRowList<T>> GetEnumerator() =>
        new(this);

    IEnumerator<MatrixRowList<T>> IEnumerable<MatrixRowList<T>>.GetEnumerator() => GetEnumerator().AsEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator().AsEnumerator();
}

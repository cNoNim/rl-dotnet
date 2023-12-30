using System.Runtime.CompilerServices;

namespace RL.Core;

public readonly struct MatrixList<T>(T[,] array) :
    IStructList<MatrixList<T>, MatrixRowList<T>>
{
    public int Count => array.GetLength(0);
    public MatrixRowList<T> this[int index] => array.Row(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructEnumerator<MatrixList<T>, MatrixRowList<T>> GetEnumerator() =>
        new(this);
}

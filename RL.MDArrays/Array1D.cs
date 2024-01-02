using RL.Core;

namespace RL.MDArrays;

public readonly struct Array1D<T>(int shape) :
    IFiniteGenerator<Array1D<T>, T>
{
    private readonly T[] _array = new T[shape];
    public ref T this[int index] => ref _array[index];
    public int Shape => shape;
    public GeneratorEnumerator<Array1D<T>, T> GetEnumerator() => new(this);
    public static implicit operator T[](Array1D<T> adapter) => adapter._array;
    int IReadOnlyCollection<T>.Count => shape;
    T IReadOnlyList<T>.this[int index] => this[index];
}
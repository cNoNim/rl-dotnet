using System;
using RL.Core;

namespace RL.Tensors;

public readonly struct BooleanTensor1D :
    IFiniteGenerator<BooleanTensor1D, bool>
{
    private readonly bool[] _array;

    private BooleanTensor1D(int shape, bool[] array)
    {
        _array = array;
        Shape = shape;
    }

    public BooleanTensor1D(ReadOnlySpan<bool> span)
        : this(span.Length, span.ToArray())
    {
    }

    public BooleanTensor1D(int shape)
        : this(shape, new bool[shape])
    {
    }

    public ref bool this[int index] => ref _array[index];
    public int Shape { get; }

    public GeneratorEnumerator<BooleanTensor1D, bool> GetEnumerator() => new(this);
    public static implicit operator bool[](BooleanTensor1D adapter) => adapter._array;
    bool IGenerator<bool>.this[int index] => this[index];

    int IGenerator<bool>.Count => Shape;
}
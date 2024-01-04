using System;
using System.ComponentModel;
using RL.Core;

namespace RL.MDArrays;

public readonly struct BooleanArray1D :
    IGenerator<BooleanArray1D, bool>
{
    private readonly bool[] _array;

    private BooleanArray1D(int shape, bool[] array)
    {
        _array = array;
        Shape = shape;
    }

    public BooleanArray1D(ReadOnlySpan<bool> span)
        : this(span.Length, span.ToArray())
    {
    }

    public BooleanArray1D(int shape)
        : this(shape, new bool[shape])
    {
    }


    public ref bool this[int index] => ref _array[index];
    public int Shape { get; }
    public GeneratorEnumerator<BooleanArray1D, bool> GetEnumerator() => new(this);
    public static implicit operator bool[](BooleanArray1D adapter) => adapter._array;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsFinite => true;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Count => Shape;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetNext(int current, out int next)
    {
        if (current >= Shape)
        {
            next = Shape + 1;
            return false;
        }

        next = current + 1;
        return true;
    }

    bool IGenerator<bool>.this[int index] => this[index];
}
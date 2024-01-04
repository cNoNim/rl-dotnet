using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RL.Core;

public class CircularBuffer<T> : IGenerator<CircularBuffer<T>, T>
{
    private readonly T?[] _buffer;
    private int _end;
    private int _start;

    public CircularBuffer(int capacity)
        : this(capacity, [])
    {
    }

    public CircularBuffer(int capacity, T[] items)
    {
        Check(capacity, items);

        _buffer = new T[capacity];
        Array.Copy(items, _buffer, items.Length);
        Count = items.Length;

        _start = 0;
        _end = Count == capacity ? 0 : Count;
        return;

        static void Check(int capacity, IReadOnlyCollection<T> items)
        {
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(capacity, 0);
            ArgumentNullException.ThrowIfNull(items);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(capacity, items.Count);
        }
    }

    private int Capacity => _buffer.Length;

    private bool IsFull => Count == Capacity;

    private bool IsEmpty => Count == 0;

    public T this[int index]
    {
        get
        {
            CheckEmpty();
            CheckCount(index);
            return _buffer[InternalIndex(index)]!;
        }
        set
        {
            CheckEmpty();
            CheckCount(index);
            _buffer[InternalIndex(index)] = value;
        }
    }

    public int Count { get; private set; }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public GeneratorEnumerator<CircularBuffer<T>, T> GetEnumerator() =>
        new(this);

    public T Front()
    {
        CheckEmpty();
        return _buffer[_start]!;
    }

    public T Back()
    {
        CheckEmpty();
        return _buffer[(_end != 0 ? _end : Capacity) - 1]!;
    }

    public void PushBack(T item)
    {
        if (IsFull)
        {
            _buffer[_end] = item;
            Increment(ref _end);
            _start = _end;
        }
        else
        {
            _buffer[_end] = item;
            Increment(ref _end);
            ++Count;
        }
    }

    public void PushFront(T item)
    {
        if (IsFull)
        {
            Decrement(ref _start);
            _end = _start;
            _buffer[_start] = item;
        }
        else
        {
            Decrement(ref _start);
            _buffer[_start] = item;
            ++Count;
        }
    }

    public void PopBack()
    {
        CheckEmpty("Cannot take elements from an empty buffer.");
        Decrement(ref _end);
        _buffer[_end] = default;
        --Count;
    }

    public void PopFront()
    {
        CheckEmpty("Cannot take elements from an empty buffer.");
        _buffer[_start] = default;
        Increment(ref _start);
        --Count;
    }

    public void Clear()
    {
        _start = 0;
        _end = 0;
        Count = 0;
        Array.Clear(_buffer, 0, _buffer.Length);
    }

    [Conditional("DEBUG")]
    private void CheckEmpty(string message = "Cannot access an empty buffer.")
    {
        if (IsEmpty)
            throw new InvalidOperationException(message);
    }

    [Conditional("DEBUG")]
    private void CheckCount(int index)
    {
        if (index >= Count)
            throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer size is {Count}");
    }

    private void Increment(ref int index)
    {
        if (++index == Capacity)
            index = 0;
    }

    private void Decrement(ref int index)
    {
        if (index == 0)
            index = Capacity;
        index--;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InternalIndex(int index) =>
        _start + (index < (Capacity - _start) ? index : index - Capacity);
}
namespace RL.MDArrays;

public readonly partial struct Array1D<T>
{
    public void Deconstruct(out T v1)
    {
        CheckShape(this, 1);
        v1 = _array[0];
    }

    public void Deconstruct(out T v1, out T v2)
    {
        CheckShape(this, 2);
        v1 = _array[0];
        v2 = _array[1];
    }

    public void Deconstruct(out T v1, out T v2, out T v3)
    {
        CheckShape(this, 3);
        v1 = _array[0];
        v2 = _array[1];
        v3 = _array[2];
    }

    public void Deconstruct(out T v1, out T v2, out T v3, out T v4)
    {
        CheckShape(this, 4);
        v1 = _array[0];
        v2 = _array[1];
        v3 = _array[2];
        v4 = _array[3];
    }

    public void Deconstruct(out T v1, out T v2, out T v3, out T v4, out T v5)
    {
        CheckShape(this, 5);
        v1 = _array[0];
        v2 = _array[1];
        v3 = _array[2];
        v4 = _array[3];
        v5 = _array[4];
    }

    public void Deconstruct(out T v1, out T v2, out T v3, out T v4, out T v5, out T v6)
    {
        CheckShape(this, 6);
        v1 = _array[0];
        v2 = _array[1];
        v3 = _array[2];
        v4 = _array[3];
        v5 = _array[4];
        v6 = _array[5];
    }

    public void Deconstruct(out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7)
    {
        CheckShape(this, 7);
        v1 = _array[0];
        v2 = _array[1];
        v3 = _array[2];
        v4 = _array[3];
        v5 = _array[4];
        v6 = _array[5];
        v7 = _array[6];
    }

    public void Deconstruct(out T v1, out T v2, out T v3, out T v4, out T v5, out T v6, out T v7, out T v8)
    {
        CheckShape(this, 8);
        v1 = _array[0];
        v2 = _array[1];
        v3 = _array[2];
        v4 = _array[3];
        v5 = _array[4];
        v6 = _array[5];
        v7 = _array[6];
        v8 = _array[7];
    }
}
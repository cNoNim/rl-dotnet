namespace RL.Core;

public static partial class List
{
    public static Matrix<T> AsMatrix<T>(this T[,] array) =>
        new(array);
}

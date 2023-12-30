namespace RL.Core;

public static class Matrix
{
    public static MatrixList<T> AsMatrix<T>(this T[,] array) =>
        new(array);

    public static MatrixRowList<T> Row<T>(this T[,] array, int row) =>
        new(array, row);
}

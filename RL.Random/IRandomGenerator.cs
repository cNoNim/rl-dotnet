using System.Numerics;

namespace RL.Random;

public interface IRandomGenerator
{
    public uint NextState();

    public int Random(int min, int max);
    public long Random(long min, long max);
    public double Random(double min, double max);
    public float Random(float min, float max);
    public Vector2 Random(Vector2 min, Vector2 max);
    public T Random<T>(T min, T max);
}

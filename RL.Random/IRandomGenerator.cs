using System.Numerics;

namespace RL.Random;

public interface IRandomGenerator
{
    public double Random(double min, double max);
    public float Random(float min, float max);

    public T Random<T>(T min, T max)
        where T : INumber<T>;
}
namespace RL.Draw;

public interface IProjectionMatrixBuilder
{
    public void BuildProjectionMatrix(Span<float> m, float zBias);
}

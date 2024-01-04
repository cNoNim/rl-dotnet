using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;

namespace RL.Algorithms;

public interface IDiscreteAlgorithm
{
    public string Name { get; }

    public Array2D<double> CreateQ(IEnvironment<Discrete, Discrete, int, int> environment);

    public (Array1D<double> rewards, Array2D<double> qTable) Train(
        IEnvironment<Discrete, Discrete, int, int> environment,
        Array2D<double>? qTable = null
    );
}
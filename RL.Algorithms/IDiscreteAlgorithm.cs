using RL.Environments;
using RL.Environments.Spaces;
using RL.Tensors;

namespace RL.Algorithms;

public interface IDiscreteAlgorithm
{
    public string Name { get; }

    public Tensor2D<double> CreateQ(IEnvironment<Discrete, Discrete, int, int> environment);

    public (Tensor1D<double> rewards, Tensor2D<double> qTable) Train(
        IEnvironment<Discrete, Discrete, int, int> environment,
        Tensor2D<double>? qTable = null
    );
}
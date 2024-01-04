using RL.Environments;
using RL.Environments.Spaces;
using RL.Tensors;

namespace RL.Algorithms;

public interface IAlgorithm<in TOSpace, in TASpace, TO, out TA>
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    public string Name { get; }

    public (Tensor1D<double> rewards, Tensor2D<double> qTable) Train(
        IEnvironment<TOSpace, TASpace, TO, TA> environment,
        Tensor2D<double>? qTable = null
    );
}
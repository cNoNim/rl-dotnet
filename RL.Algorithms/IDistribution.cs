using TorchSharp.Modules;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public interface IDistribution<in TModuleParams>
{
    public (Linear, Parameter?) CreateDistributionModule(int inputDimension, TModuleParams parameters);
}

public class CategoricalDistribution(int dimension) : IDistribution<object>
{
    public (Linear, Parameter?) CreateDistributionModule(int inputDimension, object parameters) =>
        (Linear(inputDimension, dimension), null);
}

public class DiagGaussianDistribution(int dimension) : IDistribution<float>
{
    public (Linear, Parameter?) CreateDistributionModule(int inputDimension, float logStdInit) =>
        (Linear(inputDimension, dimension), Parameter(ones(dimension) * logStdInit, requires_grad: true));
}

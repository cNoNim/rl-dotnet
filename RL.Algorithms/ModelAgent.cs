using System;
using RL.Environments.Spaces;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public class ModelAgent<TOSpace, TASpace, TO, TA>(
    IPolicyEnvironment<TOSpace, TASpace, TO, TA> environment,
    Module<Tensor, Tensor> model,
    string? name = null
) :
    IAgent<TO, TA>, IDisposable
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    public string Name => name ?? model.GetName();

    public TA Predict(TO state)
    {
        model.eval();
        using (NewDisposeScope())
        {
            var input = environment.AsTensor(state);
            var predict = model.forward(input);
            return environment.FromTensor(predict);
        }
    }

    public void Dispose()
    {
        model.Dispose();
        GC.SuppressFinalize(this);
    }
}

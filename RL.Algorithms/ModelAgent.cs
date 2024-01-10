using System;
using RL.MDArrays;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public class ModelAgent(Module<Tensor, Tensor> model) : IAgent<Array1D<float>, int>, IDisposable
{
    public string Name => model.GetName();

    public int Predict(Array1D<float> state)
    {
        model.eval();
        var input = as_tensor(state);
        var predict = model.forward(input);
        return (int)predict.max(-1).indexes.ToInt64();
    }

    public void Dispose()
    {
        model.Dispose();
        GC.SuppressFinalize(this);
    }
}

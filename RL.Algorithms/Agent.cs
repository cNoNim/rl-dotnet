using System;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public static class Agent
{
    public static FuncAgent<TO, TA> Func<TO, TA>(Func<TO, TA> func) => new(func);
    public static ModelAgent Model(Module<Tensor, Tensor> model) => new(model);
}

using System;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public static class Agent
{
    public static FuncAgent<TO, TA> Func<TO, TA>(string name, Func<TO, TA> func) => new(name, func);
    public static ModelAgent Model(Module<Tensor, Tensor> model) => new(model);
}

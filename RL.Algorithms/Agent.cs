using System;
using RL.Environments.Spaces;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public static class Agent
{
    public static FuncAgent<TO, TA> Func<TO, TA>(Func<TO, TA> func, string name) => new(name, func);

    public static ModelAgent<TOSpace, TASpace, TO, TA> Model<TOSpace, TASpace, TO, TA>(
        IPolicyEnvironment<TOSpace, TASpace, TO, TA> environment,
        Module<Tensor, Tensor> model,
        string? name = null
    )
        where TOSpace : ISpace<TO>
        where TASpace : ISpace<TA> =>
        new(environment, model, name);
}

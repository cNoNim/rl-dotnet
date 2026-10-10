using System;
using TorchSharp;

namespace RL.Algorithms;

public sealed class Network : torch.nn.Module<torch.Tensor, torch.Tensor>
{
    public Network(
        string? name = null
    ) : base(name ?? nameof(Network))
    {

        RegisterComponents();
    }

    public override torch.Tensor forward(torch.Tensor input)
    {
        throw new NotImplementedException();
    }
}

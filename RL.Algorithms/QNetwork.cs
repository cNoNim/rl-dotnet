using System;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public sealed class QNetwork : Module<Tensor, Tensor>
{
    private readonly Module<Tensor, Tensor> _network;

    public QNetwork(
        int input,
        ReadOnlySpan<int> arch,
        int output,
        Func<Module<Tensor, Tensor>>? activationFactory = null,
        bool squash = false,
        bool hasBias = true,
        string? name = null
    ) : base(name ?? "Q")
    {
        var sequential = Sequential();
        if (arch.Length > 0)
        {
            sequential.append(Linear(input, arch[0], hasBias));
            sequential.append(activationFactory?.Invoke() ?? ReLU());
        }

        for (var index = 0; index < arch.Length - 1; index++)
        {
            sequential.append(Linear(arch[index], arch[index + 1], hasBias));
            sequential.append(activationFactory?.Invoke() ?? ReLU());
        }

        if (output > 0)
            sequential.append(Linear(arch.Length > 0 ? arch[^1] : input, output, hasBias));

        if (squash)
            sequential.append(Tanh());

        _network = sequential;
        RegisterComponents();
    }

    public override Tensor forward(Tensor input) =>
        _network.forward(input);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _network.Dispose();
            ClearModules();
        }

        base.Dispose(disposing);
    }
}

using System;
using TorchSharp.Modules;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public sealed class MlpExtractor : Module<Tensor, Tensor>
{
    private readonly Sequential _sequential;

    public MlpExtractor(
        int inputDimension,
        ReadOnlySpan<int> arch,
        Func<Module<Tensor, Tensor>> activationFactory,
        bool hasBias = true,
        string? name = null
    ) : base(name ?? nameof(MlpExtractor))
    {
        var sequential = Sequential();
        var lastDimension = inputDimension;
        foreach (var dimension in arch)
        {
            sequential.append(Linear(lastDimension, dimension, hasBias));
            sequential.append(activationFactory.Invoke());
            lastDimension = dimension;
        }

        OutputDimension = lastDimension;
        _sequential = sequential;
        RegisterComponents();
    }

    public int OutputDimension { get; }

    public override Tensor forward(Tensor input) =>
        _sequential.call(input);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _sequential.Dispose();
            ClearModules();
        }
        base.Dispose(disposing);
    }
}

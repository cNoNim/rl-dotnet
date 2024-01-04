using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;
using static TorchSharp.torch.nn.functional;

namespace RL.Algorithms;

public sealed class CrossEntropyModel :
    Module<Tensor, Tensor>
{
    private readonly Module<Tensor, Tensor> _lin1;
    private readonly Module<Tensor, Tensor> _lin2;

    public CrossEntropyModel(long inputSize, long outputSize, long hiddenSize, Device? device = null) :
        base("CrossEntropy")
    {
        _lin1 = Linear(inputSize, hiddenSize);
        _lin2 = Linear(hiddenSize, outputSize);
        RegisterComponents();

        if (device is { type: DeviceType.CUDA })
            this.to(device);
    }

    public override Tensor forward(Tensor input)
    {
        using var _ = NewDisposeScope();
        return _lin2.forward(relu(_lin1.forward(input)))
            .MoveToOuterDisposeScope();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _lin1.Dispose();
            _lin2.Dispose();
            ClearModules();
        }

        base.Dispose(disposing);
    }
}
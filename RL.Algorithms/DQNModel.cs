using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;
using static TorchSharp.torch.nn.functional;

namespace RL.Algorithms;

public sealed class DQNModel : Module<Tensor, Tensor>
{
    private readonly Module<Tensor, Tensor> _lin1;
    private readonly Module<Tensor, Tensor> _lin2;
    private readonly Module<Tensor, Tensor> _lin3;

    public DQNModel(long inputSize, long outputSize, long hiddenSize, Device? device = null) :
        base("DQN")
    {
        _lin1 = Linear(inputSize, hiddenSize);
        _lin2 = Linear(hiddenSize, hiddenSize);
        _lin3 = Linear(hiddenSize, outputSize);
        RegisterComponents();

        if (device is { type: DeviceType.CUDA })
            this.to(device);
    }

    public override Tensor forward(Tensor input)
    {
        using var _ = NewDisposeScope();
        var x = _lin1.forward(input);
        x = relu(x);
        x = _lin2.forward(x);
        x = relu(x);
        return _lin3.forward(x)
            .MoveToOuterDisposeScope();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _lin1.Dispose();
            _lin2.Dispose();
            _lin3.Dispose();
            ClearModules();
        }

        base.Dispose(disposing);
    }
}
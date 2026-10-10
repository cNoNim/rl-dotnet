using TorchSharp.Modules;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace RL.Algorithms;

public sealed class FlattenExtractor : Module<Tensor, Tensor>
{
    private readonly Flatten _flatten = Flatten();

    public FlattenExtractor(int dimension, string? name = null) :
        base(name ?? nameof(FlattenExtractor))
    {
        RegisterComponents();
        OutputDimension = dimension;
    }

    public int OutputDimension { get; }

    public override Tensor forward(Tensor input) =>
        _flatten.call(input);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _flatten.Dispose();
            ClearModules();
        }
        base.Dispose(disposing);
    }
}

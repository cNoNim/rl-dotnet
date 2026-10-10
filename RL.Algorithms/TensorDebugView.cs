using System.Diagnostics;
using static TorchSharp.torch;

[assembly: DebuggerDisplay("{this.str()}", Target = typeof(Tensor))]

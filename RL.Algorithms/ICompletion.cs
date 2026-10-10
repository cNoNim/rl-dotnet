using System;
using System.Linq;
using System.Threading;
using RL.Algorithms.Callbacks;

namespace RL.Algorithms;

public interface ICompletion
{
    public bool Complete { get; }
}

public class ConstantCompletion(bool value) : ICompletion
{
    public bool Complete => value;
}

public class ProgressCompletion(IProgress progress, int target) :
    ICompletion,
    ICallbackNode
{
    public bool Complete => progress.Step >= target;

    public void Register(ICallbackRegistrationContext context) =>
        context.Register(progress);
}

public class FunctionCompletion(Func<bool> complete) :
    ICompletion
{
    public bool Complete => complete();
}

public class CancellationTokenCompletion(CancellationToken token) :
    ICompletion
{
    public bool Complete => token.IsCancellationRequested;
}

public class AnyCompletion(ReadOnlySpan<ICompletion> completions) :
    ICompletion,
    ICallbackNode
{
    private readonly ICompletion[] _completions = [..completions];

    public bool Complete => _completions.Any(static completion => completion.Complete);

    public void Register(ICallbackRegistrationContext context)
    {
        foreach (var completion in _completions)
            context.Register(completion);
    }
}

public static class Completion
{
    public static ConstantCompletion Constant(bool value) =>
        new(value);

    public static ProgressCompletion Progress(IProgress progress, int target) =>
        new(progress, target);

    public static FunctionCompletion Function(Func<bool> complete) =>
        new(complete);

    public static CancellationTokenCompletion Token(CancellationToken token) =>
        new(token);

    public static AnyCompletion Any(ReadOnlySpan<ICompletion> completions) =>
        new(completions);
}

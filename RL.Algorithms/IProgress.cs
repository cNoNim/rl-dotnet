using RL.Algorithms.Callbacks;
using RL.Environments;

namespace RL.Algorithms;

public interface IProgress
{
    public int Step { get; }
}

public class StepProgress<TO, TA> :
    IProgress,
    IBeforeCallback<TO, TA>,
    IStepCallback<TO, TA>
{
    public int Step { get; private set; }

    public void OnBefore(IAlgorithm<TO, TA> algorithm) =>
        Step = 0;
    public void OnStep(IAlgorithm<TO, TA> algorithm) =>
        Step++;
}

public class EpisodeProgress<TO, TA> :
    IProgress,
    IBeforeCallback<TO, TA>,
    IStepCallback<TO, TA>
{
    public int Step { get; private set; }
    public void OnBefore(IAlgorithm<TO, TA> algorithm) =>
        Step = 0;

    public void OnStep(IAlgorithm<TO, TA> algorithm)
    {
        if (algorithm.Transition.Truncated || algorithm.Transition.Terminated)
            Step++;
    }
}

public static class Progress
{
    public static StepProgress<TO, TA> Step<TO, TA>(IEnvironment<TO, TA> _) =>
        new();

    public static EpisodeProgress<TO, TA> Episode<TO, TA>(IEnvironment<TO, TA> _) =>
        new();
}

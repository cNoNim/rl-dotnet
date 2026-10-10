using System.Collections.Generic;

namespace RL.Algorithms.Callbacks;

public class CallbackCollection<TO, TA> : ICallback<TO, TA>, ICallbackRegistrationContext
{
    private readonly HashSet<ICallbackNode> _nodes = [];
    private readonly List<IBeforeCallback<TO, TA>> _before = [];
    private readonly List<IStepCallback<TO, TA>> _step = [];
    private readonly List<ITrainStepCallback<TO, TA>> _trainStep = [];
    private readonly List<IAfterTrainCallback<TO, TA>> _afterTrain = [];
    private readonly List<IAfterCallback<TO, TA>> _after = [];

    public bool Register<T>(T node)
    {
        if (node is not ICallbackNode callback || !_nodes.Add(callback))
            return false;
        callback.Register(this);
        Add(callback);
        return true;
    }

    private void Add(ICallbackNode callback)
    {
        if (callback is IBeforeCallback<TO, TA> before)
            _before.Add(before);
        if (callback is IStepCallback<TO, TA> step)
            _step.Add(step);
        if (callback is ITrainStepCallback<TO, TA> trainStep)
            _trainStep.Add(trainStep);
        if (callback is IAfterTrainCallback<TO, TA> afterTrain)
            _afterTrain.Add(afterTrain);
        if (callback is IAfterCallback<TO, TA> after)
            _after.Add(after);
    }

    public void Before(IAlgorithm<TO, TA> algorithm)
    {
        foreach (var callback in _before)
            callback.OnBefore(algorithm);
    }

    public void BeforeCollect(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void BeforeStep(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void Step(IAlgorithm<TO, TA> algorithm)
    {
        foreach (var callback in _step)
            callback.OnStep(algorithm);
    }

    public void AfterCollect(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void BeforeTrain(IAlgorithm<TO, TA> algorithm)
    {
    }

    public void TrainStep(IAlgorithm<TO, TA> algorithm)
    {
        foreach (var callback in _trainStep)
            callback.OnTrainStep(algorithm);
    }

    public void AfterTrain(IAlgorithm<TO, TA> algorithm)
    {
        foreach (var callback in _afterTrain)
            callback.OnAfterTrain(algorithm);
    }

    public void After(IAlgorithm<TO, TA> algorithm)
    {
        foreach (var callback in _after)
            callback.OnAfter(algorithm);
    }
}

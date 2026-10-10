using System;
using RL.Algorithms.Callbacks;
using RL.Environments;
using RL.Random;
using TorchSharp;
using static RL.Generators.Generator;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;
using static TorchSharp.torch.nn.functional;
using static TorchSharp.torch.nn.utils;
using static TorchSharp.torch.optim;

namespace RL.Algorithms;

public class DQN<TO> : IAlgorithm<TO, int>
{
    private readonly CallbackCollection<TO, int> _collection = new();
    private readonly RandomGenerator _random = new();
    private readonly ReplayBuffer<TO, int> _replayBuffer;
    private readonly IPolicyEnvironment<TO, int> _environment;
    private readonly IExploration _exploration;
    private readonly ICompletion _completion;
    private readonly ICompletion? _prologCompletion;
    private readonly ICompletion? _trainPrologCompletion;
    private readonly Func<TO, int>? _prologHeuristic;
    private readonly float _gamma;
    private readonly float _tau;
    private readonly int _trainFrequency;
    private readonly int _gradientCount;
    private readonly int _targetUpdateFrequency;
    private readonly int _maxGradNorm;

    public DQN(
        IPolicyEnvironment<TO, int> environment,
        ReplayBuffer<TO, int> replayBuffer,
        IExploration exploration,
        ICompletion completion,
        ICompletion? prologCompletion = null,
        ICompletion? trainPrologCompletion = null,
        Func<TO, int>? prologHeuristic = null,
        float gamma = 0.99f,
        float tau = 1.0f,
        int trainFrequency = 4,
        int gradientCount = 1,
        int targetUpdateFrequency = 1,
        int maxGradNorm = 10
    )
    {
        _environment = environment;
        _replayBuffer = replayBuffer;
        _exploration = exploration;
        _completion = completion;
        _prologCompletion = prologCompletion;
        _trainPrologCompletion = trainPrologCompletion ?? _prologCompletion;
        _prologHeuristic = prologHeuristic;
        _gamma = gamma;
        _tau = tau;
        _trainFrequency = trainFrequency;
        _gradientCount = gradientCount;
        _targetUpdateFrequency = targetUpdateFrequency;
        _maxGradNorm = maxGradNorm;
        _collection.Register(exploration);
        _collection.Register(completion);
        if (prologCompletion != null)
            _collection.Register(prologCompletion);
        if (trainPrologCompletion != null)
            _collection.Register(trainPrologCompletion);
    }

    public Transition<TO, int> Transition { get; private set; }
    public double Loss { get; private set; }

    public bool Register<T>(T callback) where T : ICallbackNode =>
        _collection.Register(callback);

    public void Learn(
        Module<Tensor, Tensor> policy,
        Module<Tensor, Tensor> target,
        Optimizer optimizer
    )
    {
        target.load_state_dict(policy.state_dict());

        try
        {
            _collection.Before(this);
            _replayBuffer.Clear();
            var trainStep = 0;

            _environment.Reset();
            while (true)
            {
                var (collectedCount, continueTraining) = Collect(policy);
                if (_trainPrologCompletion is { Complete: true })
                {
                    Train(policy, target, optimizer, _gradientCount <= 0 ? collectedCount : _gradientCount);
                    trainStep++;
                    if (trainStep % _targetUpdateFrequency == 0)
                        target.SoftUpdateBy(policy, _tau);
                }

                if (!continueTraining)
                    break;
            }
        }
        finally
        {
            _collection.After(this);
        }
    }

    private (int collectedCount, bool continueTraining) Collect(Module<Tensor, Tensor> policy)
    {
        try
        {
            _collection.BeforeCollect(this);
            policy.eval();
            var collectedCount = 0;
            while (NeedCollectMore())
            {
                var action = SampleAction();
                _collection.BeforeStep(this);
                Transition = _environment.Step(action);
                _replayBuffer.Push(Transition);
                collectedCount++;
                _collection.Step(this);
                if (_completion.Complete)
                    return (collectedCount, false);
            }

            return (collectedCount, true);

            bool NeedCollectMore() =>
                collectedCount < _trainFrequency;

            int SampleAction()
            {
                if (_prologCompletion is { Complete: false })
                    return _prologHeuristic?.Invoke(_environment.Observation) ?? _environment.SampleAction(_random);

                if (_random.Random(0.0, 1.0) < _exploration.Epsilon)
                    return _environment.SampleAction(_random);

                using (no_grad())
                {
                    var o = _environment.AsTensor(_environment.Observation);
                    var predict = policy.call(o);
                    return _environment.FromTensor(predict);
                }
            }
        }
        finally
        {
            _collection.AfterCollect(this);
        }
    }

    private void Train(
        Module<Tensor, Tensor> policy,
        Module<Tensor, Tensor> target,
        Optimizer optimizer,
        int count
    )
    {
        try
        {
            _collection.BeforeTrain(this);
            policy.train();
            foreach (var _ in Range(count))
                using (NewDisposeScope())
                {
                    var (observations, actions, nextObservations, rewards, nonFinals) =
                        _replayBuffer.Sample(_random);

                    Tensor targetQValues;
                    using (no_grad())
                    {
                        var nextQValues = target.call(nextObservations);
                        nextQValues = nextQValues.max(1).values;
                        nextQValues = nextQValues.reshape(-1, 1);
                        targetQValues = rewards + nonFinals * _gamma * nextQValues;
                    }

                    var currentQValues = policy.call(observations);
                    currentQValues = currentQValues.gather(1, actions.@long());

                    var loss = smooth_l1_loss(currentQValues, targetQValues);

                    optimizer.zero_grad();
                    loss.backward();
                    clip_grad_norm_(policy.parameters(), _maxGradNorm);
                    optimizer.step();

                    Loss = loss.ToDouble();
                    _collection.TrainStep(this);
                }
        }
        finally
        {
            _collection.AfterTrain(this);
        }
    }
}

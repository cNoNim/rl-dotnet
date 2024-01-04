using RL.Core;
using RL.Generators;
using RL.Random;
using static TorchSharp.torch;

namespace RL.Algorithms;

public class ReplayMemory(int capacity)
{
    private readonly CircularBuffer<(Tensor state, Tensor action, Tensor? nextState, Tensor reward)> _memory =
        new(capacity);

    public int Count => _memory.Count;

    public void Push(Tensor state, Tensor action, Tensor? nextState, Tensor reward) =>
        _memory.PushBack((state, action, nextState, reward));

    public ReadOnlyAdapter<(Tensor state, Tensor action, Tensor? nextState, Tensor reward)>
        Sample(int batchSize, IRandomGenerator random)
    {
        var batch = new (Tensor state, Tensor action, Tensor? nextState, Tensor reward)[batchSize];
        var count = _memory.Count;
        for (var i = 0; i < count && i < batchSize; i++)
        {
            var j = random.Random(i, count);
            var selected = _memory[j];
            (_memory[i], _memory[j]) = (selected, _memory[i]);
            batch[i] = selected;
        }

        return batch;
    }
}
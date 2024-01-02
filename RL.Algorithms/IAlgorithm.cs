using RL.Environments;
using RL.MDArrays;

namespace RL.Algorithms;

public interface IAlgorithm<TObservation, TAction>
{
    static abstract string Name { get; }
    public Array1D<double> Train(IEnvironment<TObservation, TAction> environment);
}
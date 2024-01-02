using RL.Environments.Spaces;
using RL.Random;

namespace RL.Environments;

public interface IEnvironment<TObservation, TAction>
{
    IRandomGenerator Random { get; }
    Space<TAction> ActionSpace { get; }
    Space<TObservation> ObservationSpace { get; }

    static abstract string Name { get; }

    (TObservation observation, double reward, bool terminated)
        Step(TAction action);

    TObservation Reset(uint? seed = null);
}
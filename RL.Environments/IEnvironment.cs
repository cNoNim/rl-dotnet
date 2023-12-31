using RL.Environments.Spaces;
using RL.Random;

namespace RL.Environments;

public interface IEnvironment<TObservation, TAction>
{
    IRandomGenerator Generator { get; }
    Space<TAction> ActionSpace { get; }
    Space<TObservation> ObservationSpace { get; }

    (TObservation observation, double reward, bool terminated)
        Step(TAction action);

    TObservation Reset(uint? seed = null);
}

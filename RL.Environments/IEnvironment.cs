using RL.Core.Spaces;
using RL.Random;

namespace RL.Core;

public interface IEnvironment<TObservation, TAction>
{
    IRandomGenerator Generator { get; }
    Space<TAction> ActionSpace { get; }
    Space<TObservation> ObservationSpace { get; }

    (TObservation observation, double reward, bool terminated, bool truncated)
        Step(TAction action);

    TObservation Reset(uint? seed = null);
}

using RL.Environments.Spaces;
using RL.Random;

namespace RL.Environments;

public interface IEnvironment<out TOSpace, out TASpace, TO, in TA>
    where TOSpace : ISpace<TO>
    where TASpace : ISpace<TA>
{
    string Name { get; }

    IRandomGenerator Random { get; }
    TOSpace ObservationSpace { get; }
    TASpace ActionSpace { get; }


    (TO observation, double reward, bool terminated)
        Step(TA action);

    TO Reset(uint? seed = null, object? options = null);
}
using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;
using RL.Random;
using static System.Math;
using static RL.Generators.Generator;

namespace RL.Toy;

public class CliffWalkingEnvironment : EnvironmentBase<int, int>, IEnvironment<int, int>
{
    private const int Up = 0;
    private const int Right = 1;
    private const int Down = 2;
    private const int Left = 3;
    internal readonly Array2D<bool> Cliff;
    internal readonly List<(double probability, int state, int reward, bool terminated)>[,] P;

    internal readonly (int x, int y) Shape = (4, 12);
    internal readonly int StartState;
    internal readonly int TerminateState;

    public CliffWalkingEnvironment()
    {
        StartState = (3, 0).Flatten(Shape);
        TerminateState = (^1, ^1).Flatten(Shape);
        var observationSpace = new Discrete(Shape.Flatten());
        var actionSpace = new Discrete(4);
        ObservationSpace = observationSpace;
        ActionSpace = actionSpace;
        var cliff = Shape.Zeroes<bool>();
        cliff[3][1..^1].Fill(true);
        Cliff = cliff;

        P = new List<(double probability, int state, int reward, bool terminated)>[observationSpace.Size,
            actionSpace.Size];

        foreach (var s in Range<int>(observationSpace.Size))
        {
            var position = s.Multi(Shape);
            P[s, Up] = CalculateTransitionProbability(position, (-1, 0));
            P[s, Right] = CalculateTransitionProbability(position, (0, 1));
            P[s, Down] = CalculateTransitionProbability(position, (1, 0));
            P[s, Left] = CalculateTransitionProbability(position, (0, -1));
        }
    }

    internal int State { get; private set; }

    public override Space<int> ActionSpace { get; }

    public override Space<int> ObservationSpace { get; }

    public override (int observation, double reward, bool terminated) Step(int action)
    {
        var transitions = P[State, action].AsGenerator();
        var index = transitions.Select(tuple => tuple.probability).ChoiceIndex(Random);
        (_, State, var reward, var terminated) = transitions[index];
        return (State, reward, terminated);
    }

    public static string Name => "Cliff-Walking";

    protected override int DoReset() => State = StartState;

    private List<(double probability, int state, int reward, bool terminated)> CalculateTransitionProbability(
        (int x, int y) position,
        (int x, int y) delta
    )
    {
        var (x, y) = position;
        var (xDelta, yDelta) = delta;
        var newPosition = LimitPosition((x + xDelta, y + yDelta));
        var newState = newPosition.Flatten(Shape);
        return Cliff[newPosition]
            ? [(1.0, StartState, -100, false)]
            : [(1.0, newState, -1, newState == TerminateState)];
    }

    private (int x, int y) LimitPosition((int x, int y) position) =>
        (Clamp(position.x, 0, Shape.x - 1), Clamp(position.y, 0, Shape.y - 1));
}
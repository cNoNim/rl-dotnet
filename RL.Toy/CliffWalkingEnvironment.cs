using System.Collections.Generic;
using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;
using RL.Random;
using static System.Math;
using static RL.Generators.Generator;

namespace RL.Toy;

public class CliffWalkingEnvironment : EnvironmentBase<Discrete, Discrete, int, int>
{
    private const int Up = 0;
    private const int Right = 1;
    private const int Down = 2;
    private const int Left = 3;
    private readonly Array2D<int> _cliff;
    private readonly List<(double probability, int state, int reward, bool terminated)>[,] _p;

    private readonly (int x, int y) _shape = (4, 12);
    private readonly int _startState;
    private readonly int _terminateState;

    public CliffWalkingEnvironment() : base("Cliff-Walking")
    {
        _startState = (3, 0).Flatten(_shape);
        _terminateState = (^1, ^1).Flatten(_shape);
        var observationSpace = new Discrete(_shape.Flatten());
        var actionSpace = new Discrete(4);
        ObservationSpace = observationSpace;
        ActionSpace = actionSpace;
        var cliff = _shape.Zeroes<int>();
        cliff[3][1..^1].Fill(1);
        _cliff = cliff;

        _p = new List<(double probability, int state, int reward, bool terminated)>[observationSpace.Size,
            actionSpace.Size];

        foreach (var s in Range<int>(observationSpace.Size))
        {
            var position = s.Multi(_shape);
            _p[s, Up] = CalculateTransitionProbability(position, (-1, 0));
            _p[s, Right] = CalculateTransitionProbability(position, (0, 1));
            _p[s, Down] = CalculateTransitionProbability(position, (1, 0));
            _p[s, Left] = CalculateTransitionProbability(position, (0, -1));
        }
    }

    public override Discrete ActionSpace { get; }
    public override Discrete ObservationSpace { get; }

    protected override (int nextState, float reward, bool terminated) DoStep(int state, int action)
    {
        var transitions = _p[state, action].AsGenerator();
        var index = transitions.Select(tuple => tuple.probability).ChoiceIndex(Random);
        var (_, nextState, reward, terminated) = transitions[index];
        return (nextState, reward, terminated);
    }

    protected override int DoReset() => _startState;

    private List<(double probability, int state, int reward, bool terminated)> CalculateTransitionProbability(
        (int x, int y) position,
        (int x, int y) delta
    )
    {
        var (x, y) = position;
        var (xDelta, yDelta) = delta;
        var newPosition = LimitPosition((x + xDelta, y + yDelta));
        var newState = newPosition.Flatten(_shape);
        return _cliff[newPosition] != 0
            ? [(1.0, _startState, -100, false)]
            : [(1.0, newState, -1, newState == _terminateState)];
    }

    private (int x, int y) LimitPosition((int x, int y) position) =>
        (Clamp(position.x, 0, _shape.x - 1), Clamp(position.y, 0, _shape.y - 1));
}
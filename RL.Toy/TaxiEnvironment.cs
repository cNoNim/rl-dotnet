using RL.Core;
using RL.Environments;
using RL.Environments.Spaces;
using RL.Random;
using static System.Math;

namespace RL.Toy;

public class TaxiEnvironment : Environment<int, int>
{
    private const int StateCount = 500;
    private const int ActionCount = 6;

    private static readonly string[] Map =
    [
        "+---------+",
        "|R: | : :G|",
        "| : | : : |",
        "| : : : : |",
        "| | : | : |",
        "|Y| : |B: |",
        "+---------+"
    ];

    private static readonly (int, int)[] Locations = [(0, 0), (0, 4), (4, 0), (4, 3)];


    internal readonly Matrix<List<(double probability, int state, int reward, bool terminated)>>
        P = new List<(double probability, int state, int reward, bool terminated)>[StateCount, ActionCount].AsMatrix();

    internal readonly IReadOnlyList<double> InitialDistribution;

    private int _state;

    public TaxiEnvironment()
    {
        const int rows = 5;
        const int maxRow = rows - 1;
        const int columns = 5;
        const int maxColumn = columns - 1;

        var distribution = new double[StateCount];
        var sum = 0.0;

        var locsLength = Locations.Length;
        foreach (var row in List.Range(rows))
        foreach (var column in List.Range(columns))
        foreach (var passIdx in List.Range(locsLength + 1))
        foreach (var destIdx in List.Range(locsLength))
        {
            var state = Encode(row, column, passIdx, destIdx);
            if (passIdx < locsLength && passIdx != destIdx)
            {
                distribution[state] += 1;
                sum++;
            }

            foreach (var action in List.Range(ActionCount))
            {
                var (newRow, newColumn, newPassIdx) = (row, column, passIdx);
                var reward = -1;
                var terminated = false;
                var taxiLocation = (row, column);

                switch (action)
                {
                    case 0:
                        newRow = Min(row + 1, maxRow);
                        break;
                    case 1:
                        newRow = Max(row - 1, 0);
                        break;
                    case 2 when Map[1 + row][2 * column + 2] == ':':
                        newColumn = Min(column + 1, maxColumn);
                        break;
                    case 3 when Map[1 + row][2 * column] == ':':
                        newColumn = Max(column - 1, 0);
                        break;
                    case 4:
                        if (passIdx < locsLength && taxiLocation == Locations[passIdx])
                            newPassIdx = 4;
                        else
                            reward = -10;
                        break;
                    case 5:
                        if (taxiLocation == Locations[destIdx] && passIdx == 4)
                        {
                            newPassIdx = destIdx;
                            terminated = true;
                            reward = 20;
                        }
                        else
                        {
                            var i = Array.IndexOf(Locations, taxiLocation);
                            if (i >= 0 && passIdx == 4)
                                newPassIdx = i;
                            else
                                reward = -10;
                        }

                        break;
                }

                var newState = Encode(newRow, newColumn, newPassIdx, destIdx);
                if (P[state][action] is not { } transitions)
                {
                    transitions = [];
                    P[state][action] = transitions;
                }
                transitions.Add((1.0, newState, reward, terminated));
            }
        }

        InitialDistribution = distribution.Select(sum, static (s, d) => d / s).ToArray();

        return;

        static int Encode(int row, int column, int pass, int dest)
        {
            var i = row;
            i *= 5;
            i += column;
            i *= 5;
            i += pass;
            i *= 4;
            i += dest;
            return i;
        }
    }

    public override Space<int> ActionSpace { get; } = new Discrete(ActionCount);
    public override Space<int> ObservationSpace { get; } = new Discrete(StateCount);

    public override (int observation, double reward, bool terminated) Step(int action)
    {
        var transitions = P[_state][action];
        var index = transitions.Select(tuple => tuple.probability).ChoiceIndex(Generator);
        (_, _state, var reward, var terminated) = transitions[index];
        return (_state, reward, terminated);
    }

    protected override int DoReset() =>
        _state = InitialDistribution.ChoiceIndex(Generator);
}

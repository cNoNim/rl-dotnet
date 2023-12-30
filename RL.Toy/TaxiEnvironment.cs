using RL.Core;
using RL.Environments;
using RL.Environments.Spaces;
using RL.Random;

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

    private static readonly (int, int)[] Locs = [(0, 0), (0, 4), (4, 0), (4, 3)];


    private readonly MatrixList<List<(double probability, int state, int reward, bool terminated)>>
        _p = new List<(double probability, int state, int reward, bool terminated)>[StateCount, ActionCount].AsMatrix();

    private readonly IReadOnlyList<double> _initialDistribution;

    private int _state;

    public TaxiEnvironment()
    {
        const int rows = 5;
        const int maxRow = rows - 1;
        const int columns = 5;
        const int maxColumn = columns - 1;

        var distribution = new double[StateCount];
        var locsLength = Locs.Length;
        foreach (var row in List.Range(rows))
        foreach (var column in List.Range(columns))
        foreach (var passIdx in List.Range(locsLength + 1))
        foreach (var destIdx in List.Range(locsLength))
        {
            var state = Encode(row, column, passIdx, destIdx);
            if (passIdx < locsLength && passIdx != destIdx)
                distribution[state] += 1;

            foreach (var action in List.Range(ActionCount))
            {
                var (newRow, newColumn, newPassIdx) = (row, column, passIdx);
                var reward = -1;
                var terminated = false;
                var taxiLocation = (row, column);

                switch (action)
                {
                    case 0:
                        newRow = int.Min(row + 1, maxRow);
                        break;
                    case 1:
                        newRow = int.Max(row - 1, 0);
                        break;
                    case 2 when Map[1 + row][2 * column + 2] == ':':
                        newColumn = int.Min(column + 1, maxColumn);
                        break;
                    case 3 when Map[1 + row][2 * column] == ':':
                        newColumn = int.Min(column - 1, 0);
                        break;
                    case 4:
                        if (passIdx < locsLength && taxiLocation == Locs[passIdx])
                            newPassIdx = 4;
                        else
                            reward = -10;
                        break;
                    case 5:
                        if (taxiLocation == Locs[destIdx] && passIdx == 4)
                        {
                            newPassIdx = destIdx;
                            terminated = true;
                            reward = 20;
                        }
                        else
                        {
                            var i = Array.IndexOf(Locs, taxiLocation);
                            if (i >= 0 && passIdx == 4)
                                newPassIdx = i;
                            else
                                reward = -10;
                        }

                        break;
                }

                var newState = Encode(newRow, newColumn, newPassIdx, destIdx);
                if (_p[state][action] is not { } transitions)
                {
                    transitions = [];
                    _p[state][action] = transitions;
                }
                transitions.Add((1.0, newState, reward, terminated));
            }
        }

        var sum = distribution.Sum();
        _initialDistribution = distribution.Select(d => d / sum);

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

    public override (int observation, double reward, bool terminated, bool truncated) Step(int action)
    {
        var transitions = _p[_state][action];
        var index = transitions.Select(tuple => tuple.probability).ChoiceIndex(Generator);
        (_, _state, var reward, var terminated) = transitions[index];
        return (_state, reward, terminated, false);
    }

    protected override int DoReset() =>
        _state = _initialDistribution.ChoiceIndex(Generator);
}

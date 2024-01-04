using System;
using System.Collections.Generic;
using RL.Environments;
using RL.Environments.Spaces;
using RL.MDArrays;
using RL.Random;
using static System.Math;
using static RL.Generators.Generator;

namespace RL.Toy;

public class TaxiEnvironment : EnvironmentBase<Discrete, Discrete, int, int>
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


    private readonly List<(double probability, int state, int reward, bool terminated)>[,]
        _p = new List<(double probability, int state, int reward, bool terminated)>[StateCount, ActionCount];

    internal readonly Array1D<double> InitialDistribution;

    public TaxiEnvironment() : base("Taxi")
    {
        const int rows = 5;
        const int maxRow = rows - 1;
        const int columns = 5;
        const int maxColumn = columns - 1;

        var distribution = StateCount.Zeroes<double>();
        var sum = 0.0;

        var locsLength = Locations.Length;
        foreach (var row in Range<int>(rows))
        foreach (var column in Range<int>(columns))
        foreach (var passIdx in Range<int>(locsLength + 1))
        foreach (var destIdx in Range<int>(locsLength))
        {
            var state = Encode(row, column, passIdx, destIdx);
            if (passIdx < locsLength && passIdx != destIdx)
            {
                distribution[state] += 1;
                sum++;
            }

            foreach (var action in Range<int>(ActionCount))
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
                if (_p[state, action] is not { } transitions)
                {
                    transitions = [];
                    _p[state, action] = transitions;
                }

                transitions.Add((1.0, newState, reward, terminated));
            }
        }

        InitialDistribution = distribution.Select(sum, static (s, d) => d / s).ToMDArray();

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

    public override Discrete ActionSpace { get; } = new(ActionCount);
    public override Discrete ObservationSpace { get; } = new(StateCount);

    protected override (int nextState, float reward, bool terminated) DoStep(int state, int action)
    {
        var transitions = _p[state, action].AsGenerator();
        var index = transitions.Select(tuple => tuple.probability).ChoiceIndex(Random);
        var (_, nextState, reward, terminated) = transitions[index];
        return (nextState, reward, terminated);
    }

    protected override int DoReset() =>
        InitialDistribution.ChoiceIndex<Array1D<double>, double>(Random);
}
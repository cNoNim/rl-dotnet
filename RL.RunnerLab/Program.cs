using RL.Algorithms;
using RL.Box2D;
using RL.MDArrays;
using RL.Plot;
using RL.Progress;
using static System.Console;
using static RL.Generators.Generator;

var rewards = new LunarLanderEnvironment().Heuristic(Range(500).Progress());

Plot.Create("Heuristic")
    .Signal(rewards)
    .ToPng("Images/Heuristic.png");

WriteLine($"MEAN: {rewards.Average():#0.00}");
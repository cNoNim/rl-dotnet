using System.Collections.Generic;
using System.Linq;
using RL.Algorithms;
using RL.Box2D;
using RL.Plot;
using static System.Console;

List<double> rewards = [];

var episode = 0;
var episodeReward = 0.0;

// Agent.Run(new LunarLanderEnvironment(), LunarLanderEnvironment.Heuristic, new Context<LunarLanderObservation, int>
// {
//     Step = (_, transition) =>
//     {
//         episodeReward += transition.Reward;
//         if (transition is { Truncated: false, Terminated: false })
//             return episode < 500;
//         rewards.Add(episodeReward);
//         episodeReward = 0.0;
//         episode++;
//         return true;
//     }
// });

Plot.Create("Heuristic")
    .Signal(rewards)
    .ToPng("Images/Heuristic.png");

WriteLine($"MEAN: {rewards.Average():#0.00}");

using System.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RL.Algorithms;
using RL.Box2D;
using RL.Plot;

var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;

const int count = 500;

var task = Plot.Create("Heuristic")
    .ConfigureAxis(configurator => configurator.SetPosition(AxisPosition.Bottom).SetMax(count))
    .Show(out var model)
    .ContinueWith(_ => tokenSource.Cancel());

var rewardSeries = new LineSeries();
model.Series.Add(rewardSeries);

var episode = 0;
var episodeReward = 0.0;

// Agent.Run(new LunarLanderEnvironment(), LunarLanderEnvironment.Heuristic, new Context<LunarLanderObservation, int>
// {
//     Step = (_, transition) =>
//     {
//         episodeReward += transition.Reward;
//         if (transition is { Truncated: false, Terminated: false })
//             return episode < count && !token.IsCancellationRequested;
//         lock (model.SyncRoot)
//             rewardSeries.Points.Add(new DataPoint(episode, episodeReward));
//         episodeReward = 0.0;
//         episode++;
//         model.InvalidatePlot(true);
//         return !token.IsCancellationRequested;
//     }
// });

await task;

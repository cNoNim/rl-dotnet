using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OxyPlot;
using OxyPlot.Series;
using RL.Algorithms;
using RL.CartPole;
using RL.Classic;
using RL.MDArrays;
using RL.Plot;
using static TorchSharp.torch.optim;

var env = new CartPoleAdapter(new CartPoleEnvironment());

var inputSize = env.InputSize;
const int hiddenSize = 128;
var outputSize = env.OutputSize;

var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;

var rewardsTask = Plot.Create(env.Name)
    .Show(out var rewardsModel, token: token)
    .ContinueWith(_ => tokenSource.Cancel());

var rewardsSeries = new LineSeries { Title = "Rewards" };
rewardsModel.Series.Add(rewardsSeries);

var lossTask = Plot.Create(env.Name)
    .Show(out var lossModel, token: token);

var lossSeries = new LineSeries { Title = "Loss" };
lossModel.Series.Add(lossSeries);

using var policyModel = new QNetwork(inputSize, [hiddenSize, hiddenSize], outputSize);
var location = $"Data/{env.Name}_{policyModel.GetName()}_{inputSize}x{hiddenSize}x{outputSize}.weights";
if (File.Exists(location))
{
    policyModel.load(location);
    Console.WriteLine($"{policyModel.GetName()} loaded from {location}");
}
else
{
    var replayBuffer = env.ReplayBuffer(10000, 128);
    var exploration = Exploration.Exponential(Progress.Step(env), 0.9, 0.05, 1000);
    var completion = Completion.Any([
        Completion.Progress(Progress.Episode(env), 500),
        Completion.Token(token)
    ]);

    using var targetModel = new QNetwork(inputSize, [hiddenSize, hiddenSize], outputSize);

    var dqn = new DQN<Array1D<float>>(
        env,
        replayBuffer,
        exploration,
        completion,
        prologCompletion: Completion.Progress(Progress.Step(env), 128),
        trainFrequency: 1,
        gradientCount: 1,
        tau: 0.005f
    );

    dqn.Register(
        (series: rewardsSeries, episodeReward: 0.0f, counter: 0),
        step: static (context, algorithm) =>
        {
            var transition = algorithm.Transition;
            context.episodeReward += transition.Reward;
            if (transition.Terminated || transition.Truncated)
            {
                var series = context.series;
                var model = series.PlotModel;
                lock (model.SyncRoot)
                    series.Points.Add(new DataPoint(context.counter, context.episodeReward));
                model.InvalidatePlot(true);
                context.counter++;
                context.episodeReward = 0.0f;
            }

            return context;
        }
    );

    dqn.Register(
        (series: lossSeries, episodeLosses: new List<double>(), counter: 0),
        trainStep: static (context, algorithm) =>
        {
            context.episodeLosses.Add(algorithm.Loss);
            return context;
        },
        step: static (context, algorithm) =>
        {
            var transition = algorithm.Transition;
            if (transition is { Terminated: false, Truncated: false })
                return context;
            if (context.episodeLosses.Count == 0)
                return context;
            var series = context.series;
            var model = series.PlotModel;
            lock (model.SyncRoot)
                series.Points.Add(new DataPoint(context.counter, context.episodeLosses.Average()));
            model.InvalidatePlot(true);
            context.episodeLosses.Clear();
            context.counter++;

            return context;
        }
    );

    using var optimizer = AdamW(policyModel.parameters(), lr: 1e-4, amsgrad: true);
    dqn.Learn(policyModel, targetModel, optimizer);
    policyModel.save(location);
}

await rewardsTask;

await lossTask;

// using var agent = Agent.Model(env, policyModel, "DQN Examine");
// Agent.Run(env, agent.Predict);
// builder.Signal(, c => c.SetTitle("DQN Examine"));

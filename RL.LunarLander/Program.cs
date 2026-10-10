using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RL.Algorithms;
using RL.Box2D;
using RL.Plot;
using RL.LunarLander;
using TorchSharp;
using static System.Console;

var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;

var window = new EnvironmentWindow<LunarLanderEnvironment, LunarLanderObservation, int>();

var model = Plot.Create("Lunar Lander")
    .ConfigureAxis(c => c.SetPosition(AxisPosition.Left).SetKey("I"))
    .ConfigureAxis(c => c.SetPosition(AxisPosition.Right).SetKey("L"))
    .ConfigureAxis(c => c.SetPosition(AxisPosition.Right).SetKey("E"))
    .ConfigureAxis(c => c.SetPosition(AxisPosition.Bottom).SetTitle("Episodes"))
    .Build();

window.AddSubView(new PlotView(model));

var task = Task.Run(() =>
{
    var env = new LunarLanderAdapter(new LunarLanderEnvironment());

    var inputSize = env.InputSize;
    const int hiddenSize = 256;
    var outputSize = env.OutputSize;

    var builder = Plot.Create(env.Name)
        .ConfigureAxis(c => c.SetPosition(AxisPosition.Bottom).SetTitle("Episodes"));

    var rewardsCollector = new EpisodeRewardsCollector<LunarLanderObservation, int>();
    var rewardsConsumer = new EpisodeRewardsConsumer<LunarLanderObservation, int>();
    var lengthConsumer = new EpisodeLengthConsumer<LunarLanderObservation, int>();
    var meanLossConsumer = new EpisodeMeanLossConsumer<LunarLanderObservation, int>();

    var heuristic = new Heuristic<LunarLanderObservation, int>(
        env,
        Completion.Any([
            Completion.Progress(Progress.Episode(env), 500),
            Completion.Token(token)
        ])
    );

    heuristic.Register(rewardsCollector);
    heuristic.Register(rewardsConsumer);

    try
    {
        try
        {
            var series = new LineSeries { Title = "Heuristic", YAxisKey = "I" };
            model.Series.Add(series);
            rewardsConsumer.On((episode, reward) =>
            {
                series.Points.Add(new DataPoint(episode, reward));
                model.InvalidatePlot(true);
            });
            heuristic.Run(LunarLanderEnvironment.Heuristic);
            builder.Signal(rewardsCollector.Rewards, c => c.SetTitle("Heuristic"));
            var agent = Agent.Func<LunarLanderObservation, int>(LunarLanderEnvironment.Heuristic, "Heuristic");
            window.NextAgent(agent);
            WriteLine($"MEAN Heuristic: {rewardsCollector.Rewards.Average():#0.00}");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            WriteLine(e);
        }

        using var policyModel = new QNetwork(inputSize, [hiddenSize, hiddenSize], outputSize);
        var location = $"Data/{env.Name}_{policyModel.GetName()}_{inputSize}x{hiddenSize}x{outputSize}.weights";
        if (File.Exists(location))
        {
            policyModel.load(location);
            WriteLine($"{policyModel.GetName()} loaded from {location}");

            var previousModel = new QNetwork(inputSize, [hiddenSize, hiddenSize], outputSize);
            previousModel.load_state_dict(policyModel.state_dict());
            var agent = Agent.Model(env, previousModel, "Previous");
            window.NextAgent(agent);
        }
        else
        {
            try
            {
                var targetModel = new QNetwork(inputSize, [hiddenSize, hiddenSize], outputSize);

                var replayBuffer = env.ReplayBuffer(100000, 128);
                var exploration = Exploration.Linear(Progress.Episode(env), 4000, 1.0f, 0.2f, 0.6f);
                var completion = Completion.Any([
                    Completion.Progress(Progress.Episode(env), 4000),
                    Completion.Token(token)
                ]);

                var dqn = new DQN<LunarLanderObservation>(
                    env,
                    replayBuffer,
                    exploration,
                    completion,
                    prologCompletion: Completion.Progress(Progress.Episode(env), 400),
                    trainPrologCompletion: Completion.Constant(true),
                    prologHeuristic: LunarLanderEnvironment.Heuristic,
                    gradientCount: 0,
                    targetUpdateFrequency: 1,
                    trainFrequency: 16,
                    tau: 0.1f
                );

                dqn.Register(rewardsCollector);
                dqn.Register(rewardsConsumer);
                dqn.Register(lengthConsumer);
                dqn.Register(meanLossConsumer);

                using var optimizer = torch.optim.AdamW(policyModel.parameters(), lr: 1e-5, amsgrad: true);

                var series = new LineSeries { Title = "DQN Train", YAxisKey = "I"};
                var lengthSeries = new LineSeries { Title = "DQN Length", YAxisKey = "I"};
                var lossSeries = new LineSeries { Title = "DQN Loss", YAxisKey = "L" };
                var epsilonSeries = new LineSeries { Title = "DQN Epsilon", YAxisKey = "E" };
                model.Series.Add(series);
                model.Series.Add(lengthSeries);
                model.Series.Add(lossSeries);
                model.Series.Add(epsilonSeries);
                rewardsConsumer.On((episode, reward) =>
                {
                    series.Points.Add(new DataPoint(episode, reward));
                    model.InvalidatePlot(true);
                });
                lengthConsumer.On((episode, length) =>
                {
                    lengthSeries.Points.Add(new DataPoint(episode, length));
                    model.InvalidatePlot(true);
                });
                meanLossConsumer.On((episode, loss) =>
                {
                    lossSeries.Points.Add(new DataPoint(episode, loss));
                    epsilonSeries.Points.Add(new DataPoint(episode, exploration.Epsilon));
                    model.InvalidatePlot(true);
                });
                dqn.Learn(policyModel, targetModel, optimizer);
                builder.Signal(rewardsCollector.Rewards, c => c.SetTitle("DQN"));
                policyModel.save(location);

                window.NextAgent(Agent.Model(env, targetModel));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteLine(e);
            }
        }

        {
            var series = new LineSeries
            {
                Title = "DQN Examine",
                YAxisKey = "I"
            };
            model.Series.Add(series);

            using var examineAgent = Agent.Model(env, policyModel, "DQN Examine");
            rewardsConsumer.On((episode, reward) =>
            {
                series.Points.Add(new DataPoint(episode, reward));
                model.InvalidatePlot(true);
            });
            heuristic.Run(examineAgent.Predict);
            builder.Signal(rewardsCollector.Rewards, c => c.SetTitle("DQN Examine"));
            WriteLine($"MEAN DQN: {rewardsCollector.Rewards.Average():#0.00}");
        }
    }
    finally
    {
        builder.ToPng($"Images/{env.Name}.png");
    }
}, token);

var environment = new LunarLanderEnvironment();

window.LoadEvent += OnLoad;
window.UnloadEvent += OnUnload;
window.UpdateEvent += OnUpdate;
window.Run(environment);

tokenSource.Cancel();

try
{
    await task;
}
catch (OperationCanceledException)
{
    WriteLine("Task status: {0}", task.Status);
}
finally
{
    tokenSource.Dispose();
}

return;

static void OnLoad(EnvironmentWindow<LunarLanderEnvironment, LunarLanderObservation, int> window)
{
    var drawer = new DrawPhysics(window.DebugDrawer);
    drawer.AppendFlags(DrawFlags.Shape);
    drawer.AppendFlags(DrawFlags.Pair);
    drawer.AppendFlags(DrawFlags.Joint);
    drawer.AppendFlags(DrawFlags.CenterOfMass);

    window.Environment.World.SetDebugDraw(drawer);
    window.Environment.Drawer = window.DebugDrawer;
}

static void OnUpdate(EnvironmentWindow<LunarLanderEnvironment, LunarLanderObservation, int> window)
{
    window.SetBody(window.Environment.Lander);
    window.Environment.World.DrawDebugData();
}

static void OnUnload(EnvironmentWindow<LunarLanderEnvironment, LunarLanderObservation, int> window)
{
    window.Environment.World.SetDebugDraw(null);
    window.Environment.Drawer = null;

    window.LoadEvent -= OnLoad;
    window.UnloadEvent -= OnUnload;
    window.UpdateEvent -= OnUpdate;
}

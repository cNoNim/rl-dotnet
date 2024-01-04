using System.IO;
using System.Text;
using System.Threading;
using Box2D.NetStandard.Dynamics.World.Callbacks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RL.Algorithms;
using RL.Box2D;
using RL.MDArrays;
using RL.Plot;
using RL.Progress;
using RL.Runner;
using static RL.Generators.Generator;
using static TorchSharp.torch;

const int count = 500;

var environment = new LunarLanderEnvironment();

var inputSize = environment.ObservationSpace.High.Count;
const int hiddenSize = 128;
var outputSize = environment.ActionSpace.Size;

var window = environment.Window();

var device = CPU;

var trainThread = new Thread(() =>
{
    var env = new LunarLanderEnvironment();

    var builder = Plot.Create(env.Name)
        .ConfigureYAxis(c => c.SetTitle("Episodes"));

    var episodeGenerator = Range(count);

    builder.Signal(env.Heuristic(episodeGenerator.Progress("Heuristic")), c => c.SetTitle("Heuristic"));

    window.AddAgent(Keys.D1, Agent.Func<Array1D<float>, int>(environment.Heuristic));

    using var crossEntropyModel = new CrossEntropyModel(inputSize, outputSize, hiddenSize, device: device);
    var crossEntropyLocation =
        $"Data/{env.Name}_{crossEntropyModel.GetName()}_{inputSize}x{hiddenSize}x{outputSize}.weights";
    if (File.Exists(crossEntropyLocation))
        crossEntropyModel.load(crossEntropyLocation);
    else
    {
        var crossEntropy = new CrossEntropy(20, 1000);
        var (rewards, steps) =
            crossEntropy.Train(episodeGenerator.Progress("Cross Entropy"), crossEntropyModel, env, device);
        builder.Signal(rewards, c => c.SetTitle($"Cross Entropy {steps}"));
        crossEntropyModel.save(crossEntropyLocation);
    }

    window.AddAgent(Keys.D2, Agent.Model(crossEntropyModel));

    using var dqnModel = new DQNModel(inputSize, outputSize, hiddenSize);
    var dqnLocation = $"Data/{env.Name}_{dqnModel.GetName()}_{inputSize}x{hiddenSize}x{outputSize}.weights";
    if (File.Exists(dqnLocation))
        dqnModel.load(dqnLocation);
    else
    {
        using var policyModel = new DQNModel(inputSize, outputSize, hiddenSize);

        var dqn = new DQN();
        var (rewards, steps) = dqn.Train(episodeGenerator.Progress("DQN"), dqnModel, policyModel, env);
        builder.Signal(rewards, c => c.SetTitle($"DQN {steps}"));
        dqnModel.save(dqnLocation);
    }


    window.AddAgent(Keys.D3, Agent.Model(dqnModel));

    builder.ToPng($"Images/{env.Name}.png");
});

trainThread.Start();

window.AddAgent(Keys.D0, Agent.Func<Array1D<float>, int>(UserControl));

window.LoadEvent += OnLoad;
window.UpdateEvent += OnUpdate;
window.CompleteEvent += OnComplete;
window.UnloadEvent += OnUnload;
window.Run();

return;

int UserControl(Array1D<float> _)
{
    if (window.Window.IsKeyDown(Keys.Up))
        return 2;
    if (window.Window.IsKeyDown(Keys.Left))
        return 1;
    if (window.Window.IsKeyDown(Keys.Right))
        return 3;
    return 0;
}

static void OnLoad(EnvironmentWindow<LunarLanderEnvironment, Array1D<float>, int> window)
{
    var drawer = new DrawPhysics(window.Window);
    drawer.AppendFlags(DrawFlags.Shape);
    drawer.AppendFlags(DrawFlags.Pair);
    drawer.AppendFlags(DrawFlags.Joint);
    drawer.AppendFlags(DrawFlags.CenterOfMass);

    window.Environment.World.SetDebugDraw(drawer);
    window.Environment.Drawer = window.Window;
}

static void OnUpdate(EnvironmentWindow<LunarLanderEnvironment, Array1D<float>, int> window)
{
    window.Window.SetBody(window.Environment.Lander);
    window.Environment.World.DrawDebugData();
}

static void OnComplete(EnvironmentWindow<LunarLanderEnvironment, Array1D<float>, int> window) =>
    PrintInfo(window, window.Episode, window.TotalReward);

static void OnUnload(EnvironmentWindow<LunarLanderEnvironment, Array1D<float>, int> window)
{
    window.Environment.World.SetDebugDraw(null);
    window.Environment.Drawer = null;

    window.LoadEvent -= OnLoad;
    window.UpdateEvent -= OnUpdate;
    window.CompleteEvent -= OnComplete;
    window.UnloadEvent -= OnUnload;
}

static void PrintInfo(EnvironmentWindow<LunarLanderEnvironment, Array1D<float>, int> window, int episode,
    float totalReward)
{
    var sb = new StringBuilder();
    sb.Append(
        $"{window.Environment.Name} episode: {episode} steps: {window.Environment.Steps} reward: {totalReward:#0.00}");
    window.Window.Title = sb.ToString();
}

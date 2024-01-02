using RL.Algorithms;
using RL.Environments;
using RL.Plot;
using RL.Toy;

const int episodeCount = 500;
const int stepCount = 1000;

var monteCarlo = new MonteCarlo(20000, 1000);
var qLearning = new QLearning(episodeCount, stepCount, gamma: 0.999);
var sarsa = new Sarsa(episodeCount, stepCount, gamma: 0.999);

TrainEnvironment(new CliffWalkingEnvironment());
TrainEnvironment(new TaxiEnvironment());

return;

void TrainEnvironment<TEnvironment>(TEnvironment environment)
    where TEnvironment : IEnvironment<int, int>
{
    Train(monteCarlo, environment);
    Train(qLearning, environment);
    Train(sarsa, environment);
}

static void Train<TAlgorithm, TEnvironment>(TAlgorithm algorithm, TEnvironment environment)
    where TAlgorithm : IAlgorithm<int, int>
    where TEnvironment : IEnvironment<int, int>
{
    var rewards = algorithm.Train(environment);

    Plot.Create($"{TAlgorithm.Name} of {TEnvironment.Name}")
        .ConfigureXAxis(c => c.SetTitle("Reward"))
        .ConfigureYAxis(c => c.SetTitle("Episodes"))
        .Signal(rewards)
        .ToPng($"Images/{TEnvironment.Name}_{TAlgorithm.Name}_{episodeCount}_{stepCount}.png");
}
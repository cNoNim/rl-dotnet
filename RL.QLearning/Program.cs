using RL.Environments;
using RL.MDArrays;
using RL.Plot;
using RL.Toy;

const int episodeCount = 500;
const int noisyEpisodeCount = 400;
const int stepCount = 1000;

var environment = new TaxiEnvironment();

var rewards = QLearning(environment, episodeCount, noisyEpisodeCount, stepCount, gamma: 0.999);

Plot.Create("Q-Learning")
    .ConfigureXAxis(c => c.SetTitle("Reward"))
    .ConfigureYAxis(c => c.SetTitle("Episodes"))
    .Signal(rewards)
    .ToPng($"Images/QLearning_{episodeCount}_{stepCount}.png");

return;

static Array1D<double> QLearning(
    IEnvironment<int, int> environment,
    int episodeCount,
    int noisyEpisodeCount,
    int stepCount = 500,
    double gamma = 0.99,
    double alpha = 0.5
)
{

}

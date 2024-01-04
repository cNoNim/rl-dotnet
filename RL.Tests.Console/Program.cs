using RL.Plot;
using static RL.Generators.Generator;

const int episodeCount = 1000;
const int noisyEpisodeCount = 800;

var epsilon1 = Range<int>(episodeCount).Select(i => 1.0 - i / (double)episodeCount);
var epsilon2 = Range<int>(episodeCount).Select(i => 1.0 / (i + 1));
var epsilon = 1.0;
var epsilon3 = Range<int>(episodeCount).Select(_ =>
{
    epsilon = Math.Max(0.0, epsilon - 1.0 / noisyEpisodeCount);
    return epsilon;
});

Plot.Create("Epsilon")
    .Signal(epsilon1, c => c.SetTitle("Monte Carlo"))
    .Signal(epsilon2, c => c.SetTitle("Sarsa"))
    .Signal(epsilon3, c => c.SetTitle("Q-Learning"))
    .ToPng("Epsilon.png");

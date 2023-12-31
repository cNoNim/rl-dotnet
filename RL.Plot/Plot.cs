namespace RL.Plot;

public static class Plot
{
    public static PlotBuilder Create(string title) =>
        new(title);
}

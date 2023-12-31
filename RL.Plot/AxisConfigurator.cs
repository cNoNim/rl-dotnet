namespace RL.Plot;

public class AxisConfigurator
{
    public string? Title { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }

    public AxisConfigurator SetTitle(string title)
    {
        Title = title;
        return this;
    }

    public AxisConfigurator SetMin(double min)
    {
        Min = min;
        return this;
    }

    public AxisConfigurator SetMax(double max)
    {
        Max = max;
        return this;
    }
}

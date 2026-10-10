using OxyPlot.Axes;

namespace RL.Plot;

public struct AxisConfigurator
{
    public AxisPosition? Position { get; set; }
    public string? Title { get; set; }
    public string? Key { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }

    public AxisConfigurator SetPosition(AxisPosition position)
    {
        Position = position;
        return this;
    }

    public AxisConfigurator SetTitle(string title)
    {
        Title = title;
        return this;
    }

    public AxisConfigurator SetKey(string key)
    {
        Key = key;
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

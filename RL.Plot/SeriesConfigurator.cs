namespace RL.Plot;

public class SeriesConfigurator
{
    public string? Color { get; set; }

    public SeriesConfigurator SetColor(string color)
    {
        Color = color;
        return this;
    }
}

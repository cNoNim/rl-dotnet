namespace RL.Plot;

public class SeriesConfigurator
{
    public string? Title { get; set; }
    public string? Color { get; set; }

    public SeriesConfigurator SetTitle(string title)
    {
        Title = title;
        return this;
    }

    public SeriesConfigurator SetColor(string color)
    {
        Color = color;
        return this;
    }
}
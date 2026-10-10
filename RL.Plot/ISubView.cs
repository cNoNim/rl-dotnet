using Silk.NET.Maths;

namespace RL.Plot;

public interface ISubView
{
    public void OnLoad();
    public void OnClosing();
    public void OnFramebufferResize(Vector2D<int> framebufferSize);
    public void OnUpdate();
    public void OnRender();
}

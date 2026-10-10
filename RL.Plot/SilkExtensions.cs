using OxyPlot;
using Silk.NET.GLFW;
using MouseButton = Silk.NET.Input.MouseButton;

namespace RL.Plot;

public static class SilkExtensions
{
    public static OxyModifierKeys ToOxy(this KeyModifiers modifiers)
    {
        var result = OxyModifierKeys.None;
        if ((modifiers & KeyModifiers.Shift) != 0)
            result |= OxyModifierKeys.Shift;
        if ((modifiers & KeyModifiers.Control) != 0)
            result |= OxyModifierKeys.Control;
        if ((modifiers & KeyModifiers.Alt) != 0)
            result |= OxyModifierKeys.Alt;
        return result;
    }

    public static OxyMouseButton ToOxy(this MouseButton button) =>
        button switch
        {
            MouseButton.Left => OxyMouseButton.Left,
            MouseButton.Right => OxyMouseButton.Right,
            MouseButton.Middle => OxyMouseButton.Middle,
            MouseButton.Button4 => OxyMouseButton.XButton1,
            MouseButton.Button5 => OxyMouseButton.XButton2,
            MouseButton.Button6 => OxyMouseButton.None,
            MouseButton.Button7 => OxyMouseButton.None,
            MouseButton.Button8 => OxyMouseButton.None,
            _ => OxyMouseButton.None
        };
}

using OxyPlot;
using OxyPlot.SkiaSharp;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SkiaSharp;

namespace RL.Plot;

public class PlotView : IPlotView, ISubView
{
    private readonly object _invalidateLock = new();

    private bool _modelInvalidated;
    private bool _updateData;

    private GRGlInterface? _interface;
    private GRContext? _context;
    private SKPaint? _paint;
    private GRBackendRenderTarget? _renderTarget;
    private SKSurface? _surface;
    private SkiaRenderContext? _renderContext;

    public PlotModel ActualModel { get; }
    public IController ActualController { get; }
    public OxyRect ClientArea { get; private set; }

    public PlotView(PlotModel model)
    {
        ActualModel = model;
        ActualController = new PlotController();
        ((IPlotModel)model).AttachPlotView(this);
    }

    public void OnLoad()
    {
        _interface = GRGlInterface.Create();
        _context = GRContext.CreateGl(_interface);
        _paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            TextAlign = SKTextAlign.Center
        };
    }

    public void OnClosing()
    {
        _renderContext?.Dispose();
        _renderContext = null;
        _surface?.Dispose();
        _surface = null;
        _renderTarget?.Dispose();
        _renderTarget = null;
        _paint?.Dispose();
        _paint = null;
        _context?.Dispose();
        _context = null;
        _interface?.Dispose();
        _interface = null;
    }

    public void OnFramebufferResize(Vector2D<int> framebufferSize)
    {
        ClientArea = new OxyRect(0, 0, framebufferSize.X, framebufferSize.Y);

        _renderTarget?.Dispose();
        _renderTarget = new GRBackendRenderTarget(
            framebufferSize.X,
            framebufferSize.Y,
            0,
            8,
            new GRGlFramebufferInfo(0, (uint)SizedInternalFormat.Rgba8)
        );
        _surface?.Dispose();
        _surface = SKSurface.Create(_context, _renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
        _renderContext?.Dispose();
        _renderContext = new SkiaRenderContext
        {
            SkCanvas = _surface.Canvas,
            RenderTarget = RenderTarget.Screen,
            UseTextShaping = true
        };
        InvalidatePlot(false);
    }

    public void OnUpdate()
    {
        lock (_invalidateLock)
        {
            if (!_modelInvalidated)
                return;
            ((IPlotModel)ActualModel).Update(_updateData);
            _updateData = false;
            _modelInvalidated = false;
        }
    }

    public void OnRender()
    {
        if (_surface == null || _context == null)
            return;

        _context.ResetContext(GRGlBackendState.All);

        var canvas = _surface.Canvas;
        var background = ActualModel.Background;
        if (!background.IsUndefined())
            canvas.Clear(background.ToSKColor());

        ((IPlotModel)ActualModel).Render(_renderContext, ClientArea);

        canvas.Flush();
    }

    public void InvalidatePlot(bool updateData = true)
    {
        lock (_invalidateLock)
        {
            _modelInvalidated = true;
            _updateData |= updateData;
        }
    }

    public void SetCursorType(CursorType cursorType)
    {
    }

    public void ShowZoomRectangle(OxyRect rectangle)
    {
    }

    public void HideZoomRectangle()
    {
    }

    public void ShowTracker(TrackerHitResult trackerHitResult)
    {
    }

    public void HideTracker()
    {
    }

    public void SetClipboardText(string text)
    {
    }

    Model IView.ActualModel => ActualModel;
}

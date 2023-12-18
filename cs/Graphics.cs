using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;

namespace cs;

internal sealed class Graphics : IDisposable
{
    private readonly WindowOptions _options;
    private readonly IWindow _window;
    private readonly GL _gl;

    public Graphics(int width, int height)
    {
        _options = WindowOptions.Default with
        {
            Size = new(width, height)
        };
        _window = Window.Create(_options);
        _gl = _window.CreateOpenGL();
    }

    public void DrawLoop(Action<double> update, Func<double, Bitmap> render, Action load = null)
    {
        if (load is not null)
            _window.Load += () =>
            {
                _gl.ClearColor(Color.Black);
                load();
            };

        _window.Render += (d) => RenderBitmap(render(d));
        _window.Update += update;

        _window.Run();
    }

    private void RenderBitmap(Bitmap bitmap)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);

    }

    public void Close() => _window.Close();

    public void Dispose()
    {
        _gl?.Dispose();
        _window.Dispose();
    }
}

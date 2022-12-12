using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Assignment8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 800),
                Title = "Title",
                NumberOfSamples = 8,
                WindowState = WindowState.Fullscreen,
            };
            var gameWindowSettings = new GameWindowSettings()
            {
                RenderFrequency = 100,
                UpdateFrequency = 100,
            };
            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}
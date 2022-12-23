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
                WindowState = WindowState.Maximized,
            };
            var gameWindowSettings = new GameWindowSettings()
            {
                RenderFrequency = 30,
                UpdateFrequency = 20,
            };
            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                try
                {
                    window.Run();
                }
                catch (Exception e)
                {
                    File.WriteAllText($"crash_{DateTime.Now.ToString("HH-mm-ss_dd-MM-yyyy")}.txt", e.ToString());
                }
            }
        }
    }
}
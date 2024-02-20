using Avalonia;
using System;
using System.Linq;

namespace ClockReadabilityTest;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => AppBuilder
        .Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace()
        .StartWithClassicDesktopLifetime(args);
}
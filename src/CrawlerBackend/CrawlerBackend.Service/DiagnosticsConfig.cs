using System.Diagnostics;
using System.Reflection;
using OpenTelemetry.Trace;

namespace CrawlerBackend.Service;

public static class DiagnosticsConfig
{
    private static readonly AssemblyName AssemblyName = typeof(DiagnosticsConfig).Assembly.GetName();

    public static readonly ActivitySource Source =
        new ActivitySource(AssemblyName.ToString(), AssemblyName.Version?.ToString());

    public static void AddCrawlerSource(this TracerProviderBuilder provider)
    {
        provider.AddSource(AssemblyName.ToString());
    }
}
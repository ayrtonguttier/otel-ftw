using CrawlerBackend.Service;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = Host.CreateApplicationBuilder(args);

var telemetryLoggingHost = builder.Configuration.GetValue<string>("Telemetry:Logging:Host") ??
                           throw new Exception("Host configuration must be set");

var telemetryTracesHost = builder.Configuration.GetValue<string>("Telemetry:Traces:Host") ??
                          throw new Exception("Host configuration must be set");

var telemetryMetricsHost = builder.Configuration.GetValue<string>("Telemetry:Metrics:Host") ??
                           throw new Exception("Host configuration must be set");


builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(builder.Environment.ApplicationName)
            .AddAttributes(new Dictionary<string, object>
            {
                ["environment"] = builder.Environment.EnvironmentName
            }));

        metrics.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(telemetryMetricsHost);
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
        });

        metrics.AddRuntimeInstrumentation();
        metrics.AddHttpClientInstrumentation();

        if (builder.Environment.IsDevelopment())
        {
            metrics.AddConsoleExporter();
        }
    })
    .WithTracing(tracing =>
    {
        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(builder.Environment.ApplicationName)
            .AddAttributes(new Dictionary<string, object>
            {
                ["environment"] = builder.Environment.EnvironmentName
            }));
        tracing.AddCrawlerSource();
        tracing.AddHttpClientInstrumentation();
        tracing.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(telemetryTracesHost);
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
        });        
    })
    .WithLogging(logging =>
    {
        logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(builder.Environment.ApplicationName)
            .AddAttributes(new Dictionary<string, object>
            {
                ["environment"] = builder.Environment.EnvironmentName
            }));

        logging.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(telemetryLoggingHost);
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
        });        
    });

builder.Services.AddHttpClient("CrawlerClient", config =>
{
    config.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Crawler:Uri") ?? throw new Exception("need to set crawler uri"));
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
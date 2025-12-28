using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var telemetryLoggingHost = builder.Configuration.GetValue<string>("Telemetry:Logging:Host") ??
                           throw new Exception("Host configuration must be set");

var telemetryTracesHost = builder.Configuration.GetValue<string>("Telemetry:Traces:Host") ??
                          throw new Exception("Host configuration must be set");

var telemetryMetricsHost = builder.Configuration.GetValue<string>("Telemetry:Metrics:Host") ??
                           throw new Exception("Host configuration must be set");

var otel = builder.Services.AddOpenTelemetry();

otel.WithMetrics(metrics =>
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
    metrics.AddAspNetCoreInstrumentation();
});

otel.WithTracing(tracing =>
{
    tracing.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(builder.Environment.ApplicationName)
        .AddAttributes(new Dictionary<string, object>
        {
            ["environment"] = builder.Environment.EnvironmentName
        }));

    tracing.AddAspNetCoreInstrumentation();
    tracing.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri(telemetryTracesHost);
        options.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
});

otel.WithLogging(logging =>
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

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
});

app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
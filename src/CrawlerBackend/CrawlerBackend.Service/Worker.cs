using System.Diagnostics;

namespace CrawlerBackend.Service;

public class Worker(ILogger<Worker> logger, IHttpClientFactory factory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            using var act = DiagnosticsConfig.Source.StartActivity("crawling");
            try
            {
                using var client = factory.CreateClient("CrawlerClient");
                var result = await client.GetAsync("", stoppingToken);
                result.EnsureSuccessStatusCode();

                act?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception e)
            {
                act?.SetStatus(ActivityStatusCode.Error);
                logger.LogError(e, "Error while crawling");
            }
        } while (!stoppingToken.IsCancellationRequested);
    }
}
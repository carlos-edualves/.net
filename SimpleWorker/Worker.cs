using System;
using System.Text.Json;
using Datadog.Trace;

namespace SimpleWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Tracer.Instance.Settings: {Settings}", JsonSerializer.Serialize(Tracer.Instance.Settings));
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = Tracer.Instance.StartActive("worker.execution"))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                scope.Span.SetTag("custom.tag", "worker.execution");
                
                try
                {
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    scope.Span.SetException(ex);
                    throw;
                }
            }
        }
    }
}


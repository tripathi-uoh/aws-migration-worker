namespace AwsMigrationWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AWS Migration Worker started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Processing migration batch at: {time}", DateTimeOffset.Now);
            }

            // Simulate batch processing interval
            await Task.Delay(10000, stoppingToken);
        }

        _logger.LogInformation("AWS Migration Worker stopping at: {time}", DateTimeOffset.Now);
    }
}

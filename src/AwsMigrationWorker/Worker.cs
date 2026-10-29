using Amazon.SQS;
using Amazon.SQS.Model;
using AwsMigrationWorker.Models;
using AwsMigrationWorker.Services;
using System.Text.Json;

namespace AwsMigrationWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IAmazonSQS _sqsClient;
    private readonly IMigrationProcessor _processor;

    public Worker(
        ILogger<Worker> logger,
        IConfiguration configuration,
        IAmazonSQS sqsClient,
        IMigrationProcessor processor)
    {
        _logger = logger;
        _configuration = configuration;
        _sqsClient = sqsClient;
        _processor = processor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = _configuration["AWS:QueueUrl"];
        _logger.LogInformation("AWS Migration Worker initialized. Target SQS Queue: {QueueUrl}", queueUrl ?? "Not configured (running in mock mode)");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(queueUrl))
                {
                    // Fallback local simulation mode when AWS credentials/queue are not set
                    var mockTask = new MigrationTask(
                        Guid.NewGuid().ToString("N")[..8],
                        "LegacyCaseSystem",
                        $"DOC-{Random.Shared.Next(1000, 9999)}",
                        "case-migration-archive-bucket",
                        DateTime.UtcNow
                    );

                    await _processor.ProcessAsync(mockTask, stoppingToken);
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 5,
                    WaitTimeSeconds = 20, // Long polling
                    VisibilityTimeout = 60
                };

                var response = await _sqsClient.ReceiveMessageAsync(receiveRequest, stoppingToken);

                if (response.Messages == null || response.Messages.Count == 0)
                {
                    continue;
                }

                foreach (var message in response.Messages)
                {
                    var task = JsonSerializer.Deserialize<MigrationTask>(message.Body);
                    if (task != null)
                    {
                        var result = await _processor.ProcessAsync(task, stoppingToken);
                        if (result.Success)
                        {
                            await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, stoppingToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in processing loop");
                await Task.Delay(3000, stoppingToken);
            }
        }

        _logger.LogInformation("AWS Migration Worker stopped cleanly at {Time}", DateTimeOffset.Now);
    }
}

using AwsMigrationWorker.Models;

namespace AwsMigrationWorker.Services;

public interface IMigrationProcessor
{
    Task<MigrationResult> ProcessAsync(MigrationTask task, CancellationToken cancellationToken);
}

public class MigrationProcessor : IMigrationProcessor
{
    private readonly ILogger<MigrationProcessor> _logger;

    public MigrationProcessor(ILogger<MigrationProcessor> logger)
    {
        _logger = logger;
    }

    public async Task<MigrationResult> ProcessAsync(MigrationTask task, CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("Starting migration for Task {TaskId} from {SourceSystem}...", task.TaskId, task.SourceSystem);

        try
        {
            // Simulate migration processing, validation, and transformation steps
            await Task.Delay(500, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("Successfully migrated Task {TaskId} for document {DocumentId} in {ElapsedMs}ms",
                task.TaskId, task.DocumentId, stopwatch.ElapsedMilliseconds);

            return new MigrationResult(task.TaskId, true, 1, null, stopwatch.Elapsed);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Failed migration for Task {TaskId}", task.TaskId);
            return new MigrationResult(task.TaskId, false, 0, ex.Message, stopwatch.Elapsed);
        }
    }
}

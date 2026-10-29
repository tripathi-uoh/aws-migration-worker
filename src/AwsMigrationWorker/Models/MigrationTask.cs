namespace AwsMigrationWorker.Models;

public record MigrationTask(
    string TaskId,
    string SourceSystem,
    string DocumentId,
    string DestinationBucket,
    DateTime CreatedAtUtc
);

public record MigrationResult(
    string TaskId,
    bool Success,
    int RecordsProcessed,
    string? ErrorMessage,
    TimeSpan Elapsed
);

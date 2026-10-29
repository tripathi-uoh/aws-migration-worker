# AWS Migration Worker

A production-ready **.NET 8 Worker Service** designed to process enterprise data migration jobs asynchronously from an Amazon SQS queue.

---

## Architecture Overview

```
[Legacy Source DB / API]
           │
           ▼
     [Amazon SQS]  <── Long polling (WaitTimeSeconds = 20)
           │
           ▼
  [AwsMigrationWorker (.NET 8)]
     ├── IMigrationProcessor (Validation, Transform, Ingest)
     ├── AWS SDK for .NET (Amazon.SQS)
     └── Error Handling & Receipt Handle Deletion
           │
           ▼
 [Target Store / S3 / SQS DLQ]
```

---

## Features

- **Long Polling SQS Consumer**: Reduces empty responses and AWS API request costs using SQS 20-second wait times.
- **Graceful Shutdown**: Honors `CancellationToken` during pod eviction or service stop.
- **Local Fallback Mode**: When `AWS:QueueUrl` is blank, the worker runs an internal generator to simulate ingestion without live AWS credentials.
- **Dockerized**: Includes multi-stage Docker build ready for ECS Fargate or Kubernetes (EKS).

---

## Getting Started

### Local Execution

```bash
cd src/AwsMigrationWorker
dotnet run
```

### Docker Container

```bash
docker build -t aws-migration-worker .
docker run --rm aws-migration-worker
```

using Amazon.SQS;
using AwsMigrationWorker;
using AwsMigrationWorker.Services;

var builder = Host.CreateApplicationBuilder(args);

// Register AWS SDK Defaults & Services
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSQS>();

// Register Domain Services
builder.Services.AddSingleton<IMigrationProcessor, MigrationProcessor>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();

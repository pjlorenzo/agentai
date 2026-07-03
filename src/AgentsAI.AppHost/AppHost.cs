using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

// Bypass SSL certificate validation for AppHost's internal HttpClient calls (like WithHttpHealthCheck)
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });
});

var acr = builder.AddAzureContainerRegistry("agentaiacr");

var aca = builder.AddAzureContainerAppEnvironment("aca-env")
    .WithAzdResourceNaming()
    .WithContainerRegistry(acr);

var apiService = builder.AddProject<Projects.AgentsAI_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithContainerRegistry(acr);

builder.AddProject<Projects.AgentsAI_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithContainerRegistry(acr);

builder.Build().Run();

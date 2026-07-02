var builder = DistributedApplication.CreateBuilder(args);

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

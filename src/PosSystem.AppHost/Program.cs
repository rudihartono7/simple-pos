var builder = DistributedApplication.CreateBuilder([
    "ASPIRE_ALLOW_UNSECURED_TRANSPORT=true",
    "DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true",
    .. args]);

// Add the backend project
var backend = builder.AddProject<Projects.PosSystem>("possystem");

// Add the Nuxt.js frontend with environment variable for API base URL
var frontendDir = "../PosSystem/client-app";
builder.AddExecutable("frontend-install", "bun", frontendDir, "i");
var frontend = builder.AddExecutable("frontend", "bun", frontendDir, "run", "dev");
    //.WithEnvironment("NUXT_PUBLIC_API_BASE", backend.GetEndpoint("https"));

builder.Build().Run();

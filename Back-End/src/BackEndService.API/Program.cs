using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.RateLimiting;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext()
      .WriteTo.Console()
      .WriteTo.File(path: "logs/app.ndjson", rollingInterval: RollingInterval.Day, formatter: new Serilog.Formatting.Compact.CompactJsonFormatter());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    var xml = System.IO.Path.ChangeExtension(typeof(Program).Assembly.Location, "xml");
    if (System.IO.File.Exists(xml)) c.IncludeXmlComments(xml);
    c.ExampleFilters();
    
    // Handle file uploads - Swashbuckle needs this to properly document IFormFile
    c.MapType<Microsoft.AspNetCore.Http.IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<BackEndService.API.Swagger.TextStreamRequestExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<BackEndService.API.Swagger.BillingRequestExample>();

// Model validation response
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
    };
});

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", o =>
    {
        o.PermitLimit = 60;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });
});

// Gateway services
builder.Services.AddScoped<BackEndService.Core.Interfaces.Gateway.IStreamProcessor, BackEndService.Gateway.Services.StreamProcessor>();
builder.Services.AddScoped<BackEndService.Core.Interfaces.Gateway.IContextProvider, BackEndService.Gateway.Services.ContextProvider>();
builder.Services.AddScoped<BackEndService.Core.Interfaces.Gateway.IMCPHandler, BackEndService.Gateway.Services.MCPHandler>();

// Context store - using JSON store for persistence
builder.Services.AddSingleton<BackEndService.Core.Interfaces.Services.IContextStore, BackEndService.Infrastructure.Storage.JsonContextStore>();
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.ILogRepository, BackEndService.Infrastructure.Logging.SerilogLogRepository>();

// Auth and Payment services
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.IAuthService, BackEndService.Workflows.Services.AuthService>();
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.IPaymentService, BackEndService.Workflows.Services.PaymentService>();
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.IIntentClassifier, BackEndService.Workflows.Services.IntentClassifierService>();

// Workflow orchestration
builder.Services.AddScoped<BackEndService.Core.Interfaces.Workflows.IWorkflowOrchestrator, BackEndService.Workflows.Orchestration.WorkflowOrchestrator>();
builder.Services.AddSingleton<BackEndService.Core.Interfaces.Workflows.IWorkflowRepository>(sp =>
{
    var registry = new BackEndService.Workflows.Orchestration.WorkflowRegistry();
    registry.Register("mcp-list-tools", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.MCP.MCPListToolsModule))
    });
    registry.Register("mcp-execute-tool", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.MCP.MCPExecuteToolModule))
    });
    registry.Register("order", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.OrderProcessing.OrderWorkflowModule))
    });
    registry.Register("stt", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.STT.STTWorkflowModule))
    });
    registry.Register("tts", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.TTS.TTSWorkflowModule))
    });
    registry.Register("rag", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.RAG.RAGWorkflowModule))
    });
    registry.Register("intent-classification", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.IntentClassification.IntentClassificationWorkflowModule))
    });
    registry.Register("create-api-key", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.Auth.CreateApiKeyWorkflowModule))
    });
    registry.Register("billing", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.Payment.BillingWorkflowModule))
    });
    registry.Register("text-to-response", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.Text.TextToResponseWorkflowModule))
    });
    registry.Register("audio-to-response", new[]
    {
        new BackEndService.Core.Models.Workflows.WorkflowStep(typeof(BackEndService.Workflows.Modules.Audio.AudioToResponseWorkflowModule))
    });
    return registry;
});

// Register workflow modules
builder.Services.AddScoped<BackEndService.Workflows.Modules.MCP.MCPListToolsModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.MCP.MCPExecuteToolModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.Text.TextToResponseWorkflowModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.Audio.AudioToResponseWorkflowModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.OrderProcessing.OrderWorkflowModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.STT.STTWorkflowModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.TTS.TTSWorkflowModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.RAG.RAGWorkflowModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.IntentClassification.IntentClassificationWorkflowModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.Auth.CreateApiKeyWorkflowModule>();
builder.Services.AddScoped<BackEndService.Workflows.Modules.Payment.BillingWorkflowModule>();

// HTTP clients for external APIs
builder.Services.AddHttpClient();

// Workflow services
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.ISTTService, BackEndService.Workflows.Services.STTService>();
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.ITTSService, BackEndService.Workflows.Services.TTSService>();
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.IRAGService, BackEndService.Workflows.Services.RAGService>();

// JSON repositories
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.ISTTRepository, BackEndService.Infrastructure.Storage.JsonSTTRepository>();
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.ITTSRepository, BackEndService.Infrastructure.Storage.JsonTTSRepository>();
builder.Services.AddScoped<BackEndService.Core.Interfaces.Services.IRAGRepository, BackEndService.Infrastructure.Storage.JsonRAGRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

// API Key middleware
app.UseMiddleware<BackEndService.API.Gateway.Middleware.ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Startup self-check: ensure critical workflows exist
var requiredWorkflows = new[] { "mcp-list-tools", "mcp-execute-tool", "text-to-response", "audio-to-response", "intent-classification", "create-api-key", "billing", "rag", "order" };
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<BackEndService.Core.Interfaces.Workflows.IWorkflowRepository>();
    foreach (var wf in requiredWorkflows)
    {
        _ = repo.GetWorkflow(wf);
    }
}

app.Run();

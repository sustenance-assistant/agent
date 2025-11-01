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
builder.Services.AddSwaggerExamplesFromAssemblyOf<FoodOrderingService.API.Swagger.TextStreamRequestExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<FoodOrderingService.API.Swagger.BillingRequestExample>();

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
        o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });
});

// Gateway services
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Gateway.IStreamProcessor, FoodOrderingService.Gateway.Services.StreamProcessor>();
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Gateway.IContextProvider, FoodOrderingService.Gateway.Services.ContextProvider>();
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Gateway.IMCPHandler, FoodOrderingService.Gateway.Services.MCPHandler>();

// Context store - using JSON store for persistence
builder.Services.AddSingleton<FoodOrderingService.Core.Interfaces.Services.IContextStore, FoodOrderingService.Infrastructure.Storage.JsonContextStore>();
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.ILogRepository, FoodOrderingService.Infrastructure.Logging.SerilogLogRepository>();

// Auth and Payment services
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.IAuthService, FoodOrderingService.Workflows.Services.AuthService>();
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.IPaymentService, FoodOrderingService.Workflows.Services.PaymentService>();
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.IIntentClassifier, FoodOrderingService.Workflows.Services.IntentClassifierService>();

// Workflow orchestration
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Workflows.IWorkflowOrchestrator, FoodOrderingService.Workflows.Orchestration.WorkflowOrchestrator>();
builder.Services.AddSingleton<FoodOrderingService.Core.Interfaces.Workflows.IWorkflowRepository>(sp =>
{
    var registry = new FoodOrderingService.Workflows.Orchestration.WorkflowRegistry();
    registry.Register("mcp-list-tools", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.MCP.MCPListToolsModule))
    });
    registry.Register("mcp-execute-tool", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.MCP.MCPExecuteToolModule))
    });
    registry.Register("order", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.OrderProcessing.OrderWorkflowModule))
    });
    registry.Register("stt", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.STT.STTWorkflowModule))
    });
    registry.Register("tts", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.TTS.TTSWorkflowModule))
    });
    registry.Register("rag", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.RAG.RAGWorkflowModule))
    });
    registry.Register("intent-classification", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.IntentClassification.IntentClassificationWorkflowModule))
    });
    registry.Register("create-api-key", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.Auth.CreateApiKeyWorkflowModule))
    });
    registry.Register("billing", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.Payment.BillingWorkflowModule))
    });
    registry.Register("text-to-response", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.Text.TextToResponseWorkflowModule))
    });
    registry.Register("audio-to-response", new[]
    {
        new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(FoodOrderingService.Workflows.Modules.Audio.AudioToResponseWorkflowModule))
    });
    return registry;
});

// Register workflow modules
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.MCP.MCPListToolsModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.MCP.MCPExecuteToolModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.Text.TextToResponseWorkflowModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.Audio.AudioToResponseWorkflowModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.OrderProcessing.OrderWorkflowModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.STT.STTWorkflowModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.TTS.TTSWorkflowModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.RAG.RAGWorkflowModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.IntentClassification.IntentClassificationWorkflowModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.Auth.CreateApiKeyWorkflowModule>();
builder.Services.AddScoped<FoodOrderingService.Workflows.Modules.Payment.BillingWorkflowModule>();

// HTTP clients for external APIs
builder.Services.AddHttpClient();

// Workflow services
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.ISTTService, FoodOrderingService.Workflows.Services.STTService>();
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.ITTSService, FoodOrderingService.Workflows.Services.TTSService>();
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.IRAGService, FoodOrderingService.Workflows.Services.RAGService>();

// JSON repositories
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.ISTTRepository, FoodOrderingService.Infrastructure.Storage.JsonSTTRepository>();
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.ITTSRepository, FoodOrderingService.Infrastructure.Storage.JsonTTSRepository>();
builder.Services.AddScoped<FoodOrderingService.Core.Interfaces.Services.IRAGRepository, FoodOrderingService.Infrastructure.Storage.JsonRAGRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

// API Key middleware
app.UseMiddleware<FoodOrderingService.API.Gateway.Middleware.ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Startup self-check: ensure critical workflows exist
var requiredWorkflows = new[] { "mcp-list-tools", "mcp-execute-tool", "text-to-response", "audio-to-response", "intent-classification", "create-api-key", "billing", "rag", "order" };
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<FoodOrderingService.Core.Interfaces.Workflows.IWorkflowRepository>();
    foreach (var wf in requiredWorkflows)
    {
        _ = repo.GetWorkflow(wf);
    }
}

app.Run();

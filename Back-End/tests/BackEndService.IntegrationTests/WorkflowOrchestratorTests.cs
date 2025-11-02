using System.Collections.Generic;
using System.Threading.Tasks;
using BackEndService.Core.Interfaces.Workflows;
using BackEndService.Core.Models.Context;
using BackEndService.Core.Models.Workflows;
using BackEndService.Workflows.Modules.OrderProcessing;
using BackEndService.Workflows.Orchestration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BackEndService.IntegrationTests
{
    public class WorkflowOrchestratorTests
    {
        [Fact]
        public async Task ExecuteAsync_WithRegisteredWorkflow_ExecutesModule()
        {
            var services = new ServiceCollection();
            var mockModule = new Mock<IWorkflowModule>();
            mockModule.Setup(x => x.ExecuteAsync(It.IsAny<WorkflowContext>(), It.IsAny<object>()))
                     .ReturnsAsync(new { result = "ok" });
            
            services.AddSingleton<IWorkflowModule>(sp => mockModule.Object);
            var sp = services.BuildServiceProvider();
            
            var registry = new WorkflowRegistry();
            registry.Register("test", new[] { new WorkflowStep(typeof(IWorkflowModule)) });
            
            var orchestrator = new WorkflowOrchestrator(sp, registry);
            var context = new WorkflowContext { UserId = "test" };
            
            var result = await orchestrator.ExecuteAsync("test", context, "input");
            
            Assert.NotNull(result.Data);
            mockModule.Verify(x => x.ExecuteAsync(context, "input"), Times.Once);
        }

        [Fact]
        public void ExecuteAsync_WithMissingWorkflow_Throws()
        {
            var services = new ServiceCollection().BuildServiceProvider();
            var registry = new WorkflowRegistry();
            var orchestrator = new WorkflowOrchestrator(services, registry);
            
            Assert.ThrowsAsync<System.InvalidOperationException>(
                () => orchestrator.ExecuteAsync("missing", new WorkflowContext(), "input"));
        }
    }
}


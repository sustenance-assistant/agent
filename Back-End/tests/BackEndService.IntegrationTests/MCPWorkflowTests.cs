using System.Text.Json;
using System.Threading.Tasks;
using BackEndService.Core.Models.Context;
using BackEndService.Core.Models.MCP;
using BackEndService.Workflows.Modules.MCP;
using Moq;
using Xunit;

namespace BackEndService.IntegrationTests
{
    public class MCPWorkflowTests
    {
        [Fact]
        public async Task MCPExecuteToolModule_WithWorkflow_RoutesToOrchestrator()
        {
            var mockHandler = new Mock<FoodOrderingService.Core.Interfaces.Gateway.IMCPHandler>();
            var mockOrchestrator = new Mock<FoodOrderingService.Core.Interfaces.Workflows.IWorkflowOrchestrator>();
            var mockContextStore = new Mock<FoodOrderingService.Core.Interfaces.Services.IContextStore>();
            var mockAuthService = new Mock<FoodOrderingService.Core.Interfaces.Services.IAuthService>();
            
            mockOrchestrator.Setup(x => x.ExecuteAsync("order", It.IsAny<WorkflowContext>(), It.IsAny<object>()))
                           .ReturnsAsync(new FoodOrderingService.Core.Models.Workflows.WorkflowResponse { Data = new { status = "ok" } });
            
            mockContextStore.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync((WorkflowContext?)null);
            
            var module = new MCPExecuteToolModule(mockHandler.Object, mockOrchestrator.Object, mockContextStore.Object, mockAuthService.Object);
            var context = new WorkflowContext { UserId = "test" };
            var call = new McpToolCall
            {
                Tool = "order",
                Workflow = "order",
                Arguments = JsonDocument.Parse("{\"items\":[]}").RootElement
            };
            
            var result = await module.ExecuteAsync(context, call);
            
            Assert.NotNull(result);
            mockOrchestrator.Verify(x => x.ExecuteAsync("order", context, It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task MCPListToolsModule_ReturnsTools()
        {
            var mockHandler = new Mock<FoodOrderingService.Core.Interfaces.Gateway.IMCPHandler>();
            mockHandler.Setup(x => x.ListToolsAsync(It.IsAny<JsonDocument>()))
                      .ReturnsAsync(JsonDocument.Parse("{\"tools\":[\"order\",\"stt\"]}"));
            
            var module = new MCPListToolsModule(mockHandler.Object);
            var context = new WorkflowContext();
            
            var result = await module.ExecuteAsync(context, new { });
            
            Assert.NotNull(result);
        }
    }
}


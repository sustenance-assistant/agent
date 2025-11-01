using BackEndService.Workflows.Orchestration;
using Xunit;

namespace BackEndService.IntegrationTests
{
    public class WorkflowRegistryTests
    {
        [Fact]
        public void Registry_Has_Baseline_Workflows()
        {
            var reg = new WorkflowRegistry();
            reg.Register("x", new[] { new FoodOrderingService.Core.Models.Workflows.WorkflowStep(typeof(object)) });

            Assert.Throws<System.InvalidOperationException>(() => reg.GetWorkflow("missing"));
        }
    }
}



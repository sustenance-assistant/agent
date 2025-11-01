using System.Text.Json;
using System.Threading.Tasks;

namespace BackEndService.Core.Interfaces.Gateway
{
    public interface IMCPHandler
    {
        Task<JsonDocument> ListToolsAsync(JsonDocument request);
        Task<JsonDocument> CallToolAsync(JsonDocument request);
    }
}

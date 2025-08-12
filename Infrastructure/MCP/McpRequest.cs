using System.Text.Json;

namespace WeatherMCP.Infrastructure.MCP
{
    public class McpRequest
    {
        public string Id { get; set; } = "";
        public string Method { get; set; } = "";
        public JsonElement? Params { get; set; }
    }
}
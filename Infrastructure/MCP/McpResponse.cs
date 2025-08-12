namespace WeatherMCP.Infrastructure.MCP
{
    public class McpResponse
    {
        public string Id { get; set; } = "";
        public object? Result { get; set; }
        public McpError? Error { get; set; }
    }
}
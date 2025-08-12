namespace WeatherMCP.Domain.Models
{
    public class McpServerOptions
    {
        public const string SectionName = "McpServer";
        
        public string Name { get; set; } = "meteochile-mcp-server";
        public string Version { get; set; } = "1.0.0";
        public string ProtocolVersion { get; set; } = "2024-11-05";
    }
}
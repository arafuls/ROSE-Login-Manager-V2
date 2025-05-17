using System.Text.Json.Serialization;

namespace ROSE.Core.Models
{
    public class Settings
    {
        [JsonPropertyName("theme")]
        public string Theme { get; set; } = "Dark"; // Default theme

        [JsonPropertyName("version")]
        public string Version { get; set; } = "0.0.0";
    }
}
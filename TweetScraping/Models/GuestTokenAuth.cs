using System.Text.Json.Serialization;

namespace TweetScraping.Models
{
    public class GuestTokenAuth
    {
        [JsonPropertyName("guest_token")]
        public string GuestToken { get; set; }
    }
}

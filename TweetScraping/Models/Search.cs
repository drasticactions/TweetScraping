using System.Text.Json.Serialization;

namespace TweetScraping.Models
{
    public class Search
    {
        [JsonPropertyName("min_position")]
        public string MinPosition { get; set; }

        [JsonPropertyName("has_more_items")]
        public bool HasMoreItems { get; set; }

        [JsonPropertyName("items_html")]
        public string ItemsHtml { get; set; }

        [JsonPropertyName("new_latent_count")]
        public int NewLatentCount { get; set; }

        [JsonPropertyName("focused_refresh_interval")]
        public int FocusedRefreshInterval { get; set; }
    }
}

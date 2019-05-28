using System;
using Newtonsoft.Json;

namespace TweetScraping.Models
{
    public class Search
    {
        [JsonProperty(PropertyName = "min_position")]
        public string MinPosition { get; set; }

        [JsonProperty(PropertyName = "has_more_items")]
        public bool HasMoreItems { get; set; }

        [JsonProperty(PropertyName = "items_html")]
        public string ItemsHtml { get; set; }

        [JsonProperty(PropertyName = "new_latent_count")]
        public int NewLatentCount { get; set; }

        [JsonProperty(PropertyName = "focused_refresh_interval")]
        public int FocusedRefreshInterval { get; set; }
    }
}

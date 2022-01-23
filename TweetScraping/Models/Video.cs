using System;
using System.Text.Json.Serialization;

namespace TweetScraping.Models
{
    public class Video
    {
        [JsonPropertyName("track")]
        public Track Track { get; set; }

        [JsonPropertyName("posterImage")]
        public Uri PosterImage { get; set; }

        [JsonPropertyName("features")]
        public Features Features { get; set; }
    }

    public class Features
    {
        [JsonPropertyName("isEdgeEnabled")]
        public bool IsEdgeEnabled { get; set; }

        [JsonPropertyName("bitrateCap")]
        public object BitrateCap { get; set; }

        [JsonPropertyName("isDebuggingEnabled")]
        public bool IsDebuggingEnabled { get; set; }

        [JsonPropertyName("isLiveTimecodeEnabled")]
        public bool IsLiveTimecodeEnabled { get; set; }

        [JsonPropertyName("isClientMediaEventScribingEnabled")]
        public bool IsClientMediaEventScribingEnabled { get; set; }
    }

    public class Track
    {
        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("publisherId")]
        public string PublisherId { get; set; }

        [JsonPropertyName("contentId")]
        public string ContentId { get; set; }

        [JsonPropertyName("durationMs")]
        public long DurationMs { get; set; }

        [JsonPropertyName("playbackUrl")]
        public Uri PlaybackUrl { get; set; }

        [JsonPropertyName("playbackType")]
        public string PlaybackType { get; set; }

        [JsonPropertyName("expandedUrl")]
        public Uri ExpandedUrl { get; set; }

        [JsonPropertyName("vmapUrl")]
        public object VmapUrl { get; set; }

        [JsonPropertyName("cta")]
        public object Cta { get; set; }

        [JsonPropertyName("shouldLoop")]
        public bool ShouldLoop { get; set; }

        [JsonPropertyName("viewCount")]
        public string ViewCount { get; set; }

        [JsonPropertyName("isEventGeoblocked")]
        public bool IsEventGeoblocked { get; set; }

        [JsonPropertyName("is360")]
        public bool Is360 { get; set; }

        [JsonPropertyName("mediaAvailability")]
        public MediaAvailability MediaAvailability { get; set; }
    }

    public class MediaAvailability
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("reason")]
        public object Reason { get; set; }
    }
}

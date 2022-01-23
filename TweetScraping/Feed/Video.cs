using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TweetScraping.Tools;

namespace TweetScraping.Feed
{
    public class Video
    {
        public static async Task<List<M3u8Playlist>> GetVideoPlaylist(string id)
        {
            using (var client = new TweetClient())
            {
                return await GetVideoPlaylist(id, client);
            }
        }

        public static async Task<List<M3u8VideoSegment>> GetVideoSegments(M3u8Playlist playlistItem)
        {
            using (var client = new TweetClient())
            {
                return await GetVideoSegments(playlistItem, client);
            }
        }

        public static async Task<List<M3u8VideoSegment>> GetVideoSegments(M3u8Playlist playlistItem, TweetClient client)
        {
            if (string.IsNullOrEmpty(playlistItem.AuthenticationToken))
                throw new Exception($"No Authorization Bearer Token set for m3u8 file!");
            var m3u3Text = await client.GetStringAsync(playlistItem.Url.ToString(), playlistItem.AuthenticationToken);
            return M3U3Parser.ParseVideoSegments(m3u3Text, playlistItem.Resolution, $"{playlistItem.Url.Scheme}://{playlistItem.Url.Host}");
        }

        public static async Task<List<M3u8Playlist>> GetVideoPlaylist(string id, TweetClient client)
        {
            var videoHtml = await client.GetHtmlAsync($"https://twitter.com/i/videos/tweet/{id}");
            var jsFileUrl = videoHtml.QuerySelector("script").GetAttribute("src");
            var jsScript = await client.GetStringAsync(jsFileUrl);
            var bearerRegex = new Regex("Bearer ([a-zA-Z0-9%-])+");
            var match = bearerRegex.Match(jsScript);
            if (match.Success)
            {
                var token = match.Value;
                var playerConfigString = await client.GetStringAsync($"https://api.twitter.com/1.1/videos/tweet/config/{id}.json", token);
                var playerConfig = System.Text.Json.JsonSerializer.Deserialize<Models.Video>(playerConfigString);
                if (playerConfig is null)
                {
                    throw new Exception($"Could not get playback item for {id}");
                }
                var playbackUri = playerConfig.Track.PlaybackUrl;
                var m3u3Text = await client.GetStringAsync(playbackUri.ToString(), token);
                return M3U3Parser.ParsePlaylist(m3u3Text, token, $"{playbackUri.Scheme}://{playbackUri.Host}");
            }
            throw new Exception($"Could not get Authentication Token for playlist item {id}");
        }
    }
}

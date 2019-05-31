using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TweetScraping.Tools
{
    public class M3u8Playlist
    {
        public long Bandwidth { get; set; }
        public Resolution Resolution { get; set; }
        public string Codecs { get; set; }
        public Uri Url { get; set; }
        public string AuthenticationToken { get; set; }
    }

    public class M3u8VideoSegment
    {
        public Resolution Resolution { get; set; }
        public decimal Length { get; set; }
        public Uri Url { get; set; }
    }

    public class Resolution
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class M3U3Parser
    {
        public static List<M3u8VideoSegment> ParseVideoSegments(string m3u8, Resolution resolution, string baseUrl = "")
        {
            if (m3u8 == null)
                throw new Exception("Must include m3u8!");
            var lines = m3u8.Replace("\r", "").Split('\n');
            var result = new List<M3u8VideoSegment>();
            if (lines.Any())
            {
                var firstLine = lines[0];
                if (firstLine != "#EXTM3U")
                {
                    throw new InvalidOperationException(
                        "The provided URL does not link to a well-formed M3U8 playlist.");
                }
                bool mediaDetected = false;
                M3u8VideoSegment streamInfo = new M3u8VideoSegment();
                streamInfo.Resolution = resolution;
                for (var i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (line.StartsWith("#", StringComparison.Ordinal))
                    {
                        var lineData = line.Substring(1);

                        var split = lineData.Split(':');
                        if (split.Length <= 1)
                            continue;
                        var name = split[0];
                        var suffix = split[1].Replace(",", "");
                        if (name == "EXTINF")
                        {
                            mediaDetected = true;
                            streamInfo.Length = decimal.Parse(suffix);
                        }
                    }
                    else
                    {
                        if (mediaDetected)
                        {
                            streamInfo.Url = new Uri(baseUrl + line);
                            mediaDetected = false;
                            result.Add(streamInfo);
                        }
                    }
                }
            }

            return result;
        }

        public static List<M3u8Playlist> ParsePlaylist(string m3u8, string token = "", string baseUrl = "")
        {
            if (m3u8 == null)
                throw new Exception("Must include m3u8!");
            var lines = m3u8.Replace("\r", "").Split('\n');
            var result = new List<M3u8Playlist>();
            if (lines.Any())
            {
                var firstLine = lines[0];
                if (firstLine != "#EXTM3U")
                {
                    throw new InvalidOperationException(
                        "The provided URL does not link to a well-formed M3U8 playlist.");
                }
                bool mediaDetected = false;
                M3u8Playlist streamInfo = null;
                for (var i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (line.StartsWith("#", StringComparison.Ordinal))
                    {
                        var lineData = line.Substring(1);

                        var split = lineData.Split(':');
                        if (split.Length <= 1)
                            continue;
                        var name = split[0];
                        var suffix = split[1];

                        if (name == "EXT-X-STREAM-INF")
                        {
                            mediaDetected = true;
                            streamInfo = new M3u8Playlist();
                        }
                        streamInfo.AuthenticationToken = token;
                        // We must be able to split the commas in the M3U8 to get the items
                        // However, they can also be quoted inside the parameters
                        // Ex. #EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=2176000,RESOLUTION=720x1280,CODECS="mp4a.40.2,avc1.640020"
                        // So we must treat it like a CSV and extract it that way.
                        var attributes = Regex.Split(suffix, "[\t,](?=(?:[^\"]|\"[^\"]*\")*$)");
                        foreach (var item in attributes)
                        {
                            var keyvalue = item.Split('=');
                            if (keyvalue.Any())
                                switch (keyvalue[0])
                                {
                                    case "BANDWIDTH":
                                        streamInfo.Bandwidth = long.Parse(keyvalue[1], CultureInfo.InvariantCulture);
                                        break;
                                    case "RESOLUTION":
                                        try
                                        {
                                            streamInfo.Resolution = new Resolution();
                                            var size = keyvalue[1].Split('x');
                                            if (size != null)
                                            {
                                                streamInfo.Resolution.Width = int.Parse(size[0], CultureInfo.InvariantCulture);
                                                streamInfo.Resolution.Height = int.Parse(size[1], CultureInfo.InvariantCulture);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine($"Could not get resolution: {ex.Message}");
                                        }
                                        break;
                                    case "CODECS":
                                        streamInfo.Codecs = keyvalue[1].Trim('"');
                                        break;
                                }
                        }
                    }
                    else
                    {
                        if (mediaDetected)
                        {
                            streamInfo.Url = new Uri(baseUrl + line);
                            mediaDetected = false;
                            result.Add(streamInfo);
                        }
                    }
                }
            }

            return result;
        }
    }
}

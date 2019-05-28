using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using TweetScraping.Tools;

namespace TweetScraping.Models
{
    public class Tweet
    {
        public long Id { get; set; }

        public string IdStr { get; set; }

        public string ConversationId { get; set; }

        public DateTimeOffset DateTimeOffset { get; set; }

        public long UserId { get; set; }

        public string UserIdStr { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string ProfileImageUrl { get; set; }

        public string Place { get; set; }

        public TimeZoneInfo TimeZone { get; set; }

        public string[] Mentions { get; set; }

        public string[] Urls { get; set; }

        public string[] Photos { get; set; }

        public bool HasVideo { get; set; }

        public string Text { get; set; }

        public string[] Hashtags { get; set; }

        public string[] Cashtags { get; set; }

        public int ReplyCount { get; set; }

        public int LikeCount { get; set; }

        public int RetweetCount { get; set; }

        public string Link { get { return $"https://twitter.com/{Username}/status/{Id}"; } }

        public bool Retweet { get; set; }

        public string QuoteUrl { get; set; }

        public static async Task<Tweet> Get(string username, string id, TweetClient client, Config config)
        {
            var tweetsDiv = await client.GetHtmlAsync($"https://twitter.com/{username}/status/{id}");
            var tweetDiv = tweetsDiv.QuerySelector($"[data-tweet-id='{id}']");
            var tweet = new Tweet();
            tweet.Scrape(config, tweetDiv);
            return tweet;
        }

        public static Tweet Get(Config config, IElement element)
        {
            var tweet = new Tweet();
            tweet.Scrape(config, element);
            return tweet;
        }

        private DateTimeOffset ParseDateTimeOffset(IElement element)
        {
            var datetime = element.QuerySelector("span._timestamp");
            if (datetime == null)
                return new DateTime();
            return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(datetime.GetAttribute("data-time-ms")));
        }

        private string[] ParseUrls(IElement element)
        {
            var urls = element.QuerySelectorAll("a[data-expanded-url]");
            if (!urls.Any())
                return new string[0];
            return urls.Select(n => n.GetAttribute("data-expanded-url")).ToArray();
        }

        private string[] ParseImageUrls(IElement element)
        {
            var urls = element.QuerySelectorAll("[data-image-url]");
            if (!urls.Any())
                return new string[0];
            return urls.Select(n => n.GetAttribute("data-image-url")).ToArray();
        }

        private int ScrapeInfoValue(IElement element, string type)
        {
            var infoList = element.QuerySelector($"div.ProfileTweet-action--{type}");
            if (infoList == null)
                return 0;
            var value = infoList.QuerySelector("span.ProfileTweet-actionCountForPresentation").TextContent;
            return string.IsNullOrEmpty(value) ? 0 : int.Parse(value, NumberStyles.AllowThousands);
        }

        private string ScrapeQuoteUrl(IElement element)
        {
            var quoteTweetDiv = element.QuerySelector("div.QuoteTweet-innerContainer");
            if (quoteTweetDiv == null)
                return "";
            return $"https://twitter.com{quoteTweetDiv.GetAttribute("href")}";
        }

        public void Scrape(Config config, IElement element)
        {
            IdStr = element.GetAttribute("data-item-id");
            Id = long.Parse(IdStr);
            UserIdStr = element.GetAttribute("data-user-id");
            UserId = long.Parse(UserIdStr);
            ConversationId = element.GetAttribute("data-conversation-id");
            DateTimeOffset = ParseDateTimeOffset(element);
            TimeZone = TimeZoneInfo.Local;
            Mentions = element.GetAttribute("data-mentions") != null ? element.GetAttribute("data-mentions").Split(' ') : new string[0];
            Urls = ParseUrls(element);
            Photos = ParseImageUrls(element);
            HasVideo = element.QuerySelector("div.AdaptiveMedia-video") != null;
            Text = element.QuerySelector("p.tweet-text").TextContent.Trim();
            Username = element.GetAttribute("data-screen-name");
            Name = element.GetAttribute("data-name");
            Retweet = config.Profile && config.Username == Username;
            Hashtags = element.QuerySelectorAll("a.twitter-hashtag").Select(n => n.TextContent).ToArray();
            Cashtags = element.QuerySelectorAll("a.twitter-cashtag").Select(n => n.TextContent).ToArray();
            RetweetCount = ScrapeInfoValue(element, "retweet");
            ReplyCount = ScrapeInfoValue(element, "reply");
            LikeCount = ScrapeInfoValue(element, "favorite");
            QuoteUrl = ScrapeQuoteUrl(element);
        }
    }
}

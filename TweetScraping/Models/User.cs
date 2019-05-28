using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using TweetScraping.Tools;

namespace TweetScraping.Models
{
    public class User
    {
        public long Id { get; set; }

        public string IdSrt { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Bio { get; set; }

        public string Location { get; set; }

        public string Url { get; set; }

        public string ShortUrl { get; set; }

        public DateTime? JoinDateTime { get; set; }

        public int Tweets { get; set; }

        public int Following { get; set; }

        public int Followers { get; set; }

        public int Likes { get; set; }

        public int MediaCount { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsVerified { get; set; }

        public string Avatar { get; set; }

        public string BackgroundImage { get; set; }

        public static async Task<User> GetAsync(string username)
        {
            using (var client = new TweetClient())
            {
                var user = new User();
                var dom = await client.GetHtmlAsync($"https://www.twitter.com/{username}?lang=en");
                user.Scrape(dom);
                return user;
            }
        }

        public void Scrape(IHtmlDocument dom)
        {
            var userElement = ScrapeUserInfoElement(dom);
            if (userElement == null)
                return;
            var infoElement = dom.QuerySelector("ul.ProfileNav-list");
            if (infoElement != null)
            {
                Following = ScrapeInfoValue(infoElement, "following");
                Followers = ScrapeInfoValue(infoElement, "followers");
                Likes = ScrapeInfoValue(infoElement, "favorites");
                Tweets = ScrapeInfoValue(infoElement, "tweets");
            }
            MediaCount = ScrapeMediaCount(dom);
            IdSrt = userElement.GetAttribute("data-user-id");
            Id = long.Parse(IdSrt);
            Name = userElement.GetAttribute("data-name");
            Username = userElement.GetAttribute("data-screen-name");
            IsPrivate = bool.Parse(userElement.GetAttribute("data-protected"));
            IsVerified = ScrapVerified(dom);
            Bio = dom.QuerySelector("p.ProfileHeaderCard-bio").TextContent;
            Location = dom.QuerySelector("span.ProfileHeaderCard-locationText").TextContent.Replace("\n", "").Trim();
            Url = ScrapeUrl(dom, "title");
            ShortUrl = ScrapeUrl(dom);
            JoinDateTime = ScrapeJoinTime(dom);
            Avatar = dom.QuerySelector("img.ProfileAvatar-image").GetAttribute("src");
            BackgroundImage = dom.QuerySelector("div.ProfileCanopy-headerBg").QuerySelector("img").GetAttribute("src");
        }

        private int ScrapeMediaCount(IHtmlDocument dom)
        {
            var mediaElement = dom.QuerySelector("a.PhotoRail-headingWithCount");
            if (mediaElement == null)
                return 0;
            var mediaCount = mediaElement.TextContent.Replace("\n", "").Trim().Split(' ').FirstOrDefault();
            return mediaCount == null ? 0 : int.Parse(mediaCount, NumberStyles.AllowThousands);
        }

        private int ScrapeInfoValue(IElement element, string type)
        {
            var infoList = element.QuerySelector($"li.ProfileNav-item--{type}");
            if (infoList == null)
                return 0;
            var value = infoList.QuerySelector("span.ProfileNav-value").GetAttribute("data-count");
            return int.Parse(value, NumberStyles.AllowThousands);
        }

        private bool ScrapVerified(IHtmlDocument dom)
        {
            var verifiedDom = dom.QuerySelector("span.ProfileHeaderCard-badges");
            if (verifiedDom == null)
                return false;
            return verifiedDom.TextContent.Contains("Verified account");
        }

        private IElement ScrapeUserInfoElement(IHtmlDocument dom)
        {
            var userElement = dom.QuerySelector("div.user-actions.btn-group.not-following");
            if (userElement == null)
                userElement = dom.QuerySelector("div.user-actions.btn-group.not-following.protected");
            return userElement;
        }

        private DateTime? ScrapeJoinTime(IHtmlDocument dom)
        {
            var joinElement = dom.QuerySelector("span.ProfileHeaderCard-joinDateText");
            if (joinElement == null)
                return null;
            var joinDateTitle = joinElement.GetAttribute("title");
            if (string.IsNullOrEmpty(joinDateTitle))
                return null;
            return DateTime.Parse(joinDateTitle.Replace("-", ""));
        }

        private string ScrapeUrl(IHtmlDocument dom, string type = "href")
        {
            var urlElement = dom.QuerySelector("span.ProfileHeaderCard-urlText");
            if (urlElement == null || !urlElement.HasChildNodes)
                return "";
            return urlElement.QuerySelector("a").GetAttribute(type);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using TweetScraping.Models;
using TweetScraping.Tools;


namespace TweetScraping.Feed
{
    public class Favorites : IDisposable
    {
        private Config _config;
        private TweetClient _client;
        public Favorites(Config config)
        {
            _config = config;
            _client = new TweetClient();
        }

        public Favorites(TweetClient client, Config config)
        {
            _config = config;
            _client = client;
        }

        public Favorites()
        {
            _config = new Config();
            _client = new TweetClient();
        }

        public Config GetConfig()
        {
            return _config;
        }

        public async Task<List<Tweet>> GetAsync()
        {
            if (string.IsNullOrEmpty(_config.Username))
                throw new Exception("You must add Username to the Config parameter first!");
            var url = $"https://mobile.twitter.com/{_config.Username}/favorites?lang=en";
            if (_config.MaxPosition != "-1")
                url += $"&max_id={_config.MaxPosition}";
            var response = await _client.GetHtmlAsync(url);
            var maxIdDiv = response.QuerySelector("div.w-button-more");
            if (maxIdDiv != null)
            {
                var link = maxIdDiv.QuerySelector("a").GetAttribute("href");
                _config.MaxPosition = HttpUtility.ParseQueryString("http://twitter.com" + link)[0];
                _config.HasMoreItems = true;
            }
            else
            {
                _config.HasMoreItems = false;
            }
            var tweets = new List<Tweet>();
            var tweetsDiv = response.QuerySelectorAll("span.metadata");
            foreach (var tweetDiv in tweetsDiv)
            {
                var href = tweetDiv.QuerySelector("a").GetAttribute("href");
                var freshLink = href.Substring(0, href.IndexOf('?'));
                var linkSplit = freshLink.Split('/');
                tweets.Add(await Tweet.Get(linkSplit[1], linkSplit[3], _client, _config));
            }
            return tweets;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

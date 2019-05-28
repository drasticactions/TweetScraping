using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using TweetScraping.Models;
using TweetScraping.Tools;

namespace TweetScraping.Feed
{
    public class Search : IDisposable
    {
        private Config _config;
        private TweetClient _client;

        public Search (Config config)
        {
            _config = config;
            _client = new TweetClient();
        }

        public Search (TweetClient client, Config config)
        {
            _config = config;
            _client = client;
        }

        public Search ()
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
            var url = GenerateUrl();
            var jsonString = await _client.GetJsonStringAsync(url.ToString());
            var search = JsonConvert.DeserializeObject<Models.Search>(jsonString);
            _config.HasMoreItems = search.HasMoreItems;
            _config.MaxPosition = search.MinPosition;
            var html = await _client.ParseHtml(search.ItemsHtml);
            var tweetsDiv = html.QuerySelectorAll("div.tweet");
            var tweets = new List<Tweet>();
            foreach (var tweetDiv in tweetsDiv)
                tweets.Add(Tweet.Get(_config, tweetDiv));
            return tweets;
        }

        private Uri GenerateUrl ()
        {
            var twitterBase = "https://twitter.com/i/search/timeline";
            var query = new List<string>();
            if (!string.IsNullOrEmpty(_config.CustomQuery))
            {
                query.Add(_config.CustomQuery);
            }
            else
            {
                if (_config.Since != null)
                    query.Add($"since:{_config.Since.Value.ToString("yyyy-MM-dd")}");
                if (_config.Until != null)
                    query.Add($"until:{_config.Since.Value.ToString("yyyy-MM-dd")}");
                if (!string.IsNullOrEmpty(_config.Username))
                    query.Add($"from:{_config.Username}");
                if (!string.IsNullOrEmpty(_config.Search))
                    query.Add(_config.Search);
                if (!string.IsNullOrEmpty(_config.To))
                    query.Add($"to:{_config.To}");
                if (!string.IsNullOrEmpty(_config.Geo))
                    query.Add($"geocode::{_config.Geo.Replace(" ", "")}");
                if (!string.IsNullOrEmpty(_config.All))
                    query.Add($"to:{_config.All} OR from:{_config.All} OR @{_config.All}");
                if (!string.IsNullOrEmpty(_config.Near))
                    query.Add($"near:{_config.Near}");
                if (_config.Verified)
                    query.Add("filter:verified");
                if (_config.Images)
                    query.Add("filter:images");
                if (_config.Videos)
                    query.Add("filter:videos");
                if (_config.Media)
                    query.Add("filter:media");
                if (_config.Replies)
                    query.Add("filter:replies");
            }
            var values = new NameValueCollection
            {
                {"vertical", "default"},
                {"src", "unkn"},
                {"include_available_features", "1"},
                {"include_entities", "1"},
                {"max_position", _config.MaxPosition },
                {"reset_error_state", "false"},
                {"q", string.Join("&", query) }
            };

            if (!_config.PopularTweets)
                values.Add("f", "tweets");

            if (!string.IsNullOrEmpty(_config.Lang))
                values.Add("l", _config.Search);
            else
                values.Add("lang", "en");

            return UrlHelper.BuildUri(twitterBase, values);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

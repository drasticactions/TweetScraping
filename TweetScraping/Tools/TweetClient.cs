﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TweetScraping.Tools
{
    public class TweetClient : IDisposable
    {
        HttpClient _client;
        HtmlParser _parser = new HtmlParser();

        #region ConstValues

        const string _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2593.0 Safari/537.36";

        const string _mobileUserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";

        #endregion

        #region

        public TweetClient(HttpClient client)
        {
            _client = client;
        }

        public TweetClient()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
        }

        #endregion

        private void SetupHeaders (string url)
        {
            _client.DefaultRequestHeaders.Remove("User-Agent");
            _client.DefaultRequestHeaders.Add("User-Agent", url.Contains("mobile.twitter.com") ? _mobileUserAgent : _userAgent);
        }

        private async Task SetupAuth (string token)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Add("Authorization", token);
            if (!_client.DefaultRequestHeaders.Contains("x-guest-token") 
                && _client.DefaultRequestHeaders.Contains("Authorization"))
            {
                var guestAuth = await GetGuestAuthToken();
                _client.DefaultRequestHeaders.Add("x-guest-token", guestAuth);
            }
        }

        public async Task<string> GetGuestAuthToken ()
        {
            var response = await _client.PostAsync("https://api.twitter.com/1.1/guest/activate.json", new StringContent(""));
            var guestTokenJson = await response.Content.ReadAsStringAsync();
            var guestAuth = JToken.Parse(guestTokenJson);
            if (guestAuth["errors"] != null)
            {
                throw new Exception($"Could not get guest auth token, {guestAuth["errors"]}");
            }
            return (string)guestAuth["guest_token"];
        }

        public async Task<string> PostStringAsync(string url, string content = "", string token = "")
        {
            SetupHeaders(url);
            await SetupAuth(token);
            var response = await _client.PostAsync(url, new StringContent(content));
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetStringAsync(string url, string token = "")
        {
            SetupHeaders(url);
            await SetupAuth(token);
            var response = await _client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<IHtmlDocument> GetHtmlAsync(string url, string token = "")
        {
            SetupHeaders(url);
            await SetupAuth(token);
            var response = await _client.GetAsync(url);
            var test = await response.Content.ReadAsStringAsync();
            return await ParseHtmlAsync(test);
        }

        public async Task<IHtmlDocument> ParseHtmlAsync(string html)
        {
            return await _parser.ParseDocumentAsync(html);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}

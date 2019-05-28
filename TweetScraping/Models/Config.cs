using System;
namespace TweetScraping.Models
{
    public class Config
    {
        public bool Profile { get; set; }

        public string Username { get; set; }

        public string UserId { get; set; }

        public string Search { get; set; }

        public string Geo { get; set; }

        public bool Location { get; set; }

        public bool Verified { get; set; }

        public string To { get; set; }

        public string All { get; set; }

        public string Near { get; set; }

        public string Lang { get; set; }

        public string Output { get; set; }

        public string MinPosition { get; set; }

        public string MaxPosition { get; set; } = "-1";

        public bool HasMoreItems { get; set; }

        public bool Images { get; set; }

        public bool Videos { get; set; }

        public bool Media { get; set; }

        public bool Replies { get; set; }

        public string CustomQuery { get; set; }

        public bool PopularTweets { get; set; }

        public TimeSpan? TimeDelta { get; set; }

        public DateTime? Since { get; set; }

        public DateTime? Until { get; set; }
    }
}

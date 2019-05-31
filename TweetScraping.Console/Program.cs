using System;
using TweetScraping.Tools;
using TweetScraping.Models;
using System.Threading.Tasks;
using TweetScraping.Feed;
using System.Linq;

namespace TweetScraping.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var results = await Models.Video.GetVideoPlaylist("1132741434409283584");
            //var segments = await Video.GetVideoSegments(results.Last());
            // TODO: Currently used to play around with Library. Eventually will be turned into CLI program.
            //var user = await User.GetAsync("drasticactionSA");
            //var config = new Config { Username = "GoOffKings" };
            //using (var favorites = new Feed.Favorites(config))
            //{
            //    do
            //    {
            //        var favoriteTweets = await favorites.GetAsync();
            //        foreach (var tweet in favoriteTweets)
            //        {
            //            System.Console.WriteLine($"{tweet.Username}: {tweet.Text}");
            //        }
            //    } while (config.HasMoreItems);
            //}
            //config = new Config { Username = "GoOffKings" };
            //using (var search = new Feed.Search(config))
            //{
            //    do
            //    {
            //        var tweets = await search.GetAsync();
            //        foreach (var tweet in tweets)
            //        {
            //            System.Console.WriteLine($"{tweet.Username}: {tweet.Text}");
            //        }
            //    } while (config.HasMoreItems);
            //}
        }
    }
}

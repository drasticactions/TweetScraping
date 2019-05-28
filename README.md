# TweetScraping

TweetScraping is a Library and CLI program to parse the Twitter Website. It's inspired by [Twint](https://github.com/twintproject/twint), an awesome parsing library with one huge problem... it's in Python. By using .NET, this library should be easier to implment and use in your own programs.

Currently, I am experimenting with the API and general flow of the methods. Expect breaking changes.

### **NOTE:**
As this uses web parsing, it's very fragle. It's subject to the whims of Twitter and their web layout, and uses many older web pages (https://mobile.twitter.com) that don't have similar functionality on their newer PWA pages. It goes without saying, but **_DON'T USE THIS LIBRARY IN PRODUCTION!!!_**

## Libraries

- [anglesharp](https://anglesharp.github.io/)
- [Json.NET](https://www.newtonsoft.com/json)

## Sample Code

```csharp
using System;
using System.Threading.Tasks;
using TweetScraping.Tools;
using TweetScraping.Models;
using TweetScraping.Feed;

// Get User Profile
var user = await User.GetAsync("drasticactionSA");
var config = new Config { Username = "GoOffKings" };
using (var favorites = new Feed.Favorites(config))
{

    do
    {
        // Gets the "Favorites" feed for "GoOffKings"
        var favoriteTweets = await favorites.GetAsync();
        foreach (var tweet in favoriteTweets)
        {
            System.Console.WriteLine($"{tweet.Username}: {tweet.Text}");
        }
    // HasMoreItems mutates with each call to the feed.
    // If it doesn't have more, it will be set to false
    } while (config.HasMoreItems);

    // Search for tweets from @GoOffKings
    config = new Config { Username = "GoOffKings" };
    using (var search = new Feed.Search(config))
    {
        do
        {
            var tweets = await search.GetAsync();
            foreach (var tweet in tweets)
            {
                System.Console.WriteLine($"{tweet.Username}: {tweet.Text}");
            }
        } while (config.HasMoreItems);
    }
}
```

## TODO

- [x] Basic Search Feed
- [x] Favorites Feed
- [x] User Profile
- [ ] Followers Feed
- [ ] Following Feed
- [ ] Parse Video Embed
- [ ] Complete Search Functions
- [ ] CLI Program

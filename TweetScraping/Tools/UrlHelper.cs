using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace TweetScraping.Tools
{
    public class UrlHelper
    {
        public static Uri BuildUri(string root, NameValueCollection query)
        {
            // omitted argument checking

            // sneaky way to get a HttpValueCollection (which is internal)
            var collection = HttpUtility.ParseQueryString(string.Empty);

            foreach (var key in query.Cast<string>().Where(key => !string.IsNullOrEmpty(query[key])))
            {
                collection[key] = query[key];
            }

            var builder = new UriBuilder(root) { Query = collection.ToString() };
            return builder.Uri;
        }
    }
}

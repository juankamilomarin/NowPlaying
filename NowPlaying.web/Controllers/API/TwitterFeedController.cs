using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace NowPlaying.Controllers.API
{
    [RoutePrefix("api/twitterfeed")]
    public class TwitterFeedController : ApiController
    {
        [HttpGet]
        [Route("latestweets")]
        public IHttpActionResult GetLatestTweets(double latitude, double longitude)
        {
            string consumerKey = ConfigurationManager.AppSettings.Get("ConsumerKey");
            string consumerSecret = ConfigurationManager.AppSettings.Get("ConsumerSecret");
            string accessKey = ConfigurationManager.AppSettings.Get("UserAccessToken");
            string accessToken = ConfigurationManager.AppSettings.Get("UserAccessSecret");
            Auth.SetUserCredentials(consumerKey, consumerSecret, accessKey, accessToken);

            SearchTweetsParameters searchParameter = new SearchTweetsParameters("#nowplaying")
            {
                SearchType = SearchResultType.Recent,
                MaximumNumberOfResults = 5, 
                GeoCode = new GeoCode(latitude, longitude, 100, DistanceMeasure.Kilometers)
            };
            IEnumerable<ITweet> tweets = Search.SearchTweets(searchParameter);
            List<IOEmbedTweet> embeddedTweets = tweets.Select(Tweet.GetOEmbedTweet).ToList();
            return Ok(embeddedTweets);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult PostTweet(string message, string videoUrl, double latitude, double longitude)
        {
            string consumerKey = ConfigurationManager.AppSettings.Get("ConsumerKey");
            string consumerSecret = ConfigurationManager.AppSettings.Get("ConsumerSecret");
            string accessKey = ConfigurationManager.AppSettings.Get("UserAccessToken");
            string accessToken = ConfigurationManager.AppSettings.Get("UserAccessSecret");
            Auth.SetUserCredentials(consumerKey, consumerSecret, accessKey, accessToken);

            ITweet publishedTweet = Tweet.PublishTweet($"{message} {videoUrl} #NowPlaying", new PublishTweetOptionalParameters
            {
                Coordinates = new Coordinates(latitude, longitude)
            });
            return Ok(Tweet.GetOEmbedTweet(publishedTweet));
        }
    }
}

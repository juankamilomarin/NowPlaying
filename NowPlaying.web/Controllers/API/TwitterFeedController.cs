// =============================================================================================================== 
// <summary>
// The class definition for the Twitter Feed Controller
// </summary>
// ===============================================================================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using NowPlaying.Filters;
using NowPlaying.Models;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace NowPlaying.Controllers.API
{
    [RoutePrefix("api/twitterfeed")]
    [TwitterAuthenticationFilter]
    public class TwitterFeedController : ApiController
    {
        #region Method

        /// <summary>
        /// Get the latests tweets based on user location (latitude and longitude)
        /// </summary>
        /// <param name="latitude">Latitude of user location</param>
        /// <param name="longitude">Longitude of user location</param>
        /// <returns>A list of the latest OEmbed tweets</returns>
        [HttpGet]
        [Route("latestweets")]
        public IHttpActionResult GetLatestTweets(double latitude, double longitude)
        {
            // Force throwing exceptions
            ExceptionHandler.SwallowWebExceptions = false;

            try
            {
                // Setup search parameters
                SearchTweetsParameters searchParameter = new SearchTweetsParameters(Constants.NOWPLAYING_HASHTAG)
                {
                    SearchType = SearchResultType.Recent,   //Recent tweets
                    MaximumNumberOfResults = Convert.ToUInt16(ConfigurationManager.AppSettings.Get("NumberOfLatestTweets")),
                    GeoCode = new GeoCode(latitude, longitude, 100, DistanceMeasure.Kilometers) //Look up in an 100km radius 
                };

                // Perform the search
                IEnumerable<ITweet> tweets = Search.SearchTweets(searchParameter);

                // Return the list of OEmbed tweeds
                // OEmbedTweet contains the HTML to comply with tweets displaying guidance (see https://developer.twitter.com/en/developer-terms/display-requirements.html)
                List<IOEmbedTweet> oEmbeddedTweets = tweets.Select(Tweet.GetOEmbedTweet).ToList();
                return Ok(oEmbeddedTweets);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        /// <summary>
        /// Post a tweet with the NowPlaying hashtag
        /// </summary>
        /// <param name="message">Message to be tweeted</param>
        /// <param name="videoUrl">Video URL associadted with the song</param>
        /// <param name="latitude">Latitude of user location</param>
        /// <param name="longitude">Longitude of user location</param>
        /// <returns>The posted OEmbed tweet</returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult PostTweet(string message, string videoUrl, double latitude, double longitude)
        {
            // Force throwing exceptions
            ExceptionHandler.SwallowWebExceptions = false;

            try
            {
                // Publish the tweet
                ITweet publishedTweet = Tweet.PublishTweet($"{message} {videoUrl} {Constants.NOWPLAYING_HASHTAG}",
                    new PublishTweetOptionalParameters
                    {
                        Coordinates = new Coordinates(latitude, longitude) //Set coordinates
                    });

                // Return oEmbed tweet
                return Ok(Tweet.GetOEmbedTweet(publishedTweet));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        #endregion

    }
}

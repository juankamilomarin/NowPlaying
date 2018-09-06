// =============================================================================================================== 
// <summary>
// The class definition for the Twitter Stream
// </summary>
// ===============================================================================================================

using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using NowPlaying.Hubs;
using NowPlaying.Models;
using NowPlaying.Utilities;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace NowPlaying.Streams
{

    public static class TwitterStream
    {
        #region Fields

        /// Filtered stream
        private static IFilteredStream _stream;

        // SignalR hub context
        private static readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<TwitterFeedHub>();

        #endregion

        #region Methods

        /// <summary>
        /// Starts a Twitter Stream based on the specified geographic coordinates and the now playing hash tag
        /// </summary>
        /// <param name="latitude1">Latitude of user location (bottom_left)</param>
        /// <param name="longitude1">Longitude of user location (bottom_left)</param>
        /// <param name="latitude2">Latitude of user location (top_right)</param>
        /// <param name="longitude2">Longitude of user location (top_right)</param>
        public static async Task StartStream(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            // Setup Twitter credentials
            TweetinviUtilities.SetTwitterCredentials();

            // If the stream does not exists...
            if (_stream == null)
            {
                //...then it is started

                // Create a filtered stream
                _stream = Stream.CreateFilteredStream();
                _stream.AddTrack(Constants.NOWPLAYING_HASHTAG); // Lookup for nowplaying hashtag

                // OPTIONAL: if you want to see how the feed is updated really quick, just comment the following line of code.
                //           You will see the effect of "infinite scroll" in the client
                _stream.AddLocation(
                    new Coordinates(latitude1, longitude1), 
                    new Coordinates(latitude2, longitude2)); // Lookup in the specific geographic coordinates

                // OPTIONAL: if you want to filter the stream just for a specific user, uncomment the following line of code
                //_stream.AddFollow(2834545563);

                // Event that handles a matching tweet
                _stream.MatchingTweetReceived += async (sender, args) =>
                {
                    // A OEmbed tweet is sent to the client
                    IOEmbedTweet embedTweet = Tweet.GetOEmbedTweet(args.Tweet);
                    await _context.Clients.All.updateFeed(embedTweet);
                };

                // Start the stream matching all conditions
                await _stream.StartStreamMatchingAllConditionsAsync();
            }
            else
            {
                //... otherwise resume it
                _stream.ResumeStream();
            }
        }

        #endregion
    }
}
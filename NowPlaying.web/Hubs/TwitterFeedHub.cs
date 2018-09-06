// =============================================================================================================== 
// <summary>
// The class definition for the Twitter Feed Hub
// </summary>
// ===============================================================================================================

using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NowPlaying.Streams;

namespace NowPlaying.Hubs
{
    [HubName("twitterFeedHub")]
    public class TwitterFeedHub : Hub
    {

        #region Methods

        /// <summary>
        /// Starts a Twitter Stream based on the specified geographic coordinates
        /// </summary>
        /// <param name="latitude1">Latitude of user location (bottom_left)</param>
        /// <param name="longitude1">Longitude of user location (bottom_left)</param>
        /// <param name="latitude2">Latitude of user location (top_right)</param>
        /// <param name="longitude2">Longitude of user location (top_right)</param>
        public async Task StartTwitterStream(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            await TwitterStream.StartStream(latitude1, longitude1, latitude2, longitude2);
        }

        #endregion

    }
}
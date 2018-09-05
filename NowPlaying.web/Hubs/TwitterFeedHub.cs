using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Tweetinvi.Streaming;
using Tweetinvi.Streaming.Parameters;


namespace NowPlaying.Hubs
{

    [HubName("twitterFeedHub")]
    public class TwitterFeedHub : Hub
    {
        public async Task StartTwitterLiveWithLocation(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            await TwitterStream.StartStream(latitude1, longitude1, latitude2, longitude2);
        }
    }

    public static class TwitterStream
    {
        private static IFilteredStream _stream;
        private static readonly IHubContext _context = GlobalHost.ConnectionManager.GetHubContext<TwitterFeedHub>();

        public static async Task StartStream(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            string consumerKey = ConfigurationManager.AppSettings.Get("ConsumerKey");
            string consumerSecret = ConfigurationManager.AppSettings.Get("ConsumerSecret");
            string accessKey = ConfigurationManager.AppSettings.Get("UserAccessToken");
            string accessToken = ConfigurationManager.AppSettings.Get("UserAccessSecret");
            Auth.SetUserCredentials(consumerKey, consumerSecret, accessKey, accessToken);

            if (_stream == null)
            {
                _stream = Stream.CreateFilteredStream();
                _stream.AddTrack("#nowplaying");
                _stream.AddLocation(new Coordinates(latitude1, longitude1), new Coordinates(latitude2, longitude2));
                //_stream.AddFollow(1053812688);

                _stream.MatchingTweetReceived += async (sender, args) =>
                {
                    IOEmbedTweet embedTweet = Tweet.GetOEmbedTweet(args.Tweet);
                    await _context.Clients.All.updateTweet(embedTweet);
                };

                await _stream.StartStreamMatchingAllConditionsAsync();
            }
            else
            {
                _stream.ResumeStream();
            }
        }
    }
}
using System.Configuration;
using Tweetinvi;

namespace NowPlaying.Utilities
{
    public static class TweetinviUtilities
    {
        /// <summary>
        /// Set user credentials for Twitter authentication
        /// </summary>
        public static void SetTwitterCredentials()
        {
            string consumerKey = ConfigurationManager.AppSettings.Get("ConsumerKey");
            string consumerSecret = ConfigurationManager.AppSettings.Get("ConsumerSecret");
            string accessKey = ConfigurationManager.AppSettings.Get("UserAccessToken");
            string accessToken = ConfigurationManager.AppSettings.Get("UserAccessSecret");
            Auth.SetUserCredentials(consumerKey, consumerSecret, accessKey, accessToken);
        }
    }
}
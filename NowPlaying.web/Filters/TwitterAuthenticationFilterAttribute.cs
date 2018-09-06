// =============================================================================================================== 
// <summary>
// The class definition for the Twitter Authentication Filter Attribute.
// </summary>
// ===============================================================================================================

using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using NowPlaying.Utilities;

namespace NowPlaying.Filters
{
    public class TwitterAuthenticationFilterAttribute : ActionFilterAttribute
    {

        #region Methods

        /// <summary>
        /// Set user credentials for Twitter authentication
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            // Setup Twitter credentials
            TweetinviUtilities.SetTwitterCredentials();
        }

        #endregion
    }
}
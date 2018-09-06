using Owin;

namespace NowPlaying
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Startup Signal R
            app.MapSignalR();
        }
    }
}
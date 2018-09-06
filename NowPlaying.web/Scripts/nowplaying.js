// =============================================================================================================== 
// <summary>
// Defines the root Now Playing namespace "np".  Includes constants, and root level functions.
// </summary>
// =============================================================================================================== 

"use string";

$(function (np, $, undefined) {

    // <field name="latitudeGlobal" type="double">Lalitude of user location.</field>
    np.userLatitude = 0;

    // <field name="longitudeGlobal" type="double">Longitude of user location.</field>
    np.userLongitude = 0;

    np.init = function() {
        /// <summary>Initializes Now Playing.</summary>

        // Setup user location
        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(

                // Set location by geolocation
                function (position) {

                    // Set latitude and longitud
                    np.userLatitude  = position.coords.latitude;
                    np.userLongitude = position.coords.longitude;

                    // Get latest tweets
                    np.getLatestTweets();
                },

                // Set location by ip address
                setLocationByIpAddress

            );
        } else {

            // Set location by ip address
            setLocationByIpAddress();
        }

    };

    np.getLatestTweets = function() {

        // Get latest tweets based on user location
        $.ajax({
            type: "GET",
            url: `/api/twitterfeed/latestweets?latitude=${np.userLatitude}&longitude=${np.userLongitude}`,
            context: this,
            contentType: "application/json"
        }).then(function(tweets) {

            // Append tweets to feed
            tweets.reverse().forEach(function(tweet) {
                $(tweet.HTML)
                    .hide()
                    .prependTo("#tweetFeed");
            });

            // Setup SignalR hup
            setupHub();
        });
    };

    function setLocationByIpAddress() {
        /// <summary>Set location coordinates based on the user ip address.</summary>

        $.ajax("http://ip-api.com/json")
            .then(
                function (response) {
                    // Set latitude and longitud
                    np.userLatitude = response.lat;
                    np.userLongitude = response.lon;

                    // Get latest tweets
                    np.getLatestTweets();
                }
            );
    }

    function setupHub() {
        /// <summary>Setup SignalR hub.</summary>

        // Get the hub
        const twitterFeedHub = $.connection.twitterFeedHub;

        // Setup update feed event
        twitterFeedHub.client.updateFeed = function (tweet) {

            // Append new tweet to feed
            // OEmbed tweet already contains the HTML that complies with Twitter specifications (see https://developer.twitter.com/en/developer-terms/display-requirements.html)
            $(tweet.HTML)
                .hide()
                .prependTo("#tweetFeed")
                .fadeIn("slow");
        };

        // Start the hub and once it finishes Twitter stream should be started
        $.connection.hub.start().done(startTwitterStream());
    }

    function startTwitterStream() {
        /// <summary>Start Twitter stream to update the feed.</summary>

        // First, user city coordinates should be calcuated. For this, reverse geolocation is applied based on the user latitude and longitude 
        // OpenStreetMap (a free project that provides geographic data) is used for this purpose. 
        // More info about openstreetmap: https://wiki.openstreetmap.org/wiki/Main_Page
        $.ajax({
            type: "GET",
            // Note that zoom level is currently set between city and county level in order to get a bigger area. If you want to use city level just set it to 10
            // More info about the zoom level: https://github.com/openstreetmap/Nominatim/blob/3af8dc95806b2784943926e383b33592a7e93bad/docs/api/Reverse.md
            url: `https://nominatim.openstreetmap.org/reverse?format=json&lat=${np.userLatitude}&lon=${np.userLongitude}&zoom=9`,
            context: this,
            contentType: "application/json"
        }).done(function (nominatimData) {

            // Show the city in the header
            const inCityHeaderDivElement = document.getElementById("headerText");
            inCityHeaderDivElement.innerHTML = `Twitter feed for #NowPlaying in ${nominatimData.address.county}`;
            inCityHeaderDivElement.hidden = false;

            // Once city geographic data is retrieved, bouding box coordinates are sent to start the Twitter Stream
            $.connection.twitterFeedHub.server.startTwitterStream(
                parseFloat(nominatimData.boundingbox[0]), parseFloat(nominatimData.boundingbox[2]),
                parseFloat(nominatimData.boundingbox[1]), parseFloat(nominatimData.boundingbox[3]) 

            // Once the stream is started, the post-tweet form is displayed and setup
            ).done(setupPostTweetForm); 
        });
    }


    function setupPostTweetForm() {
        /// <summary>Setup post-tweet form.</summary>

        // Setup submit event
        $("#tweetData").submit(function (event) {

            // Get message and video url from the form
            const message = document.getElementById("message").value;
            const videoUrl = document.getElementById("videoUrl").value;

            // Post tweet 
            $.ajax({
                type: "POST",
                //url: `/api/twitterfeed/?message=${encodeURIComponent(message)}&videourl=${videoUrl}&latitude=${np.userLatitude}&longitude=${np.userLongitude}`,
                url: `/api/twitterfeed/?message=${message}&videourl=${videoUrl}&latitude=${np.userLatitude}&longitude=${np.userLongitude}`,
                context: this,
                contentType: "application/json"
            });

            event.preventDefault();
        });

        // Show the form
        document.getElementById("postTweetForm").hidden = false;

    }

}(window.np = window.np || {}, jQuery));
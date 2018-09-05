var latitudeGlobal;
var longitudeGlobal;
$(function () {

    $("#tweetData").submit(function (event) {
        const message = document.getElementById("message").value;
        const videoUrl = document.getElementById("videoUrl").value;

        $.ajax({
            type: "POST",
            url: `/api/twitterfeed/?message=${message}&videourl=${videoUrl}&latitude=${latitudeGlobal}&longitude=${longitudeGlobal}`,
            context: this,
            contentType: "application/json"
        });

        event.preventDefault();
    });

    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition(
            function (position) {
                latitudeGlobal = position.coords.latitude;
                longitudeGlobal = position.coords.longitude;
                getLatestTweets(position.coords.latitude, position.coords.longitude);
            },
            function() {
                $.ajax('http://ip-api.com/json')
                    .then(
                        function(response) {
                            getLatestTweets(response.lat, response.lon);
                        }
                    );
            });
    } else {
        $.ajax('http://ip-api.com/json')
            .then(
                function(response) {
                    getLatestTweets(response.lat, response.lon);
                }
            );
    }

    function getLatestTweets(latitude, longitude) {

        $.ajax({
            type: "GET",
            url: `/api/twitterfeed/latestweets?latitude=${latitude}&longitude=${longitude}`,
            context: this,
            contentType: "application/json"
        }).then(function(tweets) {

                tweets.forEach(function(tweet) {
                    $(tweet.HTML)
                        .hide()
                        .prependTo("#tweetFeed");
                });

                const twitterFeedHub = $.connection.twitterFeedHub;

                twitterFeedHub.client.updateTweet = function(tweet) {
                    $(tweet.HTML)
                        .hide()
                        .prependTo("#tweetFeed")
                        .fadeIn("slow");
                };

                $.connection.hub.start().done(setupStream(latitude, longitude));
            });
    }

    function setupStream(latitude, longitude) {
        $.ajax({
            type: "GET",
            url: `https://nominatim.openstreetmap.org/reverse?format=json&lat=${latitude}&lon=${longitude}&zoom=9`,
            context: this,
            contentType: "application/json"
        }).done(function (nominatimData) {
            $.connection.twitterFeedHub.server.startTwitterLiveWithLocation(
                parseFloat(nominatimData.boundingbox[0]), parseFloat(nominatimData.boundingbox[2]),
                parseFloat(nominatimData.boundingbox[1]), parseFloat(nominatimData.boundingbox[3]) 
            );
        });
    }
});
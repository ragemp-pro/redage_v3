using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NeptuneEvo.Accounts.Models;

namespace NeptuneEvo.Utils.Analytics
{
    // More information about API - see https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide
    public class GoogleAnalyticsHelper
    {
        private readonly string endpoint = "https://www.google-analytics.com/collect";
        private readonly string googleVersion = "1";

        public GoogleAnalyticsHelper()
        {

        }
        
        public List<KeyValuePair<string, string>> AddEvent(string action, string label, string ga, int? value = null)
        {
            if (string.IsNullOrEmpty(action))
                return null;

            var rnd = new Random();
            var rand = rnd.Next(1, 100);
                
            var googleClientId =  Convert.ToInt64(DateTime.Now.Ticks / 1000000 * rand).ToString();

            if (ga.Length > 1)
                googleClientId = ga;
            
            var postData = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("v", googleVersion),
                new KeyValuePair<string, string>("tid", Main.ServerSettings.GoogleTrackingId),
                new KeyValuePair<string, string>("cid", googleClientId),
                new KeyValuePair<string, string>("t", "event"),
                new KeyValuePair<string, string>("ec", Main.ServerSettings.GoogleCategory),
                new KeyValuePair<string, string>("ea", action)
            };

            if (label != null)
            {
                postData.Add(new KeyValuePair<string, string>("el", label));
            }

            if (value != null)
            {
                postData.Add(new KeyValuePair<string, string>("ev", value.ToString()));
            }

            return postData;
        }

        public async Task TrackEvent(HttpClient httpClient,
            List<KeyValuePair<string, string>> postData) => await httpClient
            .PostAsync(endpoint, new FormUrlEncodedContent(postData)).ConfigureAwait(false);
    }
}
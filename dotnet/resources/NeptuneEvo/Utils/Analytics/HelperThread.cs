using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NeptuneEvo.Utils.Analytics.Models;

namespace NeptuneEvo.Utils.Analytics
{
    public class HelperThread
    {
        
        private static readonly nLog Log = new nLog("Utils.Analytics.HelperThread");

        private static List<List<KeyValuePair<string, string>>> PostsData = new List<List<KeyValuePair<string, string>>>();
        
        private static List<SiteCurl> SitePostsData = new List<SiteCurl>();
        private static List<string> Urls = new List<string>();

        private static GoogleAnalyticsHelper Helper = new GoogleAnalyticsHelper();
        
        public static void AddEvent(string action, string label, string ga, int? value = null) 
        {
            var postData = Helper.AddEvent(action, label, ga, value);
            
            if (postData != null)
                PostsData.Add(postData);
        } 
        
        public static void AddSend(string router, List<KeyValuePair<string, string>> postData) 
        {
            if (postData != null)
                SitePostsData.Add(new SiteCurl
                {
                    Router = router,
                    PostData = postData
                });
        } 
        public static void AddUrl(string url) 
        {
            Urls.Add(url);
        } 

        public static void Start()
        {
            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Name = "HelperThread";
            thread.Start();
        }
        
        private static async void Worker()
        {
            while (true)
            {
                try
                {
                    if (Urls.Count > 0)
                    {
                        var urls = Urls.ToList();
                        Urls.Clear();
                        using (var httpClient = new HttpClient())
                        {
                            foreach (var url in urls)
                                await TrackEvent(httpClient, url);
                        }
                    }
                    if (PostsData.Count > 0)
                    {
                        var postsData = PostsData.ToList();
                        PostsData.Clear();
                        using (var httpClient = new HttpClient())
                        {
                            foreach (var postData in postsData)
                                await Helper.TrackEvent(httpClient, postData);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Write($"Worker Exception: {e.ToString()}");
                }
            }
        }
        
        public static async Task TrackEvent(HttpClient httpClient, string url) => await httpClient
            .GetAsync(Main.ServerSettings.SiteUrl + url).ConfigureAwait(false);
    }
}
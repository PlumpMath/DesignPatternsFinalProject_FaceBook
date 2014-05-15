using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.YouTube;
using Google.GData.Client;

namespace FaceBookDPatterns
{
    public class YouTubeFacadeSingleton
    {
        private const string k_YouTubeAPIVideoAddress = "http://gdata.youtube.com/feeds/api/videos/";

        // private ref to the single instance
        private static YouTubeFacadeSingleton s_SingleInstance;

        // pulic access point to the single instance
        public static YouTubeFacadeSingleton Instance
        {
            get
            {
                if (s_SingleInstance == null)
                {
                    s_SingleInstance = new YouTubeFacadeSingleton();
                }

                return s_SingleInstance;
            }
        }
        
        private YouTubeRequestSettings m_YouTubeRequestSettings;

        private YouTubeRequest m_YouTubeRequest;
        
        // private ctor since this is a SINGLETON
        private YouTubeFacadeSingleton()
        {
            m_YouTubeRequestSettings = new Google.YouTube.YouTubeRequestSettings("facebookApp", "AI39si5sSEUY7QBk1OMmp4K7QsR-IHFmtVH2mcg6OrFCgjZCYKcmougCjB1ZDbqSe_-PcLSyJakrIF4f2H4V8jENV5Lbgnzaaw");
            m_YouTubeRequest = new Google.YouTube.YouTubeRequest(m_YouTubeRequestSettings);
        }

        // one simple intuitive public access point for the client (thus, FACADE)
        // the client no longer needs to know how to work with Youtube,
        // it just need to give the FACADE a string of the video id, and all the work with Youtube is done here!
        public string GetVideoKeyWords(string i_VideoID)
        {
            string keyWords = string.Empty;

            string videoURI = k_YouTubeAPIVideoAddress + i_VideoID;
            Feed<Video> videoFeed = m_YouTubeRequest.Get<Video>(new Uri(videoURI));

            foreach (Google.YouTube.Video video in videoFeed.Entries)
            {
                keyWords += "," + video.Keywords;
            }

            return keyWords;
        }
    }
}
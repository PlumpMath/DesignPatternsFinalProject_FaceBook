using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace FaceBookDPatterns
{
    public class PostYouTubeAdapter
    {
        private Post m_Post;
        
        private string m_YouTubeID = string.Empty;
        
        public string YouTubeID
        {
            get
            {
                return m_YouTubeID;
            }
        }
        
        private bool m_AdaptationSucceeded;
        
        public bool AdaptationSucceeded
        {
            get
            {
                return m_AdaptationSucceeded;
            }
        }

        public PostYouTubeAdapter(Post i_Post)
        {
            m_Post = i_Post;
            
            // a 'smart' adapter. Upon creation, it check if the subject is Adaptable, and update the bool : "m_AdaptionSucceded"
            // if the adaption succeded. This way, the client no longer needs to know how to check if the post contains YouTube video
            // and no longer needs to know how to convert a Youtube-video-post into a Youtube video id.
            m_AdaptationSucceeded = getYouTubeID();
        }

        // a private function that does all the 'hard work'.
        private bool getYouTubeID()
        {
            bool succeeded = false;
            string videoURL = m_Post.Source;
            string[] parts = videoURL.Split('/', '?');
            string videoID = parts[4];

            if (parts[2] == "www.youtube.com")
            {
                m_YouTubeID = videoID;
                succeeded = true;
            }

 	        return succeeded;
        }
    }
}

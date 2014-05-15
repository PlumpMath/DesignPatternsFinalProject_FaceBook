using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace FaceBookDPatterns
{
    public class PostListBoxAdapter
    {
        public Post Post { get; set; }

        public PostListBoxAdapter(Post i_Post)
        {
            Post = i_Post;
        }

        public override string ToString()
        {
            return "(" + Post.LikesCount + ") " +  Post.From.Name + ":" + Post.Name + "," + Post.Caption + "," + Post.Description + "," + Post.Message;
        }
    }
}

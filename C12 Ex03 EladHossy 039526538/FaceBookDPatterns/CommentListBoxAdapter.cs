using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace FaceBookDPatterns
{
    public class CommentListBoxAdapter
    {
        public Comment Comment;

        public CommentListBoxAdapter(Comment i_Comment)
        {
            Comment = i_Comment;
        }

        public override string ToString()
        {
            return Comment.From.Name + ": " + Comment.Message;
        }
    }
}

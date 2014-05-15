using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;

namespace FaceBookDPatterns
{
    public class UserListViewItem : ListViewItem
    {
        public User User { get; set;  }

        public UserListViewItem(User i_User)
        {
            User = i_User;
        }
    }
}

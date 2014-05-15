using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using FacebookWrapper.ObjectModel;

namespace FaceBookDPatterns
{
    public delegate void AddItemCallback(object obj);
    
    public class UserFriendsListView : ListView
    {
        public User User { get; set; }

        public UserFriendsListView(User i_User, Control i_Parent)
        {
            User = i_User;
            Parent = i_Parent;
            init();
        }

        private void addFriendToListViewAsync(object i_ListView)
        {
            if (this.InvokeRequired)
            {
                AddItemCallback a = new AddItemCallback(addFriendToListViewAsync);
                Invoke(a, new object[] { i_ListView });
            }
            else
            {
                foreach (User friend in User.Friends)
                {
                    PictureBox pb = new PictureBox();
                    pb.Load(friend.PictureSmallURL);
                    Image img = pb.Image;
                    UserListViewItem userListViewItem = new UserListViewItem(friend);
                    userListViewItem.ImageKey = friend.Name;
                    userListViewItem.Name = friend.Name;
                    userListViewItem.Text = friend.Name;
                    this.LargeImageList.Images.Add(friend.Name, img);
                    this.Items.Add(userListViewItem);
                }
            }
        }

        private void init()
        {
            this.View = View.LargeIcon;
            this.LargeImageList = new ImageList();
            this.LargeImageList.ImageSize = new Size(64, 64);
            this.LargeImageList.ColorDepth = ColorDepth.Depth24Bit;
            Thread thread = new Thread(new ParameterizedThreadStart(addFriendToListViewAsync));
            thread.Start(User.Friends);
        }
    }
}

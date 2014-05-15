using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

//// The client (facebook app) is no longer needed to know Google API namespaces!! :)
// using Google.Api.Maps.Service.Geocoding;
// using Google.Api.Maps.Service.StaticMaps;
// using Google.Api.Maps.Service;
// using Google.YouTube;
// using Google.GData.Client;
////

namespace FaceBookDPatterns
{
    public partial class FaceBookAppMainForm : Form
    {
        private User m_User;
        private LoginResult m_LoginResult;
        private AppSettings m_AppSettings = new AppSettings();
        private bool m_LoginAndDataFetchCompleted = false;

        // getting a ref to the Google Maps FACADE singleton & YouTube FACADE singleton
        private GoogleMapsFacadeSingleton mapsFacade = GoogleMapsFacadeSingleton.Instance;
        private YouTubeFacadeSingleton youtubeFacade = YouTubeFacadeSingleton.Instance;

        /* Creating 3 Observables Progress Bars (implementing the OBSERVER pattern):
         *  a.  for the Checkins around me feature
         *  b.  for the Music search feature
         *  c.  for the login and fetching when the app is loading
         */
        private ObservableProgressBar OprogressBarCheckinsAroundMe;
        private ObservableProgressBar OprogressBarMusicSearch;
        private ObservableProgressBar OprogressBarLoginAndFetchData;

        /* Creating 2 Composite Observable Progress Bars (implementing the COMPOSITE & OBSRVER patterns):
         *  1. one that will observe observable-bars a and b (from above) (== observe the features)
         *  2. one that will observe the first composite observable bar, and c. (== observe the features and login bar)
         *  */

        private CompositeObservableProgressBar COprogressBarFeatures;
        private CompositeObservableProgressBar COprogressBarOnLoad;
         
        public FaceBookAppMainForm()
        {
            InitializeComponent();
            
            OprogressBarCheckinsAroundMe = new ObservableProgressBar(progressBarCheckinsAroundMe);
            OprogressBarMusicSearch = new ObservableProgressBar(progressBarSearchMusic);
            OprogressBarLoginAndFetchData = new ObservableProgressBar(progressBarLoginAndFetchData);
            
            COprogressBarFeatures = new CompositeObservableProgressBar(
                new List<IObservableProgressBar>
            {
                OprogressBarCheckinsAroundMe,
                OprogressBarMusicSearch,
            }, 
            progressBarTotalAppLoad);

            COprogressBarOnLoad = new CompositeObservableProgressBar(
                new List<IObservableProgressBar>
            {
                COprogressBarFeatures,
                OprogressBarLoginAndFetchData
            }, 
            null);
        }

        private void manualLogin()
        {
            m_LoginResult = FacebookService.Login(
                "451136558241810",
                "user_about_me", 
                "friends_about_me",
                "read_stream", 
                "publish_stream",
                "user_status",
                "user_activities", 
                "friends_activities",
                "user_birthday", 
                "friends_birthday",
                "user_checkins", 
                "friends_checkins",
                "user_education_history", 
                "friends_education_history",
                "user_events", 
                "friends_events",
                "user_groups", 
                "friends_groups",
                "user_hometown", 
                "friends_hometown",
                "user_interests", 
                "friends_interests",
                "user_likes", 
                "friends_likes",
                "user_location", 
                "friends_location",
                "user_notes", 
                "friends_notes",
                "user_online_presence", 
                "friends_online_presence",
                "user_photo_video_tags", 
                "friends_photo_video_tags",
                "user_photos", 
                "friends_photos",
                "user_photos", 
                "friends_photos",
                "user_relationships", 
                "friends_relationships",
                "user_relationship_details", 
                "friends_relationship_details",
                "user_religion_politics", 
                "friends_religion_politics",
                "user_status", 
                "friends_status",
                "user_videos", 
                "friends_videos",
                "user_website", 
                "friends_website",
                "user_work_history", 
                "friends_work_history",
                "email",
                "read_friendlists",
                "read_insights",
                "read_mailbox",
                "read_requests",
                "read_stream",
                "xmpp_login",
                "create_event",
                "rsvp_event",
                "sms",
                "publish_checkins",
                "manage_friendlists",
                "manage_pages");

            if (string.IsNullOrEmpty(m_LoginResult.ErrorMessage))
            {
                m_User = m_LoginResult.LoggedInUser;
            }
            else
            {
                MessageBox.Show(m_LoginResult.ErrorMessage);
            }

            new Thread(afterLogin).Start();
        }
        
        private void autoLogin()
        {
            m_LoginResult = FacebookService.Connect(m_AppSettings.AccessToken);
            if (string.IsNullOrEmpty(m_LoginResult.ErrorMessage))
            {
                m_User = m_LoginResult.LoggedInUser;
            }

            Invoke(new Action(() => OprogressBarLoginAndFetchData.IncrementByOne()));

            afterLogin();
        }

        private void afterLogin()
        {
            initEventsAroundMe();
            initMeTab();
            initFriendsPanel();
        }

        private void initEventsAroundMe()
        {
            Invoke(new Action(() => textBoxMyLocation.Text = m_User.Location.Name), null);
        }

        private void initFriendsPanel()
        {
            var userFriends = m_User.Friends;

            Invoke(
                new Action(() =>
                {
                    listViewFriends.View = View.LargeIcon;
                    listViewFriends.LargeImageList.ImageSize = new Size(64, 64);
                    listViewFriends.LargeImageList.ColorDepth = ColorDepth.Depth24Bit;
                    OprogressBarLoginAndFetchData.Maximum = m_User.Friends.Count + OprogressBarLoginAndFetchData.Value;
                }), 
                null);

            foreach (User friend in m_User.Friends)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Load(friend.PictureSmallURL);
                Image friendImage = pictureBox.Image;
                UserListViewItem userListViewItem = new UserListViewItem(friend);
                userListViewItem.ImageKey = friend.Name;
                userListViewItem.Name = friend.Name;
                userListViewItem.Text = friend.Name;
                Invoke(new Action(() =>
                {
                    listViewFriends.LargeImageList.Images.Add(friend.Name, friendImage);
                    listViewFriends.Items.Add(userListViewItem);
                    OprogressBarLoginAndFetchData.IncrementByOne();
                }));
            }

            Invoke(new Action(() =>
                {
                    OprogressBarLoginAndFetchData.Reset();
                    m_LoginAndDataFetchCompleted = true;
                }));
        }
        
        private void initMeTab()
        {
            pictureBoxUserPicture.Load(m_User.PictureLargeURL);
            Invoke(
                new Action(() =>
                {
                    labelUserNameInTab.Text = m_User.Name;
                    label1.Text = m_User.Location == null ? string.Empty : m_User.Location.Name;
                    label2.Text = m_User.Birthday;
                    label3.Text = m_User.Bio;
                    label4.Text = m_User.RelationshipStatus.ToString();
                }), 
                null);

            initNewsFeeds();
            initWall();
        }

        private void initWall()
        {
            FacebookObjectCollection<Post> wallPosts = m_User.WallPosts;
            Invoke(
                new Action(() =>
                {
                    foreach (Post post in wallPosts)
                    {
                        listBoxWallPosts.Items.Add(new PostListBoxAdapter(post));
                    }

                    OprogressBarLoginAndFetchData.IncrementByOne();
                }), 
                null);
        }

        private FacebookObjectCollection<User> m_UserFriends; 
        
        private FilterableCollection<PostListBoxAdapter> m_FilterableNewsFeeds;
        
        private void initNewsFeeds()
        {
            FacebookObjectCollection<PostListBoxAdapter> newsFeeds = new FacebookObjectCollection<PostListBoxAdapter>();
            foreach (Post post in m_User.NewsFeed)
            {
                newsFeeds.Add(new PostListBoxAdapter(post));
            }

            m_FilterableNewsFeeds = new FilterableCollection<PostListBoxAdapter>(newsFeeds);
            m_UserFriends = m_User.Friends;
            Invoke(
                new Action(() =>
            {
                foreach (User friend in m_UserFriends)
                {
                    comboBoxFriendsNames.Items.Add(friend.Name);
                }

                string[] types = Enum.GetNames(typeof(Post.eType));
                foreach (string type in types)
                {
                    comboBoxPostTypes.Items.Add(type);
                }

                BindingSource binding = new BindingSource();
                binding.DataSource = newsFeeds;
                listBoxNewsFeeds.DataSource = binding;

                OprogressBarLoginAndFetchData.IncrementByOne();
            }), 
            null);
        }

        private void listBoxNewsFeeds_SelectedIndexChanged(object sender, EventArgs e)
        {
            PostListBoxAdapter selectedNewsFeed = (sender as ListBox).SelectedItem as PostListBoxAdapter;

            if (selectedNewsFeed != null)
            {
                updatePostVideoOrPhoto(selectedNewsFeed.Post, pictureBoxMePanel, webBrowserMePanel);
            }
        }

        private void updatePostVideoOrPhoto(Post i_Post, PictureBox i_PictureBox, WebBrowser i_WebBrowser)
        {
            if (i_Post.Type == Post.eType.video)
            {
                i_PictureBox.Hide();
                i_WebBrowser.Show();
                if (i_Post.Source != null)
                {
                    i_WebBrowser.Url = new Uri(i_Post.Source);
                }
            }
            else
            {
                i_WebBrowser.Hide();
                i_PictureBox.Hide();
                if (i_Post.PictureURL != null)
                {
                    i_WebBrowser.Url = new Uri(i_Post.PictureURL);                                        
                    string pictureURL = i_Post.PictureURL;
                    string enlargedPictureUrl = pictureURL.Replace("_s", "_n");
                    i_PictureBox.LoadAsync(enlargedPictureUrl);
                    i_PictureBox.Show();
                }
            }
        }

        private void listViewFriends_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as ListView).SelectedItems.Count > 0)
            {
                listBoxFriendStatuses.Items.Clear();
                listBoxFriendEvents.Items.Clear();
                listBoxFriendCheckIns.Items.Clear();
                listBoxStatusComments.Items.Clear();
                labelEventName.Text = String.Empty;
                labelEventLocation.Text = String.Empty;
                labelEventTime.Text = String.Empty;
                pictureBoxEventPicture.Hide();
                pictureBoxCheckinsMap.Hide();

                UserListViewItem userListViewItem = (sender as ListView).SelectedItems[0] as UserListViewItem;
                User friend = userListViewItem.User;
                pictureBoxFriendLarge.LoadAsync(friend.PictureLargeURL);
                labelFriendName.Text = friend.Name;
                labelFriendLocation.Text = friend.Location == null ? string.Empty : "Location: " + friend.Location.Name;
                labelFriendBirthday.Text = friend.Birthday == null ? string.Empty : "Birthday: " + friend.Birthday;
                labelFriendRelationshipStatus.Text = friend.RelationshipStatus == null ? string.Empty : "Relationship Status: " + friend.RelationshipStatus;
                labelFriendIntrestedIn.Text = friend.InterestedIn == null ? string.Empty : "Interested In: " + friend.InterestedIn.ToString();
                labelFriendBio.Text = friend.Bio == null ? string.Empty : "Bio: " + friend.Bio;

                new Thread(() =>
                    {
                        var friendStatuses = friend.Statuses;
                        var friendEvents = friend.Events;
                        var friendCheckins = friend.Checkins;
                        
                        foreach (Status status in friendStatuses)
                        {
                            Invoke(new Action(() => listBoxFriendStatuses.Items.Add(status)));
                        }
                
                        foreach (Event friendEvent in friendEvents)
                        {
                            Invoke(new Action(() => listBoxFriendEvents.Items.Add(friendEvent)));
                        }
           
                        foreach (Checkin checkin in friendCheckins)
                        {
                            Invoke(new Action(() => listBoxFriendCheckIns.Items.Add(new CheckInListBoxAdapter(checkin))));
                        }
                    }).Start();
            }
        }

        private void listBoxFriendEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            Event selectedEvent = (sender as ListBox).SelectedItem as Event;
            labelEventName.Text = selectedEvent.Name;
            labelEventLocation.Text = selectedEvent.Location;
            labelEventTime.Text = selectedEvent.StartTime.ToString();
            if (selectedEvent.PictureLargeURL != String.Empty)
            {
                pictureBoxEventPicture.Show();
                pictureBoxEventPicture.LoadAsync(selectedEvent.PictureLargeURL);
            }
        }

        private void listBoxFriendCheckIns_SelectedIndexChanged(object sender, EventArgs e)
        {
            Checkin checkin = ((sender as ListBox).SelectedItem as CheckInListBoxAdapter).Checkin;

            // use of 3 design patterns : use of the checkin-ADAPTER for a parameter for the maps-FACADE-SINGLETON
            string url = mapsFacade.GetMap(new GoogleMapsCheckinAdapter(checkin).ToCoordinates());

            pictureBoxCheckinsMap.Show();
            pictureBoxCheckinsMap.LoadAsync(url);
        }

        private void buttonCheckinsAroundMe_Click(object sender, EventArgs e)
        {
            FindCheckinsAroundMe();
        }
        
        private void FindCheckinsAroundMe()
        {
            if (m_LoginAndDataFetchCompleted)
            {
                OprogressBarCheckinsAroundMe.Maximum = m_User.Friends.Count;
                listBoxCheckinsAroundMe.Items.Clear();

                // use of 2 design patterns : get the locality by using the maps-facade-singleton
                new Thread(() =>
                    {
                        List<Checkin> checkinsAroundMe = new List<Checkin>();
                        string userCity = mapsFacade.GetCityName(textBoxMyLocation.Text);

                        foreach (User friend in m_User.Friends)
                        {
                            foreach (Checkin checkin in friend.Checkins)
                            {
                                // use of 3 design patterns : use of the checkin-adapter for a parameter for the maps-facade-singleton
                                string checkinCity =
                                    mapsFacade.GetCityName(new GoogleMapsCheckinAdapter(checkin).ToCoordinates());

                                if (checkinCity == userCity)
                                {
                                    checkinsAroundMe.Add(checkin);
                                }
                            }

                            Invoke(new Action(() => OprogressBarCheckinsAroundMe.IncrementByOne()));
                        }

                        Invoke(
                            new Action(() =>
                            {
                                foreach (Checkin checkin in checkinsAroundMe)
                                {
                                    listBoxCheckinsAroundMe.Items.Add(new CheckInListBoxAdapter(checkin));
                                }

                                OprogressBarCheckinsAroundMe.Reset();
                                if (listBoxCheckinsAroundMe.Items.Count > 0)
                                {
                                    listBoxCheckinsAroundMe.SelectedItem = listBoxCheckinsAroundMe.Items[0];
                                }
                            }), 
                            null);
                    }).Start();
            }
            else
            {
                showWaitMessage();
            }
        }

        private void listBoxCheckinsAroundMe_SelectedIndexChanged(object sender, EventArgs e)
        {
            Checkin checkin = ((sender as ListBox).SelectedItem as CheckInListBoxAdapter).Checkin;
            
            // use of 3 design patterns : use of the checkin-ADAPTER for a parameter for the maps-FACADE-SINGLETON
            string url = mapsFacade.GetMap(new GoogleMapsCheckinAdapter(checkin).ToCoordinates());
            
            pictureBoxCheckInAroundMe.LoadAsync(url);
            pictureBoxCheckInFriend.LoadAsync(checkin.From.PictureLargeURL);
            labelCheckInPlace.Text = checkin.Place.Name + "," + checkin.Place.Location.City + "," + checkin.Place.Location.Country;
            labelCheckInMsg.Text = checkin.Message;
            labelCheckInFriend.Text = checkin.From.Name;
            labelCheckinTime.Text = checkin.CreatedTime.ToString();
        }

        private void buttonSearchForMusic_Click(object sender, EventArgs e)
        {
            searchForMusic();
        }

        private void searchForMusic()
        {
            if (m_LoginAndDataFetchCompleted)
            {
                listBoxAllVideos.Items.Clear();
                int maxResults = (int)numericUpDownMaxResults.Value;
                List<Post> friendsVideos = null;
                new Thread(() =>
                {
                    friendsVideos = getAllFriendsVideos(maxResults);

                    if (radioButtonFiltered.Checked)
                    {
                        friendsVideos = filterVideoPosts(friendsVideos);
                    }

                    Invoke(new Action(() => populateListBoxWithVideos(friendsVideos)));
                }).Start();
            }
            else
            {
                showWaitMessage();
            }
        }

        private void showWaitMessage()
        {
            MessageBox.Show("Please wait for connection to complete");
        }

        private void populateListBoxWithVideos(List<Post> i_AllFriendsVideos)
        {
            foreach (Post post in i_AllFriendsVideos)
            {
               listBoxAllVideos.Items.Add(post);
            }
        }

        private List<Post> getAllFriendsVideos(int i_Limit)
        {
            Invoke(
                new Action(() =>
                {
                    OprogressBarMusicSearch.Maximum = i_Limit;
                    OprogressBarMusicSearch.Reset();
                }), 
                null);
            
            List<Post> allVideos = new List<Post>();
            foreach (User friend in m_User.Friends)
            {
                foreach (Post post in friend.Posts)
                {
                    if (post.Type == Post.eType.video)
                    {
                        allVideos.Add(post);
                        Invoke(new Action(() => OprogressBarMusicSearch.IncrementByOne()));
                        
                        if (allVideos.Count >= i_Limit)
                        {
                            Invoke(
                                new Action(() =>
                                {
                                    OprogressBarMusicSearch.FinishProgress();
                                    OprogressBarMusicSearch.Reset();
                                }), 
                                null);
                            return allVideos;
                        }
                    }
                }
            }

            if (allVideos.Count < i_Limit)
            {
                Invoke(
                    new Action(() =>
                    {
                        OprogressBarMusicSearch.FinishProgress();
                        OprogressBarMusicSearch.Reset();
                    }), 
                    null);
            }

            return allVideos;
        }

        private void buttonAddVideoKey_Click(object sender, EventArgs e)
        {
            if (textBoxAddVideoKey.Text != string.Empty)
            {
                checkedListBoxAllVideoKeys.Items.Add(textBoxAddVideoKey.Text, true);
            }
        }

        private List<Post> filterVideoPosts(List<Post> videoPosts)
        {
            List<Post> filteredPosts = new List<Post>();

            // determine keys for filter
            List<string> allKeys = new List<string>();

            if (checkBoxAlternative.Checked)
            {
                allKeys.Add("Alternative");
            }

            if (checkBoxBlues.Checked)
            {
                allKeys.Add("Blues");
            }

            if (checkBoxJazz.Checked)
            {
                allKeys.Add("Jazz");
            }

            if (checkBoxPop.Checked)
            {
                allKeys.Add("Pop");
            }

            if (checkBoxRap.Checked)
            {
                allKeys.Add("Rap");
            }

            if (checkBoxRock.Checked)
            {
                allKeys.Add("Rock");
            }

            foreach (string key in checkedListBoxAllVideoKeys.Items)
            {
                allKeys.Add(key);
            }

            foreach (Post post in videoPosts)
            {
                string videoKeyWords = string.Empty;

                // use of ADAPTER design pattern:
                PostYouTubeAdapter postYouTubeAdapter = new PostYouTubeAdapter(post);
                
                if (postYouTubeAdapter.AdaptationSucceeded)
                {
                    // use of 3 design patterns : use of the 'post-ADAPTER-to-youtube-video-id' for a parameter for the youtube-FACADE-SINGLETON
                    videoKeyWords = youtubeFacade.GetVideoKeyWords(postYouTubeAdapter.YouTubeID);
                }

                if (videoKeyWords != string.Empty)
                {
                    foreach (string key in allKeys)
                    {
                        if (videoKeyWords.ToLower().Contains(key.ToLower()))
                        {
                            filteredPosts.Add(post);
                        }
                    }
                }
            }

            return filteredPosts;
        }
            
        private void radioButtonAll_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxFilterSettings.Visible = false;
        }

        private void radioButtonFiltered_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxFilterSettings.Visible = true;
        }

        private void buttonDeleteSelectedItems_Click(object sender, EventArgs e)
        {
            List<string> itemsToClear = new List<string>();
            foreach (string key in checkedListBoxAllVideoKeys.CheckedItems)
            {
                itemsToClear.Add(key);
            }

            foreach (string key in itemsToClear)
            {
                checkedListBoxAllVideoKeys.Items.Remove(key);
            }
        }

        private void buttonClearKeys_Click(object sender, EventArgs e)
        {
            checkedListBoxAllVideoKeys.Items.Clear();
        }

        private void listBoxAllVideos_SelectedIndexChanged(object sender, EventArgs e)
        {
            Post videoPost = (sender as ListBox).SelectedItem as Post;
            webBrowserFriendYouTube.Url = new Uri(videoPost.Source);
            pictureBoxVideoPublisherFriend.LoadAsync(videoPost.From.PictureLargeURL);
            labelVideoPublisherFriend.Text = videoPost.From.Name;
            labelPostName.Text = videoPost.Name;
            labelPostMessage.Text = videoPost.Message;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            m_AppSettings.LoadFromFile();
            
            // here we gonna start observing all the progress bars:
            setObservationScenario();

            if (m_AppSettings.AutoLogin && m_AppSettings.AccessToken != null)
            {
                new Thread(autoLogin).Start();
            }
            else
            {
                manualLogin();
            }
        }

        private void setObservationScenario()
        {
            if (m_AppSettings.AutoStartFeatures || checkBoxAutoStartFeatures.Checked) // if the user chose to auto start the features
            {
                OprogressBarMusicSearch.Finished += new Action(() =>
                {
                    DialogResult result = MessageBox.Show("Music search is finised!\nGoThere?", "message", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        tabControlMain.SelectTab(tabPageMusicSearch);
                    }
                });

                OprogressBarCheckinsAroundMe.Finished += new Action(() =>
                {
                    DialogResult result = MessageBox.Show("Checkins Around Me Has Finished!\nGoThere?", "message", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        tabControlMain.SelectTab(tabPageCheckinsAroundMe);
                    }
                });

                // when the login & features completed, a "Victory" sound will play... :)
                COprogressBarOnLoad.Finished += new Action(() =>
                    {
                        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer(assembly.GetManifestResourceStream("FaceBookDPatterns.tada.wav"));
                        player.Play();
                        COprogressBarOnLoad.RemoveIOvservedPorgressBar(COprogressBarFeatures);
                        COprogressBarOnLoad.RemoveIOvservedPorgressBar(OprogressBarLoginAndFetchData);
                    });
                
                // when the login completes, its time to auto start the features:
                OprogressBarLoginAndFetchData.Finished += new Action(() =>
                    {
                       m_LoginAndDataFetchCompleted = true;
                       FindCheckinsAroundMe();
                       searchForMusic();
                    });
            }
            else // if the user chose not to auto start the features
            {
                OprogressBarLoginAndFetchData.Finished += new Action(() => MessageBox.Show("Completed Fetching data from Facebook.. enjoy!"));
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            m_AppSettings.AccessToken = m_LoginResult.AccessToken;
            m_AppSettings.AutoLogin = checkBoxAutoLogin.Checked;
            m_AppSettings.AutoStartFeatures = checkBoxAutoStartFeatures.Checked;
            m_AppSettings.SaveToFile();
        }

        private void listBoxWallPosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            PostListBoxAdapter selectedWallPost = (sender as ListBox).SelectedItem as PostListBoxAdapter;
            updatePostVideoOrPhoto(selectedWallPost.Post, pictureBoxMePanel, webBrowserMePanel);
        }

        private void listBoxFriendStatuses_SelectedIndexChanged(object sender, EventArgs e)
        {
            Status selectedStatus = (sender as ListBox).SelectedItem as Status;

            listBoxStatusComments.Items.Clear();
   
            foreach (Comment comment in selectedStatus.Comments)
            {
                listBoxStatusComments.Items.Add(new CommentListBoxAdapter(comment));
            }
        }

        private void listBoxStatusComments_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommentListBoxAdapter commentAdapter = (sender as ListBox).SelectedItem as CommentListBoxAdapter;
            pictureBox2.LoadAsync(commentAdapter.Comment.From.PictureLargeURL);
        }
         
        private void comboBoxFriendsNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_FilterableNewsFeeds.Reset();
            string selectedFriendName = (sender as ComboBox).SelectedItem as string;
            
            // passing the STRATEGY
            m_FilterableNewsFeeds.Filter((postAdapter) => { return postAdapter.Post.From.Name.Equals(selectedFriendName); });
            updateNewsFeedsListBox();
        }

        private void updateNewsFeedsListBox()
        {
            // reset the binding, to update the UI
            (listBoxNewsFeeds.DataSource as BindingSource).ResetBindings(false);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            m_FilterableNewsFeeds.Reset();
            long newValue = (long)(sender as NumericUpDown).Value;
            m_FilterableNewsFeeds.Filter((postAdapter) => postAdapter.Post.LikesCount >= newValue );
            updateNewsFeedsListBox();
        }

        private void comboBoxPostTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_FilterableNewsFeeds.Reset();
            string chosenType = (string)(sender as ComboBox).SelectedItem;
            m_FilterableNewsFeeds.Filter((postAdapter) => 
                postAdapter.Post.Type.Equals(Enum.Parse(typeof(Post.eType), chosenType)));
            updateNewsFeedsListBox();
        }

        private void buttonResetNewsFeeds_Click(object sender, EventArgs e)
        {
            m_FilterableNewsFeeds.Reset();
            updateNewsFeedsListBox();
        }
    }
}

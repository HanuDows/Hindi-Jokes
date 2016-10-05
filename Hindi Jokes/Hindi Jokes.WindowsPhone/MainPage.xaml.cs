using HanuDowsFramework;
using Hindi_Jokes.Common;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Hindi_Jokes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private int index, maxCount;
        private HanuDowsApplication hanuDowsApp;
        private ObservablePost _postData = new ObservablePost(new Post());

        public ObservablePost PostData
        {
            get { return _postData; }
        }

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            index = 0;
            hanuDowsApp = HanuDowsApplication.getInstance();
            maxCount = PostManager.getInstance().PostList.Count;
            if (maxCount > 0)
            {
                _postData.setPost(PostManager.getInstance().PostList[index]);
            }
            
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null && e.PageState.ContainsKey("PostIndex"))
            {
                index = (int)e.PageState["PostIndex"];
            }

            if (PostManager.getInstance().PostList.Count <= 0)
            {
                hanuDowsApp.GetAllPosts();
                maxCount = PostManager.getInstance().PostList.Count;
            }

        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["PostIndex"] = index;
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            if (PostManager.getInstance().PostList.Count <= 0)
            {
                hanuDowsApp.GetAllPosts();
                maxCount = PostManager.getInstance().PostList.Count;
                index = 0;
            }

            //showPostOnUI();

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareData;
            
        }

        private void ShareData(DataTransferManager sender, DataRequestedEventArgs args)
        {
            try
            {
                string content = _postData.Content;
                content += "\n\n ~via ayansh.com/hj";

                DataRequest request = args.Request;
                var deferral = request.GetDeferral();
                request.Data.Properties.Title = _postData.Title;
                request.Data.SetText("\n\n" + content);

                deferral.Complete();
            }
            catch (Exception ex)
            {
                // What to do?
            }
        }

        private void Previous_Joke_Click(object sender, RoutedEventArgs e)
        {
            if (index == 0)
            {
                index = maxCount - 1;
            }
            else
            {
                index--;
            }
            
            showPostOnUI();
        }

        private void New_Joke_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UploadPost));
        }

        private void Share_Joke_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void Next_Joke_Click(object sender, RoutedEventArgs e)
        {
            if (index == maxCount - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }

            showPostOnUI();
        }

        private void ShowHelp(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("title", "Help:");
            data.Add("file", "help.html");
            Frame.Navigate(typeof(HTMLDisplayPage), data);
        }

        private void ShowAboutUs(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("title", "About Us:");
            data.Add("file", "about.html");
            Frame.Navigate(typeof(HTMLDisplayPage), data);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void showPostOnUI()
        {
            Post post = PostManager.getInstance().PostList[index];
            _postData.setPost(post);
            //_postData.DataChanged();
        }

        #endregion
    }
}

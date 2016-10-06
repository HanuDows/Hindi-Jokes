using HanuDowsFramework;
using Hindi_Jokes.Common;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Hindi_Jokes
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservablePost _postData = ObservablePost.getInstance();

        public ObservablePost PostData
        {
            get { return _postData; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="Common.NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (e.PageState != null && e.PageState.ContainsKey("PostIndex"))
            {
                _postData.CurrentIndex = (int)e.PageState["PostIndex"];
            }

        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="Common.SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="Common.NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["PostIndex"] = _postData.CurrentIndex;
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);

            _postData.Reset();
            //showPostOnUI();

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareData;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void nextJoke_Click(object sender, RoutedEventArgs e)
        {
            _postData.NextPost();
            //showPostOnUI();

        }

        private void New_Joke_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UploadPost));
        }

        private void Share_Joke_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
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

        private void previousJoke_Click(object sender, RoutedEventArgs e)
        {
            _postData.PreviousPost();
            //showPostOnUI();

        }

        private void showPostOnUI()
        {
            //postTitle.Text = _postData.Title;
            //postMeta.Text = _postData.MetaData;
            //postView.Text = _postData.Content;
        }

    }
}

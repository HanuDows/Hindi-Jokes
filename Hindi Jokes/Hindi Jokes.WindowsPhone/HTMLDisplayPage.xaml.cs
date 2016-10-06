using HanuDowsFramework;
using Hindi_Jokes.Common;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Hindi_Jokes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HTMLDisplayPage : Page
    {
        private NavigationHelper navigationHelper;

        public HTMLDisplayPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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

            string title = "";

            if (e.Parameter == null)
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                title = localSettings.Values["ToastMessageTitle"].ToString();
                string content = localSettings.Values["ToastMessageContent"].ToString();

                string htmlText = "<html><head></head><body>" + 
                        "<h2>" + title + "</h2>" + 
                        "<p>" + content + "</p>" + 
                        "</body></html>";

                webView.NavigateToString(htmlText);

            }
            else
            {
                string fileName;
                Dictionary<string, string> data = (Dictionary<string, string>)e.Parameter;
                data.TryGetValue("file", out fileName);
                data.TryGetValue("title", out title);

                pageTitle.Text = title;

                string htmlFile = "ms-appx-web:///Assets/" + fileName;
                webView.Navigate(new Uri(htmlFile));

            }

            // If this is EULA, then show command bar as well.
            if (title.Equals("EULA"))
            {
                commandBar.Visibility = Visibility.Visible;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void OnAccepted(object sender, RoutedEventArgs e)
        {
            commandBar.Visibility = Visibility.Collapsed;

            // Show progress bar
            progressBar.IsIndeterminate = true;
            progressBar.Visibility = Visibility.Visible;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["EULA"] = "Accepted";

            // Now navigate to main page
            await HanuDowsApplication.getInstance().InitializeApplication();

            progressBar.IsIndeterminate = false;
            progressBar.Visibility = Visibility.Collapsed;

            Frame.Navigate(typeof(MainPage));
            Frame.BackStack.RemoveAt(0);

        }

        private void OnRejected(object sender, RoutedEventArgs e)
        {
            commandBar.Visibility = Visibility.Collapsed;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["EULA"] = "Rejected";
            Application.Current.Exit();
            //this.navigationHelper.GoBack();

        }
    }
}

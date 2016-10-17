using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using HanuDowsFramework;
using Windows.ApplicationModel.DataTransfer;

namespace Hindi_Jokes.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _postTitle = "Post Title will come here";
                _postMeta = "Meta will come here";
                _postContent = "Content will come here";
            }

        }

        string _postTitle, _postMeta, _postContent;
        public string PostTitle { get { return _postTitle; } set { Set(ref _postTitle, value); } }
        public string PostMeta { get { return _postMeta; } set { Set(ref _postMeta, value); } }
        public string PostContent { get { return _postContent; } set { Set(ref _postContent, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                //Value = suspensionState[nameof(Value)]?.ToString();
            }

            // Read Posts from DB
            HanuDowsApplication.getInstance().ReadPostsFromDB(false);

            ObservablePost op = ObservablePost.Instance();
            op.Reset();
            PostTitle = op.PostTitle;
            PostMeta = op.PostMeta;
            PostContent = op.PostContent;

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareData;

            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                //suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void UploadPost() =>
            NavigationService.Navigate(typeof(Views.UploadPost), 0);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoHelp() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        public void GotoOurApps() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 3);

        public void PreviousPost()
        {
            ObservablePost op = ObservablePost.Instance();
            op.PreviousPost();

            PostTitle = op.PostTitle;
            PostMeta = op.PostMeta;
            PostContent = op.PostContent;
        }

        public void NextPost()
        {
            ObservablePost op = ObservablePost.Instance();
            op.NextPost();

            PostTitle = op.PostTitle;
            PostMeta = op.PostMeta;
            PostContent = op.PostContent;
        }

        public void SharePost()
        {
            DataTransferManager.ShowShareUI();
        }

        private void ShareData(DataTransferManager sender, DataRequestedEventArgs args)
        {
            try
            {
                string content = PostContent;
                content += "\n\n ~via ayansh.com/hj";

                DataRequest request = args.Request;
                var deferral = request.GetDeferral();
                request.Data.Properties.Title = PostTitle;
                request.Data.SetText("\n\n" + content);

                deferral.Complete();
            }
            catch (Exception ex)
            {
            }
        }

    }
}
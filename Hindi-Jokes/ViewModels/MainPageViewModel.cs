using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using HanuDowsFramework;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.Storage;
using Windows.Storage.Streams;

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
                _textVisibility = Visibility.Visible;
                _memeVisibility = Visibility.Collapsed;
            }

        }

        private Visibility _memeVisibility, _textVisibility;
        public Visibility MemeVisibility { get { return _memeVisibility; } set { Set(ref _memeVisibility, value); } }
        public Visibility TextVisibility { get { return _textVisibility; } set { Set(ref _textVisibility, value); } }

        private Uri _memeContent;
        public Uri MemeContent { get { return _memeContent; } set { Set(ref _memeContent, value); } }

        string _postTitle, _postMeta, _postContent;
        public string PostTitle { get { return _postTitle; } set { Set(ref _postTitle, value); } }
        public string PostMeta { get { return _postMeta; } set { Set(ref _postMeta, value); } }
        public string PostContent { get { return _postContent; } set { Set(ref _postContent, value); } }

        private ObservablePost op;
        private StorageFile _image_file;

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendView("MainPage");

            if (suspensionState.Any())
            {
                //Value = suspensionState[nameof(Value)]?.ToString();
            }

            // Read Posts from DB
            HanuDowsApplication.getInstance().ReadPostsFromDB(false);

            op = ObservablePost.Instance();
            op.Reset();
            __setPostData();

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
            op = ObservablePost.Instance();
            op.PreviousPost();
            __setPostData();
        }

        public void NextPost()
        {
            op = ObservablePost.Instance();
            op.NextPost();
            __setPostData();
        }

        private async void __setPostData()
        {
            PostTitle = op.PostTitle;
            PostMeta = op.PostMeta;
            PostContent = op.PostContent;

            if (op.HasCategory("Meme"))
            {
                MemeVisibility = Visibility.Visible;
                TextVisibility = Visibility.Collapsed;

                // Prepare Image URI
                try
                {
                    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                    StorageFolder imageFolder = await storageFolder.GetFolderAsync("images");
                    StorageFolder postFolder = await imageFolder.GetFolderAsync(op.PostID.ToString());

                    IReadOnlyList<StorageFile> image_files = await postFolder.GetFilesAsync();
                    _image_file = image_files.ElementAt(0);

                    MemeContent = new Uri(_image_file.Path);
                }
                catch(Exception nofile)
                {
                    NextPost();
                }
                
            }
            else
            {
                MemeVisibility = Visibility.Collapsed;
                TextVisibility = Visibility.Visible;
            }
        }

        public void SharePost()
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendSocial("WhatsApp", "share", "");
            DataTransferManager.ShowShareUI();
        }

        private void ShareData(DataTransferManager sender, DataRequestedEventArgs args)
        {
            try
            {
                DataRequest request = args.Request;
                var deferral = request.GetDeferral();

                request.Data.Properties.Title = PostTitle;

                if (op.HasCategory("Meme"))
                {
                    request.Data.SetBitmap(RandomAccessStreamReference.CreateFromFile(_image_file));
                }
                else
                {
                    string content = PostContent;
                    content += "\n\n ~via ayansh.com/hj";
                    request.Data.SetText("\n\n" + content);
                }
                
                deferral.Complete();
            }
            catch (Exception ex)
            {
            }
        }

    }
}
using HanuDowsFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

namespace Hindi_Jokes.ViewModels
{
    public class UploadPostViewModel : ViewModelBase
    {
        public UploadPostViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                _postTitle = "Post Title will come here";
                _postContent = "Content will come here";
            }
        }

        string _postTitle, _postContent;
        public string PostTitle { get { return _postTitle; } set { Set(ref _postTitle, value); } }
        public string PostContent { get { return _postContent; } set { Set(ref _postContent, value); } }


        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
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

        public async void UploadPost()
        {
            MessageDialog messageDialog;

            // Sanity Check
            if (string.IsNullOrWhiteSpace(_postTitle))
            {
                messageDialog = new MessageDialog("Please enter the Joke Title");
                await messageDialog.ShowAsync();
                return;
            }

            if (string.IsNullOrWhiteSpace(_postContent))
            {
                messageDialog = new MessageDialog("The content is empty. Please make sure you have typed something.");
                await messageDialog.ShowAsync();
                return;
            }

            Views.Busy.SetBusy(true, "Uploading your joke...");

            HanuDowsApplication app = HanuDowsApplication.getInstance();
            bool success = await app.UploadNewPost(_postTitle, _postContent);

            Views.Busy.SetBusy(false);

            if (success)
            {
                messageDialog = new MessageDialog("Your joke was uploaded successfully. Thanks for sharing.");
                await messageDialog.ShowAsync();
                NavigationService.GoBack();
            }
            else
            {
                messageDialog = new MessageDialog("Could not upload your joke. Please try again.");
                await messageDialog.ShowAsync();
            }
        }

        public void CancelPost()
        {
            NavigationService.GoBack();
        }
    }
}
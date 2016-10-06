using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Hindi_Jokes.Common;
using System.Threading.Tasks;
using HanuDowsFramework;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Background;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace Hindi_Jokes
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes the singleton instance of the <see cref="App"/> class. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            // Get Launch Parameters
            string launchString = e.Arguments;

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                // Navigate to main page only if the EULA is accepted
                
                if (localSettings.Values["EULA"] == null || !localSettings.Values["EULA"].Equals("Accepted"))
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("title", "EULA");
                    data.Add("file", "eula.html");

                    if (!rootFrame.Navigate(typeof(HTMLDisplayPage), data))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }
                else
                {
                    if (launchString != null && launchString.Equals("ShowInfoMessage")) {

                        if (!rootFrame.Navigate(typeof(HTMLDisplayPage), null))
                        {
                            throw new Exception("Failed to create initial page");
                        }

                    }
                    else
                    {
                        // Initialize applicatio before use
                        await HanuDowsApplication.getInstance().InitializeApplication();

                        // Register backgroud task for Push Notifications
                        await registerBackgroundTaskForPushNotification();

                        if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                        {
                            throw new Exception("Failed to create initial page");
                        }
                    }
                    
                }
                
            }

            HanuDowsApplication.getInstance().ReadPostsFromDB(false);
            
            // Ensure the current window is active
            Window.Current.Activate();
            ObservablePost.getInstance().Reset();

        }

        private async Task<bool> registerBackgroundTaskForPushNotification()
        {

            ResourceLoader rl = new ResourceLoader();
            string app_id = rl.GetString("ApplicationID");

            var taskRegistered = false;
            var exampleTaskName = app_id + "_NotificationBackgroundTask";

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == exampleTaskName)
                {
                    taskRegistered = true;
                    break;
                }
            }

            if (taskRegistered)
            {
                //OutputText.Text = "Task already registered.";
                return true;
            }

            // Register background task
            BackgroundAccessStatus backgroundStatus = await BackgroundExecutionManager.RequestAccessAsync();

            if (backgroundStatus != BackgroundAccessStatus.Denied && backgroundStatus != BackgroundAccessStatus.Unspecified)
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = exampleTaskName;
                builder.TaskEntryPoint = "HindiJokes_BackgroundTasks.NotificationBackgroundTask";
                builder.SetTrigger(new PushNotificationTrigger());
                BackgroundTaskRegistration task = builder.Register();
                return true;
            }
            else
            {
                return false;
            }

        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
using Hindi_Jokes.HanuDows;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.Networking.PushNotifications;

namespace Hindi_Jokes
{
    public sealed class NotificationBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            RawNotification notification = (RawNotification)taskInstance.TriggerDetails;

            XDocument xdoc = XDocument.Parse(notification.Content);
            XElement notificationData = xdoc.Root;

            HanuDowsApplication app = HanuDowsApplication.getInstance();

            _deferral = taskInstance.GetDeferral();

            // Call async tasks and wait
            if (notificationData.Attribute("Task").Equals("SyncData"))
            {
                // Sync Data
                bool done = await app.PerformSync();
            }

            _deferral.Complete();
        }

    }
}

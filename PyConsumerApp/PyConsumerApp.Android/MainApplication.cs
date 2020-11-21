using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Plugin.FirebasePushNotification;

namespace PyConsumerApp.Droid
{
    [Application]
    public class MainApplication : Application
    {
        NotificationManager manager;
        const int pendingIntentId = 0;
        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {
        }
        public override void OnCreate()
        {
            base.OnCreate();
            try
            {
                //Set the default notification channel for your app when running Android Oreo
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    //Change for your default notification channel id here
                    FirebasePushNotificationManager.DefaultNotificationChannelId = "FirebasePushNotificationChannel";

                    //Change for your default notification channel name here
                    FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
                }
                manager = (NotificationManager)MainApplication.Context.GetSystemService(MainApplication.NotificationService);

                //If debug you should reset the token each time.
#if DEBUG
                FirebasePushNotificationManager.Initialize(this, true);
#else
            FirebasePushNotificationManager.Initialize(this, false);
#endif

                //Handle notification when app is closed here
                CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
                {
                    Intent intent = new Intent(MainApplication.Context, typeof(MainActivity));
                    PendingIntent pendingIntent = PendingIntent.GetActivity(MainApplication.Context, pendingIntentId, intent, PendingIntentFlags.OneShot);
                    var seed = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
                    int id = new Random(seed).Next(000000000, 999999999);
                    NotificationCompat.Builder builder = new NotificationCompat.Builder(MainApplication.Context, FirebasePushNotificationManager.DefaultNotificationChannelId)
                        .SetContentIntent(pendingIntent)
                        .SetContentTitle(Convert.ToString(p.Data["title"]))
                        .SetContentText(Convert.ToString("Test ->" + p.Data["body"]))
                        .SetLargeIcon(BitmapFactory.DecodeResource(MainApplication.Context.Resources, Resource.Drawable.ssIcon))
                        .SetSmallIcon(Resource.Drawable.ssIcon)
                        .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

                    var notification = builder.Build();
                    manager.Notify(id, notification);

                };
            }
            catch
            {

            }
        }
    }
 }
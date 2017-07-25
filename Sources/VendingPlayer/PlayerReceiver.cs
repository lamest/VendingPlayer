﻿using Android.App;
using Android.Content;

namespace VendingPlayer
{
    [BroadcastReceiver]
    [IntentFilter(new[] {Intent.ActionBootCompleted}, Priority = (int) IntentFilterPriority.HighPriority)]
    public class PlayerReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Intent serviceStart = new Intent(context, typeof(MainActivity));
            serviceStart.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(serviceStart);
        }
    }
}
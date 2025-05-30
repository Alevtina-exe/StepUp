using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Core.Content;
namespace WeightTracker;


[BroadcastReceiver(Enabled = true, Exported = false)]
public class BootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        if (intent.Action == Intent.ActionBootCompleted)
        {
            var serviceIntent = new Intent(context, typeof(StepCounterService));
            ContextCompat.StartForegroundService(context, serviceIntent);
        }
    }
}

// Name = "com.DaryaO.WeightTracker.BootReceiver",
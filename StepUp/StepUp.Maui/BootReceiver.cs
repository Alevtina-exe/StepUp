using Android.Content;
using AndroidX.Core.Content;

namespace StepUp;


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
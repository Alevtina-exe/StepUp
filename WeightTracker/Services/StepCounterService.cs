using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Microsoft.Maui.Controls;

[Service(ForegroundServiceType = ForegroundService.TypeHealth, Exported = false)]
public class StepCounterService : Service, ISensorEventListener
{
    private SensorManager _sensorManager;
    private Sensor _stepSensor;
    private int _initialSteps;
    private int _todaySteps;
    private const int NotificationId = 1001;

    public static event Action<int> StepsUpdated;

    public override IBinder OnBind(Intent intent) => null;

    public override void OnCreate()
    {
        base.OnCreate();
        InitializeSensor();
        StartForegroundNotification();
    }

    private void InitializeSensor()
    {
        _sensorManager = (SensorManager)GetSystemService(SensorService);
        _stepSensor = _sensorManager.GetDefaultSensor(SensorType.StepCounter);

        if (_stepSensor == null)
        {
            StopSelf();
            return;
        }
        _sensorManager.RegisterListener(this, _stepSensor, SensorDelay.Normal);
    }

    private void StartForegroundNotification()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                "step_channel",
                "Шагомер",
                NotificationImportance.Low);

            var manager = GetSystemService(NotificationService) as NotificationManager;
            manager?.CreateNotificationChannel(channel);
        }

        var notification = new Notification.Builder(this, "step_channel")
            .SetContentTitle("Шагомер активен")
            .SetSmallIcon(CommunityToolkit.Maui.Resource.Drawable.notification)
            .SetOngoing(true)
            .Build();

        StartForeground(NotificationId, notification);
    }

    public void OnSensorChanged(SensorEvent e)
    {
        if (e.Sensor.Type != SensorType.StepCounter) return;

        var totalSteps = (int)e.Values[0];

        if (_initialSteps == 0)
        {
            _initialSteps = totalSteps;
            return;
        }

        _todaySteps = totalSteps - _initialSteps;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            StepsUpdated?.Invoke(_todaySteps);
            UpdateNotification(_todaySteps);
        });
    }

    private void UpdateNotification(int steps)
    {
        var notification = new Notification.Builder(this, "step_channel")
            .SetContentTitle($"Шагов сегодня: {steps}")
            .SetSmallIcon(CommunityToolkit.Maui.Resource.Drawable.notification)
            .SetOngoing(true)
            .Build();

        var manager = GetSystemService(NotificationService) as NotificationManager;
        manager?.Notify(NotificationId, notification);
    }

    public override void OnDestroy()
    {
        _sensorManager?.UnregisterListener(this);
        base.OnDestroy();
    }

    public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy) { }
}
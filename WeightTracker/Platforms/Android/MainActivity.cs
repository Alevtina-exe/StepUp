using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        StartStepCounterService();
    }

    private void StartStepCounterService()
    {
        if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ActivityRecognition) == Permission.Granted)
        {
            var intent = new Intent(this, typeof(StepCounterService));
            ContextCompat.StartForegroundService(this, intent);
        }
        else
        {
            ActivityCompat.RequestPermissions(this,
                new[] { Android.Manifest.Permission.ActivityRecognition }, 100);
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (requestCode == 100 && grantResults.Length > 0 &&
            grantResults[0] == Permission.Granted)
        {
            StartStepCounterService();
        }
    }
}
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.Permissions;
using System;

namespace EasyARX.Droid
{
    /// <summary>
    /// The Xamarin equivalent of the Android Activity.  This is unchanged from the Xamarin template except that the EasyAR Engine is 
    /// initialized and the Permissions is added the request permissions callback.
    /// </summary>
    [Activity(Label = "EasyARXSamples", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Initialize the EasyAR Engine
            if( !EasyAR.Engine.initialize(this, EasyARUtil.KEY))
            {
                string message = $"Failed intializing EasyAR: {EasyAR.Engine.errorMessage()}.";
                Console.WriteLine(message);
                throw new Exception(message);
            }

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            // Added callback required by thre permissions plugin
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
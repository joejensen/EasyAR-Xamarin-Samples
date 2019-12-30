using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using Urho;
using Urho.Forms;
using Xamarin.Forms;

namespace EasyARX
{
    class UrhoPage : ContentPage
    {
        private readonly UrhoSurface urhoSurface;
        private UrhoApp urhoApp;

        public UrhoPage()
        {
            Title = EasyAR.Engine.name();
            urhoSurface = new UrhoSurface();
            urhoSurface.VerticalOptions = LayoutOptions.FillAndExpand;
            urhoSurface.HorizontalOptions = LayoutOptions.FillAndExpand;

            Content = new StackLayout
            {
                Padding = new Thickness(0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { urhoSurface }
            };
        }

        protected override async void OnAppearing()
        {
            GetPermissions();

            ApplicationOptions urhoApplicationOptions = new ApplicationOptions()
            {
                ResourcePaths = new[] { "UrhoData" },
                Orientation = ApplicationOptions.OrientationType.LandscapeAndPortrait
            };

            urhoApp = await urhoSurface.Show<UrhoApp>(urhoApplicationOptions);
        }

        protected override void OnDisappearing()
        {
            UrhoSurface.OnDestroy();
            base.OnDisappearing();
        }

        private async void GetPermissions()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await DisplayAlert("Need Camera", "Gunna need that camera", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                    status = results[Permission.Camera];
                }

                if (status == PermissionStatus.Granted)
                {
                    //Query permission
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //location denied
                }
            }
            catch (Exception ex)
            {
                //Something went wrong
                Console.WriteLine(ex.Message);
            }
        }
    }
}

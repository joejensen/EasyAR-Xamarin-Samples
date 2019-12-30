using Xamarin.Forms;

namespace EasyARX
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new UrhoPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

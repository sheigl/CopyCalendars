using Xamarin.Forms;
using CopyCalendars.Views;

namespace CopyCalendars
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            GoToMainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static void GoToMainPage()
        {
            TabbedPage returnValue = new TabbedPage();
            returnValue.Children.Add(new NavigationPage(new MainPage()) { Title = "Copy Calendars" });
            returnValue.Children.Add(new NavigationPage(new SettingsPage()) { Title = "Settings" });

            Current.MainPage = returnValue;
        }
    }
}

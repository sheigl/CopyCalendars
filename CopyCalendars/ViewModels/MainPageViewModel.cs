using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CopyCalendars.Models;
using CopyCalendars.Services;
using Xamarin.Forms;

namespace CopyCalendars.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        string message = string.Empty;
		public string Message
		{
			get { return message; }
			set { message = value; OnPropertyChanged(); }
		}

		string _log = string.Empty;
		public string Log
		{
			get { return _log; }
			set { _log = value; OnPropertyChanged(); }
		}

        List<CalendarItem> items = new List<CalendarItem>();
		public List<CalendarItem> Items
		{
			get { return items; }
			set { items = value; OnPropertyChanged(); }
		}

        public ICommand LoadItemsCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand QuitApplicationCommand { get; }

		public MainPageViewModel()
		{			
            LoadItemsCommand = new Command(async () => await LoadItemsFromApi());
            CopyCommand = new Command(async () => {
                var copy = new CopyCalendarService(items, msg => Log = Log + $"\n\n[{DateTime.Now.ToString()}] {msg}");

                await copy.CopyCalendars();
            });
            items.Add(new CalendarItem { CommunityCode = 0, CommunityName = "No calendars available" });

            QuitApplicationCommand = new Command(() => System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow());
		}
        async Task LoadItemsFromApi()
        {
            try
            {
				IsBusy = true;
				Message = "Loading...";

                ApiService service = new ApiService(Settings.Current.APIUrl);
                Items = await service.GetCalendarsFromWeb(Settings.Current.APIKey, Settings.Current.SecretKey);

            }
            catch (Exception ex)
            {
                Message = ex.ToString();
            }
            finally
            {
				Message = string.Empty;
				IsBusy = false;
            }
        }
    }
}

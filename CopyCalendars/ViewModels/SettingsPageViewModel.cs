using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CopyCalendars.Models;
using CopyCalendars.Services;
using Xamarin.Forms;

namespace CopyCalendars.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
		string message = string.Empty;
		public string Message
		{
			get { return message; }
			set { message = value; OnPropertyChanged(); }
		}

        private Settings _settings = new Settings();
        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value; OnPropertyChanged(); }
        }

        private string _masterTemplateFile;
		public string MasterTemplateFile
        {
            get { return _masterTemplateFile; }
            set { _masterTemplateFile = value; Settings.MasterTemplateFile = value; OnPropertyChanged(); }
        }

        private string _proofingFolder;
		public string ProofingFolder
		{
            get { return _proofingFolder; }
            set { _proofingFolder = value; Settings.ProofingFolder = value; OnPropertyChanged(); }
		}

        private string _workingCalendarFolder;
		public string WorkingCalendarFolder
		{
            get { return _workingCalendarFolder; }
            set { _workingCalendarFolder = value; Settings.WorkingCalendarFolder = value; OnPropertyChanged(); }
		}

        private string _apiKey;
		public string APIKey
		{
            get { return _apiKey; }
            set { _apiKey = value; Settings.APIKey = value; OnPropertyChanged(); }
		}

        private string _apiUrl;
		public string APIUrl
		{
            get { return _apiUrl; }
            set { _apiUrl = value; Settings.APIUrl = value; OnPropertyChanged(); }
		}

        private string _secretKey;
		public string SecretKey
		{
			get { return _secretKey; }
            set { _secretKey = value; Settings.SecretKey = value; OnPropertyChanged(); }
		}

        private string _prooferInitials;
		public string ProoferInitials
		{
			get { return _prooferInitials; }
            set { _prooferInitials = value; Settings.ProoferInitials = value; OnPropertyChanged(); }
		}

        public ICommand LoadSettingsCommand { get; }
        public ICommand SaveSettingsCommand { get; }

        private readonly SettingsRepository _repo = new SettingsRepository();
        public SettingsPageViewModel()
        {
            LoadSettingsCommand = new Command(() => {
                var settings = _repo.Get();
                if (settings != null)
                {
					MasterTemplateFile = settings.MasterTemplateFile;
					ProofingFolder = settings.ProofingFolder;
					WorkingCalendarFolder = settings.WorkingCalendarFolder;
					APIKey = settings.APIKey;
					APIUrl = settings.APIUrl;
					SecretKey = settings.SecretKey;
					ProoferInitials = settings.ProoferInitials;
                }

            });
            SaveSettingsCommand = new Command(() => {
                this.Settings.LastModifiedDate = DateTime.Now;
                _repo.Save(this.Settings);
            });
        }
    }
}

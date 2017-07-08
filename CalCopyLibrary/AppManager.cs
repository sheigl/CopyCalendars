using System;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CalCopyLibrary
{
	public class AppManager
	{
		private readonly string settingsPath;
		private Settings _settings;
		public Settings Settings
		{
			get 
			{ 
				if (_settings == null)
				{
					GetSettings();
				}

				return _settings;
			}
		}
		
		public AppManager(string settingsPath)
		{
			this.settingsPath = settingsPath;
			GetSettings();
		}

		public void PrintCalendars()
		{
			Console.Clear();
			List<CalendarEntryViewModel> cals = GetCalendarsFromWeb().Result;
			Console.WriteLine();
			if (cals == null || cals.Count == 0)
			{
				Logger.Log("No calendars available!");
			}

			foreach (var item in cals)
			{
				Logger.Log($"[{item.CommunityCode}][{item.CalendarType}] {item.CommunityName}");
			}
			Console.WriteLine();
		}

		public void GetSettings()
		{
			if (File.Exists(settingsPath)) 
			{
				_settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));
				bool expiredSettings = _settings.LastModifiedDate.Value.Month != DateTime.Now.Month;
				
				if (String.IsNullOrEmpty(_settings.APIUrl))
				{
					Logger.Log("API Url:");
					_settings.APIUrl = Console.ReadLine().Trim();
				}
				if (String.IsNullOrEmpty(_settings.APIKey))
				{
					Logger.Log("API Key:");
					_settings.APIKey = Console.ReadLine().Trim();
				}
				if (String.IsNullOrEmpty(_settings.SecretKey))
				{
					Logger.Log("API Secret:");
					_settings.SecretKey = Console.ReadLine().Trim();
				}
				if (String.IsNullOrEmpty(_settings.MasterTemplateFile) || expiredSettings)
				{
					Logger.Log("Master Template File:");
					_settings.MasterTemplateFile = Console.ReadLine().Trim().Replace("\\(", "(").Replace("\\)", ")");
				}
				if (String.IsNullOrEmpty(_settings.ProoferInitials))
				{
					Logger.Log("Proofer Initials:");
					_settings.ProoferInitials = Console.ReadLine().Trim();
				}
				if (String.IsNullOrEmpty(_settings.ProofingFolder) || expiredSettings)
				{
					Logger.Log("Proofing Folder:");
					_settings.ProofingFolder = Console.ReadLine().Trim();	

					if (_settings.ProofingFolder.Substring(_settings.ProofingFolder.Length) != "/") 
					{
						_settings.ProofingFolder = _settings.ProofingFolder + "/";
					}
				}
				if (String.IsNullOrEmpty(_settings.WorkingCalendarFolder) || expiredSettings)
				{
					Logger.Log("Working Calendar Folder:");
					_settings.WorkingCalendarFolder = Console.ReadLine().Trim();

					if (_settings.WorkingCalendarFolder.Substring(_settings.WorkingCalendarFolder.Length) != "/") 
					{
						_settings.WorkingCalendarFolder = _settings.WorkingCalendarFolder + "/";
					}
				}
				if (_settings.CreatedDate == null)
				{
					_settings.CreatedDate = DateTime.Now;
				}

				_settings.LastModifiedDate = DateTime.Now;

				SaveSettings();
			}
			else {
				Console.Clear();
				
				Logger.Log("No settings found! Please complete your settings");
				_settings = new Settings();

				Logger.Log("API Key:");
				_settings.APIKey = Console.ReadLine().Trim();
				Logger.Log("API Secret:");
				_settings.SecretKey = Console.ReadLine().Trim();
				Logger.Log("API Url:");
				_settings.APIUrl = Console.ReadLine().Trim();
				Logger.Log("Master Template File:");
				_settings.MasterTemplateFile = Console.ReadLine().Trim();
				Logger.Log("Proofer Initials:");
				_settings.ProoferInitials = Console.ReadLine().Trim();
				Logger.Log("Proofing Folder:");
				_settings.ProofingFolder = Console.ReadLine().Trim();				
				Logger.Log("Working Calendar Folder:");
				_settings.WorkingCalendarFolder = Console.ReadLine().Trim();

				_settings.CreatedDate = DateTime.Now;
				_settings.LastModifiedDate = DateTime.Now;

				SaveSettings();
				GetSettings();				
			}
		}
		public void SaveSettings()
		{
			if(_settings != null)
			{
				File.WriteAllText(settingsPath, JsonConvert.SerializeObject(_settings));
			}
		}
		public void SaveSettings(Settings settings)
		{
			File.WriteAllText(settingsPath, JsonConvert.SerializeObject(Settings));
		}

		public async Task<List<CalendarEntryViewModel>> GetCalendarsFromWeb()
		{
			Logger.Log($"Getting calendars from {_settings.APIUrl}");
			using(HttpClientHandler handler = new HttpClientHandler())
			using(HttpClient client = new HttpClient(handler))
			{
				handler.ServerCertificateCustomValidationCallback += (message, xcert, chain, errors) => true;
				string url = $"{_settings.APIUrl}/calendar?apikey={_settings.APIKey}";
				var response = await client.PostAsync(url, 
				new StringContent(JsonConvert.SerializeObject(
					new ApiKeyViewModel{ SecretKey = _settings.SecretKey }), Encoding.UTF8, "application/json"
					));

				List<CalendarEntryViewModel> calendars = JsonConvert.DeserializeObject<List<CalendarEntryViewModel>>(await response.Content.ReadAsStringAsync());
				
				if(calendars != null)
				{
					Logger.Log($"Got {calendars.Count} calendars!");
					return calendars;
				}
				else
				{
					return new List<CalendarEntryViewModel>();
				}
			}			
		}
	}
}


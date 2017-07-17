using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CopyCalendars.Models;
using Newtonsoft.Json;

namespace CopyCalendars.Services
{
    public class ApiService
    {
        private readonly string _apiUrl;
        public ApiService(string apiUrl)
        {
            _apiUrl = apiUrl;
        }
		public async Task<List<CalendarItem>> GetCalendarsFromWeb(string apiKey, string apiSecret)
		{
			using (HttpClientHandler handler = new HttpClientHandler())
			using (HttpClient client = new HttpClient(handler))
			{
				//handler.ServerCertificateCustomValidationCallback += (message, xcert, chain, errors) => true;
				string url = $"{_apiUrl}/calendar?apikey={apiKey}";
				var response = await client.PostAsync(url,
				new StringContent(JsonConvert.SerializeObject(
					new { SecretKey = apiSecret }), System.Text.Encoding.UTF8, "application/json"
					));

                string result = await response.Content.ReadAsStringAsync();
				List<CalendarItem> calendars = JsonConvert.DeserializeObject<List<CalendarItem>>(result);

				if (calendars != null)
				{
					return calendars;
				}
				else
				{
					return new List<CalendarItem>();
				}
			}
		}
    }
}

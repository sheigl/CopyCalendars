using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CopyCalendars.Services;

namespace CopyCalendars.Models
{
    public class Settings
    {
        public string MasterTemplateFile { get; set; }
		public string ProofingFolder { get; set; }
		public string WorkingCalendarFolder { get; set; }
		public string APIKey { get; set; }
		public string APIUrl { get; set; }
		public string SecretKey { get; set; }
		public string ProoferInitials { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? LastModifiedDate { get; set; }

        public static Settings Current = new Settings();
        public static void SetCurrent(Settings settings) => Current = settings;
    }
}

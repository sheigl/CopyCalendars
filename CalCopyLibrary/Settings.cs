using System;

namespace CalCopyLibrary
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
	}
}

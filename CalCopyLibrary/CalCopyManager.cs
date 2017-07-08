using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CalCopyLibrary
{
    public class CalCopyManager
    {
        private readonly AppManager _manager;
        public CalCopyManager(AppManager manager)
        {
            _manager = manager;
        }
        public async Task<List<CalendarEntryViewModel>> GetCalendars()
		{
			List<CalendarEntryViewModel> calendars = await _manager.GetCalendarsFromWeb();
            return calendars;
		}

        public async Task CopyCalendars()
		{			
			if (!String.IsNullOrEmpty(_manager.Settings.ProofingFolder))
			{
				List<CalendarEntryViewModel> selectedCalendars = (await GetCalendars())
					.GroupBy(g => g.CommunityID)
					.Select(s => s.FirstOrDefault())
					.Select(c => new CalendarEntryViewModel()
					{
						CommunityCode = c.CommunityCode,
						CommunityName = c.CommunityName,
						CalendarType = c.CalendarType
					}).ToList();								

				List<string> ProofingFolder = new List<string>(Directory.EnumerateDirectories(_manager.Settings.ProofingFolder));

				foreach (var calendar in selectedCalendars) 
				{
					foreach (var folder in ProofingFolder) 
					{
						string folderName = folder.Replace(_manager.Settings.ProofingFolder, "");
						string communityCodeFromFolder = folderName.Substring(0, 3);

						if (String.Equals(communityCodeFromFolder, calendar.CommunityCode.ToString())) 
						{
							string newFolder = String.Format("{0}{1}", _manager.Settings.WorkingCalendarFolder, folderName);

							if (!Directory.Exists(newFolder)) {
								Logger.Log(String.Format ("Creating: {0}", newFolder));
								Directory.CreateDirectory(newFolder);

								if (File.Exists(_manager.Settings.MasterTemplateFile)) {
									string templateName = _manager.Settings.MasterTemplateFile.Substring(_manager.Settings.MasterTemplateFile.LastIndexOf("/") + 1);
									string newTemplateName = String.Format("10{0}_{1}{2}_{3}_{4}.indd", calendar.CommunityCode, DateTime.Now.AddMonths(1).Month < 10 ? "0" + DateTime.Now.AddMonths(1).Month.ToString() : DateTime.Now.AddMonths(1).Month.ToString(), DateTime.Now.AddMonths(1).Year.ToString().Substring(2), calendar.CommunityName.Replace(" ", ""), _manager.Settings.ProoferInitials);
									if (!File.Exists(String.Format("{0}/{1}", newFolder, newTemplateName))) {
										File.Copy(_manager.Settings.MasterTemplateFile, String.Format("{0}/{1}", newFolder, newTemplateName));
									}
								}

								List<string> files = new List<string> (Directory.EnumerateFiles (folder));

								foreach (var file in files) {

									Logger.Log(String.Format ("Copying: {0}", file));
									string fileName = file.Replace(folder, "");
									File.Copy(file, String.Format("{0}{1}", newFolder, fileName));
								}
							}

						}
					}
				}

				renameFolders(_manager.Settings.WorkingCalendarFolder, _manager.Settings.ProoferInitials);
			}
			else 
			{
				Logger.Log("Please select Atria Folder Path");
			}
		}

        public void Rename ()
		{
			bool processFolders = false;
			if (String.IsNullOrEmpty(_manager.Settings.WorkingCalendarFolder)) {
				processFolders = false;
				Logger.Log("Please select a folder");
			}
			if (String.IsNullOrEmpty(_manager.Settings.ProoferInitials)) {
				processFolders = false;
			}
			if (!String.IsNullOrEmpty(_manager.Settings.WorkingCalendarFolder) && !String.IsNullOrEmpty(_manager.Settings.ProoferInitials)) {
				processFolders = true;
			}
			if (processFolders) {
				renameFolders(_manager.Settings.WorkingCalendarFolder, _manager.Settings.ProoferInitials);
			}
		}

		private void renameFolders(string path, string proofer)
		{
			string folderPath = path;
			List<string> folders = new List<string>(Directory.EnumerateDirectories (folderPath));

			Logger.Log(String.Format ("Getting List of Folders"));

			foreach (var folder in folders) {
				string folderName = folder.Replace (folderPath, "");
				int tempOut = 0;
				if (!String.Equals(folderName.Substring(0, 2), "10") && Int32.TryParse(folderName.Substring(0, 2), out tempOut)) {
					Logger.Log(String.Format ("Old Folder: {0}", folder));
					string communityNumber = "10" + folderName.Substring (0, 3);
					string communityName = folderName.Substring(4);
					string newFolderPath = String.Format ("{0}{1}_{2}{3}_{4}_{5}", folderPath, communityNumber, DateTime.Now.AddMonths(1).Month < 10 ? "0" + DateTime.Now.AddMonths(1).Month.ToString() : DateTime.Now.AddMonths(1).Month.ToString(), DateTime.Now.AddMonths(1).Year.ToString().Substring(2), communityName, proofer);

					Logger.Log(String.Format ("Creating: {0}", newFolderPath));
					Directory.CreateDirectory (newFolderPath);

					List<string> files = new List<string> (Directory.EnumerateFiles (folder));
					Logger.Log(String.Format ("Getting list of Files in: {0}", folder));
					foreach (var file in files) {
						string fileName = file.Replace (folder + "/", "");
						Logger.Log(String.Format ("Copying: {0} From: {1} To: {2}", fileName, folder, newFolderPath));
						File.Copy (file, String.Format ("{0}/{1}", newFolderPath, fileName), true);
					}

					List<string> newFiles = new List<string> (Directory.EnumerateFiles (newFolderPath));

					Logger.Log(String.Format ("Comparing Old to New Folder"));
					if (files.Count == newFiles.Count) {
						Logger.Log(String.Format ("Folders Match! Removing Old Folder"));
						Directory.Delete (folder, true);
					}

				}
			}

			Logger.Log(String.Format ("Done!"));
		}	
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CopyCalendars.Models;

namespace CopyCalendars.Services
{
    public class CopyCalendarService
    {
        private readonly List<CalendarItem> _calendars;
        private Action<string> _logger;
        public CopyCalendarService(List<CalendarItem> calendars, Action<string> logger)
        {
            _calendars = calendars;
            _logger = logger;
        }
		public async Task CopyCalendars()
		{
            await Task.Run(() => {
				if (!String.IsNullOrEmpty(Settings.Current.ProofingFolder))
				{
					List<CalendarItem> selectedCalendars = _calendars
						.GroupBy(g => g.CommunityID)
						.Select(s => s.FirstOrDefault())
						.Select(c => new CalendarItem()
						{
							CommunityCode = c.CommunityCode,
							CommunityName = c.CommunityName,
							CalendarType = c.CalendarType
						}).ToList();

					List<string> ProofingFolder = new List<string>(Directory.EnumerateDirectories(Settings.Current.ProofingFolder));

					foreach (var calendar in selectedCalendars)
					{
						foreach (var folder in ProofingFolder)
						{
                            DirectoryInfo d = new DirectoryInfo(folder);

                            string folderName = d.Name;
                            string communityCodeFromFolder = folderName.Substring(0, folderName.IndexOf("_"));

							if (String.Equals(communityCodeFromFolder, calendar.CommunityCode.ToString()))
							{
								string newFolder = String.Format("{0}{1}", Settings.Current.WorkingCalendarFolder + "/", folderName);

								if (!Directory.Exists(newFolder))
								{
									_logger(String.Format("Creating: {0}", newFolder));
									Directory.CreateDirectory(newFolder);

									if (File.Exists(Settings.Current.MasterTemplateFile))
									{
										string templateName = Settings.Current.MasterTemplateFile.Substring(Settings.Current.MasterTemplateFile.LastIndexOf("/") + 1);
										string newTemplateName = String.Format("10{0}_{1}{2}_{3}_{4}.indd", calendar.CommunityCode, DateTime.Now.AddMonths(1).Month < 10 ? "0" + DateTime.Now.AddMonths(1).Month.ToString() : DateTime.Now.AddMonths(1).Month.ToString(), DateTime.Now.AddMonths(1).Year.ToString().Substring(2), calendar.CommunityName.Replace(" ", ""), Settings.Current.ProoferInitials);
										if (!File.Exists(String.Format("{0}/{1}", newFolder, newTemplateName)))
										{
											File.Copy(Settings.Current.MasterTemplateFile, String.Format("{0}/{1}", newFolder, newTemplateName));
										}
									}

									List<string> files = new List<string>(Directory.EnumerateFiles(folder));

									foreach (var file in files)
									{

										_logger(String.Format("Copying: {0}", file));
										string fileName = file.Replace(folder, "");
										File.Copy(file, String.Format("{0}{1}", newFolder, fileName));
									}
								}

							}
						}
					}
				}
            });

            await renameFolders(Settings.Current.WorkingCalendarFolder + "/", Settings.Current.ProoferInitials);
		}

		public async Task Rename()
		{
			bool processFolders = false;
			if (String.IsNullOrEmpty(Settings.Current.WorkingCalendarFolder))
			{
				processFolders = false;
				_logger("Please select a folder");
			}
			if (String.IsNullOrEmpty(Settings.Current.ProoferInitials))
			{
				processFolders = false;
			}
			if (!String.IsNullOrEmpty(Settings.Current.WorkingCalendarFolder) && !String.IsNullOrEmpty(Settings.Current.ProoferInitials))
			{
				processFolders = true;
			}
			if (processFolders)
			{
				await renameFolders(Settings.Current.WorkingCalendarFolder, Settings.Current.ProoferInitials);
			}
		}

		private async Task renameFolders(string path, string proofer)
		{
            await Task.Run(() => {
				string folderPath = path;
				List<string> folders = new List<string>(Directory.EnumerateDirectories(folderPath));

				_logger(String.Format("Getting List of Folders"));

				foreach (var folder in folders)
				{
					string folderName = folder.Replace(folderPath, "");
					int tempOut = 0;
					if (!String.Equals(folderName.Substring(0, 2), "10") && Int32.TryParse(folderName.Substring(0, 2), out tempOut))
					{
						_logger(String.Format("Old Folder: {0}", folder));
						string communityNumber = "10" + folderName.Substring(0, 3);
						string communityName = folderName.Substring(4);
						string newFolderPath = String.Format("{0}{1}_{2}{3}_{4}_{5}", folderPath, communityNumber, DateTime.Now.AddMonths(1).Month < 10 ? "0" + DateTime.Now.AddMonths(1).Month.ToString() : DateTime.Now.AddMonths(1).Month.ToString(), DateTime.Now.AddMonths(1).Year.ToString().Substring(2), communityName, proofer);

						_logger(String.Format("Creating: {0}", newFolderPath));
						Directory.CreateDirectory(newFolderPath);

						List<string> files = new List<string>(Directory.EnumerateFiles(folder));
						_logger(String.Format("Getting list of Files in: {0}", folder));
						foreach (var file in files)
						{
							string fileName = file.Replace(folder + "/", "");
							_logger(String.Format("Copying: {0} From: {1} To: {2}", fileName, folder, newFolderPath));
							File.Copy(file, String.Format("{0}/{1}", newFolderPath, fileName), true);
						}

						List<string> newFiles = new List<string>(Directory.EnumerateFiles(newFolderPath));

						_logger(String.Format("Comparing Old to New Folder"));
						if (files.Count == newFiles.Count)
						{
							_logger(String.Format("Folders Match! Removing Old Folder"));
							Directory.Delete(folder, true);
						}

					}
				}
            });

		}
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CopyCalendars.Models;
using System.Text.RegularExpressions;

namespace CopyCalendars.Services
{
    public class CopyCalendarService
    {
        private readonly List<CalendarItem> _calendars;
        private Action<string> _logger;
        private Regex _communityNumberRegex = new Regex(@"(\d{5})");
        public CopyCalendarService(List<CalendarItem> calendars, Action<string> logger)
        {
            _calendars = calendars;
            _logger = logger;
        }
		public async Task CopyCalendars()
		{
            if (_calendars.Count == 0)
            {
                return;
            }

            await Task.Run(() => {
				try
                {
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

                                if (!_communityNumberRegex.Match(d.Name).Success)
                                {
                                    continue;
                                }

                                string folderName = d.Name;
                                string communityCodeFromFolder = _communityNumberRegex.Match(folderName).Value;

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

											string fileNameTemplate = "10{0}_{1}{2}_{3}_{4}.indd";

											if (calendar.CommunityCode > 9999)
											{
												fileNameTemplate = "{0}_{1}{2}_{3}_{4}.indd";
											}

											string newTemplateName = String.Format(fileNameTemplate, calendar.CommunityCode, DateTime.Now.AddMonths(1).Month < 10 ? "0" + DateTime.Now.AddMonths(1).Month.ToString() : DateTime.Now.AddMonths(1).Month.ToString(), DateTime.Now.AddMonths(1).Year.ToString().Substring(2), calendar.CommunityName.Replace(" ", ""), Settings.Current.ProoferInitials);
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
                }
                catch (Exception ex) {
                    _logger(ex.ToString());
				}
            });

            await renameFolders(Settings.Current.WorkingCalendarFolder + "/", Settings.Current.ProoferInitials);
		}

		private async Task renameFolders(string path, string proofer)
		{
            await Task.Run(() => {
                try
                {
					DirectoryInfo d = new DirectoryInfo(path);

					List<string> folders = new List<string>(Directory.EnumerateDirectories(d.FullName));

					_logger(String.Format("Getting List of Folders"));

					foreach (var folder in folders)
					{
						DirectoryInfo f = new DirectoryInfo(folder);

                        if (!_communityNumberRegex.Match(f.Name).Success)
                        {
                            continue;
                        }

                        if (Int32.TryParse(_communityNumberRegex.Match(f.Name).Value, out int communityNumber))
						{
                            CalendarItem cal = _calendars.FirstOrDefault(x => x.CommunityCode == communityNumber);
                            if (cal == null)
                            {
                                continue;
                            }

                            _logger(String.Format("Old Folder: {0}", folder));
                            string communityName = cal.CommunityName;
                            string newFolderName = String.Format("{0}_{1}{2}_{3}_{4}", 
                                                                 communityNumber < 9999 ? $"10{communityNumber}" : communityNumber.ToString(), 
                                                                 DateTime.Now.AddMonths(1).Month < 10 ? "0" + DateTime.Now.AddMonths(1).Month.ToString() : DateTime.Now.AddMonths(1).Month.ToString(), 
                                                                 DateTime.Now.AddMonths(1).Year.ToString().Substring(2), 
                                                                 communityName, 
                                                                 proofer);
                            string newFolderPath = String.Format("{0}", Path.Combine(d.FullName, newFolderName));

                            if (f.Name == newFolderName)
                            {
                                continue;
                            }

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
                }
                catch (Exception ex)
                {
                    _logger(ex.ToString());
                }
            });

		}
    }
}

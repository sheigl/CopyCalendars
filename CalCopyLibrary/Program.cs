using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CalCopyLibrary
{
    public class Program
    {
        private const string _regex = @"(?<=[-{1,2}|\/])(?<name>[a-zA-Z0-9]*)[ |:|""]*(?<value>[\w|.|?|=|&|+| |:|\/|\\]*)(?=[ |""]|$)";
        private static string _basePath = System.AppContext.BaseDirectory;
        private readonly static Regex _reg = new Regex(_regex);
        private static Dictionary<string, string> _argsDictionary;
        private string _activeMonthName = DateTime.Now.AddMonths(1).ToString("MMMM");
        private static AppManager _manager;
        public static void Main(string[] args)
        {
             /*_argsDictionary = getArgsDictionary(args);
            if (_argsDictionary == null) {
                Console.Clear();
                Logger.Log("angular-scaffold");
                Logger.Log("-type [init, section, directive, service, factory]");
                Logger.Log("-name [name of the type] ex. product");
                Logger.Log("-overwrite [true, false] overwrites files. note: defaults to true.");
                return;
            }

            Console.Clear();

            string path;
            if (!tryGetArg("path", out path))
            {
                path = Directory.GetCurrentDirectory();
            }*/

            _manager = new AppManager(Path.Combine(_basePath, "Settings.json"));     

            int selection = mainMenu();       

            if (selection == 4)
            {
                Logger.Log("Bye!");
                return;
            }

            pickMenu(selection);
        }
        private static int mainMenu()
        {
            Logger.TimeStamp = false;
            Console.Clear();
            Logger.Log("===================================");
            Logger.Log("Main Menu");
            Logger.Log("===================================");
            Logger.Log("1. Update settings");
            Logger.Log("2. Show checked out calendars");
            Logger.Log("3. Copy calendars");
            Logger.Log("4. Quit");
            Logger.TimeStamp = true;
            try
            {
                return Convert.ToInt32(Console.ReadLine());
            }
            catch (System.Exception)
            {
                Logger.Log("Invalid Selection... press any key to try again.");
                Console.ReadLine();
                pickMenu(mainMenu());
            }

            return 0;
        }
        private static void settings()
        {
            Logger.TimeStamp = false;
            Console.Clear();
            Logger.Log("===================================");
            Logger.Log("Settings Menu");
            Logger.Log("===================================");
            Logger.Log("1. Update API Url");            
            Logger.Log("2. Update API Key");
            Logger.Log("3. Update API Secret");
            Logger.Log("4. Update Master Template File");
            Logger.Log("5. Update Proofing Folder");
            Logger.Log("6. Update Proofer Initials");
            Logger.Log("7. Update Working Calendar Folder");
            Logger.Log("8. Return to Main Menu");
            
            try
            {
                int selection = Convert.ToInt32(Console.ReadLine());

                if (selection == 1)
				{
                    Logger.Log($"Current Value: {_manager.Settings.APIUrl} ");
                    Console.Write("New Value: ");
					string val = Console.ReadLine().Trim();
                    if (!String.IsNullOrEmpty(val))
                    {
                        _manager.Settings.APIUrl = val;
                        _manager.Settings.LastModifiedDate = DateTime.Now;
                        _manager.SaveSettings();
                    }
                    
                    settings();
				}
				else if (selection == 2)
				{
					Logger.Log($"Current Value: {_manager.Settings.APIKey} ");
                    Console.Write("New Value: ");
                    string val = Console.ReadLine().Trim();
                    if (!String.IsNullOrEmpty(val))
                    {
                       _manager.Settings.APIKey = val;
                    _manager.Settings.LastModifiedDate = DateTime.Now;
                    _manager.SaveSettings(); 
                    }
					
                    settings();
				}
				else if (selection == 3)
				{
					Logger.Log($"Current Value: {_manager.Settings.SecretKey} ");
                    Console.Write("New Value: ");
                    string val = Console.ReadLine().Trim();
                    if (!String.IsNullOrEmpty(val))
                    {
                        _manager.Settings.SecretKey = val;
                    _manager.Settings.LastModifiedDate = DateTime.Now;
                    _manager.SaveSettings();
                    }
					
                    settings();
				}
				else if (selection == 4)
				{
					Logger.Log($"Current Value: {_manager.Settings.MasterTemplateFile} ");
                    Console.Write("New Value: ");
                    string val = Console.ReadLine().Trim();
                    if (!String.IsNullOrEmpty(val))
                    {
                        _manager.Settings.MasterTemplateFile = val.Replace("\\(", "(").Replace("\\)", ")");
                    _manager.Settings.LastModifiedDate = DateTime.Now;
                    _manager.SaveSettings();
                    }
					
                    settings();
				}
				else if (selection == 6)
				{
					Logger.Log($"Current Value: {_manager.Settings.ProoferInitials} ");
                    Console.Write("New Value: ");
                    string val = Console.ReadLine().Trim();
                    if (!String.IsNullOrEmpty(val))
                    {
                        _manager.Settings.ProoferInitials = val;
                    _manager.Settings.LastModifiedDate = DateTime.Now;
                    _manager.SaveSettings();
                    }
					
                    settings();
				}
				else if (selection == 5)
				{
					Logger.Log($"Current Value: {_manager.Settings.ProofingFolder} ");
                    Console.Write("New Value: ");
                    string val = Console.ReadLine().Trim();
                    if (!String.IsNullOrEmpty(val))
                    {
                        if (val.Substring(val.Length) != "/") 
                        {
                            val = val + "/";
                        }
                        
                        _manager.Settings.ProofingFolder = val;
                    _manager.Settings.LastModifiedDate = DateTime.Now;
                    _manager.SaveSettings();
                    }
					
                    settings();
				}
				else if (selection == 7)
				{
					Logger.Log($"Current Value: {_manager.Settings.WorkingCalendarFolder} ");
                    Console.Write("New Value: ");
                    string val = Console.ReadLine().Trim();
                    if (!String.IsNullOrEmpty(val))
                    {
                        if (val.Substring(val.Length) != "/") 
                        {
                            val = val + "/";
                        }

                        _manager.Settings.WorkingCalendarFolder = val;
                    _manager.Settings.LastModifiedDate = DateTime.Now;
                    _manager.SaveSettings();
                    }
					
                    settings();
				}
                else if (selection == 8)
                {
                    pickMenu(mainMenu());
                }
                else 
                {
                    Logger.Log("Invalid Selection... press any key to try again.");
                    Console.ReadLine();
                    settings();
                }

                Logger.TimeStamp = true;
            }
            catch (System.Exception)
            {
                Logger.TimeStamp = true;
                Logger.Log("Invalid Selection... press any key to try again.");
                Console.ReadLine();
                settings();
            }
        }
        private static void showCalendars()
        {
            _manager.PrintCalendars();
            Logger.Log("Press any key to continue...");
            Console.ReadLine();
            pickMenu(0);
        }
        private static void copyCalendars()
        {
            CalCopyManager copy = new CalCopyManager(_manager);
            copy.CopyCalendars().Wait();
            Logger.Log("Press any key to continue...");
            Console.ReadLine();
            pickMenu(0);
        }
        private static void pickMenu(int menuNumber)
        {
            if (menuNumber == 0)
            {
                pickMenu(mainMenu());
            }
            else if (menuNumber == 1)
            {
                settings();
            }
            else if (menuNumber == 2)
            {
                showCalendars();
            }
            else if (menuNumber == 3)
            {
                copyCalendars();
            }
        }

        private static void createFile(string filePath, string contents, bool overwrite = true)
        {
            if (!File.Exists(filePath) || overwrite) {
                using (FileStream fs = File.Create(filePath))
                using (TextWriter tw = new StreamWriter(fs))
                {
                    Logger.Log($"Creating file... {filePath}\n");
                    tw.Write(contents);
                }
            }
        }
        private static void createDirectory(string folder)
        {
            if (!Directory.Exists(folder)) {
                Logger.Log($"Creating folder... {folder}\n");
                Directory.CreateDirectory(folder);
            } 
        }
        private static Dictionary<string, string> getArgsDictionary(string[] args)
        {
            Dictionary<string, string> returnValue = new Dictionary<string, string>();

            string argsString = String.Join(" ", args);            
            MatchCollection matches = _reg.Matches(argsString);

            if (matches.Count == 0) {
                return null;
            }

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    returnValue[match.Groups["name"].Value] = match.Groups["value"].Value;
                }
            }

            return returnValue;
        }

        private static bool tryGetArg(string key, out string argValue)
        {
            bool returnValue = false;

            if (_argsDictionary.ContainsKey(key)) {
                returnValue = true;
                argValue = _argsDictionary[key];
            }
            else {
                argValue = "";
            }

            return returnValue;
        }        
    }
}

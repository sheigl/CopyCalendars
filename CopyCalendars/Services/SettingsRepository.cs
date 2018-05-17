using System;
using System.Threading.Tasks;
using CopyCalendars.Models;
using System.IO;
using CopyCalendars.Data;

namespace CopyCalendars.Services
{
    public class SettingsRepository
    {
        private JsonDB<Settings> _db;
        public SettingsRepository()
        {
            _db = new JsonDB<Settings>();
        }

        public Settings Get()
        {
            Settings settings = _db.Read();

            if (settings == null)
            {
                _db.Write(new Settings());
            }

            Settings.SetCurrent(settings);
            return Settings.Current;
        }
        public void Save(Settings settings)
        {
            _db.Write(settings);

            Settings.SetCurrent(settings);                     
        }
    }
}

using System;
using System.Threading.Tasks;
using CopyCalendars.Data;
using CopyCalendars.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CopyCalendars.Services
{
    public class SettingsRepository
    {
        public SettingsRepository()
        {
            using(var db = new LocalDbContext())
            {
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "CopyCalendars");

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

                db.Database.EnsureCreated();
            }
        }

        public async Task<Settings> Get()
        {
            using(var db = new LocalDbContext()) 
            {
                Settings.SetCurrent(await db.Settings.FirstOrDefaultAsync());
                return Settings.Current;
            }
        }
        public async Task Save(Settings settings)
        {
            using(var db = new LocalDbContext())
            {
                if (settings.Id > 0)
                {
                    db.Settings.Update(settings);
                }
                else
                {
                    await db.Settings.AddAsync(settings);
                }

                Settings.SetCurrent(settings);

                await db.SaveChangesAsync();
            }
        }
    }
}

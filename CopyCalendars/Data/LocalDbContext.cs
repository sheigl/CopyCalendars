using System;
using System.IO;
using CopyCalendars.Models;
using Microsoft.EntityFrameworkCore;

namespace CopyCalendars.Data
{
    public class LocalDbContext : DbContext
    {
        public DbSet<Settings> Settings { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "CopyCalendars");
            optionsBuilder.UseSqlite($@"Data Source={path}/localdb.db");
		}
    }
}

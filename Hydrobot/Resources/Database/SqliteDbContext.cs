﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hydrobot.Resources.Database {
    public class SqliteDbContext : DbContext {

        public DbSet<Ounce> ounces { get; set; }
        public DbSet<Server> servers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder Options) {
            string DbLocation = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\HydroBot.dll", @"Data\");
            Options.UseSqlite($"Data source=Data/Database.sqlite");

        }
    }
}

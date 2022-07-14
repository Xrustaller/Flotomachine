﻿using Microsoft.EntityFrameworkCore;

namespace Flotomachine.Services;

public class MainBaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<CardId> CardIds { get; set; }

    public DbSet<ModuleType> ModuleTypes { get; set; }
    public DbSet<ModuleData> ModuleData { get; set; }

    public MainBaseContext()
    {

    }

    public MainBaseContext(DbContextOptions<MainBaseContext> options) : base(options)
    {
          
    }

    public static DbContextOptions<MainBaseContext> BuildDbContextOptionsSqlite(string serverOptions)
    {
        DbContextOptionsBuilder<MainBaseContext> optionsBuilder = new();
        DbContextOptions<MainBaseContext> options = optionsBuilder.UseSqlite(serverOptions).Options;
        return options;
    }

}
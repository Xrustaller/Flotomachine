using Microsoft.EntityFrameworkCore;

namespace Flotomachine.Services;

public class MainBaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<CardId> CardIds { get; set; }

    public DbSet<Module> Modules { get; set; }
    public DbSet<ModuleField> ModuleFields { get; set; }
    public DbSet<Experiment> Experiments { get; set; }
    public DbSet<ExperimentData> ExperimentDatas { get; set; }

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
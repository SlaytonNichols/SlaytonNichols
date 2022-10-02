using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using SlaytonNichols.Migrations;

[assembly: HostingStartup(typeof(SlaytonNichols.ConfigureDbMigrations))]

namespace SlaytonNichols;

public class ConfigureDbMigrations : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureAppHost(afterAppHostInit:appHost => {
            var migrator = new Migrator(appHost.Resolve<IDbConnectionFactory>(), typeof(Migration1020).Assembly);
            AppTasks.Register("migrate", _ => migrator.Run());
            AppTasks.Register("migrate.revert", args => migrator.Revert(args[0]));
            AppTasks.Run();
        });
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleWarehouse.Common;
using SimpleWarehouse.EFService;
using System.IO;
using System.Windows;
namespace SimpleWarehouse.WPFView
{
  public partial class App : Application
  {
    public static IHost Host { get; private set; } = null!;
    private readonly ILogger _log;
    public App()
    {
      var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("configuration.json");
      var config = builder.Build();
      Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
      .ConfigureServices((hostContext, services) =>
      {
        services
        .AddSingleton<MainWindow>()
        .AddTransient<IRepository, EFRepository>()
        .AddTransient<ILogger>(_ => new NLogger(config["LoggerName"]!))
        .AddDbContext<DataDbContext>(options => options.UseSqlite(config["ConnectionString"]));
      })
      .Build();
      _log = Host.Services.GetRequiredService<ILogger>();
    }
    protected override async void OnStartup(StartupEventArgs e)
    {
      _log.Write(LogType.Info, $"{typeof(App).Assembly.GetName().Name} started");
      await Host.StartAsync();
      MainWindow = Host.Services.GetRequiredService<MainWindow>();
      MainWindow.Show();

      base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
      await Host.StopAsync();
      Host.Dispose();
      _log.Write(LogType.Info, $"{typeof(App).Assembly.GetName().Name} exited");
      base.OnExit(e);
    }
  }
}

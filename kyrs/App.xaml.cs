using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using kyrs.Data;


namespace kyrs
{
   
    public partial class App : Application
    {
        private IHost _host;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");


                    services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));


                    services.AddSingleton<MainWindow>();
                })
                .Build();


            _host.Start();


            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }


        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);


        }


    }


}

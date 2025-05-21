using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoneService.BLL.Services;
using PhoneService.DAL.Data;
using PhoneService.DAL.Repository;
using PhoneService.Models;
using PhoneService.ViewModels;
using PhoneService.Views;
using System.IO;

namespace PhoneService
{
    internal class Program
    {
        [STAThread]
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<App>();
                    services.AddSingleton<MainWindow>();

                    services.AddDbContext<ApplicationContext>(options =>
                    {
                        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        string phoneServicePath = Path.Combine(documentsPath, "PhoneService");
                        Directory.CreateDirectory(phoneServicePath);
                        options.UseSqlite("Data Source=" + phoneServicePath + "\\Database.db");
                    });

                    services.AddTransient<UserRepository>();
                    services.AddTransient<ServiceRepository>();
                    services.AddTransient<DetailRepository>();
                    services.AddTransient<ServiceRepository>();
                    services.AddTransient<StockRepository>();
                    services.AddTransient<RequestRepository>();
                    services.AddTransient<UsedDetailRepository>();
                    services.AddTransient<ReviewRepository>();

                    services.AddTransient<AuthService>();
                    services.AddTransient<PasswordService>();
                    services.AddTransient<ImageService>();
                    services.AddTransient<StorageManager>();
                    services.AddTransient<UserManager>();
                    services.AddTransient<ServiceManager>();
                    services.AddTransient<RequestManager>();
                    services.AddTransient<ReviewManager>();
                    services.AddTransient<StockManager>();
                    services.AddTransient<EmailSender>();

                    services.AddSingleton<ViewsVM>();
                    services.AddSingleton<AuthVM>();
                    services.AddSingleton<StorageVM>();
                    services.AddSingleton<UserVM>();
                    services.AddSingleton<ServicesVM>();
                    services.AddSingleton<CreateRequestVM>();
                    services.AddSingleton<StockVM>();
                    services.AddSingleton<RequestsVM>();
                    services.AddSingleton<SingleRequestVM>();
                    services.AddSingleton<ReviewVM>();

                    services.AddSingleton<AuthorizationPage>();
                    services.AddSingleton<RegistrationPage>();
                    services.AddSingleton<StoragePage>();
                    services.AddSingleton<UserPage>();
                    services.AddSingleton<ServicesPage>();
                    services.AddSingleton<CreateRequestPage>();
                    services.AddSingleton<StockPage>();
                    services.AddSingleton<RequestsPage>();
                    services.AddSingleton<SingleRequestPage>();
                    services.AddSingleton<ReviewPage>();

                    services.AddSingleton<Mediator>();
                })
                .Build();


            var app = host.Services.GetService<App>();
            app?.Run();
        }
    }
}

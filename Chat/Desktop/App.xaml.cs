using ChatCore.Factories;
using ChatDesktop.Factories;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using ChatDesktop.Services;
using ChatDesktop.ViewModels.Authorization;
using ChatDesktop.ViewModels.Base;
using ChatDesktop.ViewModels.SlideMenu;
using ChatDesktop.ViewModels.BaseWindow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using ChatDesktop.Views.BaseWindow;
using ChatCore.Services;

namespace ChatDesktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            Client.SetLanguage(ChatDesktop.Properties.UserSettings.Default.Lang);
            Rsa.GenerateKey();

            var mainWindow = ServiceProvider.GetRequiredService<AuthorizeWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<AuthorizeVM>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // services
            services.AddTransient(typeof(IClone<>), typeof(CloneService<>));
            services.AddTransient<IAccount, AccountService>();
            services.AddSingleton<IChatHub, ChatHubService>();
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<IFile, FileService>();
            services.AddTransient<IConference, ConferenceService>();

            // client
            services.AddSingleton(typeof(Client));

            // config
            services.AddSingleton(Configuration);

            // main view
            services.AddSingleton<MainVM>();

            // authorize
            services.AddScoped<AuthBase, LoginVM>();
            services.AddScoped<AuthBase, RegisterVM>();
            services.AddScoped<AuthBase, ForgotPassword>();
            services.AddScoped<AuthorizeVM>();

            // Base content
            services.AddScoped<BaseContentVM, UserVM>();
            services.AddScoped<BaseContentVM, ChatVM>();
            services.AddScoped<BaseContentVM, GroupCreateVM>();
            services.AddScoped<BaseContentVM, AppSettingsVM>();

            // slide menu
            services.AddScoped<SlideMenuBase, AddUserVM>();
            services.AddScoped<SlideMenuBase, ChatListVM>();
            services.AddScoped<SlideMenuBase, ContactsVM>();
            services.AddSingleton<SlideMenuVM>();

            // factory
            services.AddTransient<IAbstractFactory<Window>, WindowFactory>();
            services.AddTransient<IAbstractFactory<UserVM>, BaseUserFactory>();
            services.AddTransient<IAbstractFactory<ChatVM>, BaseChatFactory>();

            // windows
            services.AddSingleton(typeof(MainWindow));
            services.AddSingleton(typeof(AuthorizeWindow));
        }
    }
}

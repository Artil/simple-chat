using ChatCore.Factories;
using ChatDesktop.ViewModels.BaseWindow;
using ChatDesktop.Views.BaseWindow;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace ChatDesktop.Factories
{
    public class WindowFactory : IAbstractFactory<Window>
    {
        private readonly IServiceProvider _serviceProvider;
        public WindowFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Window GetInstance()
        {
            var window = _serviceProvider.GetService<MainWindow>();
            window.DataContext = _serviceProvider.GetService<MainVM>();
            return window;
        }
    }
}

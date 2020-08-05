using ChatCore.Factories;
using ChatDesktop.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ChatDesktop.Factories
{
    public class BaseChatFactory : IAbstractFactory<ChatVM>
    {
        private readonly IServiceProvider _serviceProvider;
        public BaseChatFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ChatVM GetInstance()
        {
            return _serviceProvider.GetServices<BaseContentVM>()
                .FirstOrDefault(x => x.GetType() == typeof(ChatVM)) 
                as ChatVM;
        }
    }
}

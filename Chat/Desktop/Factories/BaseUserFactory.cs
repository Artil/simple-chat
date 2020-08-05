using ChatCore.Factories;
using ChatDesktop.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatDesktop.Factories
{
    public class BaseUserFactory : IAbstractFactory<UserVM>
    {
        private readonly IServiceProvider _serviceProvider;
        public BaseUserFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public UserVM GetInstance()
        {
            return _serviceProvider.GetServices<BaseContentVM>()
                .FirstOrDefault(x => x.GetType() == typeof(UserVM))
                as UserVM;
        }
    }
}

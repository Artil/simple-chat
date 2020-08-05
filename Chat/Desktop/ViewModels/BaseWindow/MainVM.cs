using ChatCore.Models;
using ChatDesktop.Enums;
using ChatDesktop.Models;
using ChatDesktop.ViewModels.Base;
using ChatDesktop.ViewModels.SlideMenu;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChatDesktop.ViewModels.BaseWindow
{
    public class MainVM : BaseWindow
    {
        public MainVM(SlideMenuVM slideMenu, IEnumerable<BaseContentVM> tabs) 
        {
            SlideMenu = slideMenu;

            var userVM = tabs.FirstOrDefault(x => x.GetType() == typeof(UserVM)) as UserVM;
            userVM.CurrentUserLoaded += slideMenu.GetCurrentUser;

            Tabs = new ObservableCollection<BaseContentVM>(tabs);
            baseContent = Tabs.FirstOrDefault();

            Client.BaseContentChanged += ChangeBaseContent;
        }

        private void ChangeBaseContent(BaseContentEnum baseContent, object value)
        {
            BaseContent?.OnLostSelection();
            // set current tab
            BaseContent = Tabs.ElementAtOrDefault((int)baseContent); 

            if(!(value is null)) // fill current view values
                Client.SetValues(BaseContent, value); 

            BaseContent?.OnSelected();
        }

        public SlideMenuVM SlideMenu { get; set; }

        public ObservableCollection<BaseContentVM> Tabs;

        private BaseContentVM baseContent;

        public BaseContentVM BaseContent
        {
            get => baseContent;
            set
            {
                if (baseContent.Equals(value))
                    return;
                baseContent = value;
                OnPropertyChanged();
            }
        }
    }
}

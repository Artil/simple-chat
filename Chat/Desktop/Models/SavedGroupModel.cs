using ChatCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ChatDesktop.Models
{
    public class SavedGroupModel
    {
        public ObservableCollection<UserListModel> AvaibleUserList { get; set; }
        public ObservableCollection<UserListModel> UserList { get; set; }
        public bool IsExpanded { get; set; }
        public string ChatName { get; set; }
        public string ChatId { get; set; }
        public bool IsPublic { get; set; }
        public bool CreatorMode { get; set; }
        public string Color { get; set; }
        public string PhotoPath { get; set; }
    }
}

using ChatServer.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServer.Models.ChatModels
{
    public class ChatForGroup : BaseDbModel
    {
        [ForeignKey("Chat")]
        public string ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        public bool isPublic { get; set; }

        public virtual ICollection<GroupsUsers> GroupsUsers { get; set; }

        public ChatForGroup()
        {
            GroupsUsers = new HashSet<GroupsUsers>();
        }
    }
}

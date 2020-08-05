
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatDbCore.ChatModels
{
    public class ChatForGroup : BaseDbModel
    {
        [ForeignKey("Chat")]
        public string ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        public bool IsPublic { get; set; }
        public bool CreatorMode { get; set; }
        [ForeignKey("FileDb")]
        public string File { get; set; }
        public virtual FileDb FileDb { get; set; }

        public virtual IEnumerable<GroupsUsers> GroupsUsers { get; set; }

        public ChatForGroup()
        {
            GroupsUsers = new HashSet<GroupsUsers>();
        }
    }
}

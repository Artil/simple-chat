using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ChatCore.Enums;
using ChatDbCore.Account;

namespace ChatDbCore.ChatModels
{
    public class Chat : BaseDbModel
    {
        public string ChatName { get; set; }
        [ForeignKey("User")]
        public string CreatorId { get; set; }
        public virtual User User { get; set; }
        public ChatEnum ChatType { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }
    }
}

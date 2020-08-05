using ChatServer.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServer.Models.ChatModels
{
    public class GroupsUsers
    {
        [ForeignKey("ChatForGroup")]
        public string GroupId { get; set; }
        public virtual ChatForGroup ChatForGroup { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public bool? isRemoved { get; set; }
    }
}

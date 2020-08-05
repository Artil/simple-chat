using ChatServer.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServer.Models.ChatModels
{
    public class ChatForTwo : BaseDbModel
    {
        [ForeignKey("Chat")]
        public string ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        [ForeignKey("FirstUser")]
        public string FirstUserId { get; set; }
        public virtual User FirstUser { get; set; }
        [ForeignKey("SecondUser")]
        public string SecondUserId { get; set; }
        public virtual User SecondUser { get; set; }
    }
}

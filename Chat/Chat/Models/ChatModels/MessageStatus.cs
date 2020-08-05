using ChatServer.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServer.Models.ChatModels
{
    public class MessageStatus : BaseDbModel
    {
        [ForeignKey("Message")]
        public string MessageId { get; set; }
        public virtual Message Message { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public bool isRead { get; set; }
        public bool isDeleted { get; set; }
    }
}

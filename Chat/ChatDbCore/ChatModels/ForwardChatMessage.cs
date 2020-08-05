using ChatDbCore.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatDbCore.ChatModels
{
    public class ForwardChatMessage : BaseDbModel
    {
        [ForeignKey("Chat")]
        public string ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("Message")]
        public string MessageId { get; set; }
        public virtual Message Message { get; set; }
        public DateTime Created { get; set; }
    }
}

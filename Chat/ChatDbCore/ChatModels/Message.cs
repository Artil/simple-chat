using ChatDbCore.Account;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatDbCore.ChatModels
{
    public class Message : BaseDbModel
    {
        [ForeignKey("Chat")]
        public string ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string Content { get; set; }
        [ForeignKey("FileDb")]
        public string File { get; set; }
        public virtual FileDb FileDb { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}

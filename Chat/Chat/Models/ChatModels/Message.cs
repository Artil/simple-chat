using ChatServer.Models;
using ChatServer.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServer.Models.ChatModels
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
        public string File { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}

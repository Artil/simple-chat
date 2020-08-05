using ChatDbCore.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatDbCore.ChatModels
{
    public class MessageStatus : BaseDbModel
    {
        [ForeignKey("Message")]
        public string MessageId { get; set; }
        public virtual Message Message { get; set; }
        [ForeignKey("ForwardMessage")]
        public string ForwardId { get; set; }
        public virtual ForwardChatMessage ForwardMessage { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
    }
}

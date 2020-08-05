using ChatDbCore.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatDbCore.ChatModels
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

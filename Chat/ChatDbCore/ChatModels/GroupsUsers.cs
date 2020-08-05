using ChatDbCore.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatDbCore.ChatModels
{
    public class GroupsUsers
    {
        [ForeignKey("ChatForGroup")]
        public string GroupId { get; set; }
        public virtual ChatForGroup ChatForGroup { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsRemoved { get; set; }
    }
}

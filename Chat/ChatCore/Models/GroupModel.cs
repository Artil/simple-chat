using System;
using System.Collections.Generic;
using System.Text;

namespace ChatCore.Models
{
    public class GroupModel : BaseUserModel
    {
        public string GroupId { get; set; }
        public bool IsPublic { get; set; }
        public bool CreatorMode { get; set; }
        public IEnumerable<string> Users { get; set; }
        public int UsersCount { get; set; }
    }
}

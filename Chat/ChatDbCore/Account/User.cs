using ChatDbCore.ChatModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatDbCore.Account
{
    public class User : IdentityUser
    {
        public virtual ICollection<GroupsUsers> GroupsUsers { get; set; }
        public User()
        {
            GroupsUsers = new HashSet<GroupsUsers>();
        }
    }
}

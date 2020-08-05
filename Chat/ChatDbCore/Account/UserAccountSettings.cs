using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatDbCore.Account
{
    public class UserAccountSettings
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public bool isEmailPublic { get; set; }
        public bool isPhonePublic { get; set; }
        public bool isAccountPublic { get; set; }
    }
}

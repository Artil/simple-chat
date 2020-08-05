using ChatDbCore.Account;
using ChatDbCore.ChatModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatDbCore.Account
{
    public class UserAccountInfo : BaseDbModel
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Job { get; set; }
        public string Company { get; set; }
        public DateTime? Birthday { get; set; }
        public string PostAdrress { get; set; }
        [ForeignKey("FileDb")]
        public string File { get; set; }
        public virtual FileDb FileDb { get; set; }
        public string UserPhoto { get; set; }
        public DateTime? DateCreate { get; set; }
    }
}

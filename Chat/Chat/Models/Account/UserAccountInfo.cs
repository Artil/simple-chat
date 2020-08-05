using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ChatServer.Models.Account
{
    public class UserAccountInfo : BaseDbModel
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string Status { get; set; }
        public string Birthday { get; set; }
        public string UserPhoto { get; set; }
        public DateTime DateCreate { get; set; }
    }
}

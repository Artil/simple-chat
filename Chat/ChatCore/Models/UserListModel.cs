using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatCore.Models
{
    public class UserListModel : BaseUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}

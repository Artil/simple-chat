using System;
using System.Collections.Generic;
using System.Text;

namespace ChatServer.Models.Account
{
    class ChangePasswordViewModel
    {
        public string Id { get; set; }
        public string userName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}

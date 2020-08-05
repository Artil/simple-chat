using System;
using System.Collections.Generic;
using System.Text;

namespace ChatCore.Enums
{
    public enum AuthorizeResultEnum
    {
        Ok,
        UserNotFound,
        WrongLoginOrPassword,
        EmailExist,
        UserNameExist
    }
}

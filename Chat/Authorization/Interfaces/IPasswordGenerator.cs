using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Interfaces
{
    public interface IPasswordGenerator
    {
        string GeneratePassword(bool useLowercase, bool useUppercase, bool useNumbers, bool useSpecial, int passwordSize);
    }
}

using Authorization.Interfaces;
using ChatDbCore.ChatModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Authorization.Services
{
    public class PasswordGenerator : IPasswordGenerator
    {
        const string LOWER_CASE = "abcdefghijklmnopqursuvwxyz";
        const string UPPER_CASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string NUMBERS = "123456789";
        const string SPECIALS = @"#?!@$%^&*,.-";

        public string GeneratePassword(bool useLowercase, bool useUppercase, bool useNumbers, bool useSpecial, int passwordSize)
        {
            var password = new StringBuilder();
            var charSet = String.Empty; // Initialise to blank
            var random = new Random();
            int counter;

            // Build up the character set to choose from
            if (useLowercase) charSet += LOWER_CASE;

            if (useUppercase) charSet += UPPER_CASE;

            if (useNumbers) charSet += NUMBERS;

            if (useSpecial) charSet += SPECIALS;

            for (counter = 0; counter < passwordSize; counter++)
            {
                password.Append(charSet[random.Next(charSet.Length - 1)]);
            }

            if (!password.ToString().Any(x => Char.IsDigit(x)))
                password.Append(NUMBERS[random.Next(NUMBERS.Length - 1)]);

            if (!password.ToString().Any(x => Char.IsUpper(x)))
                password.Append(UPPER_CASE[random.Next(UPPER_CASE.Length - 1)]);

            if (!Regex.IsMatch(password.ToString(), @"#?!@$%^&*,.-"))
                password.Append(SPECIALS[random.Next(SPECIALS.Length - 1)]);

            return password.ToString();
        }
    }
}

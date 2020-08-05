using Authorization.Interfaces;
using ChatDbCore.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Controllers
{
    [ApiController]
    public class EmailController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _email;
        private readonly IPasswordGenerator _passwordGenerator;

        public EmailController(UserManager<User> userManager, IEmailSender email, IPasswordGenerator passwordGenerator)
        {
            _userManager = userManager;
            _email = email;
            _passwordGenerator = passwordGenerator;
        }

        [Route("ConfirmEmail")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return Content("Error");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Content("Error");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return Content("Succes");
            else
                return Content("Error");
        }

        [Route("ChangePassword")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(string userId, string code)
        {
            if (userId == null || code == null)
                return Content("Error");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Content("Error");

            var password = _passwordGenerator.GeneratePassword(true, true, true, true, 7);
            var result = await _userManager.ResetPasswordAsync(user, code, password);
            if (result.Succeeded)
            {
                return Content($"New password: {password} \n Please change it after login.");
            }
            else
                return Content("Error");
        }
    }
}

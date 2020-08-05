using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Authorization.Interfaces;
using ChatCore.Enums;
using ChatCore.Models;
using ChatCore.Services;
using ChatDbCore;
using ChatDbCore.Account;
using ChatDbCore.ChatModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Authorization.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly Context _context;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;

        private static Dictionary<string, (string Key, string IV)> SecretKeys = new Dictionary<string, (string Key, string IV)>();

        public AccountController(Context context, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _emailSender = emailSender;
        }

        [Route("Login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] KeyValuePair<string, string> content)
        {
            var crypt = SecretKeys.FirstOrDefault(x => x.Key == content.Key).Value;
            var lm = JsonConvert.DeserializeObject<LoginModel>(RijndaelCrypt.DecryptString(content.Value, crypt.Key, crypt.IV));

            var user = await _userManager.FindByNameAsync(lm.UserName);
            if (ReferenceEquals(user, null))
                return Ok(AuthorizeResultEnum.UserNotFound);
            try
            {
                var result = await _signInManager.PasswordSignInAsync(lm.UserName, lm.Password, lm.RememberMe, false);

                if (result.Succeeded)
                    return Ok(RijndaelCrypt.EncryptString(await GenerateJwtToken(user), crypt.Key, crypt.IV));
                else
                    return Ok(AuthorizeResultEnum.WrongLoginOrPassword);
            }
            catch(Exception ex)
            {
                return Ok(AuthorizeResultEnum.WrongLoginOrPassword);
            }
        }

        [Route("Register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] KeyValuePair<string, string> content)
        {
            var crypt = SecretKeys.FirstOrDefault(x => x.Key == content.Key).Value;
            var rm = JsonConvert.DeserializeObject<RegisterModel>(RijndaelCrypt.DecryptString(content.Value, crypt.Key, crypt.IV));

            var exist_email = await _userManager.FindByEmailAsync(rm.Email);

            if (!(ReferenceEquals(exist_email, null)))
                return Ok(AuthorizeResultEnum.EmailExist);

            var user = new User { Email = rm.Email, UserName = rm.UserName };
            var result = await _userManager.CreateAsync(user, rm.Password);

            if (result.Succeeded)
            {
                await _context.UsersAccountInfo.AddAsync(new UserAccountInfo() { UserId = user.Id });
                await _context.SaveChangesAsync();
                await _emailSender.ConfigureRegisterEmailAsync(user);
                await _signInManager.SignInAsync(user, false);
                return Ok(RijndaelCrypt.EncryptString(await GenerateJwtToken(user), crypt.Key, crypt.IV));
            }
            else
                return Ok(AuthorizeResultEnum.UserNameExist);
        }

        [Route("ChangePassword")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<bool> ChangePassword([FromBody]string userName)
        {
            return await _emailSender.ConfigureChangePasswordEmailAsync(await _userManager.FindByNameAsync(userName));
        }

        [Route("SwapKeys")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<string> SwapKeys([FromBody] string clientPublicKey)
        {
            var crypt = RijndaelCrypt.GenerateKey();

            if (SecretKeys.ContainsKey(clientPublicKey))
                SecretKeys.Remove(clientPublicKey);

            SecretKeys.Add(clientPublicKey, crypt);

            return $"{Rsa.Encrypt(crypt.Key, clientPublicKey)},{Rsa.Encrypt(crypt.IV, clientPublicKey)}";
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt").GetSection("Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(
                _config.GetSection("Jwt").GetSection("Issuer").Value,
                _config.GetSection("Jwt").GetSection("Issuer").Value,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

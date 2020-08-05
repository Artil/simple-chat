using ChatCore.Enums;
using ChatCore.Models;
using ChatCore.Services;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ChatDesktop.Services
{
    public class AccountService : IAccount
    {
        private readonly IHttpService _httpService;
        private readonly string baseUrl;

        public AccountService(IConfiguration config, IHttpService httpService)
        {
            _httpService = httpService;
            baseUrl = config.GetSection("Connection").GetSection("account").Value;
        }

        public async Task<dynamic> Login(string data)
        {
            string url = $"{baseUrl}/Login";

            var result = await _httpService.PostApiResult(url, KeyValuePair.Create(Rsa.PublicKey, RijndaelCrypt.EncryptString(data, Client.ServerKey, Client.ServerIV)));

            if (result.All(char.IsDigit))
                return JsonConvert.DeserializeObject<AuthorizeResultEnum>(result);
            else
                Client.AccessToken = RijndaelCrypt.DecryptString(JsonConvert.DeserializeObject<string>(result), Client.ServerKey, Client.ServerIV);

            return AuthorizeResultEnum.Ok;
        }

        public async Task<dynamic> Register(string data)
        {
            string url = $"{baseUrl}/Register";
            var result = await _httpService.PostApiResult(url, KeyValuePair.Create(Rsa.PublicKey, RijndaelCrypt.EncryptString(data, Client.ServerKey, Client.ServerIV)));

            if (result.All(char.IsDigit))
                return JsonConvert.DeserializeObject<AuthorizeResultEnum>(result);
            else
                Client.AccessToken = RijndaelCrypt.DecryptString(JsonConvert.DeserializeObject<string>(result), Client.ServerKey, Client.ServerIV);

            return AuthorizeResultEnum.Ok;
        }

        public async Task<bool> ResetPassword(string userName)
        {
            string url = $"{baseUrl}/ChangePassword";
            var result = await _httpService.PostApiResult(url, userName);

            if (String.IsNullOrEmpty(result))
                return false;
            else
                return JsonConvert.DeserializeObject<bool>(result);
        }

        public async Task SwapKeys(string publicKey)
        {
            string url = $"{baseUrl}/SwapKeys";
            var result = await _httpService.PostApiResult(url, publicKey);

            if (String.IsNullOrEmpty(result))
                throw new NullReferenceException();
            else
            {
                var crypt = JsonConvert.DeserializeObject<string>(result).Split(",");

                Client.ServerKey = Rsa.Decrypt(crypt[0], Rsa.PrivateKey);
                Client.ServerIV = Rsa.Decrypt(crypt[1], Rsa.PrivateKey);
            }
        }
    }
}

using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatDesktop.Services
{
    public class HttpService : IHttpService
    {

        public async Task<string> PostApiResult(string url, dynamic data)
        {
            using (var httpCLient = new HttpClient())
            {
                httpCLient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpCLient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Client.AccessToken);

                using (var httpContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"))
                {
                    using (var response = await httpCLient.PostAsync(url, httpContent))
                    {
                        if (response.IsSuccessStatusCode)
                            return await response.Content.ReadAsStringAsync();

                        return null;
                    }
                }
            }
        }

        public async Task<byte[]> GetApiResultByte(string url)
        {
            using (var _httpClient = new HttpClient())
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Client.AccessToken);
                using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                        return await response.Content.ReadAsByteArrayAsync();

                    return null;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatDesktop.Interfaces
{
    public interface IHttpService
    {
        Task<string> PostApiResult(string url, dynamic data);
        Task<byte[]> GetApiResultByte(string url);
    }
}

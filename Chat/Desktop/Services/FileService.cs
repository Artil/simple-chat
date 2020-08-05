using ChatCore.Models;
using ChatDesktop.Interfaces;
using ChatDesktop.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ChatDesktop.Services
{
    public class FileService : IFile
    {
        private readonly string baseUrl;
        private readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private readonly IHttpService _httpService;
        public FileService(IHttpService httpService, IConfiguration config)
        {
            _httpService = httpService;
            baseUrl = config.GetSection("Connection").GetSection("files").Value;
        }

        public async Task<string> GetFile(string path)
        {
            try
            {
                var fileDirectory = baseDirectory + "AppFiles/TempFiles/";
                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                var newFilePath = fileDirectory + Path.GetFileName(path);
                if (File.Exists(newFilePath))
                    return newFilePath;

                var result = await _httpService.GetApiResultByte($"{baseUrl}/GetFile?path={path}");

                if (ReferenceEquals(result, null))
                    return String.Empty;

                await File.WriteAllBytesAsync(newFilePath, result);
                return newFilePath;
            }
            catch
            {
                return null;
            }
        }

        public async Task GetPhotos(IEnumerable<BaseUserModel> items)
        {
            try
            {
                foreach (var item in items.Where(x => !String.IsNullOrEmpty(x.PhotoPath)))
                {
                    item.PhotoPath = await GetFile(item.PhotoPath);
                }
                Client.GetSavedMedia(items);
            }
            catch { }
        }

        public async Task<string> SendFileToApi(string url, FileModel file)
        {
            return JsonConvert.DeserializeObject<string>(await _httpService.PostApiResult($"{baseUrl}{url}", file));
        }

        public async Task GetFileWithProgress(FileModel file)
        {
            try
            {
                if (!ReferenceEquals(file.Cancellation, null))
                {
                    file.Cancellation.Cancel();
                    file.DownloadProgress = 0;
                    file.Cancellation = null;
                    return;
                }

                var openFolderDialog = new CommonOpenFileDialog();
                openFolderDialog.IsFolderPicker = true;
                if (openFolderDialog.ShowDialog() != CommonFileDialogResult.Ok)
                    return;

                string folder = openFolderDialog.FileName + "/" + Path.GetFileName(file.FileFullName);

                var url = $"{baseUrl}/GetFile?path={file.FileFullName}";

                file.DownloadProgress = 0;
                file.Cancellation = new CancellationTokenSource();

                using (var client = new HttpDownloadService(url, folder, file.Cancellation.Token))
                {
                    client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                    {
                        file.DownloadProgress = (int)progressPercentage;
                    };

                    await client.StartDownload();

                    file.Cancellation = null;
                }
            }
            catch { }
        }
    }
}

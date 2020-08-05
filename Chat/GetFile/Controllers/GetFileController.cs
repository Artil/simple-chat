using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ChatCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GetFile.Controllers
{
    [Authorize]
    public class GetFileController : ControllerBase
    {
        private readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        [Route("GetFile")]
        [HttpGet]
        public async Task<IActionResult> GetFile([FromQuery]string path)
        {
            path = baseDirectory + path;
            if (!System.IO.File.Exists(path))
                return null;

            var fileContent = await System.IO.File.ReadAllBytesAsync(path);
            return File(fileContent, "application/force-download", Path.GetFileName(path));
        }

        [Route("SendFile")]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [HttpPost]
        public async Task<string> SaveChatFile([FromQuery]string chatId, [FromBody]FileModel file)
        {
            string path = file.IsVoice ? "/VoiceMessages" : file.IsVideo ? "/VideoMessages" : file.IsImage ? "/Images" : "/Files";
            string fileDirectory = baseDirectory + "/AppFiles/ChatFiles/" + chatId + path;
            var filePath = await SaveFile(fileDirectory, file);
            return filePath;
        }

        [Route("SendPhoto")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        [HttpPost]
        public async Task<string> SendUserPhoto([FromQuery]string userId, [FromBody]FileModel file)
        {
            string fileDirectory = baseDirectory + "/AppFiles/UserPhotos/" + userId;

            file.FileContent = await ResizeImage(file.FileContent, 100, 100);

            var filePath = await SaveFile(fileDirectory, file);
            return filePath;
        }

        [Route("SendChatPhoto")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        [HttpPost]
        public async Task<string> SendChatPhoto([FromQuery]string chatId, [FromBody]FileModel file)
        {
            var a = HttpContext.User.Identity.Name;
            var path = String.IsNullOrEmpty(chatId) ? "/AppFiles/GroupPhotos" : "/AppFiles/GroupPhotos/" + chatId;
            string fileDirectory = baseDirectory + path;

            file.FileContent = await ResizeImage(file.FileContent, 100, 100);

            var filePath = await SaveFile(fileDirectory, file);
            return filePath;
        }

        private async Task<byte[]> ResizeImage(byte[] imageContent, int width, int height)
        {
            var ms = new MemoryStream(imageContent);
            var image = Image.FromStream(ms);
            ms.Dispose();

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            var mem = new MemoryStream();
            destImage.Save(mem, System.Drawing.Imaging.ImageFormat.Jpeg);

            return mem.ToArray();
        }

        private async Task<string> SaveFile(string fileDirectory, FileModel file)
        {
            try
            {
                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                System.IO.File.WriteAllBytes(fileDirectory + "/" + file.FileName, file.FileContent);

                return fileDirectory.Remove(0, baseDirectory.Length) + "/" + file.FileFullName;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
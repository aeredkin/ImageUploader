using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PhotoSauce.MagicScaler;

namespace ImageUploader.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageUploaderController : ControllerBase
    {
        public ImageUploaderController()
        {
            Directory.CreateDirectory("Images");
            Directory.CreateDirectory("Previews");
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            if (Request.ContentType.StartsWith("multipart/form-data"))
            {
                foreach (IFormFile file in Request.Form.Files)
                {
                    await SaveFile(file.OpenReadStream(), file.FileName);
                }
            }
            else if (Request.ContentType.StartsWith("application/json"))
            {
                using StreamReader streamReader = new StreamReader(Request.Body);
                JArray jArray = JArray.Parse(await streamReader.ReadToEndAsync());
                foreach (JObject jObject in jArray)
                {
                    using MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(jObject["file"].ToString()));
                    await SaveFile(memoryStream, jObject["name"].ToString());
                }
            }
            else if (Request.ContentType.StartsWith("text"))
            {
                using StreamReader streamReader = new StreamReader(Request.Body);
                string uri = await streamReader.ReadToEndAsync();
                using HttpClient httpClient = new HttpClient();
                await SaveFile(await httpClient.GetStreamAsync(uri), uri);
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }

        private readonly ProcessImageSettings settings = new ProcessImageSettings { Height = 100, Width = 100 };

        private async Task SaveFile(Stream file, string path)
        {
            using FileStream imageStream = System.IO.File.Create("Images/" + Guid.NewGuid().ToString() + Path.GetExtension(path));
            await file.CopyToAsync(imageStream);
            imageStream.Position = 0;
            using FileStream previewStream = System.IO.File.Create("Previews/" + Path.GetFileName(imageStream.Name));
            MagicImageProcessor.ProcessImage(imageStream, previewStream, settings);
        }
    }
}

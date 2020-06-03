using ImageUploader;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageUploaderTests
{
    [TestClass]
    public class ImageUploaderControllerTests
    {
        private readonly WebApplicationFactory<Startup> factory = new WebApplicationFactory<Startup>();

        [TestMethod]
        public async Task PostFormData()
        {
            HttpResponseMessage result;
            using (HttpClient client = factory.CreateClient())
            {
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(System.IO.File.OpenRead("1.png")), "file", "1.png");
                    content.Add(new StreamContent(System.IO.File.OpenRead("2.png")), "file", "2.png");
                    result = await client.PostAsync("http://localhost:5000/imageuploader", content);
                };
            }
            Assert.IsTrue(result.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task PostJson()
        {
            HttpResponseMessage result;
            using (HttpClient client = factory.CreateClient())
            {
                using (StreamContent content = new StreamContent(System.IO.File.OpenRead("images.json")))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    result = await client.PostAsync("http://localhost:5000/imageuploader", content);
                };
            }
            Assert.IsTrue(result.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task PostUrl()
        {
            HttpResponseMessage result;
            using (HttpClient client = factory.CreateClient())
            {
                using (StringContent content = new StringContent("https://cdn.pixabay.com/photo/2020/05/29/22/00/field-5236879_1280.jpg"))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    result = await client.PostAsync("http://localhost:5000/imageuploader", content);
                };
            }
            Assert.IsTrue(result.IsSuccessStatusCode);
        }
    }
}

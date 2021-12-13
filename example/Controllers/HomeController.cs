using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;

namespace example.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        private readonly string endpoint = "18.141.10.239:9000";
        private readonly string accessKey = "minio";
        private readonly string secretKey = "apple102030+++";

        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<string> output = new List<string>();

            MinioClient minio = new MinioClient(endpoint, accessKey, secretKey); //.WithSSL();

            // Create an async task for listing buckets.
            var bucketResult = await minio.ListBucketsAsync();

            // Iterate over the list of buckets.
            foreach (Bucket bucket in bucketResult.Buckets)
            {
                output.Add(bucket.Name + " " + bucket.CreationDateDateTime);
            }

            return Content(string.Join("<br/>", output.ToArray()));
        }

        public async Task<IActionResult> UpLoadFile([FromForm] IFormFile file)
        {
            string[] fileNameMsg = file.FileName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            string newName = DateTime.Now.ToString("yyMMddHHmmssfff") + new Random().Next(1000, 9999)
                             + "." + fileNameMsg[fileNameMsg.Length - 1];

            string objectName = newName;


            MinioClient minio = new MinioClient(endpoint, accessKey, secretKey); //.WithSSL();
            var bucketName = "test";
            var data = file.OpenReadStream();
            var contentType = file.ContentType;

            try
            {
                // Make a bucket on the server, if not already present.
                bool found = await minio.BucketExistsAsync(bucketName);
                if (!found)
                {
                    await minio.MakeBucketAsync(bucketName);
                }

                // Upload a file to bucket.
                await minio.PutObjectAsync(bucketName, objectName, data, data.Length, contentType);

                return Content("Successfully uploaded " + objectName);
            }
            catch (Exception e)
            {
                return Content("File Upload Error: {0}", e.Message);
            }
        }
    }
}

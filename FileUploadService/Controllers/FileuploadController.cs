using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FileUploadService.Controllers
{
    public class FileuploadController : ApiController
    {
        public async Task<bool> Upload()
        {
            try
            {
                var fileuploadPath = ConfigurationManager.AppSettings["FileUploadLocation"];

                var provider = new MultipartFormDataStreamProvider(fileuploadPath);
                var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                foreach (var header in Request.Content.Headers)
                {
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                await content.ReadAsMultipartAsync(provider);

                //Code for renaming the random file to Original file name
                string uploadingFileName = provider.FileData.Select(x => x.LocalFileName).FirstOrDefault();
                string originalFileName = String.Concat(fileuploadPath, "\\" + (provider.Contents[0].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' }));

                if (File.Exists(originalFileName))
                {
                    File.Delete(originalFileName);
                }

                File.Move(uploadingFileName, originalFileName);
                //Code renaming ends...

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}

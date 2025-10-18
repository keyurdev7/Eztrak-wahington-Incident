using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Extensions
{
    public static class ImageUrlExtension
    {
        public static async Task<string> ConvertToBase64(string path, string domain)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var relativePath = domain + path;
                    var bytes = await client.GetByteArrayAsync(relativePath); // there are other methods if you want to get involved with stream processing etc
                    var base64String = Convert.ToBase64String(bytes);
                    return "data:image/png;base64, " + base64String;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}

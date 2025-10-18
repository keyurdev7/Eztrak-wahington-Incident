using Interfaces;
using System.Net;
using System.Web;

namespace Repository
{
    public class SmsService : ISms
    {
        static string MyUsername = "centangle@bizsms.pk"; //Your Username At Sendpk.com 
        static string MyPassword = "c3tng39"; //Your Password At Sendpk.com 
        static string Masking = "TimeKeeping"; //Your Company Brand Name 
        public async Task<bool> SendSms(string toNumber, string MessageText)
        {
            if (!string.IsNullOrEmpty(toNumber))
            {
                if (toNumber.StartsWith("0"))
                {
                    toNumber = toNumber.Remove(0, 1);
                    toNumber = "92" + toNumber;
                }
                toNumber = toNumber.Replace("-", string.Empty);

                MessageText = HttpUtility.UrlEncode(MessageText, System.Text.Encoding.GetEncoding("ISO-8859-1"));

                string URI = "http://api.bizsms.pk" +
                "/api-send-branded-sms.aspx?" +
                "username=" + MyUsername +
                "&pass=" + MyPassword +
                "&text=" + MessageText +
                "&masking=" + Masking +
                "&destinationnum=" + toNumber +
                "&language=English";

                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                    | SecurityProtocolType.Tls11
                    | SecurityProtocolType.Tls12;

                    // Skip validation of SSL/TLS certificate
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    WebRequest req = WebRequest.Create(URI);
                    WebResponse resp = await req.GetResponseAsync();
                    var sr = new System.IO.StreamReader(resp.GetResponseStream());
                    return true;
                }
                catch (WebException ex)
                {
                    var httpWebResponse = ex.Response as HttpWebResponse;
                    if (httpWebResponse != null)
                    {
                        var error = "";
                        switch (httpWebResponse.StatusCode)
                        {
                            case HttpStatusCode.NotFound:
                                error = "404:URL not found :" + URI; break;
                            case HttpStatusCode.BadRequest:
                                error = "400:Bad Request"; break;
                            default:
                                error = httpWebResponse.StatusCode.ToString(); break;
                        }
                    }
                    return false;
                }
            }
            return false;
        }
    }
}

using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Security.Policy;

namespace Web.Extensions
{
    public class FileImageTagHelper : TagHelper
    {
        public string FileType { get; set; }
        public int Size { get;set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;
            output.Attributes.Add("src", GetFileIconUrl(FileType));
            output.Attributes.Add("height", Size);
            output.Attributes.Add("width", Size);
            base.Process(context, output);
        }

        private string GetFileIconUrl(string filtType)
        {
            string baseUrl = "/img/file-icons/";
            string url = "";
            switch (filtType)
            {
                case ".pdf":
                    url = "pdf.png";
                    break;
                case ".docx":
                    url = "docx.png";
                    break;
                case ".jpeg":
                    url = "jpeg.png";
                    break;
                case ".jpg":
                    url = "jpg.png";
                    break;
                case ".png":
                    url = "png.png";
                    break;
                case ".pptx":
                    url = "pptx.png";
                    break;
                case ".xlsx":
                    url = "xlsx.png";
                    break;
                case "link":
                    url = "link.png";
                    break;
                default:
                    url = "default.png";
                    break;
            }
            return baseUrl + url;

        }

    }
}

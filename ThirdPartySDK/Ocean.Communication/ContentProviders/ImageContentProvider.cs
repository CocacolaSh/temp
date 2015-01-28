using System;
using System.Threading.Tasks;
using Ocean.Communication.ContentProviders.Core;
using Microsoft.Security.Application;
using Ocean.Communication.Common;

namespace Ocean.Communication.ContentProviders
{
    public class ImageContentProvider : CollapsibleContentProvider
    {
        protected override Task<ContentProviderResult> GetCollapsibleContent(ContentProviderHttpRequest request)
        {
            string url = request.RequestUri.ToString();

            return TaskAsyncHelper.FromResult(new ContentProviderResult()
            {
                Content = String.Format(@"<img style='max-width:300px;' src='{0}' />", Encoder.HtmlAttributeEncode(url)),
                Title = url
            });
        }

        public override bool IsValidContent(Uri uri)
        {
            string path = uri.AbsolutePath.ToLower();

            return IsValidImagePath(path);
        }

        public static bool IsValidImagePath(string path)
        {
            return path.EndsWith(".png") ||
                   path.EndsWith(".bmp") ||
                   path.EndsWith(".jpg") ||
                   path.EndsWith(".jpeg") ||
                   path.EndsWith(".gif");
        }
    }
}
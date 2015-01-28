using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ocean.Communication.ContentProviders.Core;

namespace Ocean.Communication.ContentProviders
{
    public class YoukuContentProvider : EmbedContentProvider
    {
        //URL格式如：http://v.youku.com/v_show/id_XNDkxNDE3MDQw.html
        private static readonly Regex YoukuRegex = new Regex(@"id_(\w+)[=.]", RegexOptions.IgnoreCase);

        public override string MediaFormatString
        {
            get
            {
                return @"<embed src='http://player.youku.com/player.php/sid/{0}/v.swf' allowFullScreen='true' quality='high' width='100%' height='200' align='middle' allowScriptAccess='always' type='application/x-shockwave-flash'></embed>";
            }
        }

        public override IEnumerable<string> Domains
        {
            get
            {
                yield return "http://v.youku.com";
            }
        }

        protected override IList<string> ExtractParameters(Uri responseUri)
        {
            Match match = YoukuRegex.Match(responseUri.ToString());

            if (!match.Success)
            {
                return null;
            }

            string videoId = match.Groups[1].Value;
            return new List<string> { videoId };
        }
    }
}
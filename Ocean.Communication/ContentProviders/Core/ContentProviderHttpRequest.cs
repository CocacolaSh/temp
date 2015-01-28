using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocean.Communication.ContentProviders.Core
{
    public class ContentProviderHttpRequest
    {
        public ContentProviderHttpRequest(Uri url)
        {
            RequestUri = url;
        }

        public Uri RequestUri { get; private set; }
    }
}
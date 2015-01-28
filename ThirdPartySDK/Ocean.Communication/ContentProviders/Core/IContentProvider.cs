using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Ocean.Communication.ContentProviders.Core
{
    [InheritedExport]
    public interface IContentProvider
    {
        Task<ContentProviderResult> GetContent(ContentProviderHttpRequest request);
        bool IsValidContent(Uri uri);
    }
}
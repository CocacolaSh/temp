using System.Threading.Tasks;

namespace Ocean.Communication.ContentProviders.Core
{
    public interface IResourceProcessor
    {
        Task<ContentProviderResult> ExtractResource(string url);
    }
}
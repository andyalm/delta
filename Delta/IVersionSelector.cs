using System.Net.Http;

namespace Delta
{
    public interface IVersionSelector
    {
        int GetVersion(HttpRequestMessage request);
    }
}
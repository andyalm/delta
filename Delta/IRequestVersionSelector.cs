using System.Net.Http;

namespace Delta
{
    public interface IRequestVersionSelector
    {
        int GetVersion(HttpRequestMessage request);
    }
}
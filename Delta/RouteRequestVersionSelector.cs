using System;
using System.Net.Http;

namespace Delta
{
    public class RouteRequestVersionSelector : IRequestVersionSelector
    {
        public int GetVersion(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();
            object version;
            if (routeData.Values.TryGetValue("Version", out version))
                return Convert.ToInt32(version);
            
            throw new InvalidOperationException("Could not determine the version from the Route Data.");
        }
    }
}
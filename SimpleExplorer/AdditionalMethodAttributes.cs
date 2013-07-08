using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AttributeRouting.Web.Http;

namespace RouteAttribExplorer
{
    public class HEADAttribute : HttpRouteAttribute
    {
        public HEADAttribute(string routeUrl)
            : base(routeUrl, HttpMethod.Head)
        {
        }
    }

    public class OPTIONSAttribute : HttpRouteAttribute
    {
        public OPTIONSAttribute(string routeUrl)
            : base(routeUrl, HttpMethod.Options)
        {
        }
    }
}

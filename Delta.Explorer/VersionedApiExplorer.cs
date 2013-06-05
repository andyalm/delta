using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Delta.Explorer;
using Delta;

namespace DeltaExplorer
{
    public class VersionedApiExplorer : ApiExplorer
    {
        public VersionedApiExplorer(HttpConfiguration configuration) : base(configuration)
        {
        }


        //We're overriding this to determine which routes validly map to a given controller using our adjusted architecture.
        public override bool ShouldExploreController(string controllerVariableValue, System.Web.Http.Controllers.HttpControllerDescriptor controllerDescriptor, System.Web.Http.Routing.IHttpRoute route)
        {
            var routeVersion = int.Parse(route.Defaults["Version"].ToString());

            int controllerVersion =
                new NamespaceControllerVersionSelector().GetVersion(controllerDescriptor.ControllerType);

            if (controllerVersion > routeVersion || controllerDescriptor.DeprecatedVersion() < routeVersion) return false;

            //Is route version greater than controller version? If so, we need to check to see whether another
            //controller will take this or not...

            //This appears to work interestingly in the short term by tacking a Deprecated flag onto Cart1, but we should
            //be able to do it through reflection..

            if (routeVersion > controllerVersion)
            {
                
            }



            return base.ShouldExploreController(controllerVariableValue, controllerDescriptor, route);
        }

    }
}

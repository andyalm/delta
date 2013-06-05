using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using Delta;

namespace Harness2
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            const bool useCustom = true;


            var config = GlobalConfiguration.Configuration;

            if(useCustom)
                config.Services.Replace(typeof(IHttpControllerSelector), new DeltaVersionedControllerSelector(config, new RouteRequestVersionSelector(), new NamespaceControllerVersionSelector()));
            

            AreaRegistration.RegisterAllAreas();

            var customConfig = DocConfiguration.Configuration;

            if (useCustom)
            {
                config.Routes.MapHttpRoute("main", "api/{Version}/{controller}");

                
                customConfig.Routes.MapHttpRoute("main.v1", "api/V1/{controller}",new {Version=1}); //Of course, we'd need a safer name to ensure uniqueness...
                customConfig.Routes.MapHttpRoute("main.v2", "api/V2/{controller}", new { Version = 2 });
                customConfig.Routes.MapHttpRoute("main.v3", "api/V3/{controller}", new { Version = 3 });
            }
            else
                config.Routes.MapHttpRoute("main", "api/{controller}");

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
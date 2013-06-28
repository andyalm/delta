using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Http.Description;

namespace RouteAttribExplorer
{
    public static class Extensions
    {
        public static void SetParameterDescriptions(this ApiDescription description, Collection<ApiParameterDescription> apiParameterDescriptions)
        {
            typeof(ApiDescription).GetProperty("ParameterDescriptions").SetValue(description, apiParameterDescriptions,null);
        }

    }
}

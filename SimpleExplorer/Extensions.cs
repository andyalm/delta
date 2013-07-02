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
        private const string VersionProperty = "Version";

        public static void SetParameterDescriptions(this ApiDescription description, Collection<ApiParameterDescription> apiParameterDescriptions)
        {
            typeof(ApiDescription).GetProperty("ParameterDescriptions").SetValue(description, apiParameterDescriptions,null);
        }

        internal static void SetVersion(this ApiDescription description, int version)
        {
            if(description.ActionDescriptor == null) throw new NullReferenceException("ActionDescriptor can't be null.");
            description.ActionDescriptor.Properties[VersionProperty] = version;
        }

        public static int Version(this ApiDescription description)
        {
            if (!description.ActionDescriptor.Properties.ContainsKey(VersionProperty)) return 0;
            return (int) description.ActionDescriptor.Properties[VersionProperty];
        }

    }
}

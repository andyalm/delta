using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http.Description;

namespace Delta.ApiExplorer
{
    public static class ApiDescriptionExtensions
    {
        private const string VersionProperty = "GettyVersion";
        private const string DeprecatedInVersionProperty = "GettyDeprecatedInVersion";

        internal static void SetParameterDescriptions(this ApiDescription description, Collection<ApiParameterDescription> apiParameterDescriptions)
        {
            typeof(ApiDescription).GetProperty("ParameterDescriptions").SetValue(description, apiParameterDescriptions, null);
        }

        internal static void SetSupportedRequestBodyFormatters(this ApiDescription description, Collection<MediaTypeFormatter> formatters)
        {
            typeof(ApiDescription).GetProperty("SupportedRequestBodyFormatters").SetValue(description, formatters, null);
        }

        internal static void SetSupportedResponseFormatters(this ApiDescription description, Collection<MediaTypeFormatter> formatters)
        {
            typeof(ApiDescription).GetProperty("SupportedResponseFormatters").SetValue(description, formatters, null);
        }

        internal static void SetVersion(this ApiDescription description, int version)
        {
            description.ActionDescriptor.Properties[VersionProperty] = version;
        }

        public static int Version(this ApiDescription description)
        {
            if (!description.ActionDescriptor.Properties.ContainsKey(VersionProperty)) return 0;
            return (int)description.ActionDescriptor.Properties[VersionProperty];
        }

        internal static void SetDeprecatedVersion(this ApiDescription description, int version)
        {
            description.ActionDescriptor.Properties[DeprecatedInVersionProperty] = version;
        }

        public static int DeprecatedVersion(this ApiDescription description)
        {
            if (!description.ActionDescriptor.Properties.ContainsKey(DeprecatedInVersionProperty)) return int.MaxValue;
            return (int)description.ActionDescriptor.Properties[DeprecatedInVersionProperty];
        }
    }
}

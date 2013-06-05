using System;
using System.Linq;
using System.Web.Http.Controllers;

namespace Delta
{
    public static class HttpControllerDescriptorExtensions
    {
        private const string VersionKey = "DeltaVersion";
        private const string DeprecatedVersionKey = "DeltaDeprecatedVersion";

        public static int Version(this HttpControllerDescriptor descriptor)
        {
            return (int) descriptor.Properties[VersionKey];
        }

        public static int DeprecatedVersion(this HttpControllerDescriptor descriptor)
        {
            return (int)descriptor.Properties.GetOrAdd(DeprecatedVersionKey, _ =>
            {
                var deprecatedAttribute = descriptor.GetCustomAttributes<DeprecatedInVersionAttribute>().SingleOrDefault();
                return deprecatedAttribute != null ? deprecatedAttribute.Version : Int32.MaxValue;
            });
        }

        public static void SetVersion(this HttpControllerDescriptor descriptor, int version)
        {
            descriptor.Properties[VersionKey] = version;
        }
    }
}
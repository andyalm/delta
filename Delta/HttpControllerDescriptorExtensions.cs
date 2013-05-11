using System;
using System.Web.Http.Controllers;

namespace Delta
{
    internal static class HttpControllerDescriptorExtensions
    {
        private const string VersionKey = "DeltaVersion";

        public static int Version(this HttpControllerDescriptor descriptor)
         {
             return (int)descriptor.Properties[VersionKey];
         }

         public static void SetVersion(this HttpControllerDescriptor descriptor, int version)
         {
             descriptor.Properties[VersionKey] = version;
         }
    }
}
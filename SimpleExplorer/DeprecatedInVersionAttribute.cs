using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouteAttribExplorer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DeprecatedInVersionAttribute : Attribute
    {
        public int Version { get; private set; }

        public DeprecatedInVersionAttribute(int version)
        {
            Version = version;
        }
    }
}

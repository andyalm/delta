using System;

namespace Delta
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DeprecatedInVersionAttribute : Attribute
    {
        public int Version { get; private set; }

        public DeprecatedInVersionAttribute(int version)
        {
            Version = version;
        }
    }
}
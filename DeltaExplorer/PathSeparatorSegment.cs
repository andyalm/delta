// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace DeltaExplorer
{
    // Represents a "/" separator in a URI
    internal sealed class PathSeparatorSegment : PathSegment
    {
#if ROUTE_DEBUGGING
        public override string LiteralText
        {
            get
            {
                return "/";
            }
        }

        public override string ToString()
        {
            return "\"/\"";
        }
#endif
    }
}

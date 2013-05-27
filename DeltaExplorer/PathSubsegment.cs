// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace DeltaExplorer
{
    // Represents a subsegment of a ContentPathSegment such as a parameter or a literal.
    internal abstract class PathSubsegment
    {
#if ROUTE_DEBUGGING
        public abstract string LiteralText
        {
            get;
        }
#endif
    }
}

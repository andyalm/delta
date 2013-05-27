// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Web.Http.Routing;

namespace DeltaExplorer
{
    /// <summary>
    /// Represents a URI generated from a <see cref="HttpParsedRoute"/>. 
    /// </summary>
    internal class BoundRouteTemplate
    {
        public string BoundTemplate { get; set; }

        public HttpRouteValueDictionary Values { get; set; }
    }
}

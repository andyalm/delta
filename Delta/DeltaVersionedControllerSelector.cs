// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Properties;
using System.Web.Http.Routing;

namespace Delta
{
    public class DeltaVersionedControllerSelector : IHttpControllerSelector
    {
        public static readonly string ControllerSuffix = "Controller";

        private const string ControllerKey = "controller";

        private readonly HttpConfiguration _configuration;
        
        private readonly Lazy<ConcurrentDictionary<string, SortedList<int, HttpControllerDescriptor>>> _controllerInfoCache;
        private readonly IRequestVersionSelector _requestVersionSelector;
        private readonly IControllerVersionSelector _controllerVersionSelector;

        private readonly IAssembliesResolver _assembliesResolver;
        private readonly IHttpControllerTypeResolver _controllersResolver;

        
        public DeltaVersionedControllerSelector(HttpConfiguration configuration, IRequestVersionSelector requestVersionSelector, IControllerVersionSelector controllerVersionSelector)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _controllerInfoCache = new Lazy<ConcurrentDictionary<string, SortedList<int, HttpControllerDescriptor>>>(InitializeControllerInfoCache);
            _configuration = configuration;
            _requestVersionSelector = requestVersionSelector;
            _controllerVersionSelector = controllerVersionSelector;

            _assembliesResolver = _configuration.Services.GetAssembliesResolver();
            _controllersResolver = _configuration.Services.GetHttpControllerTypeResolver();

        }

        public virtual HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            string controllerName = GetControllerName(request);
            if (String.IsNullOrEmpty(controllerName))
            {
                throw ResourceNotFound(request);
            }

            SortedList<int,HttpControllerDescriptor> controllerDescriptors;
            if (_controllerInfoCache.Value.TryGetValue(controllerName, out controllerDescriptors))
            {
                int currentVersion = _requestVersionSelector.GetVersion(request);
                var matchingController = controllerDescriptors.Values.FirstOrDefault(d => d.Version() <= currentVersion && currentVersion < d.DeprecatedVersion());
                if (matchingController != null)
                    return matchingController;
            }

            throw ResourceNotFound(request);
        }

        private static HttpResponseException ResourceNotFound(HttpRequestMessage request)
        {
            return new HttpResponseException(request.CreateErrorResponse(HttpStatusCode.NotFound, "Resource not found"));
        }

        public virtual IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            throw new NotImplementedException();
            //return _controllerInfoCache.Value.ToDictionary(c => c.Key, c => c.Value, StringComparer.OrdinalIgnoreCase);
        }

        public virtual string GetControllerName(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                return null;
            }

            // Look up controller in route data
            object controllerName = null;
            routeData.Values.TryGetValue(ControllerKey, out controllerName);
            return (string)controllerName;
        }

        private ConcurrentDictionary<string, SortedList<int, HttpControllerDescriptor>> InitializeControllerInfoCache()
        {
            var result = new ConcurrentDictionary<string, SortedList<int, HttpControllerDescriptor>>(StringComparer.OrdinalIgnoreCase);
            var controllerTypes = _controllersResolver.GetControllerTypes(_assembliesResolver);
            foreach (var controllerType in controllerTypes)
            {
                string name = controllerType.Name.Substring(0,
                                                            controllerType.Name.Length -
                                                            DefaultHttpControllerSelector.ControllerSuffix.Length);
                int version = _controllerVersionSelector.GetVersion(controllerType);

                var descriptors = result.GetOrAdd(name, _ => new SortedList<int,HttpControllerDescriptor>(new ReverseComparer()) );
                var newDescriptor = new HttpControllerDescriptor(_configuration, name, controllerType);
                newDescriptor.SetVersion(version);
                descriptors.Add(version, newDescriptor);
            }

            return result;
        }

        private class ReverseComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                if (x == y) return 0;
                return x < y ? 1 : -1;
            }
        }
    }
}

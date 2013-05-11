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
        private readonly IVersionSelector _versionSelector;
        private readonly IControllerVersionSelector _controllerVersionSelector;

        private readonly IAssembliesResolver _assembliesResolver;
        private readonly IHttpControllerTypeResolver _controllersResolver;

        
        public DeltaVersionedControllerSelector(HttpConfiguration configuration, IVersionSelector versionSelector, IControllerVersionSelector controllerVersionSelector)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _controllerInfoCache = new Lazy<ConcurrentDictionary<string, SortedList<int, HttpControllerDescriptor>>>(InitializeControllerInfoCache);
            _configuration = configuration;
            _versionSelector = versionSelector;
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
                int currentVersion = _versionSelector.GetVersion(request);
                var matchingController = controllerDescriptors.Values.FirstOrDefault(d => d.Version() <= currentVersion);
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
            //var duplicateControllers = new HashSet<string>();
            //Dictionary<string, ILookup<string, Type>> controllerTypeGroups = _controllerTypeCache.Cache;

            //foreach (KeyValuePair<string, ILookup<string, Type>> controllerTypeGroup in controllerTypeGroups)
            //{
            //    string controllerName = controllerTypeGroup.Key;

            //    foreach (IGrouping<string, Type> controllerTypesGroupedByNs in controllerTypeGroup.Value)
            //    {
            //        foreach (Type controllerType in controllerTypesGroupedByNs)
            //        {
            //            if (result.Keys.Contains(controllerName))
            //            {
            //                duplicateControllers.Add(controllerName);
            //                break;
            //            }
            //            else
            //            {
            //                result.TryAdd(controllerName, new HttpControllerDescriptor(_configuration, controllerName, controllerType));
            //            }
            //        }
            //    }
            //}

            //foreach (string duplicateController in duplicateControllers)
            //{
            //    HttpControllerDescriptor descriptor;
            //    result.TryRemove(duplicateController, out descriptor);
            //}

            var controllerTypes = _controllersResolver.GetControllerTypes(_assembliesResolver);
            foreach (var controllerType in controllerTypes)
            {
                int version = _controllerVersionSelector.GetVersion(controllerType);

                string name = controllerType.Name.Substring(0,
                                                            controllerType.Name.Length -
                                                            DefaultHttpControllerSelector.ControllerSuffix.Length);

                var descriptors = result.GetOrAdd(name, _ => new SortedList<int,HttpControllerDescriptor>(new ReverseComparer()) );
                HttpControllerDescriptor newDescriptor;
                descriptors.Add(version, newDescriptor = new HttpControllerDescriptor(_configuration, name, controllerType));

                newDescriptor.SetVersion(version);
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

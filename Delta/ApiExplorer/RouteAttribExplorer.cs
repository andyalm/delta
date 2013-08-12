using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using AttributeRouting.Web.Http;
using HttpConfiguration = System.Web.Http.HttpConfiguration;

namespace Delta.ApiExplorer
{
    public class Explorer : IApiExplorer
    {
        private readonly Assembly _routeAssembly;
        private readonly HttpConfiguration _configuration;

        private Collection<ApiDescription> _descriptions = null;
        private IDocumentationProvider _documentationProvider;
        private readonly IControllerVersionSelector _controllerVersionSelector;

        public Explorer(Assembly routeAssembly, HttpConfiguration configuration)
        {
            _routeAssembly = routeAssembly;
            _configuration = configuration;
            _controllerVersionSelector = new NamespaceControllerVersionSelector();
        }

        public Collection<ApiDescription> ApiDescriptions
        {
            get { return _descriptions ?? (_descriptions = ExtractApiDescriptions()); }
        }

        /// <summary>
        /// Gets or sets the documentation provider. The provider will be responsible for documenting the API.
        /// </summary>
        /// <value>
        /// The documentation provider.
        /// </value>
        public IDocumentationProvider DocumentationProvider
        {
            get
            {
                return _documentationProvider ?? _configuration.Services.GetDocumentationProvider();
            }
            set { _documentationProvider = value; }
        }

        private Collection<ApiDescription> ExtractApiDescriptions()
        {
            var collection = new Collection<ApiDescription>();
            var controllers = _routeAssembly.GetTypes().Where(t => typeof(ApiController).IsAssignableFrom(t));

            foreach (var controller in controllers)
            {
                var controllerDeprecatedAtVersion = GetControllerDeprecatedAtVersion(controller);

                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(MethodIsHttpRoute);
                foreach (var method in methods)
                {
                    int methodDeprecatedAtVersion = GetMethodDeprecatedAtVersion(method);

                    //We can have multiple routes on a method.
                    foreach (var routeAttribute in method.GetCustomAttributes(true).OfType<HttpRouteAttribute>())
                    {
                        var actionDescriptor = new ReflectedHttpActionDescriptor(new HttpControllerDescriptor(new HttpConfiguration(), controller.Name, controller), method)
                        {
                            Configuration = _configuration
                        };

                        var description = new ApiDescription
                        {
                            HttpMethod = RoutingAttributeMethod(routeAttribute),
                            ActionDescriptor = actionDescriptor,
                            RelativePath = routeAttribute.RouteUrl,
                            Documentation = GetApiDocumentation(actionDescriptor)
                        };

                        var parameterDesciptions = new Collection<ApiParameterDescription>(new ParameterTools(DocumentationProvider).CreateParameterDescriptions(description.ActionDescriptor));

                        description.SetParameterDescriptions(parameterDesciptions);

                        description.SupportedRequestBodyFormatters.Add(new JsonMediaTypeFormatter());
                        description.SupportedResponseFormatters.Add(new JsonMediaTypeFormatter());
                        description.SetVersion(_controllerVersionSelector.GetVersion(actionDescriptor.ControllerDescriptor.ControllerType));

                        description.SetDeprecatedVersion(methodDeprecatedAtVersion < controllerDeprecatedAtVersion ? methodDeprecatedAtVersion : controllerDeprecatedAtVersion);

                        collection.Add(description);
                    }
                }
            }

            return ExtrapolateVersions(collection);
        }

        private static int GetControllerDeprecatedAtVersion(Type controller)
        {
            int controllerDeprecatedAtVersion = int.MaxValue;

            var deprecationAttribute = controller.GetCustomAttributes(typeof(DeprecatedInVersionAttribute), true)
                                                 .Cast<DeprecatedInVersionAttribute>()
                                                 .SingleOrDefault();

            if (deprecationAttribute != null) controllerDeprecatedAtVersion = deprecationAttribute.Version;
            return controllerDeprecatedAtVersion;
        }

        private static int GetMethodDeprecatedAtVersion(MethodInfo method)
        {
            int methodDeprecatedAtVersion = int.MaxValue;

            var methodDeprecationAttribute =
                method.GetCustomAttributes(typeof(DeprecatedInVersionAttribute), true)
                      .Cast<DeprecatedInVersionAttribute>()
                      .SingleOrDefault();

            if (methodDeprecationAttribute != null)
                methodDeprecatedAtVersion = methodDeprecationAttribute.Version;
            return methodDeprecatedAtVersion;
        }

        private Collection<ApiDescription> ExtrapolateVersions(Collection<ApiDescription> descriptions)
        {
            var newDescriptions = new List<ApiDescription>();

            var versionList = descriptions.Select(d => d.Version()).Distinct().OrderBy(v => v);
            IEnumerable<ApiDescription> previousVersionDescriptions = null;
            foreach (var version in versionList)
            {
                var thisVersionDescriptions = descriptions.Where(d => d.Version() == version).ToList();
                if (previousVersionDescriptions != null)
                {
                    //check the previous version; if it's got anything we don't have, add it in.
                    foreach (var previousDescription in previousVersionDescriptions)
                    {
                        if (thisVersionDescriptions.All(d => d.RelativePath != previousDescription.RelativePath) && version < previousDescription.DeprecatedVersion())
                        {
                            var newDescription = CloneToVersion(previousDescription, version);
                            thisVersionDescriptions.Add(newDescription);
                        }
                    }
                }

                previousVersionDescriptions = thisVersionDescriptions;
                newDescriptions.AddRange(thisVersionDescriptions);
            }

            return new Collection<ApiDescription>(newDescriptions.ToList());
        }

        private ApiDescription CloneToVersion(ApiDescription originalDescription, int newVersion)
        {
            var newDescription = new ApiDescription
            {
                Documentation = originalDescription.Documentation,
                HttpMethod = originalDescription.HttpMethod,
                RelativePath = originalDescription.RelativePath,
                Route = originalDescription.Route,
            };

            //Can't just copy the reference, since we're putting the version in the ActionDescriptor's properties
            var methodInfo = originalDescription.ActionDescriptor.ControllerDescriptor.ControllerType.GetMethod(originalDescription.ActionDescriptor.ActionName);
            newDescription.ActionDescriptor = new ReflectedHttpActionDescriptor(
                new HttpControllerDescriptor(
                    new HttpConfiguration(),
                    originalDescription.ActionDescriptor.ControllerDescriptor.ControllerName,
                    originalDescription.ActionDescriptor.ControllerDescriptor.ControllerType),
                methodInfo)
            {
                Configuration = _configuration
            };

            newDescription.SetParameterDescriptions(originalDescription.ParameterDescriptions);
            newDescription.SetSupportedRequestBodyFormatters(originalDescription.SupportedRequestBodyFormatters);
            newDescription.SetSupportedResponseFormatters(originalDescription.SupportedResponseFormatters);
            newDescription.SetDeprecatedVersion(originalDescription.DeprecatedVersion());
            newDescription.SetVersion(newVersion);
            //ID isn't setable as it's calculated.  This means we could have some version conflicts if there's something that assumes the ID is unique...
            return newDescription;
        }

        private string GetApiDocumentation(HttpActionDescriptor actionDescriptor)
        {
            IDocumentationProvider documentationProvider = DocumentationProvider ?? actionDescriptor.Configuration.Services.GetDocumentationProvider();
            if (documentationProvider == null)
            {
                return string.Format("Documentation for {0}", actionDescriptor.ActionName);
            }

            return documentationProvider.GetDocumentation(actionDescriptor);
        }

        private HttpMethod RoutingAttributeMethod(HttpRouteAttribute routeAttribute)
        {
            if (routeAttribute is GETAttribute)
                return HttpMethod.Get;
            if (routeAttribute is POSTAttribute)
                return HttpMethod.Post;
            if (routeAttribute is PUTAttribute)
                return HttpMethod.Put;
            if (routeAttribute is DELETEAttribute)
                return HttpMethod.Delete;
            if (routeAttribute is HEADAttribute)
                return HttpMethod.Head;
            if (routeAttribute is OPTIONSAttribute)
                return HttpMethod.Options;

            throw new ApplicationException("Can't get HttpAttribute from attribute:" + routeAttribute.GetType().Name);
        }

        private bool MethodIsHttpRoute(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes(true).Any(attrib => attrib is HttpRouteAttribute);
        }
    }
}

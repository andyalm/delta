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

namespace RouteAttribExplorer
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
                var methods =
                    controller.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(MethodIsHttpRoute);
                foreach (var method in methods)
                {
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

                        collection.Add(description);
                    }
                }
            }


            return collection;
        }

        private string GetApiDocumentation(HttpActionDescriptor actionDescriptor)
        {
            IDocumentationProvider documentationProvider = DocumentationProvider ?? actionDescriptor.Configuration.Services.GetDocumentationProvider();
            if (documentationProvider == null)
            {
                return string.Format("Documentation for {0}" , actionDescriptor.ActionName);
            }

            return documentationProvider.GetDocumentation(actionDescriptor);
        }

        private HttpMethod RoutingAttributeMethod(object routeAttribute)
        {
            if(routeAttribute is GETAttribute)
                return HttpMethod.Get;
            if (routeAttribute is POSTAttribute)
                return HttpMethod.Post;
            if (routeAttribute is PUTAttribute)
                return HttpMethod.Put;
            if (routeAttribute is DELETEAttribute)
                return HttpMethod.Delete;

            //TODO: we can also have other attributes (e.g. HEAD, options) which we can extract from the base class.
            
            throw new ApplicationException("Can't get HttpAttribute from attribute:" + routeAttribute.GetType().Name);
        }

        private bool MethodIsHttpRoute(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttributes(true).Any(attrib => attrib is HttpRouteAttribute);
        }
    }
}

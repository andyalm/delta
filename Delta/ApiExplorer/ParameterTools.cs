using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace Delta.ApiExplorer
{
    class ParameterTools
    {
        private readonly IDocumentationProvider _documentationProvider;

        public ParameterTools(IDocumentationProvider documentationProvider)
        {
            _documentationProvider = documentationProvider;
        }

        public IList<ApiParameterDescription> CreateParameterDescriptions(HttpActionDescriptor actionDescriptor)
        {
            IList<ApiParameterDescription> parameterDescriptions = new List<ApiParameterDescription>();
            HttpActionBinding actionBinding = GetActionBinding(actionDescriptor);

            // try get parameter binding information if available
            if (actionBinding != null)
            {
                HttpParameterBinding[] parameterBindings = actionBinding.ParameterBindings;
                if (parameterBindings != null)
                {
                    foreach (HttpParameterBinding parameter in parameterBindings)
                    {
                        parameterDescriptions.Add(CreateParameterDescriptionFromBinding(parameter));
                    }
                }
            }
            else
            {
                Collection<HttpParameterDescriptor> parameters = actionDescriptor.GetParameters();
                if (parameters != null)
                {
                    foreach (HttpParameterDescriptor parameter in parameters)
                    {
                        parameterDescriptions.Add(CreateParameterDescriptionFromDescriptor(parameter));
                    }
                }
            }

            return parameterDescriptions;
        }

        private ApiParameterDescription CreateParameterDescriptionFromDescriptor(HttpParameterDescriptor parameter)
        {
            var parameterDescription = new ApiParameterDescription
            {
                ParameterDescriptor = parameter,
                Name = parameter.Prefix ?? parameter.ParameterName,
                Documentation = GetApiParameterDocumentation(parameter),
                Source = ApiParameterSource.Unknown
            };
            return parameterDescription;
        }

        private ApiParameterDescription CreateParameterDescriptionFromBinding(HttpParameterBinding parameterBinding)
        {
            ApiParameterDescription parameterDescription = CreateParameterDescriptionFromDescriptor(parameterBinding.Descriptor);
            if (parameterBinding.WillReadBody)
            {
                parameterDescription.Source = ApiParameterSource.FromBody;
            }
            else if (parameterBinding.WillReadUri())
            {
                parameterDescription.Source = ApiParameterSource.FromUri;
            }

            return parameterDescription;
        }

        private static HttpActionBinding GetActionBinding(HttpActionDescriptor actionDescriptor)
        {
            HttpControllerDescriptor controllerDescriptor = actionDescriptor.ControllerDescriptor;
            if (controllerDescriptor == null)
            {
                return null;
            }

            ServicesContainer controllerServices = controllerDescriptor.Configuration.Services;
            IActionValueBinder actionValueBinder = controllerServices.GetActionValueBinder();
            HttpActionBinding actionBinding = actionValueBinder != null ? actionValueBinder.GetBinding(actionDescriptor) : null;
            return actionBinding;
        }

        private string GetApiParameterDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            IDocumentationProvider documentationProvider = _documentationProvider ?? parameterDescriptor.Configuration.Services.GetDocumentationProvider();
            if (documentationProvider == null)
            {
                return string.Format("Documentation for '{0}'.", parameterDescriptor.Prefix ?? parameterDescriptor.ParameterName);
            }

            return documentationProvider.GetDocumentation(parameterDescriptor);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Delta.ApiExplorer
{
    internal static class HttpParameterBindingExtensions
    {
        public static bool WillReadUri(this HttpParameterBinding parameterBinding)
        {
            if (parameterBinding == null)
            {
                throw new ArgumentNullException("parameterBinding");
            }

            IValueProviderParameterBinding valueProviderParameterBinding = parameterBinding as IValueProviderParameterBinding;
            if (valueProviderParameterBinding != null)
            {
                IEnumerable<ValueProviderFactory> valueProviderFactories = valueProviderParameterBinding.ValueProviderFactories;
                if (valueProviderFactories.Any() && valueProviderFactories.All(factory => ImplementsIUriValueProviderFactory(factory)))
                {
                    return true;
                }
            }

            return false;
        }

        //Need this because IUriValueProviderFactory is internal to System.Web.Http.
        private static bool ImplementsIUriValueProviderFactory(ValueProviderFactory factory)
        {
            return (factory.GetType().GetInterface("IUriValueProviderFactory") != null);
        }
    }
}

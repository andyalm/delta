using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DeltaExplorer;

namespace Harness2.Areas
{
    public static class ApiExplorerFactory
    {
        private static readonly Lazy<ApiExplorer> _apiExplorer = new Lazy<ApiExplorer>(InitExplorer);

        private static ApiExplorer InitExplorer()
        {
            return new ApiExplorer(GlobalConfiguration.Configuration);
        }

        public static IApiExplorer Current
        {
            get
            {
                return _apiExplorer.Value;
            }
        }

    }
}
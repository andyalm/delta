using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Delta
{
    public static class DocConfiguration
    {
        private static HttpConfiguration _configuration;
        public static HttpConfiguration Configuration
        {
            get { return _configuration ?? (_configuration = new HttpConfiguration()); }
            set { _configuration = value; }
        }
    }
}

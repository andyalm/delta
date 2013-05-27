using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Delta.Tests.Controllers.V1
{
    public class CartController : ApiController
    {
        public int Post()
        {
            return 99;
        }

        public string Get()
        {
            return "Hello world!  (v1)";
        }
    }
}

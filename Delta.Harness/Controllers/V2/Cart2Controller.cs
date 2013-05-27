using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Delta.Tests.Controllers.V2
{
    //For short term we're naming this incorrectly until we get Delta wired in.
    public class Cart2Controller : ApiController
    {
        public int Post()
        {
            return 99;
        }

        public string Get()
        {
            return "Hello world! (v2)";
        }

        public string Put()
        {
            return "thingy";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Delta.Tests.Controllers.V2
{
    /// <summary>
    /// This is the 2nd cart thingy.
    /// </summary>
    public class CartController : ApiController
    {

        /// <summary>
        /// Post (v2)
        /// </summary>
        /// <returns>Arbitrary integer</returns>
        public int Post()
        {
            return 99;
        }

        /// <summary>
        /// Get (v2)
        /// </summary>
        /// <returns></returns>
        public string Get()
        {
            return "Hello world! (v2)";
        }

        /// <summary>
        /// Put (v2)
        /// </summary>
        /// <returns></returns>
        
        public string Put()
        {
            return "thingy";
        }
    }
}

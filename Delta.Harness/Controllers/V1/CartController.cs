﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using AttributeRouting.Web.Http;

namespace Delta.Tests.Controllers.V1
{
    /// <summary>
    /// First cart controller
    /// </summary>
    [DeprecatedInVersion(1)]
    public class CartController : ApiController
    {

        /// <summary>
        /// Post (v1)
        /// </summary>
        /// <returns></returns>
        [POST("Cart")]
        public int Post()
        {
            return 99;
        }

        /// <summary>
        /// Get (v1)
        /// </summary>
        /// <returns></returns>
        [GET("api/cart")]
        public string Get()
        {
            return "Hello world!  (v1)";
        }
    }
}

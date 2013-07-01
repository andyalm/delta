using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;
namespace RouteAttribHarness.Controllers.V1
{
    public class CartController : ApiController
    {
        /// <summary>
        /// Get a cart
        /// </summary>
        /// <param name="id">The ID of the cart</param>
        /// <returns>A fully formed cart, I suppose...</returns>
        [GET("/cart/{id}")]
        public Cart Get(int id)
        {
            return null;
        }

    }

    public class Cart
    {
        public string Name { get; set; }
        public int ItemCount { get; set; }
    }

}
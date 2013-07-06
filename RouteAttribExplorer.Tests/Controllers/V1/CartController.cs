using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using AttributeRouting.Web.Http;

namespace RouteAttribExplorer.Tests.Controllers.V1
{
    public class CartController : ApiController
    {
        /// <summary>
        /// Get a cart
        /// </summary>
        /// <param name="id">The ID of the cart</param>
        /// <returns>A fully formed cart, I suppose...</returns>
        [GET("/cart/{id}")]
        public Cart GetCart(int id)
        {
            return null;
        }

        [DeprecatedInVersion(2)]
        [POST("/cart/default")]
        public void SetDefaultCart()
        {
        }

    }

    public class Cart
    {
        public string Name { get; set; }
        public int ItemCount { get; set; }
    }
}

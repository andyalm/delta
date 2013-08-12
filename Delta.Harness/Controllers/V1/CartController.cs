using System.Web.Http;
using AttributeRouting.Web.Http;

namespace Harness2.Controllers.V1
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

    }

    public class Cart
    {
        public string Name { get; set; }
        public int ItemCount { get; set; }
    }
}

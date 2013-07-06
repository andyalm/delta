using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using AttributeRouting.Web.Http;

namespace RouteAttribExplorer.Tests.Controllers.V1
{
    /// <summary>
    /// Does stuff with Orders
    /// </summary>
    [DeprecatedInVersion(2)]
    public class OrderController : ApiController
    {
        [GET("orders/{id}")]
        public Order GetOrder(int id)
        {
            return new Order();
        }
    }

    public class Order
    {
    }
}

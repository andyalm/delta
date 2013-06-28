using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;

namespace Delta.Tests.Controllers.V1
{
    [RoutePrefix("api")]
    [RoutePrefix("api2")]
    public class LightboxController : ApiController
    {
        [GET("lightbox/{id}")]
        public string Get(int id)
        {
            return "I guess we should return a lightbox.";
        }
    }
}
using System.Web.Http;

namespace Delta.Tests.Controllers.V1
{
    public class LightboxController : ApiController
    {
        public string Get(int id)
        {
            return "I guess we should return a lightbox.";
        }
    }
}
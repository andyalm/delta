using System.Web.Http;
using AttributeRouting.Web.Http;

namespace RouteAttribExplorer.Tests.Controllers.V1
{
    public class LightboxController : ApiController
    {
        [GET("lightboxes/{id}")]
        public Lightbox Get(int id)
        {
            return new Lightbox();
        }
    }

    public class Lightbox
    {
        string Id { get; set; }
        public string Name { get; set; }
    }

}
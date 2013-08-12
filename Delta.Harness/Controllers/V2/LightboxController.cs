using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;

namespace Harness2.Controllers.V2
{
    [RoutePrefix("api")]
    [RoutePrefix("api2")]
    public class LightboxController : ApiController
    {
        /// <summary>
        /// Get lightboxes in the new way
        /// </summary>
        /// <param name="id">Lightbox Id</param>
        [GET("lightbox/{id}")]
        public string NewGet(int id)
        {
            return "I guess we should return a lightbox.";
        }
    }

    public class Thingy
    {
        public string Whatsit;
        public int Whoa;
    }
}
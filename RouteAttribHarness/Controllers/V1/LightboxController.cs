using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;

namespace RouteAttribHarness.Controllers.V1
{
    [RoutePrefix("api")]
    [RoutePrefix("api2")]
    public class LightboxController : ApiController
    {
        /// <summary>
        /// Get lightboxes in the old way.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [GET("lightbox/{id}")]
        public string OldGet(int id)
        {
            return "I guess we should return a lightbox.";
        }

        /// <summary>
        /// Adding stuff to a thingy.
        /// </summary>
        /// <param name="thingy">Thingy is the parameter!  W00t!</param>
        /// <returns></returns>
        [POST("lightbox")]
        public int AddStuff(Thingy thingy)
        {
            return 99;
        }
    }

    public class Thingy
    {
        public string Whatsit;
        public int Whoa;
    }
}
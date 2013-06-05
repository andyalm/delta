using System;
//using System.Web.Http;
using System.Web.Mvc;
using Delta;
using DeltaExplorer;
using Harness2.Areas.HelpPage.Models;
using Delta.Explorer;
namespace Harness2.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        public HelpController()
            : this(System.Web.Http.GlobalConfiguration.Configuration)
        {
        }

        public HelpController(System.Web.Http.HttpConfiguration config)
        {
            Configuration = config;
        }

        public System.Web.Http.HttpConfiguration Configuration { get; private set; }

        public ActionResult Index()
        {
            var explorer = new VersionedApiExplorer(DocConfiguration.Configuration);
            return View(explorer.ApiDescriptions);
        }

        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View("Error"); //Doesn't exist right now, apparently.
        }
    }
}
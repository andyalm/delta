using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Delta.Tests
{
    [TestFixture]
    public class ControllerSelectionTests
    {
        [Test]
        public void OneControllerForRequestedVersion()
        {
            var controllerDescriptor = GetControllerDescriptorFor("lightbox", 1);
            controllerDescriptor.ControllerType.Should().Be(typeof (Controllers.V1.LightboxController));
        }

        [Test]
        public void TwoControllersForRequestedVersion()
        {
            var controllerDescriptor = GetControllerDescriptorFor("cart", 2);
            controllerDescriptor.ControllerType.Should().Be(typeof(Controllers.V2.CartController));
        }

        [Test]
        public void RequestForNotLatestVersion()
        {
            var controllerDescriptor = GetControllerDescriptorFor("cart", 1);
            controllerDescriptor.ControllerType.Should().Be(typeof(Controllers.V1.CartController));
        }

        [Test]
        public void RequestForControllerInheritedFromPreviousVersion()
        {
            var controllerDescriptor = GetControllerDescriptorFor("lightbox", 2);
            controllerDescriptor.ControllerType.Should().Be(typeof (Controllers.V1.LightboxController));
        }

        [Test]
        public void ControllerIsNotInheritedWhenItIsDepricated()
        {
            Action getControllerDescriptor = () => GetControllerDescriptorFor("badidea", 2);
            getControllerDescriptor.ShouldThrow<HttpResponseException>().Where(ex => ex.Response.StatusCode == HttpStatusCode.NotFound);
        }

        [Test]
        public void DepricatedControllerIsStillValidInPreviousVersions()
        {
            var controllerDescriptor = GetControllerDescriptorFor("badidea", 1);
            controllerDescriptor.ControllerType.Should().Be(typeof (Controllers.V1.BadIdeaController));
        }


        [Test]
        public void ControllerNotFound()
        {
            Action getControllerDescriptor = () => GetControllerDescriptorFor("nosuchcontroller", 1);
            getControllerDescriptor.ShouldThrow<HttpResponseException>().Where(ex => ex.Response.StatusCode == HttpStatusCode.NotFound);
        }
        
        private static HttpControllerDescriptor GetControllerDescriptorFor(string controllerNameInRoute, int versionInRoute)
        {
            var request = new HttpRequestMessage();
            request.SetRouteData(new{ controller = controllerNameInRoute, version = versionInRoute.ToString() });
            var config = new HttpConfiguration();
            var controllerSelector = new DeltaVersionedControllerSelector(config, new RouteRequestVersionSelector(), 
                                                                          new NamespaceControllerVersionSelector());
            var controllerDescriptor = controllerSelector.SelectController(request);
            return controllerDescriptor;
        }



    }

    internal static class HttpRequestMessageExtensions
    {
        public static void SetRouteData(this HttpRequestMessage request, object values)
        {
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = new HttpRouteData(new HttpRoute(), new HttpRouteValueDictionary(values));
        }
    }
}
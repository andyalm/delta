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
        public void ControllerNotFound()
        {
            Action getControllerDescriptor = () => GetControllerDescriptorFor("nosuchcontroller", 1);
            getControllerDescriptor.ShouldThrow<HttpResponseException>().Where(ex => ex.Response.StatusCode == HttpStatusCode.NotFound);
        }
        
        private static HttpControllerDescriptor GetControllerDescriptorFor(string controllerNameInRoute, int versionInRoute)
        {
            var versionSelector = new Mock<IVersionSelector>();
            versionSelector.Setup(v => v.GetVersion(It.IsAny<HttpRequestMessage>())).Returns(versionInRoute);
            var request = new HttpRequestMessage();
            request.SetController(controllerNameInRoute);
            var config = new HttpConfiguration();
            var controllerSelector = new DeltaVersionedControllerSelector(config, versionSelector.Object,
                                                                          new NamespaceControllerVersionSelector());
            var controllerDescriptor = controllerSelector.SelectController(request);
            return controllerDescriptor;
        }



    }

    internal static class HttpRequestMessageExtensions
    {
        public static void SetController(this HttpRequestMessage request, string controllerName)
        {
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = new HttpRouteData(new HttpRoute(), new HttpRouteValueDictionary(new { controller = controllerName }));
        }
    }
}
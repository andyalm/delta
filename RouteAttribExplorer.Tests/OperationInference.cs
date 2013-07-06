using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace RouteAttribExplorer.Tests
{
    [TestFixture]
    public class OperationInference
    {
        private Collection<ApiDescription> _apiDescriptions;
        
        [SetUp]
        public void Setup()
        {
            var configuration = new HttpConfiguration();

            var explorer = new RouteAttribExplorer.Explorer(this.GetType().Assembly, configuration);
            _apiDescriptions = explorer.ApiDescriptions;
        }

        [Test]
        public void MultipleVersionsObserved()
        {
            _apiDescriptions.Any(d => d.Version() == 1).Should().BeTrue("there should be at least one v1 operation");
            _apiDescriptions.Any(d => d.Version() == 2).Should().BeTrue("there should be at least one v2 operation");
        }

        [Test]
        public void V1OperationInferredInV2()
        {
            AssertExists(2, "LightboxController", "AddStuff");
        }

        [Test]
        public void DeprecatedControllerNotInV2()
        {
            AssertNotExists(2, "OrderController", "GetOrder");
        }

        [Test]
        public void DeprecatedOperationNotInV2()
        {
            AssertNotExists(2, "CartController", "SetDefaultCart");
        }

        [Test]
        public void NewOperationReplacesOldWhenDuplicating()
        {
            AssertExists(2, "LightboxController", "NewGet");
            AssertNotExists(2, "LightboxController", "OldGet");
        }

        private void AssertExists(int version, string controllername, string actionName)
        {
            _apiDescriptions.Any(
                d => d.Version() == version && d.ActionDescriptor.ControllerDescriptor.ControllerName == controllername && d.ActionDescriptor.ActionName == actionName)
                            .Should()
                            .BeTrue();
        }

        private void AssertNotExists(int version, string controllerName, string actionName)
        {
            _apiDescriptions.Any(
                d => d.Version() == version && d.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName && d.ActionDescriptor.ActionName == actionName)
                            .Should()
                            .BeFalse();
        }


        //[Test]
        //public void SimpleGet()
        //{
        //    var explorer = new RouteAttribExplorer.Explorer(this.GetType().Assembly, GlobalConfiguration.Configuration);

        //    var descriptions = explorer.ApiDescriptions;

        //    descriptions.Should().HaveCount(1);
        //}
    }
}

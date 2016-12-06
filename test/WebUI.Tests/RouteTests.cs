using System;
using System.Web;
using System.Web.Routing;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WebUI.CompositionRoot;

namespace WebUI.Tests
{
    [TestFixture]
    public class RouteTests
    {
        private void TestRouteMatch(string url, object expectations)
        {
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            var httpContextStub = Substitute.For<HttpContextBase>();
            httpContextStub.Request.AppRelativeCurrentExecutionFilePath
                .Returns(url);

            RouteData routeData = routes.GetRouteData(httpContextStub);

            routeData.Should().NotBeNull();
            foreach (var kvp in new RouteValueDictionary(expectations)) {
                Assert.True(string.Equals(
                    kvp.Value.ToString(),
                    routeData.Values[kvp.Key].ToString(),
                    StringComparison.OrdinalIgnoreCase),
                    $"Expected '{kvp.Value}', not '{routeData.Values[kvp.Key]}' for '{kvp.Key}'.");
            }
        }

        [Test]
        public void TestIndexRoute()
        {
            TestRouteMatch("~/", new { controller = "Browser", action = "Index" });
        }

        [Test]
        public void TestPageRoute()
        {
            TestRouteMatch("~/Page", new { controller = "Browser", action = "Page" });
        }
    }
}

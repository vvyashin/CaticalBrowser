using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using WebUI.Controllers;
using WebUI.Services;
using WebUI.ViewModel;

namespace WebUI.Tests
{
    [TestFixture]
    public class BrowserControllerTests
    {
        public ISubstituteProxyService FakeSubstituteProxyService { get; set; }
        
        [SetUp]
        public void SetupService()
        {
            FakeSubstituteProxyService = Substitute.For<ISubstituteProxyService>();
        }

        private BrowserController CreateBrowserController()
        {
            var controller = new BrowserController(FakeSubstituteProxyService);
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            request.Url.Returns(new Uri("http://website.com"));
            httpContext.Request.Returns(request);

            controller.ControllerContext = new ControllerContext(httpContext, new RouteData(),
                controller);
            
            return controller;
        }

        [Test]
        public void TestIndex()
        {
            var browserController = CreateBrowserController();

            var actionResult = browserController.Index();

            actionResult.ViewName.Should().Be("Index");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\n")]
        public void Page_IfUrlIsNullOrWhiteSpace_ReturnsPageViewWithEmptyContent(string url)
        {
            var browserController = CreateBrowserController();

            var viewResult = (ViewResult)browserController.Page(url).Result;

            viewResult.ShouldBeEquivalentTo(new { ViewName = "Page", Model = new PageViewModel() { Content = "" } }, 
                opt => opt.ExcludingMissingMembers());
        }

        [Test]
        public void Page_IfUrlIsNotNullOrWhiteSpace_ReturnsPageFromSubstituteService()
        {
            var browserController = CreateBrowserController();
            var url = "http://website.com";
            var page = "Html page";
            FakeSubstituteProxyService.GetSubstitutePage(Arg.Is(url), Arg.Any<IHeaders>(),
                Arg.Any<string>()).Returns(Task.FromResult(page));
            
            var viewResult = (ViewResult)browserController.Page(url).Result;

            viewResult.ShouldBeEquivalentTo(new {ViewName = "Page", Model = new PageViewModel() {Content = page}},
                opt => opt.ExcludingMissingMembers());
        }

        [TestCase(typeof(UriFormatException))]
        [TestCase(typeof(NotSupportedException))]
        [TestCase(typeof(HttpRequestException))]
        public void Page_IfThrow_DisplayError(Type exceptionType)
        {
            var browserController = CreateBrowserController();
            var url = "http://website.com";
            FakeSubstituteProxyService.GetSubstitutePage(Arg.Is(url), Arg.Any<IHeaders>(),
                 Arg.Any<string>())
                 .Throws((Exception)Activator.CreateInstance(exceptionType, "message"));

            var viewResult = (ViewResult)browserController.Page(url).Result;

            viewResult.ShouldBeEquivalentTo(new { ViewName = "Page", Model = new PageViewModel() { Content = "message" } },
                opt => opt.ExcludingMissingMembers());
        }

        [Test]
        public void Page_IfExceptionThrow_DisplayUnknownError()
        {
            var browserController = CreateBrowserController();
            var url = "http://website.com";

            FakeSubstituteProxyService.GetSubstitutePage(Arg.Is(url), Arg.Any<IHeaders>(),
                 Arg.Any<string>()).Throws(new Exception());

            var viewResult = (ViewResult)browserController.Page(url).Result;

            viewResult.ShouldBeEquivalentTo(new { ViewName = "Page", Model = new PageViewModel()
            {
                Content = "Something went wrong while displaying this webpage"
            }}, opt => opt.ExcludingMissingMembers());
        }
    }
}

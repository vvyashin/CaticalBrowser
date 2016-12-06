using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WebUI.Services;

namespace SubstituteProxy.Tests
{
    [TestFixture]
    public class SubstituteProxyServiceTests
    {
        public WebPageReader FakeWebPageReader { get; set; }
        public CatPicturesGenerator CatPicturesGeneratorFake { get; set; }

        [SetUp]
        public void SetupFakes()
        {
            FakeWebPageReader = Substitute.For<WebPageReader>();
            CatPicturesGeneratorFake = Substitute.For<CatPicturesGenerator>();
        }

        private SubstituteProxyService CreateSubstituteProxyService()
        {
            return new SubstituteProxyService(FakeWebPageReader, CatPicturesGeneratorFake);
        }

        [Test]
        public void GetSubstitutePage_ReplaceLinksWithProxyLink()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            FakeWebPageReader.LoadPage(Arg.Is(url), Arg.Any<IHeaders>()).Returns(
                @"<html><head></head><body><div><a href=""http://www.website1.com/""></a><p><a href = ""http://www.website2.com/""></a></p></div></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), new Uri("http://w1.ru"), "/proxyPage?=").Result;

            html.Should().Be(
                @"<html><head><base href=""http://www.website1.com""></head><body><div><a href=""/proxyPage?=http://www.website1.com/""></a><p><a href=""/proxyPage?=http://www.website2.com/""></a></p></div></body></html>");
        }

        [Test]
        public void GetSubstitutePage_ReplaceAllImagesWithCatPictures()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            CatPicturesGeneratorFake.GetNextCatPicture(Arg.Is(new Uri("http://w1.ru"))).Returns(new Uri("http://w1.ru/1.jpg"));
            FakeWebPageReader.LoadPage(Arg.Is(url), Arg.Any<IHeaders>()).Returns(
                @"<html><head></head><body><div><img src=""/sky.jpg""><p><img src = ""/green.png""></p></div></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), new Uri("http://w1.ru"), "proxyUrl").Result;

            html.Should().Be(
                @"<html><head><base href=""http://www.website1.com""></head><body><div><img src=""http://w1.ru/1.jpg""><p><img src=""http://w1.ru/1.jpg""></p></div></body></html>");
        }

        [Test]
        public void GetSubstitutePage_ReplaceRelativeCssUriWithAbsoluteUri()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            FakeWebPageReader.LoadPage(Arg.Is(url), Arg.Any<IHeaders>()).Returns(
                @"<html><head><link href=""style.css"" /></head><body></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), new Uri("http://w1.ru"), "/proxyPage?=").Result;

            html.Should().Be(
                @"<html><head><link href=""http://www.website1.com/style.css""><base href=""http://www.website1.com""></head><body></body></html>");
        }
    }
}

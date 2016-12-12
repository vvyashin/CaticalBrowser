using System;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using WebUI.Services;

namespace SubstituteProxy.Tests
{
    [TestFixture]
    public class SubstituteProxyServiceTests
    {
        public WebPageReader WebPageReaderFake { get; set; }
        public CatPicturesGenerator CatPicturesGeneratorFake { get; set; }
        public UriBuilder UriBuilderFake { get; set; }

        [SetUp]
        public void SetupFakes()
        {
            WebPageReaderFake = Substitute.For<WebPageReader>();
            CatPicturesGeneratorFake = Substitute.For<CatPicturesGenerator>();
            UriBuilderFake = Substitute.For<UriBuilder>();
        }

        private SubstituteProxyService CreateSubstituteProxyService()
        {
            return new SubstituteProxyService(WebPageReaderFake, CatPicturesGeneratorFake, UriBuilderFake);
        }

        private Uri SetUrl(string url)
        {
            var uri = new Uri(url);
            UriBuilderFake.GetUri(Arg.Is(url)).Returns(uri);

            return uri;
        }

        private string SetNextCatPicture()
        {
            var uri = "http://w1.ru/1.jpg";
            CatPicturesGeneratorFake.GetNextCatPicture(Arg.Any<string>()).Returns(uri);

            return uri;
        }

        private void SetReturnPage(Uri uri, string page)
        {
            WebPageReaderFake.LoadPage(Arg.Is(uri), Arg.Any<IHeaders>()).Returns(
                Task.FromResult((IDocument)new HtmlParser(new Configuration().WithCss()).Parse(page)));
        }

        [Test]
        public void GetSubstitutePage_ReplaceLinksWithProxyLink()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            var uri = SetUrl(url);
            SetReturnPage(uri,
                @"<html><head></head><body><div><a href=""http://www.website1.com/""></a><p><a href = ""http://www.website2.com/""></a></p></div></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), "/proxyPage?=").Result;

            html.Should().Be(
                @"<html><head><base href=""http://www.website1.com/""></head><body><div><a href=""/proxyPage?=http://www.website1.com/""></a><p><a href=""/proxyPage?=http://www.website2.com/""></a></p></div></body></html>");
        }

        [Test]
        public void GetSubstitutePage_ReplaceAllImagesWithCatPictures()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            var uri = SetUrl(url);
            var imagePictureUri = SetNextCatPicture();
            SetReturnPage(uri,
                @"<html><head></head><body><div><img src=""/sky.jpg""><p><img src = ""/green.png""></p></div></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), "proxyUrl").Result;

            html.Should().Be(
                $@"<html><head><base href=""http://www.website1.com/""></head><body><div><img src=""{imagePictureUri}""><p><img src=""{imagePictureUri}""></p></div></body></html>");
        }

        [Test]
        public void GetSubstitutePage_ReplaceRelativeCssUriWithAbsoluteUri()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            var uri = SetUrl(url);
            SetReturnPage(uri,
                @"<html><head><link href=""style.css"" /></head><body></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), "/proxyPage?=").Result;

            html.Should().Be(
                @"<html><head><link href=""http://www.website1.com/style.css""><base href=""http://www.website1.com/""></head><body></body></html>");
        }

        [Test]
        public void GetSubstringPage_ReplaceBackgroundImagesWithCatPictures()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            var uri = SetUrl(url);
            var imagePictureUri = SetNextCatPicture();
            SetReturnPage(uri,
                @"<html><head></head><body><div style=""background-image: url(/image.jpg);""></div></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), "proxyUrl").Result;

            html.Should().Be(
                $@"<html><head><base href=""http://www.website1.com/""></head><body><div style=""background-image: url(&quot;{imagePictureUri}&quot;)""></div></body></html>");
        }

        [Test]
        public void GetSubstringPage_ReplaceBackgroundImageFromCssWithCatPictures()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            var uri = SetUrl(url);
            var imagePictureUri = SetNextCatPicture();
            SetReturnPage(uri,
                @"<html><head><style>.bi{background-image: url(/image.jpg);}</style></head><body><div class=""bi""></div></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), "proxyUrl").Result;

            html.Should().Be(
                $@"<html><head><style>.bi{{background-image: url(/image.jpg);}}</style><base href=""http://www.website1.com/""></head><body><div class=""bi"" style=""background-image: url(&quot;{imagePictureUri}&quot;)""></div></body></html>");
        }

        [Test]
        public void GetSubstitutePage_IfSrcSetExists_SrcSetIsEmpty()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            var uri = SetUrl(url);
            var imagePictureUri = SetNextCatPicture();
            SetReturnPage(uri,
                @"<html><head></head><body><div><img srcset=""/sky.jpg""></div></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), "proxyUrl").Result;

            html.Should().Be(
                $@"<html><head><base href=""http://www.website1.com/""></head><body><div><img src=""{imagePictureUri}""></div></body></html>");
        }

        [Test]
        public void GetSubstitutePage_ReplacePicture()
        {
            var substituteProxyService = CreateSubstituteProxyService();
            var url = "http://www.website1.com";
            var uri = SetUrl(url);
            var imagePictureUri = SetNextCatPicture();
            SetReturnPage(uri,
                @"<html><head></head><body><div><picture><source srcset=""/sky.jpg""></picture></div></body></html>");

            var html = substituteProxyService.GetSubstitutePage(url, Substitute.For<IHeaders>(), "proxyUrl").Result;

            html.Should().Be(
                $@"<html><head><base href=""http://www.website1.com/""></head><body><div><picture><source src=""{imagePictureUri}""></picture></div></body></html>");
        }
    }
}

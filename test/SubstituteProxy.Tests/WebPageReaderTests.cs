using System;
using NSubstitute;
using NUnit.Framework;
using WebUI.Services;

namespace SubstituteProxy.Tests
{
    [TestFixture]
    public class WebPageReaderTests
    {
        private WebPageReader CreateWebPageReader()
        {
            return new WebPageReader();
        }

        [Test]
        public void LoadPage_IfUrlIsNull_Throw()
        {
            var webPageReader = CreateWebPageReader();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await webPageReader.LoadPage(null, Substitute.For<IHeaders>()));
        }

        [TestCase("ftp://ftp.ru", "ftp")]
        [TestCase("file://file.ru", "file")]
        [TestCase("NNTP://News.contoso.com", "nntp")]
        public void LoadPage_IfUrlIsHttpOrHttps_Throw(string url, string scheme)
        {
            var webPageReader = CreateWebPageReader();

            Assert.ThrowsAsync(Is.TypeOf<NotSupportedException>()
                .And.Message.EqualTo($"Sorry! Protocol '{scheme}' is not supported. Only http and https are supported"),
                async () => await webPageReader.LoadPage(url, Substitute.For<IHeaders>()));
        }

        [Test]
        public void LoadPage_IfUrlIsNotCorrect_Throw()
        {
            var webPageReader = CreateWebPageReader();

            Assert.ThrowsAsync(Is.TypeOf<UriFormatException>()
                .And.Message.EqualTo("Url format is not correct"),
                async () => await webPageReader.LoadPage("ur..d", Substitute.For<IHeaders>()));
        }
    }
}

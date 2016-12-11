using System;
using FluentAssertions;
using NUnit.Framework;

namespace SubstituteProxy.Tests
{
    [TestFixture]
    public class UriBuilderTests
    {
        public UriBuilder CreateUriBuilder()
        {
            return new UriBuilder();
        }

        [Test]
        public void GetUri_IfSchemeIsEmptyReturnHttp()
        {
            var url = "susu.ru";

            var uri = CreateUriBuilder().GetUri(url);

            uri.Should().Be(new Uri("http://susu.ru"));
        }

        [TestCase("ftp://ftp.ru", "ftp")]
        [TestCase("file://file.ru", "file")]
        [TestCase("NNTP://News.contoso.com", "nntp")]
        public void GetUri_IfUrlIsNotHttpOrHttps_Throw(string url, string scheme)
        {
            var uriBuilder = CreateUriBuilder();

            Assert.Throws(Is.TypeOf<NotSupportedException>()
                .And.Message.EqualTo($"Sorry! Protocol '{scheme}' is not supported. Only http and https are supported"),
                () => uriBuilder.GetUri(url));
        }

        [Test]
        public void LoadPage_IfUrlIsNotCorrect_Throw()
        {
            var uriBuilder = CreateUriBuilder();

            Assert.Throws(Is.TypeOf<UriFormatException>()
                .And.Message.EqualTo("Url format is not correct"),
                () => uriBuilder.GetUri("url..d"));
        }
    }
}

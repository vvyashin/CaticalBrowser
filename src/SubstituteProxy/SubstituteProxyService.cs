using System;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using WebUI.Services;

namespace SubstituteProxy
{
    public class SubstituteProxyService : ISubstituteProxyService
    {
        private readonly WebPageReader _webPageReader;
        private readonly CatPicturesGenerator _catPicturesGenerator;
        private readonly UriBuilder _uriBuilder;

        public SubstituteProxyService(WebPageReader webPageReader, CatPicturesGenerator catPicturesGenerator,
            UriBuilder uriBuilder)
        {
            if (webPageReader == null) throw new ArgumentNullException(nameof(webPageReader));
            if (catPicturesGenerator == null) throw new ArgumentNullException(nameof(catPicturesGenerator));
            if (uriBuilder == null) throw new ArgumentNullException(nameof(uriBuilder));

            _webPageReader = webPageReader;
            _catPicturesGenerator = catPicturesGenerator;
            _uriBuilder = uriBuilder;
        }

        /// <summary>
        /// Gets page where all images are substituted with cat pictures
        /// </summary>
        /// <param name="url">Url of loading web site</param>
        /// <param name="headers">Headers of http request</param>
        /// <param name="imageFolderUrl">Absolute url to folder with cat pictures</param>
        /// <param name="proxyPrefix">Proxy prefix for links</param>
        /// <returns>HTML page</returns>
        public async Task<string> GetSubstitutePage(string url, IHeaders headers, Uri imageFolderUrl, string proxyPrefix)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (imageFolderUrl == null) throw new ArgumentNullException(nameof(imageFolderUrl));
            if (proxyPrefix == null) throw new ArgumentNullException(nameof(proxyPrefix));

            var uri = _uriBuilder.GetUri(url);
            var page = await _webPageReader.LoadPage(uri, headers);

            var parser = new HtmlParser();
            var document = parser.Parse(page);
            
            SetBaseUri(url, document);

            ReplaceLinksWithProxy(proxyPrefix, document, uri);
            ReplaceImagesWithCats(imageFolderUrl, document);
            ReplaceRelativeHeadLinksWithAbsolute(document, uri);

            return document.ToHtml();
        }

        private void ReplaceImagesWithCats(Uri imageFolderUrl, IHtmlDocument document)
        {
            foreach (var element in document.QuerySelectorAll("img")) {
                element.SetAttribute("src", $"{_catPicturesGenerator.GetNextCatPicture(imageFolderUrl)}");
            }
        }

        private void ReplaceLinksWithProxy(string proxyUrl, IHtmlDocument document, Uri baseUri)
        {
            foreach (var element in document.QuerySelectorAll("a")) {
                var href = element.GetAttribute("href");
                var absoluteUri = new Uri(baseUri, href);
                element.SetAttribute("href", $"{proxyUrl}{absoluteUri}");
            }
        }

        private void ReplaceRelativeHeadLinksWithAbsolute(IHtmlDocument document, Uri baseUri)
        {
            foreach (var element in document.QuerySelectorAll("link")) {
                var href = element.GetAttribute("href");
                var absoluteUri = new Uri(baseUri, href);
                element.SetAttribute("href", absoluteUri.ToString());
            }

            foreach (var element in document.QuerySelectorAll("script")) {
                var href = element.GetAttribute("src");
                var absoluteUri = new Uri(baseUri, href);
                element.SetAttribute("src", absoluteUri.ToString());
            }
        }

        private void SetBaseUri(string url, IHtmlDocument document)
        {
            var baseTag = document.CreateElement("base");
            baseTag.SetAttribute("href", url);
            document.Head.AppendChild(baseTag);
        }
    }
}

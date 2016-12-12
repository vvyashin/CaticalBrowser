using System;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Extensions;
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
        /// <param name="proxyPrefix">Proxy prefix for links</param>
        /// <returns>HTML page</returns>
        public async Task<string> GetSubstitutePage(string url, IHeaders headers, string proxyPrefix)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (proxyPrefix == null) throw new ArgumentNullException(nameof(proxyPrefix));

            var uri = _uriBuilder.GetUri(url);
            var document = await _webPageReader.LoadPage(uri, headers);
            
            SetBaseUri(url, document);

            ReplaceRelativeHeadLinksWithAbsolute(document, uri);
            ReplaceLinksWithProxy(proxyPrefix, document, uri);
            ReplaceImagesWithCats(document, uri);
            
            return document.DocumentElement.OuterHtml;
        }

        private void ReplaceImagesWithCats(IDocument document, Uri baseUri)
        {
            foreach (var htmlImageElement in document.Images) {
                htmlImageElement.Source = _catPicturesGenerator.GetNextCatPicture(
                    new Uri(baseUri, htmlImageElement.Source).ToString());
            }
            foreach (var element in document.All) {
                var image = element.ComputeCurrentStyle().BackgroundImage;
                if (!String.IsNullOrWhiteSpace(image) && image.Contains("url")) {
                    var uri = new Uri(baseUri, image.Replace("url", "").Trim(' ', '(', ')', '"', '\''));
                    element.Style.BackgroundImage = $"url({_catPicturesGenerator.GetNextCatPicture(uri.ToString())})";
                }
            }
        }

        private void ReplaceLinksWithProxy(string proxyUrl, IDocument document, Uri baseUri)
        {
            foreach (var element in document.QuerySelectorAll("a")) {
                var href = element.GetAttribute("href");
                var absoluteUri = new Uri(baseUri, href);
                element.SetAttribute("href", $"{proxyUrl}{absoluteUri}");
            }
        }

        private void ReplaceRelativeHeadLinksWithAbsolute(IDocument document, Uri baseUri)
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

        private void SetBaseUri(string url, IDocument document)
        {
            var baseTag = document.CreateElement("base");
            baseTag.SetAttribute("href", url);
            document.Head.AppendChild(baseTag);
        }
    }
}

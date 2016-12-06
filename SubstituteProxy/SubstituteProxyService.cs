using System;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using WebUI.Services;

namespace SubstituteProxy
{
    public class SubstituteProxyService : ISubstituteProxyService
    {
        private readonly WebPageReader _webPageReader;
        private readonly CatPicturesGenerator _catPicturesGenerator;

        public SubstituteProxyService(WebPageReader webPageReader, CatPicturesGenerator catPicturesGenerator)
        {
            if (webPageReader == null) throw new ArgumentNullException(nameof(webPageReader));
            if (catPicturesGenerator == null) throw new ArgumentNullException(nameof(catPicturesGenerator));

            _webPageReader = webPageReader;
            _catPicturesGenerator = catPicturesGenerator;
        }
        
        public async Task<string> GetSubstitutePage(string url, IHeaders headers, string imageFolder, string proxyUrl)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (headers == null) throw new ArgumentNullException(nameof(headers));
            if (imageFolder == null) throw new ArgumentNullException(nameof(imageFolder));
            if (proxyUrl == null) throw new ArgumentNullException(nameof(proxyUrl));

            var page = await _webPageReader.LoadPage(url, headers);

            var parser = new HtmlParser();
            var document = parser.Parse(page);

            var baseUri = new Uri(url, UriKind.Absolute);

            foreach (var element in document.QuerySelectorAll("a")) {
                var href = element.GetAttribute("href");
                var absoluteUri = new Uri(baseUri, href);
                element.SetAttribute("href", $"{proxyUrl}{absoluteUri}");
            }

            foreach (var element in document.QuerySelectorAll("img")) {
                element.SetAttribute("src", $"{_catPicturesGenerator.GetNextCatPicture(imageFolder)}");
            }

            foreach (var element in document.QuerySelectorAll("link")) {
                var href = element.GetAttribute("href");
                element.SetAttribute("href", new Uri(baseUri, href).ToString());
            }

            return document.DocumentElement.OuterHtml;
        }
    }
}

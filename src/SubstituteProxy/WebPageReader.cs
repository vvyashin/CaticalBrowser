using System;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Network;
using AngleSharp.Network.Default;
using WebUI.Services;

namespace SubstituteProxy
{
    public class WebPageReader
    {
        /// <summary>
        /// Loads html page by http get request
        /// </summary>
        /// <param name="uri">URL of page for loading</param>
        /// <param name="headers">Http headers</param>
        /// <returns>AngleSharp.IDocument</returns>
        public virtual async Task<IDocument> LoadPage(Uri uri, IHeaders headers)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            var requester = new HttpRequester();
            requester.Headers["User-Agent"] = headers.UserAgent;
            requester.Headers["Accept"] = headers.Accept;
            
            var config = Configuration.Default
                .WithDefaultLoader(ldr => ldr.IsNavigationEnabled = true, 
                    requesters: new IRequester[] { requester })
                .WithJavaScript()
                .WithCss();

            return await BrowsingContext.New(config).OpenAsync(uri.ToString());
        }
    }
}

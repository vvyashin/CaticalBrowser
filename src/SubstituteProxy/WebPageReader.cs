using System;
using System.Net.Http;
using System.Threading.Tasks;
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
        /// <returns>Html code as string</returns>
        public virtual async Task<string> LoadPage(Uri uri, IHeaders headers)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            using (var httpClient = new HttpClient()) {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", headers.Accept);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", headers.UserAgent);

                var response = await httpClient.GetAsync(uri);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}

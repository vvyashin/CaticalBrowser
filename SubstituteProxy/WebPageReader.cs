using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebUI.Services;

namespace SubstituteProxy
{
    public class WebPageReader
    {
        private async Task<string> GetStringAsync(IHeaders headers, Uri uri)
        {
            using (var httpClient = new HttpClient()) {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", headers.Accept);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", headers.UserAgent);
                
                var response = await httpClient.GetAsync(uri);
                return await response.Content.ReadAsStringAsync();
            }
        }

        private Uri IfUrlIsNotHttpOrHttpsThrow(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri)) {
                throw new UriFormatException("Url format is not correct");
            }
            else if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) {
                return uri;
            }
            else {
                throw new NotSupportedException(
                    $"Sorry! Protocol '{uri.Scheme}' is not supported. Only http and https are supported");
            }
        }

        /// <summary>
        /// Loads html by http get request
        /// </summary>
        /// <param name="url">URL of page for loading</param>
        /// <param name="headers">Http headers</param>
        /// <returns>Html code as string</returns>
        public virtual async Task<string> LoadPage(string url, IHeaders headers)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (headers == null) throw new ArgumentNullException(nameof(headers));

            Uri uri = IfUrlIsNotHttpOrHttpsThrow(url);

            return await GetStringAsync(headers, uri);
        }
    }
}

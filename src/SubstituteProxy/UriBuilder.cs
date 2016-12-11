using System;

namespace SubstituteProxy
{
    public class UriBuilder
    {
        /// <summary>
        /// Get uri from url with http or https scheme
        /// </summary>
        /// <param name="url">url as string</param>
        /// <returns>URI</returns>
        /// <exception cref="ArgumentNullException">url is null</exception>
        /// <exception cref="UriFormatException">url format is not correct</exception>
        /// <exception cref="NotSupportedException">url scheme is not http or https</exception>
        public virtual Uri GetUri(string url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            Uri uri;
            try {
                uri = new System.UriBuilder(url).Uri;
            }
            catch (UriFormatException) {
                throw new UriFormatException("Url format is not correct");
            }
            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) {
                return uri;
            }
            else {
                throw new NotSupportedException(
                    $"Sorry! Protocol '{uri.Scheme}' is not supported. Only http and https are supported");
            }
        }
    }
}

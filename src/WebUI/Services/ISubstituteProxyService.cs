using System;
using System.Threading.Tasks;

namespace WebUI.Services
{
    public interface ISubstituteProxyService
    {
        /// <summary>
        /// Gets page where all images are substituted with cat pictures
        /// </summary>
        /// <param name="url">Url of loading web site</param>
        /// <param name="headers">Headers of http request</param>
        /// <param name="imageFolderUrl">Absolute url to folder with cat pictures</param>
        /// <param name="proxyPrefix">Proxy prefix for links</param>
        /// <returns>HTML page</returns>
        Task<string> GetSubstitutePage(string url, IHeaders headers, Uri imageFolderUrl, string proxyPrefix);
    }
}

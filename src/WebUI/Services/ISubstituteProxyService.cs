using System.Threading.Tasks;

namespace WebUI.Services
{
    public interface ISubstituteProxyService
    {
        /// <summary>
        /// Gets page on which all images are substituted with cat pictures
        /// </summary>
        /// <param name="url">url of loading web site</param>
        /// <param name="imageFolder">relative path to folder with cat pictures</param>
        /// <param name="proxyUrl"></param>
        /// <returns>HTML page</returns>
        Task<string> GetSubstitutePage(string url, IHeaders headers, string imageFolder, string proxyUrl);
    }
}

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebUI.Services;
using WebUI.ViewModel;

namespace WebUI.Controllers
{
    public class BrowserController : Controller
    {
        private readonly ISubstituteProxyService _substituteProxyService;

        public BrowserController(ISubstituteProxyService substituteProxyService)
        {
            if (substituteProxyService == null) throw new ArgumentNullException(nameof(substituteProxyService));

            _substituteProxyService = substituteProxyService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        [ValidateInput(false)]
        public async Task<ActionResult> Page(string url)
        {
            var model = new PageViewModel();
            
            if (String.IsNullOrWhiteSpace(url)) {
                model.Content = String.Empty;
            }
            else {
                try {
                    var uriBase = HttpContext.Request.Url;
                    var proxyPrefix = new Uri(uriBase, "/Page?url=").ToString();

                    model.Content = await _substituteProxyService.GetSubstitutePage(
                        url, new Headers(HttpContext), proxyPrefix);
                }
                catch (UriFormatException ufe) {
                    model.Content = ufe.Message;
                }
                catch (NotSupportedException nse) {
                    model.Content = nse.Message;
                }
                catch (HttpRequestException rex) {
                    model.Content = rex.Message;
                }
                catch (Exception e) {
                    model.Content = "Something went wrong while displaying this webpage";
                }
            }
            return View("Page", model);
        }
    }
}

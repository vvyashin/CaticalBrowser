using System;
using System.Web;

namespace WebUI.Services
{
    public class Headers : IHeaders
    {
        private readonly HttpContextBase _httpContext;

        public Headers(HttpContextBase httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            _httpContext = httpContext;
        }


        public string UserAgent => _httpContext.Request.UserAgent;
        public string Accept => String.Join(",", _httpContext.Request.AcceptTypes ?? new string[0]);
    }
}

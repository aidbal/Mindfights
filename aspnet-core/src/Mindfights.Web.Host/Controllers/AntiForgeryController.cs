using Microsoft.AspNetCore.Antiforgery;
using Mindfights.Controllers;

namespace Mindfights.Web.Host.Controllers
{
    public class AntiForgeryController : MindfightsControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}

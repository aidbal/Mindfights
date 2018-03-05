using Microsoft.AspNetCore.Antiforgery;
using Skautatinklis.Controllers;

namespace Skautatinklis.Web.Host.Controllers
{
    public class AntiForgeryController : SkautatinklisControllerBase
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

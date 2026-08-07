using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcAuthDemo.Controllers
{
    [Authorize] 
    public class PortfolioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
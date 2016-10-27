using Microsoft.AspNetCore.Mvc;

using NuClear.ValidationRules.WebApp.DataAccess;

namespace NuClear.ValidationRules.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserRepositiory _userRepositiory;

        public HomeController(UserRepositiory userRepositiory)
        {
            _userRepositiory = userRepositiory;
        }

        public IActionResult Index()
        {
            var account = _userRepositiory.GetUserId(User.Identity.Name);
            var project = _userRepositiory.GetDefaultProject(account);
            return new RedirectToActionResult("Index", "Search", new { account = account, project = project });
        }
    }
}

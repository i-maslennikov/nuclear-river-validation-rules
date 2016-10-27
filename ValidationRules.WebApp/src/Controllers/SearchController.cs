using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using NuClear.ValidationRules.WebApp.DataAccess;
using NuClear.ValidationRules.WebApp.Model;

namespace NuClear.ValidationRules.WebApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly UserRepositiory _userRepositiory;
        private readonly ProjectRepositiory _projectRepositiory;

        public SearchController(UserRepositiory userRepositiory, ProjectRepositiory projectRepositiory, IHostingEnvironment env)
        {
            _userRepositiory = userRepositiory;
            _projectRepositiory = projectRepositiory;
            _env = env;
        }

        [HttpGet]
        public IActionResult Test()
        {
            return new JsonResult(new { _env.ApplicationName, _env.EnvironmentName });
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ModelBase());
        }

        [HttpPost]
        public IActionResult Index([FromForm]long? account, [FromForm]long? project)
        {
            if(account == null && project == null)
            {
                ViewBag.Error = $"Выберите пользователя и/или город";
                return View(new ModelBase());
            }

            return new RedirectToActionResult("Public", "Order", new { account, project });
        }

        [HttpGet]
        public IActionResult Account(string query)
        {
            var users = _userRepositiory.Search(query);
            return new JsonResult(users.Select(x => new { Id = x.Id.ToString(), Value = $"{x.LastName} {x.FirstName}" }));
        }

        [HttpGet]
        public IActionResult Project(string query)
        {
            var projects = _projectRepositiory.Search(query);
            return new JsonResult(projects.Select(x => new { Id = x.Id.ToString(), Value = x.DisplayName }));
        }
    }
}

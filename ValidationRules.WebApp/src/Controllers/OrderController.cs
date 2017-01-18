using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using NuClear.ValidationRules.WebApp.Configuration;
using NuClear.ValidationRules.WebApp.DataAccess;
using NuClear.ValidationRules.WebApp.Model;
using NuClear.ValidationRules.WebApp.Serializers;

namespace NuClear.ValidationRules.WebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderRepositiory _orderRepositiory;
        private readonly UserRepositiory _userRepositiory;
        private readonly ProjectRepositiory _projectRepositiory;
        private readonly QueryingClient _queryingClient;
        private readonly IOptions<LinkFactorySettings> _linkSettings;

        public OrderController(OrderRepositiory orderRepositiory,
                               UserRepositiory userRepositiory,
                               QueryingClient queryingClient,
                               ProjectRepositiory projectRepositiory, 
                               IOptions<LinkFactorySettings> linkSettings)
        {
            _orderRepositiory = orderRepositiory;
            _userRepositiory = userRepositiory;
            _queryingClient = queryingClient;
            _projectRepositiory = projectRepositiory;
            _linkSettings = linkSettings;
        }

        public async Task<IActionResult> Draft(long? account, long? project)
        {
            if (account == null && project == null)
            {
                return new RedirectToActionResult("Index", "Search", null);
            }

            var date = project.HasValue
                ? _projectRepositiory.GetNextRelease(project.Value)
                : _projectRepositiory.GetNextRelease(_userRepositiory.GetDefaultProject(account.Value));
            var orders = _orderRepositiory.GetDraftOrders(account, project, date);

            var validationResults = await _queryingClient.Manual(orders.Keys.ToArray(), date, project);

            var factory = new MessageFactory(_linkSettings, orders);

            ViewBag.Message = $"Выведены результаты за {date:Y}";

            return View(new MessageContainerModel
            {
                AccountId = account,
                AccountName = account.HasValue ? _userRepositiory.GetAccountName(account.Value) : null,
                ProjectId = project,
                ProjectName = project.HasValue ? _projectRepositiory.GetProjectName(project.Value) : null,
                Results = validationResults.Select(factory.CreateMessage),
            });
        }

        public async Task<IActionResult> Public(long? account, long? project, int? rule)
        {
            if (account == null && project == null)
            {
                return new RedirectToActionResult("Index", "Search", null);
            }

            var date = project.HasValue
                           ? _projectRepositiory.GetNextRelease(project.Value)
                           : _projectRepositiory.GetNextRelease(_userRepositiory.GetDefaultProject(account.Value));
            var orders = _orderRepositiory.GetPublicOrders(account, project, date);

            var validationResults = await _queryingClient.Manual(orders.Keys.ToArray(), date, project);

            ViewBag.Message = $"Выведены результаты за {date:Y}";

            var factory = new MessageFactory(_linkSettings, orders);

            if (rule.HasValue)
            {
                return Content(string.Join(Environment.NewLine,
                                        validationResults.Where(x => x.Rule == rule).Select(factory.CreatePlainTextMessage)));
            }

            return View(new MessageContainerModel
                {
                    AccountId = account,
                    AccountName = account.HasValue ? _userRepositiory.GetAccountName(account.Value) : null,
                    ProjectId = project,
                    ProjectName = project.HasValue ? _projectRepositiory.GetProjectName(project.Value) : null,
                    Results = validationResults.Select(factory.CreateMessage),
                });
        }

        public async Task<IActionResult> Single(long id)
        {
            var validationResults = await _queryingClient.Single(id);

            var factory = new MessageFactory(_linkSettings, new Dictionary<long,OrderDto>());

            return View(new MessageContainerModel
                {
                    Results = validationResults.Select(factory.CreateMessage),
                });
        }
    }
}
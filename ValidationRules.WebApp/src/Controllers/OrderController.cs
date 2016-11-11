using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

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
        private readonly LinkFactory _linkFactory;

        public OrderController(OrderRepositiory orderRepositiory,
                               UserRepositiory userRepositiory,
                               QueryingClient queryingClient,
                               LinkFactory linkFactory,
                               ProjectRepositiory projectRepositiory)
        {
            _orderRepositiory = orderRepositiory;
            _userRepositiory = userRepositiory;
            _queryingClient = queryingClient;
            _linkFactory = linkFactory;
            _projectRepositiory = projectRepositiory;
        }

        public IActionResult Draft(long? account, long? project)
        {
            var date = project.HasValue
                ? _projectRepositiory.GetNextRelease(project.Value)
                : _projectRepositiory.GetNextRelease(_userRepositiory.GetDefaultProject(account.Value));
            var orders = _orderRepositiory.GetDraftOrders(account, project, date);

            var validationResults = _queryingClient.Manual(orders.Keys.ToArray(), date, project);

            var factory = new MessageFactory(_linkFactory, orders);

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

        public IActionResult Public(long? account, long? project, int? rule)
        {
            var date = project.HasValue
                           ? _projectRepositiory.GetNextRelease(project.Value)
                           : _projectRepositiory.GetNextRelease(_userRepositiory.GetDefaultProject(account.Value));
            var orders = _orderRepositiory.GetPublicOrders(account, project, date);

            var validationResults = _queryingClient.Manual(orders.Keys.ToArray(), date, project);

            ViewBag.Message = $"Выведены результаты за {date:Y}";

            var factory = new MessageFactory(_linkFactory, orders);

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

        public IActionResult Single(long id)
        {
            var validationResults = _queryingClient.Single(id);

            var factory = new MessageFactory(_linkFactory, new Dictionary<long,OrderDto>());

            return View(new MessageContainerModel
                {
                    Results = validationResults.Select(factory.CreateMessage),
                });
        }
    }
}
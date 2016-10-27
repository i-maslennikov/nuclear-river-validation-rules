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
        private readonly MessageRepositiory _messageRepositiory;
        private readonly LocalizedMessageFactory _localizedMessageFactory;
        private readonly LinkFactory _linkFactory;

        public OrderController(OrderRepositiory orderRepositiory, UserRepositiory userRepositiory, MessageRepositiory messageRepositiory, LocalizedMessageFactory localizedMessageFactory, LinkFactory linkFactory, ProjectRepositiory projectRepositiory)
        {
            _orderRepositiory = orderRepositiory;
            _userRepositiory = userRepositiory;
            _messageRepositiory = messageRepositiory;
            _localizedMessageFactory = localizedMessageFactory;
            _linkFactory = linkFactory;
            _projectRepositiory = projectRepositiory;
        }

        public IActionResult Draft(long? account, long? project)
        {
            var date = project.HasValue
                ? _projectRepositiory.GetNextRelease(project.Value)
                : _projectRepositiory.GetNextRelease(_userRepositiory.GetDefaultProject(account.Value));
            var orders = _orderRepositiory.GetDraftOrders(account, project, date);

            ViewBag.Message = $"Выведены результаты за {date:Y}";

            var validationResults = _messageRepositiory.GetMessages(orders.Keys.ToArray(), project);
            var messages = Filter(validationResults, x => x.WhenSingle(), Result.Info);

            var model = new ResultContainer(_linkFactory)
            {
                AccountId = account,
                AccountName = account.HasValue ? _userRepositiory.GetAccountName(account.Value) : null,
                ProjectId = project,
                ProjectName = project.HasValue ? _projectRepositiory.GetProjectName(project.Value) : null,
                Results = messages,
                Orders = orders,
            };

            return View(model);
        }

        public IActionResult Public(long? account, long? project, int? rule)
        {
            var date = project.HasValue
                ? _projectRepositiory.GetNextRelease(project.Value)
                : _projectRepositiory.GetNextRelease(_userRepositiory.GetDefaultProject(account.Value));
            var orders = _orderRepositiory.GetPublicOrders(account, project, date);

            var validationResults = _messageRepositiory.GetMessages(orders.Keys.ToArray(), project);
            validationResults = validationResults.Where(x => x.PeriodStart <= date && x.PeriodEnd >= date.AddMonths(1)).ToArray();
            var messages = Filter(validationResults, x => x.WhenMass(), Result.Warning);

            ViewBag.Message = $"Выведены результаты за {date:Y}";

            if (rule.HasValue)
                return Content(string.Join(Environment.NewLine, messages.Where(x => x.MessageType == rule).Select(x => x.Text.RemoveLinks())));

            var model = new ResultContainer(_linkFactory)
            {
                AccountId = account,
                AccountName = account.HasValue ? _userRepositiory.GetAccountName(account.Value) : null,
                ProjectId = project,
                ProjectName = project.HasValue ? _projectRepositiory.GetProjectName(project.Value) : null,
                Results = messages,
                Orders = orders,
            };

            return View(model);
        }

        public IActionResult Single(long id)
        {
            var validationResults = _messageRepositiory.GetMessages(new[] { id }, null);

            var model = new SingleCheckContainer(_linkFactory)
                {
                    WhenSingle = Filter(validationResults, builder => builder.WhenSingle(), Result.Info),
                    WhenMass = Filter(validationResults, builder => builder.WhenMass(), Result.Info),
                    WhenMassPrerelease = Filter(validationResults, builder => builder.WhenMassPrerelease(), Result.Info),
                    WhenMassRelease = Filter(validationResults, builder => builder.WhenMassRelease(), Result.Info),
                };

            return View(model);
        }

        private IReadOnlyCollection<MessageViewModel> Filter(IEnumerable<Entity.ValidationResult> results, Func<ResultBuilder, Result> func, Result minLevel)
            => results.Select(x => Tuple.Create(func.Invoke(new ResultBuilder(x.Result)), _localizedMessageFactory.Localize(x)))
                      .Where(x => x.Item1 >= minLevel)
                      .Distinct(new MessageTemplateComparer())
                      .Select(x => new MessageViewModel(x.Item1, x.Item2, _linkFactory.CreateLink(x.Item2.Order)))
                      .ToArray();

        internal class MessageTemplateComparer : IEqualityComparer<Tuple<Result, MessageTemplate>>
        {
            public bool Equals(Tuple<Result, MessageTemplate> x, Tuple<Result, MessageTemplate> y)
            {
                return x.Item2.Equals(y.Item2);
            }

            public int GetHashCode(Tuple<Result, MessageTemplate> obj)
            {
                return obj.Item2.GetHashCode();
            }
        }
    }
}

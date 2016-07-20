//using System.Collections.Generic;
//using System.Linq;

//using NuClear.ValidationRules.Replication.PriceRules.Validation.Dto;
//using NuClear.ValidationRules.Replication.Specifications;
//using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

//using NUnit.Framework;

//namespace NuClear.ValidationRules.Replication.Tests
//{
//    [TestFixture]
//    public sealed class BindingObjectSpecificationTests
//    {
//        [TestCaseSource(nameof(Examples))]
//        public void Ok(Parameter left, Parameter right, bool matches)
//        {
//            var func = Specs.Find.Aggs.BindingObjectMatch().Compile();
//            var test1 = new PrincipalAssociatedPostionPair
//                {
//                    OrderAssociatedPosition = CreateOrderAssociatedPosition(left),
//                    OrderPrincipalPosition = CreateOrderPrincipalPosition(right),
//                };

//            var test2 = new PrincipalAssociatedPostionPair
//                {
//                    OrderAssociatedPosition = CreateOrderAssociatedPosition(right),
//                    OrderPrincipalPosition = CreateOrderPrincipalPosition(left),
//                };

//            Assert.That(func.Invoke(test1), Is.EqualTo(matches), "Expression is not correct");
//            Assert.That(func.Invoke(test1), Is.EqualTo(func.Invoke(test2)), "Expression is not commutative");
//        }

//        private static OrderPosition CreateOrderPrincipalPosition(Parameter parameter)
//            => new OrderPosition
//            {
//                HasNoBinding = parameter.HasNoBinding,
//                Category1Id = parameter.Category1Id,
//                Category3Id = parameter.Category3Id,
//                FirmAddressId = parameter.FirmAddressId,
//            };

//        private static OrderAssociatedPosition CreateOrderAssociatedPosition(Parameter parameter)
//            => new OrderAssociatedPosition
//                {
//                    HasNoBinding = parameter.HasNoBinding,
//                    Category1Id = parameter.Category1Id,
//                    Category3Id = parameter.Category3Id,
//                    FirmAddressId = parameter.FirmAddressId,
//                };

//        private IEnumerable<TestCaseData> Examples()
//        {
//            var noBinding = new Parameter { HasNoBinding = true };
//            var category3 = new Parameter { Category3Id = 3, Category1Id = 1 };
//            var category1 = new Parameter { Category1Id = 1 }; // Не бывает по бизнесу
//            var addressCategory3 = new Parameter { Category3Id = 3, Category1Id = 1, FirmAddressId = 1 };
//            var addressCategory1 = new Parameter { Category1Id = 1, FirmAddressId = 1 };
//            var address = new Parameter { FirmAddressId = 1 };

//            var anotherCategory3 = new Parameter { Category3Id = 33, Category1Id = 11 };
//            var anotherCategory1 = new Parameter { Category1Id = 11 }; // Не бывает по бизнесу
//            var anotherAddressCategory3 = new Parameter { Category3Id = 33, Category1Id = 11, FirmAddressId = 11 };
//            var anotherAddressCategory1 = new Parameter { Category1Id = 11, FirmAddressId = 11 };
//            var anotherAddress = new Parameter { FirmAddressId = 11 };

//            return new[]
//                {
//                    new TestCaseData(noBinding, noBinding, true),
//                    new TestCaseData(noBinding, category3, false),
//                    new TestCaseData(noBinding, category1, false),
//                    new TestCaseData(noBinding, addressCategory3, false),
//                    new TestCaseData(noBinding, addressCategory1, false),
//                    new TestCaseData(noBinding, address, false),

//                    new TestCaseData(category3, category3, true),
//                    new TestCaseData(category3, category1, true),
//                    new TestCaseData(category3, addressCategory3, true),
//                    new TestCaseData(category3, addressCategory1, true),
//                    new TestCaseData(category3, address, false),

//                    new TestCaseData(category3, anotherCategory3, false),
//                    new TestCaseData(category3, anotherCategory1, false),
//                    new TestCaseData(category3, anotherAddressCategory3, false),
//                    new TestCaseData(category3, anotherAddressCategory1, false),
//                    new TestCaseData(category3, anotherAddress, false),

//                    new TestCaseData(category1, category1, true),
//                    new TestCaseData(category1, addressCategory3, true),
//                    new TestCaseData(category1, addressCategory1, true),
//                    new TestCaseData(category1, address, false),

//                    new TestCaseData(category1, anotherCategory1, false),
//                    new TestCaseData(category1, anotherAddressCategory3, false),
//                    new TestCaseData(category1, anotherAddressCategory1, false),
//                    new TestCaseData(category1, anotherAddress, false),

//                    new TestCaseData(addressCategory3, addressCategory3, true),
//                    new TestCaseData(addressCategory3, addressCategory1, true),
//                    new TestCaseData(addressCategory3, address, true),

//                    new TestCaseData(addressCategory3, anotherAddressCategory3, false),
//                    new TestCaseData(addressCategory3, anotherAddressCategory1, false),
//                    new TestCaseData(addressCategory3, anotherAddress, false),

//                    new TestCaseData(addressCategory1, addressCategory1, true),
//                    new TestCaseData(addressCategory1, address, true),

//                    new TestCaseData(addressCategory1, anotherAddressCategory1, false),
//                    new TestCaseData(addressCategory1, anotherAddress, false),

//                    new TestCaseData(address, address, true),

//                    new TestCaseData(address, anotherAddress, false),

//                };
//        }

//        public class Parameter
//        {
//            public bool HasNoBinding { get; set; }
//            public long? Category1Id { get; set; }
//            public long? Category3Id { get; set; }
//            public long? FirmAddressId { get; set; }

//            public override string ToString()
//            {
//                // Для читаемых результатов теста
//                var sb = new List<string>();
//                if (Category3Id.HasValue) sb.Add("Рубрика 3-го уровня ");
//                else if(Category1Id.HasValue) sb.Add("Рубрика 1-го уровня ");
//                if (FirmAddressId.HasValue) sb.Add("Адрес");
//                return sb.Any() ? string.Join(" + ", sb) : "Без привязки";
//            }
//        }
//    }
//}

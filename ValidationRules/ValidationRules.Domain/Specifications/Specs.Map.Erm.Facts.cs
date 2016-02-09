using System;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Domain.Specifications
{
    using Erm = Model.Erm;
    using Facts = Model.Facts;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static class Erm
            {
                public static class ToFacts
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Facts::AssociatedPosition>> AssociatedPosition =
                        new MapSpecification<IQuery, IQueryable<Facts::AssociatedPosition>>(
                            q => q.For<Erm::AssociatedPosition>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.AssociatedPosition));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::AssociatedPositionsGroup>> AssociatedPositionsGroup =
                        new MapSpecification<IQuery, IQueryable<Facts::AssociatedPositionsGroup>>(
                            q => q.For<Erm::AssociatedPositionsGroup>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.AssociatedPositionsGroup));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::Category>> Category =
                        new MapSpecification<IQuery, IQueryable<Facts::Category>>(
                            q => q.For<Erm::Category>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.Category));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::DeniedPosition>> DeniedPosition =
                        new MapSpecification<IQuery, IQueryable<Facts::DeniedPosition>>(
                            q => q.For<Erm::DeniedPosition>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.DeniedPosition));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::Order>> Order =
                        new MapSpecification<IQuery, IQueryable<Facts::Order>>(
                            q => q.For<Erm::Order>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.Order));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::OrderPosition>> OrderPosition =
                        new MapSpecification<IQuery, IQueryable<Facts::OrderPosition>>(
                            q => q.For<Erm::OrderPosition>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.OrderPosition));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::OrderPositionAdvertisement>> OrderPositionAdvertisement =
                        new MapSpecification<IQuery, IQueryable<Facts::OrderPositionAdvertisement>>(
                            q => q.For<Erm::OrderPositionAdvertisement>()
                                  .Select(Transform.OrderPositionAdvertisement));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::OrganizationUnit>> OrganizationUnit =
                        new MapSpecification<IQuery, IQueryable<Facts::OrganizationUnit>>(
                            q => q.For<Erm::OrganizationUnit>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.OrganizationUnit));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::Position>> Position =
                        new MapSpecification<IQuery, IQueryable<Facts::Position>>(
                            q => q.For<Erm::Position>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.Position));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::Price>> Price =
                        new MapSpecification<IQuery, IQueryable<Facts::Price>>(
                            q => q.For<Erm::Price>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.Price));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::PricePosition>> PricePosition =
                        new MapSpecification<IQuery, IQueryable<Facts::PricePosition>>(
                            q => q.For<Erm::PricePosition>()
                                  .Where(entity => entity.IsActive && !entity.IsDeleted)
                                  .Select(Transform.PricePosition));

                    public static readonly MapSpecification<IQuery, IQueryable<Facts::Project>> Project =
                        new MapSpecification<IQuery, IQueryable<Facts::Project>>(
                            q => q.For<Erm::Project>()
                                  .Where(entity => entity.IsActive && entity.OrganizationUnitId.HasValue)
                                  .Select(Transform.Project));

                    private static class Transform
                    {
                        public static readonly Expression<Func<Erm::AssociatedPosition, Facts::AssociatedPosition>> AssociatedPosition =
                            x => new Facts::AssociatedPosition
                                {
                                    Id = x.Id,
                                    AssociatedPositionsGroupId = x.AssociatedPositionsGroupId,
                                    PositionId = x.PositionId,
                                    ObjectBindingType = x.ObjectBindingType,
                                };

                        public static readonly Expression<Func<Erm::AssociatedPositionsGroup, Facts::AssociatedPositionsGroup>> AssociatedPositionsGroup =
                            x => new Facts::AssociatedPositionsGroup
                                {
                                    Id = x.Id,
                                    PricePositionId = x.PricePositionId,
                                };

                        public static readonly Expression<Func<Erm::Category, Facts::Category>> Category =
                            x => new Facts::Category
                                {
                                    Id = x.Id,
                                    ParentId = x.ParentId
                                };

                        public static readonly Expression<Func<Erm::DeniedPosition, Facts::DeniedPosition>> DeniedPosition =
                            x => new Facts::DeniedPosition
                                {
                                    Id = x.Id,
                                    PriceId = x.PriceId,
                                    PositionDeniedId = x.PositionDeniedId,
                                    PositionId = x.PositionId,
                                    ObjectBindingType = x.ObjectBindingType,
                                };

                        public static readonly Expression<Func<Erm::Order, Facts::Order>> Order =
                            x => new Facts::Order
                                {
                                    Id = x.Id,
                                    FirmId = x.FirmId,
                                    OwnerId = x.OwnerCode,
                                    DestOrganizationUnitId = x.DestOrganizationUnitId,
                                    SourceOrganizationUnitId = x.SourceOrganizationUnitId,
                                    WorkflowStepId = x.WorkflowStepId,
                                    BeginDistributionDate = x.BeginDistributionDate,
                                    EndDistributionDateFact = x.EndDistributionDateFact,
                                    BeginReleaseNumber = x.BeginReleaseNumber,
                                    EndReleaseNumberPlan = x.EndReleaseNumberPlan,
                                    EndReleaseNumberFact = x.EndReleaseNumberFact,
                                    Number = x.Number,
                                };

                        public static readonly Expression<Func<Erm::OrderPosition, Facts::OrderPosition>> OrderPosition =
                            x => new Facts::OrderPosition
                                {
                                    Id = x.Id,
                                    OrderId = x.OrderId,
                                    PricePositionId = x.PricePositionId,
                                };

                        public static readonly Expression<Func<Erm::OrderPositionAdvertisement, Facts::OrderPositionAdvertisement>> OrderPositionAdvertisement =
                            x => new Facts::OrderPositionAdvertisement
                                {
                                    Id = x.Id,
                                    CategoryId = x.CategoryId,
                                    FirmAddressId = x.FirmAddressId,
                                    PositionId = x.PositionId,
                                    OrderPositionId = x.OrderPositionId,
                                };

                        public static readonly Expression<Func<Erm::OrganizationUnit, Facts::OrganizationUnit>> OrganizationUnit =
                            x => new Facts::OrganizationUnit
                                {
                                    Id = x.Id,
                                };

                        public static readonly Expression<Func<Erm::Position, Facts::Position>> Position =
                            x => new Facts::Position
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    IsComposite = x.IsComposite,
                                    IsControlledByAmount = x.IsControlledByAmount,
                                    PositionCategoryId = x.CategoryId,
                                };

                        public static readonly Expression<Func<Erm::Price, Facts::Price>> Price =
                            x => new Facts::Price
                                {
                                    Id = x.Id,
                                    BeginDate = x.BeginDate,
                                    IsPublished = x.IsPublished,
                                    OrganizationUnitId = x.OrganizationUnitId,
                                };

                        public static readonly Expression<Func<Erm::PricePosition, Facts::PricePosition>> PricePosition =
                            x => new Facts::PricePosition
                                {
                                    Id = x.Id,
                                    MaxAdvertisementAmount = x.MaxAdvertisementAmount,
                                    MinAdvertisementAmount = x.MinAdvertisementAmount,
                                    PositionId = x.PositionId,
                                    PriceId = x.PriceId,
                                };

                        public static readonly Expression<Func<Erm::Project, Facts::Project>> Project =
                            x => new Facts::Project
                                {
                                    Id = x.Id,
                                    OrganizationUnitId = x.OrganizationUnitId.Value
                                };
                    }
                }
            }
        }
    }
}
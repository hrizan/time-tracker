using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TimeTracker.Backend.Filters;
using TimeTracker.Backend.Models;

namespace TimeTracker.Backend.Controllers
{
    public class ReportController : ApiControllerBase
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        [AuthorizeToken]
        public ProductivityPointsDTO GetProductivityPointsForDate(DateTime forDate)
        {
            Guid consumerId = CurrentUserConsumerId.Value;

            DateTime dateFrom = forDate.Date;
            DateTime dateTo = forDate.Date.AddDays(1);
           
            var query = db.Activities.Where(
                w => w.ConsumerId == consumerId
                && w.TimeFrom > dateFrom
                && w.TimeTo < dateTo)
                .GroupBy(g => g.IsProductive)
                .Select(sa => new
                {
                    Positive = sa.Key,
                    Sum = sa.Sum(sum => Math.Abs(sum.ProductivityScore))
                }).ToList();

            ProductivityPointsDTO result = new ProductivityPointsDTO();

            result.Productive = query.Single(w => w.Positive == true).Sum;
            result.Distractive = query.Single(w => w.Positive == false).Sum;

            return result;
        }

        [AuthorizeToken]
        public ProductivityPointsByHours GetProductivityPointsForDateByHours(DateTime forDate)
        {
            Guid consumerId = CurrentUserConsumerId.Value;

            DateTime dateFrom = forDate.Date;
            DateTime dateTo = forDate.Date.AddDays(1);

            var query = db.Activities.Where(
                w => w.ConsumerId == consumerId
                && w.TimeFrom > dateFrom
                && w.TimeTo < dateTo)
                .GroupBy(g => new { g.IsProductive, g.TimeTo.Hour })
                .Select(sa => new
                {
                    Positive = sa.Key.IsProductive,
                    Sum = sa.Sum(sum => Math.Abs(sum.ProductivityScore)),
                    Hour = sa.Key.Hour
                }).ToList();

            ProductivityPointsByHours result = new ProductivityPointsByHours();

            result.Productive = query
                .Where(w => w.Positive == true)
                .Select(s => s.Sum)
                .ToList();

            result.Distractive = query
                .Where(w => w.Positive == false)
                .Select(s => s.Sum)
                .ToList();

            return result;
        }

        public List<ProductivityByCategoryDTO> GetProductivityByCategoriesForDate(DateTime forDate)
        {
            Guid consumerId = CurrentUserConsumerId.Value;

            DateTime dateFrom = forDate.Date;
            DateTime dateTo = forDate.Date.AddDays(1);

            var result = db.Activities.Where(a =>
                a.ConsumerId == consumerId
                && a.TimeFrom > dateFrom
                && a.TimeTo < dateTo
                && a.Category != null)
                .GroupBy(g => g.CategoryId)
                .Select(s => new ProductivityByCategoryDTO()
                {
                    Label = s.First().Category.Name,
                    Value = (int)s.Sum(sum => sum.DurationInSec / 60)//I dont know if this is working
                }).
                Where(pbc => pbc.Value >= 1)
                .OrderBy(o => o.Value)
                .Take(5)
                .ToList();

            return result;
        }

        public List<ProductivityByCategoryByHoursDTO> GetProductivityByCategoriesForDateByHours(DateTime forDate)
        {
            Guid consumerId = CurrentUserConsumerId.Value;

            DateTime dateFrom = forDate.Date;
            DateTime dateTo = forDate.Date.AddDays(1);

            var result = db.Activities.Where(a =>
                a.ConsumerId == consumerId
                && a.TimeFrom > dateFrom
                && a.TimeTo < dateTo
                && a.Category != null)
                .GroupBy(g => new { g.CategoryId, g.TimeTo.Hour })
                .Select(ans => new
                {
                    CategoryName = ans.First().Category.Name,
                    Sum = (int)ans.Sum(sum => sum.DurationInSec),
                    Hour = ans.Key.Hour
                })
                .Where(pbc => pbc.Sum >= 1)
                .GroupBy(gg => new { gg.CategoryName, gg.Hour })
                .Select(s => new ProductivityByCategoryByHoursDTO()
                {
                    Label = s.Key.CategoryName,
                    Values = s.Select(ss => ss.Sum).ToList()
                })
                .OrderBy(o => o.Values.Sum())
                .Take(5)
                .ToList();

            return result;
        }
    }
}

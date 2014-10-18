using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TimeTracker.Backend.Models;

namespace TimeTracker.Backend.Controllers
{
    public class ReportController : ApiController
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        public ProductivityPointsDTO GetProductivityPoints()
        {
            Guid userId = new Guid();
            var query = db.Activities.Where(
                w => w.ConsumerId == userId
                && w.TimeFrom > DateTime.Today
                && w.TimeTo < DateTime.Today.AddDays(1))
                .GroupBy(g => g.ProductiveMultiplier > 0)
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

        public List<ProductivityByCategoryDTO> GetProductivityByCategories()
        {
            Guid userId = new Guid();
            var result = db.Activities.Where(a =>
                a.ConsumerId == userId
                && a.TimeFrom > DateTime.Today
                && a.TimeTo < DateTime.Today.AddDays(1)
                && a.Category != null)
                .GroupBy(g => g.CategoryId)
                .Select(s => new ProductivityByCategoryDTO()
                {
                    Label = s.First().Category.Name,
                    Value = s.Sum(sum => sum.Duration / 60)
                }).
                Where(pbc => pbc.Value >= 1)
                .OrderBy(o => o.Value)
                .Take(5)
                .ToList();

            return result;
        }
    }
}

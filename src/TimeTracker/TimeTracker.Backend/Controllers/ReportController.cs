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
            //return new ProductivityPointsDTO() { Distractive = 1, Productive = 2 };
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

            var positiveQuery = query.SingleOrDefault(w => w.Positive == true);
            var negativeQuery = query.SingleOrDefault(w => w.Positive == false);
            result.Productive = positiveQuery != null ? positiveQuery.Sum : 0;
            result.Distractive = negativeQuery != null ? negativeQuery.Sum : 0;

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

        [AuthorizeToken]
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
                    Label = s.FirstOrDefault() == null ? "No category" : s.FirstOrDefault().Category.Name,
                    Value = (int)s.Sum(sum => sum.DurationInSec / 60)//I dont know if this is working
                }).
                Where(pbc => pbc.Value >= 1)
                .OrderBy(o => o.Value)
                .Take(5)
                .ToList();

            return result;
        }

        [AuthorizeToken]
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
                    CategoryName = ans.FirstOrDefault() == null ? "No category" : ans.FirstOrDefault().Category.Name,
                    Sum = (int)ans.Sum(sum => sum.DurationInSec / 60),
                    Hour = ans.Key.Hour
                })
                .Where(pbc => pbc.Sum >= 1)
                .ToList()
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

        //Demo
        [AuthorizeToken]
        public ProductivityPointsDTO GetProductivityPointsForDateDemo(DateTime forDate)
        {

            if (forDate > DateTime.Today)
            {
                return new ProductivityPointsDTO()
                {
                    Productive = 0,
                    Distractive = 0,
                };
            }

            if (forDate == DateTime.Today)
            {
                return new ProductivityPointsDTO()
                {
                    Productive = 540,
                    Distractive = 160,
                };
            }
            else if (forDate == DateTime.Today.AddDays(-1))
            {
                return new ProductivityPointsDTO()
                {
                    Productive = 120,
                    Distractive = 160,
                };
            }
            else
            {
                return new ProductivityPointsDTO()
                {
                    Productive = 600,
                    Distractive = 400,
                };
            }
        }

        [AuthorizeToken]
        public ProductivityPointsByHours GetProductivityPointsForDateByHoursDemo(DateTime forDate)
        {
            if (forDate > DateTime.Today)
            {
                return new ProductivityPointsByHours()
                {
                    Productive = new List<int>() { 0 },
                    Distractive = new List<int>() { 0 }
                };
            }

            if (forDate == DateTime.Today)
            {
                return new ProductivityPointsByHours()
                {
                    Productive = new List<int>() { 40, 2, 30, 120, 3, 4, 120, 213, 100, 12, 123, 334, 123 },
                    Distractive = new List<int>() { 12, 200, 30, 120, 323, 4, 12, 213, 12, 13, 14, 7, 6 }
                };
            }
            else if (forDate == DateTime.Today.AddDays(-1))
            {
                return new ProductivityPointsByHours()
                {
                    Productive = new List<int>() { 0, 0, 0, 120, 32, 43, 33, 32, 110, 145, 120, 140, 123, 230, 1, 75, 60, 70, 650, 12, 320, 430, 150, 123 },
                    Distractive = new List<int>() { 0, 0, 0, 12, 120, 323, 33, 32, 110, 145, 12, 140, 123, 23, 1, 45, 78, 33, 100, 128, 15, 43, 13, 123 }
                };
            }
            else
            {
                return new ProductivityPointsByHours()
                {
                    Productive = new List<int>() { 120, 32, 43, 33, 650, 12, 320, 430, 32, 110, 0, 0, 0, 123, 230, 1, 75, 60, 70, 145, 120, 140, 150, 123 },
                    Distractive = new List<int>() { 100, 128, 15, 43, 13, 123, 0, 23, 1, 45, 78, 33, 120, 323, 0, 0, 12, 12, 140, 123, 33, 32, 110, 145, }
                };
            }
        }

        [AuthorizeToken]
        public List<ProductivityByCategoryDTO> GetProductivityByCategoriesForDateDemo(DateTime forDate)
        {
            if (forDate > DateTime.Today)
            {
                return new List<ProductivityByCategoryDTO>()
                {
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Uncategorized",
                        Value = 0,
                    },
                };
            }

            if (forDate == DateTime.Today)
            {
                return new List<ProductivityByCategoryDTO>()
                {
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Software Development",
                        Value = 600,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Social Networks",
                        Value = 300,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Comunications",
                        Value = 250,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Research",
                        Value = 150,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Games",
                        Value = 50,
                    },
                };
            }
            else if (forDate == DateTime.Today.AddDays(-1))
            {
                return new List<ProductivityByCategoryDTO>()
                {
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Games",
                        Value = 650,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Social Networks",
                        Value = 400,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Research",
                        Value = 400,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Comunications",
                        Value = 300,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Software Development",
                        Value = 78,
                    },
                };
            }
            else
            {
                return new List<ProductivityByCategoryDTO>()
                {
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Research",
                        Value = 1000,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Software Development",
                        Value = 500,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Social Networks",
                        Value = 100,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Games",
                        Value = 0,
                    },
                    new ProductivityByCategoryDTO()
                    {
                        Label = "Comunications",
                        Value = 0,
                    },
                    
                };
            }
            throw new NotImplementedException();

        }

        [AuthorizeToken]
        public List<ProductivityByCategoryByHoursDTO> GetProductivityByCategoriesForDateByHoursDemo(DateTime forDate)
        {
            if (forDate > DateTime.Today)
            {
                return new List<ProductivityByCategoryByHoursDTO>()
                {
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Uncategorized",
                        Values = new List<int>(){ 0},
                    },
                };
            }

            if (forDate == DateTime.Today)
            {
                return new List<ProductivityByCategoryByHoursDTO>()
                {
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Software Development",
                        Values = new List<int>(){ 40, 30, 30, 20, 12, 10, 34, 12, 40, 40, 45, 11, 50, 56, 0 },
                    },
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Social Networks",
                        Values = new List<int>(){ 12, 12, 5, 7, 0, 0, 0, 12, 15, 0, 5, 11, 3, 4, 2},
                    },
                     new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Communication",
                        Values = new List<int>(){ 3, 8, 8, 12, 10, 20, 10, 12, 5, 0, 0, 2, 5, 3},
                    },
                };
            }
            else if (forDate == DateTime.Today.AddDays(-1))
            {
                return new List<ProductivityByCategoryByHoursDTO>()
                {
                     new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Social Networks",
                        Values = new List<int>(){ 40, 30, 30, 20, 12, 10, 34, 12, 40, 40, 45, 11, 50, 56, 0, 12, 43, 12, 3, 43, 0, 0, 0,12 },
                    },
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Software Development",
                        Values = new List<int>(){ 15, 12,20, 12, 10, 56, 0, 0, 0,12, 12, 5, 7, 11, 50, 40, 12, 43, 12, 3, 43, 0, 30, 30 },
                    },
                     new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Communication",
                        Values = new List<int>(){ 3, 2,20, 12, 1, 56, 0, 50, 0, 12, 43, 12, 3, 43, 0, 30, 30, 12, 12, 5, 7, 11, 50, 40,},
                    },
                };
            }
            else
            {
                return new List<ProductivityByCategoryByHoursDTO>()
                {
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Communication",
                        Values = new List<int>(){ 30, 12, 20, 12, 10, 56, 0, 40, 0, 12, 13, 12, 3, 43, 0,0, 0, 12,0, 5, 7, 21, 30, 40, 2, 2},
                    },
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Social Networks",
                        Values = new List<int>(){ 12, 20, 12, 10, 56, 0, 0,0, 0, 30, 12,0, 5, 7, 21, 30, 40, 40, 0, 12, 13, 12, 3, 43, 2, 2},
                    },
                     new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Software Development",                        
                        Values = new List<int>(){ 30, 40, 0, 12, 13, 12, 7, 12, 3, 43, 0, 12, 10, 56, 0, 40, 2, 20, 0, 12,0, 5, 21, 30, 20,},
                    },
                };
            }
            throw new NotImplementedException();

        }
    }
}

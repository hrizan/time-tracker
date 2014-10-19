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
            if (forDate == DateTime.Today)
            {
                return new ProductivityPointsByHours()
                {
                    Productive = new List<int>() { 40, 2, 30, 120, 3, 4, 120, 213 },
                    Distractive = new List<int>() { 12, 200, 30, 120, 323, 4, 12, 213 }
                };
            }
            else if (forDate == DateTime.Today.AddDays(-1))
            {
                return new ProductivityPointsByHours()
                {
                    Productive = new List<int>() { 140, 123, 23, 1, 75, 60, 70, 100 },
                    Distractive = new List<int>() { 120, 32, 432, 32, 3, 60, 50, 1 }
                };
            }
            else
            {
                return new ProductivityPointsByHours()
                {
                    Productive = new List<int>() { 12, 233, 302, 23, 12, 43, 16, 23 },
                    Distractive = new List<int>() { 40, 2, 30, 120, 3, 4, 120, 213 }
                };
            }
        }

        [AuthorizeToken]
        public List<ProductivityByCategoryDTO> GetProductivityByCategoriesForDateDemo(DateTime forDate)
        {
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
            if (forDate == DateTime.Today)
            {
                return new List<ProductivityByCategoryByHoursDTO>()
                {
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Software Development",
                        Values = new List<int>(){ 40, 30, 15, 25, 24},
                    },
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Social Networks",
                        Values = new List<int>(){ 10, 20, 20, 10, 15},
                    },
                     new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Communication",
                        Values = new List<int>(){ 10, 10, 15, 25, 10},
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
                        Values = new List<int>(){ 10, 20, 20, 10, 15},
                    },
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Software Development",
                        Values = new List<int>(){ 40, 30, 15, 25, 24},
                    },
                     new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Communication",
                        Values = new List<int>(){ 10, 10, 15, 25, 10},
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
                        Values = new List<int>(){ 40, 30, 15, 25, 24},
                    },
                    new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Social Networks",
                        Values = new List<int>(){ 10, 20, 20, 10, 15},
                    },
                     new ProductivityByCategoryByHoursDTO()
                    {
                        Label = "Software Development",                        
                        Values = new List<int>(){ 10, 10, 15, 25, 10},
                    },
                };
            }
            throw new NotImplementedException();

        }
    }
}

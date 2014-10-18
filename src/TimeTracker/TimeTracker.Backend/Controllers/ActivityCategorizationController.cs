using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TimeTracker.Backend.Filters;
using TimeTracker.Backend.Models;

namespace TimeTracker.Backend.Controllers
{
    public class ActivityCategorizationController : ApiControllerBase
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/ActivityCategorization
        [AuthorizeToken]
        public IEnumerable<ActivityCategorization> GetActivityCategorizations()
        {
            var activitycategorizations = db.ActivityCategorizations.Include(a => a.Category);
            return activitycategorizations.AsEnumerable();
        }

        [AuthorizeToken]
        public IEnumerable<ActivityGroupsCategorizationDTO> GetActivityCategorizationsGrouped()
        {
            var withCategory= 
                db.ActivityCategorizations
                .Select(s => s.Category)
                .SelectMany(sm => sm.ActivityCategorizations)
                .Select(s => new ActivityGroupsCategorizationDTO() 
                {
                    ActivityCategorization = s,
                    Category = s.Category,
                    ProcessName = s.ProcessName,
                    ProductivityScore = s.ProductivityScore,
                    Resource = s.Resource,                    
                })
                .ToList();

            var withoutCategory =
                db.Activities
                .Where(w => w.CategoryId == null)
                .GroupBy(gb => new { gb.ProcessName, gb.Resource})
                .Select(s => new ActivityGroupsCategorizationDTO()
                {
                    ActivityCategorization = null,
                    Category = null,
                    ProcessName = s.Key.ProcessName,
                    Resource = s.Key.Resource,
                    ProductivityScore = 0, //Neutral value
                })
                .ToList();
            List<ActivityGroupsCategorizationDTO> result = new List<ActivityGroupsCategorizationDTO>();
            result.AddRange(withoutCategory);
            result.AddRange(withCategory);

            return result;
        }

        // GET api/ActivityCategorization/5
        [AuthorizeToken]
        public ActivityCategorization GetActivityCategorization(Guid id)
        {
            ActivityCategorization activitycategorization = db.ActivityCategorizations.Find(id);
            if (activitycategorization == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return activitycategorization;
        }

        // PUT api/ActivityCategorization/5
        [AuthorizeToken]
        public HttpResponseMessage PutActivityCategorization(Guid id, ActivityCategorization activitycategorization)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != activitycategorization.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(activitycategorization).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/ActivityCategorization
        [AuthorizeToken]
        public HttpResponseMessage PostActivityCategorization(ActivityCategorization activitycategorization)
        {
            if (ModelState.IsValid)
            {
                db.ActivityCategorizations.Add(activitycategorization);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, activitycategorization);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = activitycategorization.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/ActivityCategorization/5
        [AuthorizeToken]
        public HttpResponseMessage DeleteActivityCategorization(Guid id)
        {
            ActivityCategorization activitycategorization = db.ActivityCategorizations.Find(id);
            if (activitycategorization == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.ActivityCategorizations.Remove(activitycategorization);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, activitycategorization);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
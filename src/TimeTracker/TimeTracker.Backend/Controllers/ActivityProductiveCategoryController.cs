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
using TimeTracker.Backend.Models;

namespace TimeTracker.Backend.Controllers
{
    public class ActivityProductiveCategoryController : ApiController
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/ActivityProductiveCategory
        public IEnumerable<ActivityProductiveCategory> GetActivityProductiveCategories()
        {
            return db.ActivityProductiveCategories.AsEnumerable();
        }

        // GET api/ActivityProductiveCategory/5
        public ActivityProductiveCategory GetActivityProductiveCategory(Guid id)
        {
            ActivityProductiveCategory activityproductivecategory = db.ActivityProductiveCategories.Find(id);
            if (activityproductivecategory == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return activityproductivecategory;
        }

        // PUT api/ActivityProductiveCategory/5
        public HttpResponseMessage PutActivityProductiveCategory(Guid id, ActivityProductiveCategory activityproductivecategory)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != activityproductivecategory.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(activityproductivecategory).State = EntityState.Modified;

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

        // POST api/ActivityProductiveCategory
        public HttpResponseMessage PostActivityProductiveCategory(ActivityProductiveCategory activityproductivecategory)
        {
            if (ModelState.IsValid)
            {
                db.ActivityProductiveCategories.Add(activityproductivecategory);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, activityproductivecategory);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = activityproductivecategory.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/ActivityProductiveCategory/5
        public HttpResponseMessage DeleteActivityProductiveCategory(Guid id)
        {
            ActivityProductiveCategory activityproductivecategory = db.ActivityProductiveCategories.Find(id);
            if (activityproductivecategory == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.ActivityProductiveCategories.Remove(activityproductivecategory);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, activityproductivecategory);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
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
    public class ActivitiesController : ApiController
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/Activities
        public IEnumerable<Activity> GetActivities()
        {
            var activities = db.Activities.Include(a => a.Consumer).Include(a => a.Device).Include(a => a.Category);
            return activities.AsEnumerable();
        }

        public int GetAllActivitiesCount()
        {
            var activities = db.Activities;
            return activities.Count();
        }

        // GET api/Activities/5
        public Activity GetActivity(Guid id)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return activity;
        }

        // PUT api/Activities/5
        public HttpResponseMessage PutActivity(Guid id, Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != activity.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(activity).State = EntityState.Modified;

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

        // POST api/Activities
        public HttpResponseMessage PostActivity(Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Activities.Add(activity);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, activity);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = activity.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Activities/5
        public HttpResponseMessage DeleteActivity(Guid id)
        {
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Activities.Remove(activity);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, activity);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
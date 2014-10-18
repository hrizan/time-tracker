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
using TimeTracker.Models;

namespace TimeTracker.Backend.Controllers
{
    public class ActivitiesController : ApiControllerBase
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/Activities
        [AuthorizeToken]
        public IEnumerable<Activity> GetAllActivities()
        {
            Guid consumerId = CurrentUserConsumerId.Value;
            var activities = db.Activities.AsQueryable().ForConsumer(consumerId);

            return activities.AsEnumerable();
        }

        
        public int GetAllActivitiesCount()
        {
            var activities = db.Activities;
            return activities.Count();
        }

        // GET api/Activities/5
        [AuthorizeToken]
        public Activity GetActivity(Guid id)
        {
            Guid consumerId = CurrentUserConsumerId.Value;
            Activity activity = db.Activities.ForConsumer(consumerId).SingleOrDefault(a => a.Id == id);

            if (activity == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return activity;
        }

        // PUT api/Activities/5
        //[AuthorizeToken]
        //public HttpResponseMessage PutActivity(Guid id, Activity activity)
        //{
        //    Guid consumerId = CurrentUserConsumerId.Value;
            


        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    if (id != activity.Id)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //    db.Entry(activity).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        // POST api/Activities
        [AuthorizeToken]
        public HttpResponseMessage PostActivity(ActivityUpdateDto activityDto)
        {
            Guid consumerId = CurrentUserConsumerId.Value;
            
            if (ModelState.IsValid)
            {
                var activity = new Activity();
                AutoMapper.Mapper.Map(activityDto, activity);

                //system data updates
                activity.ConsumerId = consumerId;

                var categorization = GetActivityCategorization(consumerId, activity.ProcessName, activity.Resource);
                if (categorization != null)
                {
                    activity.ProductivityScore = categorization.ProductivityScore;
                    activity.CategoryId = categorization.CategoryId;
                }
                else
                {
                    activity.ProductivityScore = (int)ProductivityTypeEnum.Neutral;
                    activity.CategoryId = null;
                }


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

        private ActivityCategorization GetActivityCategorization(Guid consumerId, string processName, string resource)
        {
            var productivityScore = db.GetActivityCategorization(consumerId, processName, resource);

            return productivityScore;
        }


        // DELETE api/Activities/5
        [AuthorizeToken]
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
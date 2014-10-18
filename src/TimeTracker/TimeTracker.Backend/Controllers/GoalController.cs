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
    public class GoalController : ApiControllerBase
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/Goal
        [AuthorizeToken]
        public IEnumerable<Goal> GetGoals()
        {
            Guid consumerId = CurrentUserConsumerId.Value;
            var goals = db.Goals.ForConsumer(consumerId);
            return goals.AsEnumerable();
        }

        // GET api/Goal/5
        [AuthorizeToken]
        public Goal GetGoal(Guid id)
        {
            Guid consumerId = CurrentUserConsumerId.Value;

            Goal goal = db.Goals.ForConsumer(consumerId).SingleOrDefault();
            if (goal == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return goal;
        }

        //// PUT api/Goal/5
        //[AuthorizeToken]
        //public HttpResponseMessage PutGoal(Guid id, Goal goal)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    if (id != goal.Id)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //    db.Entry(goal).State = EntityState.Modified;

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

        // POST api/Goal
        [AuthorizeToken]
        public HttpResponseMessage PostGoal(GoalUpdateDTO goalDTO)
        {
            Guid consumerId = CurrentUserConsumerId.Value;

            if (ModelState.IsValid)
            {
                var goal = new Goal();
                AutoMapper.Mapper.Map(goalDTO, goal);
 
                goal.ConsumerId = consumerId;

                goal.Category = db.Categories.SingleOrDefault(c => c.Id == goalDTO.CategoryId);
                if (goal.Category != null)
                {
                    goal.CategoryId = goal.Category.Id;
                }

                if (db.Goals.Count(s => s.Id == goal.Id) != 0)
                {
                    db.Entry(goal).State = EntityState.Modified;
                }
                else
                {
                    db.Goals.Add(goal);
                }
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, goal);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = goal.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Goal/5
        [AuthorizeToken]
        public HttpResponseMessage DeleteGoal(Guid id)
        {
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Goals.Remove(goal);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, goal);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
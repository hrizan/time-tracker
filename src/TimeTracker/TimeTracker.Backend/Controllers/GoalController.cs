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
    public class GoalController : ApiController
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/Goal
        public IEnumerable<Goal> GetGoals()
        {
            var goals = db.Goals.Include(g => g.Consumer).Include(g => g.Category);
            return goals.AsEnumerable();
        }

        // GET api/Goal/5
        public Goal GetGoal(Guid id)
        {
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return goal;
        }

        // PUT api/Goal/5
        public HttpResponseMessage PutGoal(Guid id, Goal goal)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != goal.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(goal).State = EntityState.Modified;

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

        // POST api/Goal
        public HttpResponseMessage PostGoal(Goal goal)
        {
            if (ModelState.IsValid)
            {
                db.Goals.Add(goal);
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
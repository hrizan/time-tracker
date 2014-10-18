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
    public class ConsumerController : ApiController
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/Consumer
        public IEnumerable<Consumer> GetConsumers()
        {
            return db.Consumers.AsEnumerable();
        }

        // GET api/Consumer/5
        public Consumer GetConsumer(Guid id)
        {
            Consumer consumer = db.Consumers.Find(id);
            if (consumer == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return consumer;
        }

        // PUT api/Consumer/5
        public HttpResponseMessage PutConsumer(Guid id, Consumer consumer)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != consumer.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(consumer).State = EntityState.Modified;

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

        // POST api/Consumer
        public HttpResponseMessage PostConsumer(Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                db.Consumers.Add(consumer);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, consumer);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = consumer.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Consumer/5
        public HttpResponseMessage DeleteConsumer(Guid id)
        {
            Consumer consumer = db.Consumers.Find(id);
            if (consumer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Consumers.Remove(consumer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, consumer);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
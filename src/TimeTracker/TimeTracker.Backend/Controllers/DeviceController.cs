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
    public class DeviceController : ApiController
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/Device
        public IEnumerable<Device> GetDevices()
        {
            var devices = db.Devices.Include(d => d.Consumer);
            return devices.AsEnumerable();
        }

        // GET api/Device/5
        public Device GetDevice(Guid id)
        {
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return device;
        }

        // PUT api/Device/5
        public HttpResponseMessage PutDevice(Guid id, Device device)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != device.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(device).State = EntityState.Modified;

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

        // POST api/Device
        public HttpResponseMessage PostDevice(Device device)
        {
            if (ModelState.IsValid)
            {
                db.Devices.Add(device);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, device);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = device.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Device/5
        public HttpResponseMessage DeleteDevice(Guid id)
        {
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Devices.Remove(device);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, device);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
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
    public class DeviceController : ApiControllerBase
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/Device
        [AuthorizeToken]
        public IEnumerable<Device> GetDevices()
        {
            Guid user_id = CurrentUserConsumerId.Value;
            var devices = db.Devices.ForConsumer(user_id).ToList();

            return devices;
        }

        // GET api/Device/5
        [AuthorizeToken]
        public Device GetDevice(Guid id)
        {
            Guid user_id = CurrentUserConsumerId.Value;

            Device device = db.Devices.ForConsumer(user_id).SingleOrDefault(sd => sd.Id == id);
            if (device == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return device;
        }

        // PUT api/Device/5
        [AuthorizeToken]
        public HttpResponseMessage PutDevice(Guid id, DeviceUpdateDTO deviceDto)
        {
            Guid user_id = CurrentUserConsumerId.Value;

            Device device = new Device();
            AutoMapper.Mapper.Map(deviceDto, device);

            device.ConsumerId = user_id;

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
        [AuthorizeToken]
        public HttpResponseMessage PostDevice(DeviceUpdateDTO devicedto)
        {
            Guid user_id = CurrentUserConsumerId.Value;

            if (ModelState.IsValid)
            {
                Device device = new Device();
                AutoMapper.Mapper.Map(devicedto, device);

                device.ConsumerId = user_id;

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
        [AuthorizeToken]
        public HttpResponseMessage DeleteDevice(Guid id)
        {
            Guid user_id = CurrentUserConsumerId.Value;

            Device device = db.Devices.ForConsumer(user_id).SingleOrDefault(sd => sd.Id == id);

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
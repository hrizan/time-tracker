﻿using System;
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
    public class CategoryController : ApiControllerBase
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        // GET api/Category
        [AuthorizeToken]
        public IEnumerable<Category> GetCategories()
        {
            Guid consumerId = CurrentUserConsumerId.Value;

            var categories = db.Categories.ForConsumer(consumerId);
            return categories.AsEnumerable();
        }

        // GET api/Category/5
        [AuthorizeToken]
        public Category GetCategory(Guid id)
        {
            Guid consumerId = CurrentUserConsumerId.Value;

            Category category = db.Categories
                .ForConsumer(consumerId)
                .SingleOrDefault(s => s.Id == id);
            if (category == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return category;
        }

        // PUT api/Category/5
        [AuthorizeToken]
        public HttpResponseMessage PutCategory(Guid id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != category.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(category).State = EntityState.Modified;

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

        // POST api/Category
        [AuthorizeToken]
        public HttpResponseMessage PostCategory(CategoryUpdateDTO categoryDto)
        {
            Guid consumerId = CurrentUserConsumerId.Value;

            if (ModelState.IsValid)
            {
                var category = new Category();
                AutoMapper.Mapper.Map(categoryDto, category);

                category.ConsumerId = consumerId;
                category.Consumer = db.Consumers.SingleOrDefault(s => s.Id == consumerId);

                db.Categories.Add(category);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, category);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = category.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Category/5
        [AuthorizeToken]
        public HttpResponseMessage DeleteCategory(Guid id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Categories.Remove(category);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, category);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
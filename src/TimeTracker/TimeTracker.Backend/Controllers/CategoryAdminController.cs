using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeTracker.Backend.Models;

namespace TimeTracker.Backend.Controllers
{
    public class CategoryAdminController : Controller
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        //
        // GET: /CategoryAdmin/

        public ActionResult Index()
        {
            var categories = db.Categories.Include(c => c.Consumer);
            return View(categories.ToList());
        }

        //
        // GET: /CategoryAdmin/Details/5

        public ActionResult Details(Guid id )
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // GET: /CategoryAdmin/Create

        public ActionResult Create()
        {
            ViewBag.ConsumerId = new SelectList(db.Consumers, "Id", "Name");
            return View();
        }

        //
        // POST: /CategoryAdmin/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Id = Guid.NewGuid();
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ConsumerId = new SelectList(db.Consumers, "Id", "Name", category.ConsumerId);
            return View(category);
        }

        //
        // GET: /CategoryAdmin/Edit/5

        public ActionResult Edit(Guid id )
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConsumerId = new SelectList(db.Consumers, "Id", "Name", category.ConsumerId);
            return View(category);
        }

        //
        // POST: /CategoryAdmin/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConsumerId = new SelectList(db.Consumers, "Id", "Name", category.ConsumerId);
            return View(category);
        }

        //
        // GET: /CategoryAdmin/Delete/5

        public ActionResult Delete(Guid id )
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /CategoryAdmin/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
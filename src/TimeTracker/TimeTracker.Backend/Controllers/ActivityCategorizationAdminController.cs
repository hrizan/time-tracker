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
    public class ActivityCategorizationAdminController : Controller
    {
        private TimeTrackerContext db = new TimeTrackerContext();

        //
        // GET: /ActivityCategorizationAdmin/

        public ActionResult Index()
        {
            var activitycategorizations = db.ActivityCategorizations.Include(a => a.Consumer).Include(a => a.Category);
            return View(activitycategorizations.ToList());
        }

        //
        // GET: /ActivityCategorizationAdmin/Details/5

        public ActionResult Details(Guid id)
        {
            ActivityCategorization activitycategorization = db.ActivityCategorizations.Find(id);
            if (activitycategorization == null)
            {
                return HttpNotFound();
            }
            return View(activitycategorization);
        }

        //
        // GET: /ActivityCategorizationAdmin/Create

        public ActionResult Create()
        {
            ViewBag.ConsumerId = new SelectList(db.Consumers, "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        }

        //
        // POST: /ActivityCategorizationAdmin/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ActivityCategorization activitycategorization)
        {
            if (ModelState.IsValid)
            {
                activitycategorization.Id = Guid.NewGuid();
                db.ActivityCategorizations.Add(activitycategorization);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ConsumerId = new SelectList(db.Consumers, "Id", "Name", activitycategorization.ConsumerId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", activitycategorization.CategoryId);
            return View(activitycategorization);
        }

        //
        // GET: /ActivityCategorizationAdmin/Edit/5

        public ActionResult Edit(Guid id)
        {
            ActivityCategorization activitycategorization = db.ActivityCategorizations.Find(id);
            if (activitycategorization == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConsumerId = new SelectList(db.Consumers, "Id", "Name", activitycategorization.ConsumerId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", activitycategorization.CategoryId);
            return View(activitycategorization);
        }

        //
        // POST: /ActivityCategorizationAdmin/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ActivityCategorization activitycategorization)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activitycategorization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConsumerId = new SelectList(db.Consumers, "Id", "Name", activitycategorization.ConsumerId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", activitycategorization.CategoryId);
            return View(activitycategorization);
        }

        //
        // GET: /ActivityCategorizationAdmin/Delete/5

        public ActionResult Delete(Guid id)
        {
            ActivityCategorization activitycategorization = db.ActivityCategorizations.Find(id);
            if (activitycategorization == null)
            {
                return HttpNotFound();
            }
            return View(activitycategorization);
        }

        //
        // POST: /ActivityCategorizationAdmin/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            ActivityCategorization activitycategorization = db.ActivityCategorizations.Find(id);
            db.ActivityCategorizations.Remove(activitycategorization);
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
﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LoadDist.Models;
using LoadDist.Models.DataModels;
using LoadDist.Models.ViewModels;

namespace LoadDist.Controllers
{
    public class StreamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Streams
        public async Task<ActionResult> Index()
        {
            return View(await db.Streams.ToListAsync());
        }

        // GET: Streams/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stream stream = await db.Streams.Include(s => s.Groups).FirstOrDefaultAsync(s => s.Id == id);
            if (stream == null)
            {
                return HttpNotFound();
            }
            return View(stream);
        }

        // GET: Streams/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Streams/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title")] Stream stream)
        {
            if (ModelState.IsValid)
            {
                db.Streams.Add(stream);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(stream);
        }

        // GET: Streams/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stream stream = await db.Streams.FindAsync(id);
            if (stream == null)
            {
                return HttpNotFound();
            }
            return View(stream);
        }

        // POST: Streams/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title")] Stream stream)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stream).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(stream);
        }

        // GET: Streams/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stream stream = await db.Streams.FindAsync(id);
            if (stream == null)
            {
                return HttpNotFound();
            }
            return View(stream);
        }

        // POST: Streams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Stream stream = await db.Streams.FindAsync(id);
            db.Streams.Remove(stream);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Lecturers/Create
        public ActionResult AddGroup(int? id)
        {
            Stream stream = db.Streams.Include(s => s.Groups).SingleOrDefault(s => s.Id == id);
            var allGroups = db.Groups.ToList();
            var groupsToAdd = allGroups.Except(stream.Groups);
            SelectList groups = new SelectList(groupsToAdd, "Id", "GroupNumber");
            ViewBag.Groups = groups;
            return View(new AddGroupViewModel { StreamId = stream.Id });
        }

        // POST: Lecturers/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddGroup(AddGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                Stream thisStream = db.Streams.Find(model.StreamId);
                Group thisGroup = db.Groups.Find(model.GroupId);
                if (thisStream.Groups != null)
                {
                    thisStream.Groups.Add(thisGroup);
                }
                thisStream.Groups = new List<Group> { thisGroup };
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = thisStream.Id });
            }
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

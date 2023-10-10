using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Lifetrons.Erp.Controllers;
using Lifetrons.Erp.Data;

namespace Lifetrons.Erp.People.Controllers
{
    public class AccountTypesController : BaseController
    {
        private Entities db = new Entities();

        // GET: AccountTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.AccountTypes.ToListAsync());
        }

        // GET: AccountTypes/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountType accountType = await db.AccountTypes.FindAsync(id);
            if (accountType == null)
            {
                return HttpNotFound();
            }
            return View(accountType);
        }

        // GET: AccountTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Code,ShrtDesc,Authorized,Active")] AccountType accountType)
        {
            if (ModelState.IsValid)
            {
                accountType.Id = Guid.NewGuid();
                db.AccountTypes.Add(accountType);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(accountType);
        }

        // GET: AccountTypes/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountType accountType = await db.AccountTypes.FindAsync(id);
            if (accountType == null)
            {
                return HttpNotFound();
            }
            return View(accountType);
        }

        // POST: AccountTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Code,ShrtDesc,Authorized,Active")] AccountType accountType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(accountType);
        }

        // GET: AccountTypes/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountType accountType = await db.AccountTypes.FindAsync(id);
            if (accountType == null)
            {
                return HttpNotFound();
            }
            return View(accountType);
        }

        // POST: AccountTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            AccountType accountType = await db.AccountTypes.FindAsync(id);
            db.AccountTypes.Remove(accountType);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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

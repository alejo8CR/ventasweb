using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ventasP2Web.Models;

namespace ventasP2Web.Controllers
{
    public class cuentasPorCobrarController : Controller
    {
        private ventasBDEntities1 db = new ventasBDEntities1();

        // GET: cuentasPorCobrar
        public ActionResult Index()
        {
            var cuentaPorCobrar = db.cuentaPorCobrar.Include(c => c.factura).OrderBy(c=>c.facturaID);
            return View(cuentaPorCobrar.ToList());
        }

        public ActionResult imprimirIndex()
        {
            return new ActionAsPdf("Index") { FileName = "cuentasporcobrar.pdf" };
        }

        // GET: cuentasPorCobrar/Details/5
        public ActionResult Details(int? id, int? idfactura)
        {
            cuentaPorCobrar cuentaPorCobrar;
            if (idfactura == null)
                cuentaPorCobrar = db.cuentaPorCobrar.Find(id);
            else
                cuentaPorCobrar = db.cuentaPorCobrar.Where(c => c.facturaID == idfactura).First();

            if (cuentaPorCobrar == null)
            {
                return HttpNotFound();
            }
            return View(cuentaPorCobrar);
        }

        // GET: cuentasPorCobrar/Create
        public ActionResult Create()
        {
            ViewBag.facturaID = new SelectList(db.factura, "facturaID", "facturaID");
            return View();
        }

        // POST: cuentasPorCobrar/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cuentaPorCobrar1,facturaID,totalImpuesto,totalAPagar,totalPagado")] cuentaPorCobrar cuentaPorCobrar)
        {
            if (ModelState.IsValid)
            {
                db.cuentaPorCobrar.Add(cuentaPorCobrar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.facturaID = new SelectList(db.factura, "facturaID", "facturaID", cuentaPorCobrar.facturaID);
            return View(cuentaPorCobrar);
        }

        // GET: cuentasPorCobrar/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cuentaPorCobrar cuentaPorCobrar = db.cuentaPorCobrar.Find(id);
            if (cuentaPorCobrar == null)
            {
                return HttpNotFound();
            }
            ViewBag.facturaID = new SelectList(db.factura, "facturaID", "facturaID", cuentaPorCobrar.facturaID);
            return View(cuentaPorCobrar);
        }

        // POST: cuentasPorCobrar/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "cuentaPorCobrar1,facturaID,totalImpuesto,totalAPagar,totalPagado")] cuentaPorCobrar cuentaPorCobrar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentaPorCobrar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.facturaID = new SelectList(db.factura, "facturaID", "facturaID", cuentaPorCobrar.facturaID);
            return View(cuentaPorCobrar);
        }

        // GET: cuentasPorCobrar/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cuentaPorCobrar cuentaPorCobrar = db.cuentaPorCobrar.Find(id);
            if (cuentaPorCobrar == null)
            {
                return HttpNotFound();
            }
            return View(cuentaPorCobrar);
        }

        // POST: cuentasPorCobrar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cuentaPorCobrar cuentaPorCobrar = db.cuentaPorCobrar.Find(id);
            db.cuentaPorCobrar.Remove(cuentaPorCobrar);
            db.SaveChanges();
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

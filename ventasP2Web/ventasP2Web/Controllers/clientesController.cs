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
    public class clientesController : Controller
    {
        private ventasBDEntities1 db = new ventasBDEntities1();

        // GET: clientes
        public ActionResult Index()
        {
            var cliente = db.cliente.Include(c => c.metodoPago).OrderBy(c=>c.nombre);
            return View(cliente.ToList());
        }

        public ActionResult imprimirIndex()
        {
            return new ActionAsPdf("Index") { FileName = "clientes.pdf" };
        }

        // GET: clientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cliente cliente = db.cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // GET: clientes/Create
        public ActionResult Create()
        {
            ViewBag.metodoPagoID = new SelectList(db.metodoPago, "metodoPagoID", "condicion");
            return View();
        }

        // POST: clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "clienteID,metodoPagoID,nombre,pais,direccion,codigoPostal,telefono,limiteCredito,direccionFisica,correo,fechaInicio")] cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.fechaInicio = DateTime.Now;
                db.cliente.Add(cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.metodoPagoID = new SelectList(db.metodoPago, "metodoPagoID", "condicion", cliente.metodoPagoID);
            return View(cliente);
        }

        // GET: clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cliente cliente = db.cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.metodoPagoID = new SelectList(db.metodoPago, "metodoPagoID", "condicion", cliente.metodoPagoID);
            return View(cliente);
        }

        // POST: clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "clienteID,metodoPagoID,nombre,pais,direccion,codigoPostal,telefono,limiteCredito,direccionFisica,correo,fechaInicio")] cliente cliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.metodoPagoID = new SelectList(db.metodoPago, "metodoPagoID", "condicion", cliente.metodoPagoID);
            return View(cliente);
        }

        // GET: clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cliente cliente = db.cliente.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var pedidos = db.pedido.Where(p => p.clienteID == id);
            if (pedidos != null)
            {
                cliente e = db.cliente.Find(id);
                ViewData["error"] = "Este cliente tiene pedidos relacionados";
                return View("delete", e);
            }

            cliente cliente = db.cliente.Find(id);
            db.cliente.Remove(cliente);
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

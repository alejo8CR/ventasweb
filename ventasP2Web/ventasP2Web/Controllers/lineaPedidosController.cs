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
    public class lineaPedidosController : Controller
    {
        private ventasBDEntities1 db = new ventasBDEntities1();

        // GET: lineaPedidos
        public ActionResult Index()
        {
            var lineaPedido = db.lineaPedido.Include(l => l.pedido).Include(l => l.producto).OrderBy(l =>l.pedidoID);
            return View(lineaPedido.ToList());
        }

        // GET: lineaPedidos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaPedido lineaPedido = db.lineaPedido.Find(id);
            if (lineaPedido == null)
            {
                return HttpNotFound();
            }
            return View(lineaPedido);
        }

        public ActionResult Details2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaPedido lineaPedido = db.lineaPedido.Find(id);
            if (lineaPedido == null)
            {
                return HttpNotFound();
            }
            return View(lineaPedido);
        }

        // GET: lineaPedidos/Create
        public ActionResult Create(string pedido)
        {
            ViewBag.pedidoID = new SelectList(db.pedido, "pedidoID", "pedidoID", pedido);
            ViewBag.productoID = new SelectList(db.producto, "SKU", "descripcion");
            return View();
        }

        // POST: lineaPedidos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "lineaPedidoID,pedidoID,productoID,cantidad,precioVenta,descuento,impuesto,precioTotal")] lineaPedido lineaPedido)
        {
            try
            {
                var pd = db.producto.Where(a => a.SKU == lineaPedido.productoID).FirstOrDefault();
                if (pd.stock<lineaPedido.cantidad)
                    ModelState.AddModelError("cantidad", "Cantidad supera el stock");
            }
            catch (Exception e) { }

            if (ModelState.IsValid)
            {
                double preciov = 0, impuesto = 0, preciot = 0, descuento = 0;
                int cantidad = (int)lineaPedido.cantidad;
                try
                {
                    var p = db.producto.Where(a => a.SKU==lineaPedido.productoID).FirstOrDefault();
                    preciov = (double)p.precioVenta;
                    impuesto = (double)p.impuesto;
                    descuento = cantidad * ((preciov * (double)lineaPedido.descuento) / 100);
                    preciot = (cantidad * preciov) + (cantidad * ((preciov * impuesto) / 100)) - descuento;
                }
                catch (Exception e) { }

                lineaPedido.precioVenta = preciov;
                lineaPedido.impuesto= impuesto;
                lineaPedido.precioTotal = preciot;

                db.lineaPedido.Add(lineaPedido);
                db.SaveChanges();

                //Modifica stock producto
                producto pdr = db.producto.Where(a => a.SKU == lineaPedido.productoID).FirstOrDefault();
                pdr.stock = pdr.stock - cantidad;
                db.Entry(pdr).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index","pedidos","");
            }

            ViewBag.pedidoID = new SelectList(db.pedido, "pedidoID", "pedidoID", lineaPedido.pedidoID);
            ViewBag.productoID = new SelectList(db.producto, "SKU", "descripcion", lineaPedido.productoID);
            return View(lineaPedido);
        }

        // GET: lineaPedidos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaPedido lineaPedido = db.lineaPedido.Find(id);
            if (lineaPedido == null)
            {
                return HttpNotFound();
            }
            ViewBag.pedidoID = new SelectList(db.pedido, "pedidoID", "pedidoID", lineaPedido.pedidoID);
            ViewBag.productoID = new SelectList(db.producto, "SKU", "descripcion", lineaPedido.productoID);
            return View(lineaPedido);
        }

        // POST: lineaPedidos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "lineaPedidoID,pedidoID,productoID,cantidad,precioVenta,descuento,impuesto,precioTotal")] lineaPedido lineaPedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lineaPedido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.pedidoID = new SelectList(db.pedido, "pedidoID", "pedidoID", lineaPedido.pedidoID);
            ViewBag.productoID = new SelectList(db.producto, "SKU", "descripcion", lineaPedido.productoID);
            return View(lineaPedido);
        }

        // GET: lineaPedidos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaPedido lineaPedido = db.lineaPedido.Find(id);
            if (lineaPedido == null)
            {
                return HttpNotFound();
            }
            return View(lineaPedido);
        }

        public ActionResult Delete2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaPedido lineaPedido = db.lineaPedido.Find(id);
            if (lineaPedido == null)
            {
                return HttpNotFound();
            }
            return View(lineaPedido);
        }

        // POST: lineaPedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var a = db.lineaFactura.Where(p => p.lineaPedidoID == id);
            if (a != null)
            {
                lineaPedido e = db.lineaPedido.Find(id);
                ViewData["error"] = "Esta linea pedido tiene facturas relacionadas";
                return View("delete", e);
            }

            lineaPedido lineaPedido = db.lineaPedido.Find(id);
            db.lineaPedido.Remove(lineaPedido);
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

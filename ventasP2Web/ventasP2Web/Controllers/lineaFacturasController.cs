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
    public class lineaFacturasController : Controller
    {
        private ventasBDEntities1 db = new ventasBDEntities1();

        // GET: lineaFacturas
        public ActionResult Index()
        {
            var lineaFactura = db.lineaFactura.Include(l => l.factura).Include(l => l.lineaPedido);
            return View(lineaFactura.ToList());
        }

        // GET: lineaFacturas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaFactura lineaFactura = db.lineaFactura.Find(id);
            if (lineaFactura == null)
            {
                return HttpNotFound();
            }
            return View(lineaFactura);
        }

        // GET: lineaFacturas/Details/5
        public ActionResult Details2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaFactura lineaFactura = db.lineaFactura.Find(id);
            if (lineaFactura == null)
            {
                return HttpNotFound();
            }
            return View(lineaFactura);
        }

        // GET: lineaFacturas/Create
        public ActionResult Create()
        {
            ViewBag.facturaID = new SelectList(db.factura, "facturaID", "facturaID");
            ViewBag.lineaPedidoID = new SelectList(db.lineaPedido.Where(p => p.pedido.estado == "CONFIRMADO"), "lineaPedidoID", "lineaPedidoID");
            return View();
        }

        // POST: lineaFacturas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "facturaID,lineaPedidoID,cantidadFacturada,descripcion,subtotal,totalDescuento,totalImpuesto,totalPagar")] lineaFactura lineaFactura)
        {
            try
            {
                var lp1 = db.lineaPedido.Where(a => a.lineaPedidoID == lineaFactura.lineaPedidoID).FirstOrDefault();
                if (lp1.cantidad < lineaFactura.cantidadFacturada)
                    ModelState.AddModelError("cantidadFacturada", "Cantidad a facturar supera la cantidad del pedido");
            }
            catch (Exception e) { }
            if(lineaFactura.lineaPedidoID==0)
                ModelState.AddModelError("lineaPedidoID", "No productos confirmados para facturar");

            if (ModelState.IsValid)
            {
                double preciov=0, sub = 0, totald = 0, totali = 0, totalp = 0;
                int cantidad = (int)lineaFactura.cantidadFacturada;
                try
                {
                    var lp2 = db.lineaPedido.Where(a => a.lineaPedidoID == lineaFactura.lineaPedidoID).FirstOrDefault();
                    preciov = (double)lp2.precioVenta;
                    sub = preciov * cantidad;
                    totald = cantidad * (((double)lp2.descuento) * preciov / 100);
                    totali = cantidad * (((double)lp2.impuesto) * preciov / 100);
                    totalp = sub+totali-totald;
                }
                catch (Exception e) { }

                lineaFactura.subtotal = sub;
                lineaFactura.totalDescuento = totald;
                lineaFactura.totalImpuesto = totali;
                lineaFactura.totalPagar = totalp;

                db.lineaFactura.Add(lineaFactura);
                db.SaveChanges();

                //Se actualiza una cuenta por cobrar
                try
                {
                    double totalapagar = 0, totalimpuestos=0,totalpagado=0;
                    var cuenta = db.cuentaPorCobrar.Where(x => x.facturaID == lineaFactura.facturaID).First();
                    var totalfa = db.lineaFactura.Where(l => l.facturaID == lineaFactura.facturaID).Sum(l => l.totalPagar);

                    var lpedidos = db.lineaFactura.Where(l => l.facturaID == lineaFactura.facturaID).Select(l => l.lineaPedidoID).ToArray();
                    var pedidosID = db.lineaPedido.Where(s => lpedidos.Contains(s.lineaPedidoID)).Select(s => s.pedidoID).ToArray();
                    var pedidos = db.lineaPedido.Where(s => pedidosID.Contains(s.pedidoID)).Sum(s => s.precioTotal);

                    var totalim = db.lineaPedido.Where(s => pedidosID.Contains(s.pedidoID));
                    if (totalim != null)
                    {
                        foreach(lineaPedido linea in totalim)
                        {
                            totalimpuestos += ((double)linea.impuesto * (double)linea.precioTotal / 100);
                        }
                    }
                    totalpagado = (double)totalfa;
                    if (pedidos != null) { 
                        totalapagar = (double)pedidos;
                    }
                    cuenta.totalPagado = totalpagado;
                    cuenta.totalImpuesto = totalimpuestos;
                    cuenta.totalAPagar = totalapagar;
                    
                    db.Entry(cuenta).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e) { }

                return RedirectToAction("Index","facturas","");
            }

            ViewBag.facturaID = new SelectList(db.factura, "facturaID", "facturaID", lineaFactura.facturaID);
            ViewBag.lineaPedidoID = new SelectList(db.lineaPedido, "lineaPedidoID", "lineaPedidoID", lineaFactura.lineaPedidoID);
            return View(lineaFactura);
        }

        // GET: lineaFacturas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaFactura lineaFactura = db.lineaFactura.Find(id);
            if (lineaFactura == null)
            {
                return HttpNotFound();
            }
            ViewBag.facturaID = new SelectList(db.factura, "facturaID", "facturaID", lineaFactura.facturaID);
            ViewBag.lineaPedidoID = new SelectList(db.lineaPedido, "lineaPedidoID", "lineaPedidoID", lineaFactura.lineaPedidoID);
            return View(lineaFactura);
        }

        // POST: lineaFacturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "facturaID,lineaPedidoID,cantidadFacturada,descripcion,subtotal,totalDescuento,totalImpuesto,totalPagar")] lineaFactura lineaFactura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lineaFactura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.facturaID = new SelectList(db.factura, "facturaID", "facturaID", lineaFactura.facturaID);
            ViewBag.lineaPedidoID = new SelectList(db.lineaPedido, "lineaPedidoID", "lineaPedidoID", lineaFactura.lineaPedidoID);
            return View(lineaFactura);
        }

        // GET: lineaFacturas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaFactura lineaFactura = db.lineaFactura.Find(id);
            if (lineaFactura == null)
            {
                return HttpNotFound();
            }
            return View(lineaFactura);
        }

        // GET: lineaFacturas/Delete/5
        public ActionResult Delete2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            lineaFactura lineaFactura = db.lineaFactura.Find(id);
            if (lineaFactura == null)
            {
                return HttpNotFound();
            }
            return View(lineaFactura);
        }

        // POST: lineaFacturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            lineaFactura lineaFactura = db.lineaFactura.Find(id);
            db.lineaFactura.Remove(lineaFactura);
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

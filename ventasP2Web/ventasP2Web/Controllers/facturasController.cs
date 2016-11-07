using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using ventasP2Web.Models;

namespace ventasP2Web.Controllers
{
    public class facturasController : Controller
    {
        private ventasBDEntities1 db = new ventasBDEntities1();

        // GET: facturas
        public ActionResult Index(string fecha)
        {
            var factura = db.factura.Include(f => f.cliente).OrderBy(f=>f.facturaID);
            if (!String.IsNullOrEmpty(fecha))
            {
                try
                {
                    DateTime fecha2 = Convert.ToDateTime(fecha);
                    factura = db.factura.Include(f => f.cliente).Where(f => f.fechaCreacion == fecha2).OrderByDescending(p => p.clienteID);
                } catch(Exception e) { 
                    ViewData["error"] = "Formato de fecha inválido";
                    return View(factura.ToList());
                }
            }
            return View(factura.ToList());
        }

        public PartialViewResult detalleFactura(int id)
        {
            var lineaFactura = db.lineaFactura.Include(l => l.factura).Include(l => l.lineaPedido).Where(l => l.facturaID==id);
            return PartialView(lineaFactura.ToList());
        }

        // GET: facturas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            ViewBag.detalle = db.lineaFactura.Where(l => l.facturaID == id);
            return View(factura);
        }

        public void enviarCorreo(string id, string correo)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            NetworkCredential credentials = new NetworkCredential("tp2basestec", "nomelase");
            client.Credentials = credentials;

            MailMessage mnsj = new MailMessage();
            mnsj.Subject = "Comprobante de Factura " + id;
            mnsj.To.Add(new MailAddress(correo));
            mnsj.From = new MailAddress("tp2basestec@gmail.com", "Sistema Ventas");
            /* Si deseamos Adjuntar algún archivo*/
            //mnsj.Attachments.Add(new Attachment("C:\\archivo.pdf"));
            mnsj.Body = "\n Enviado desde C#\n\n *VER EL ARCHIVO ADJUNTO*";

            try
            {
                client.Send(mnsj);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.ToString());
            }
            Console.WriteLine("Correo enviado");
        }

        // GET: facturas/Create
        public ActionResult Create()
        {
            ViewBag.clienteID = new SelectList(db.cliente, "clienteID", "nombre");
            return View();
        }

        // POST: facturas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "facturaID,clienteID,fechaCreacion,fechaVencimiento,direccionEntrega")] factura factura)
        {
            if (ModelState.IsValid)
            {
                factura.fechaCreacion = DateTime.Now;
                db.factura.Add(factura);
                db.SaveChanges();

                string correo = db.cliente.Where(p => p.clienteID == factura.clienteID).Select(p => p.correo).First();
                enviarCorreo(factura.facturaID.ToString(), correo);

                //Se crea una cuenta por cobrar
                try
                {
                    cuentaPorCobrar c = new cuentaPorCobrar
                    {
                        facturaID = factura.facturaID,
                        totalImpuesto = 0,
                        totalAPagar = 0,
                        totalPagado = 0,
                    };

                    db.cuentaPorCobrar.Add(c);
                    db.SaveChanges();
                }
                catch (Exception e) { }

                return RedirectToAction("Index");
            }

            ViewBag.clienteID = new SelectList(db.cliente, "clienteID", "nombre", factura.clienteID);
            return View(factura);
        }


        // GET: facturas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteID = new SelectList(db.cliente, "clienteID", "nombre", factura.clienteID);
            return View(factura);
        }

        // POST: facturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "facturaID,clienteID,fechaCreacion,fechaVencimiento,direccionEntrega")] factura factura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(factura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteID = new SelectList(db.cliente, "clienteID", "nombre", factura.clienteID);
            return View(factura);
        }

        // GET: facturas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // POST: facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var a = db.cuentaPorCobrar.Where(p => p.facturaID == id);
            if (a != null)
            {
                ViewData["error1"] = "Esta factura tiene cuentas por cobrar relacionadas";
            }
            var b = db.lineaFactura.Where(p => p.facturaID == id);
            if (a!=null || b != null)
            {
                factura e = db.factura.Find(id);
                ViewData["error2"] = "Esta factura tiene lineas facturas relacionadas";
                return View("delete", e);
            }

            factura factura = db.factura.Find(id);
            db.factura.Remove(factura);
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

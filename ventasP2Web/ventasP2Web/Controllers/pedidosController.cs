using Rotativa;
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
    public class pedidosController : Controller
    {
        private ventasBDEntities1 db = new ventasBDEntities1();

        // GET: pedidos
        public ActionResult Index()
        {
            //Para saber si esta facturado parcialmente o no
            int sum1 = 0,sum2=0;
            List<string> lista = new List<string>();
            var pedidos = db.pedido.Include(p => p.cliente).Include(p => p.empleado).OrderBy(p=>p.pedidoID);

            try
            {
                foreach (pedido p in pedidos)
                {
                    sum1 = 0; sum2 = 0;
                    var sumlineapedidos = db.lineaPedido.Where(l => l.pedidoID == p.pedidoID).Sum(l => l.cantidad);
                    if (sumlineapedidos != null)
                        sum1 = (int)sumlineapedidos;

                    var lineapedidos = db.lineaPedido.Where(l => l.pedidoID == p.pedidoID);
                    foreach (lineaPedido lp in lineapedidos)
                    {
                        var sumlineafacturas = db.lineaFactura.Where(l => l.lineaPedido.lineaPedidoID == lp.lineaPedidoID).Sum(l => l.cantidadFacturada);
                        if (sumlineafacturas != null)
                            sum2 += (int)sumlineafacturas;
                    }
                    if(sum2==0)
                        lista.Add("VACIA");
                    else if (sum2 < sum1)
                        lista.Add("PARCIALMENTE");
                    else
                        lista.Add("COMPLETA");
                }
            }
            catch (Exception e) { }
            ViewBag.facturacion = lista;

            return View(pedidos.ToList());
        }

        public ActionResult imprimirIndex()
        {
            return new ActionAsPdf("Index") { FileName = "pedidos.pdf" };
        }

        // GET: pedidos
        public ActionResult IndexCliente(string searchString)
        {
            int sum1 = 0, sum2 = 0;
            List<string> lista = new List<string>();
            var pedidos = db.pedido.Include(p => p.cliente).Include(p => p.empleado).Where(p => p.cliente.nombre == searchString);

            try
            {
                foreach (pedido p in pedidos)
                {
                    sum1 = 0; sum2 = 0;
                    var sumlineapedidos = db.lineaPedido.Where(l => l.pedidoID == p.pedidoID).Sum(l => l.cantidad);
                    if (sumlineapedidos != null)
                        sum1 = (int)sumlineapedidos;

                    var lineapedidos = db.lineaPedido.Where(l => l.pedidoID == p.pedidoID);
                    foreach (lineaPedido lp in lineapedidos)
                    {
                        var sumlineafacturas = db.lineaFactura.Where(l => l.lineaPedido.lineaPedidoID == lp.lineaPedidoID).Sum(l => l.cantidadFacturada);
                        if (sumlineafacturas != null)
                            sum2 += (int)sumlineafacturas;
                    }
                    if (sum2 == 0)
                        lista.Add("VACIA");
                    else if (sum2 < sum1)
                        lista.Add("PARCIALMENTE");
                    else
                        lista.Add("COMPLETA");
                }
            }
            catch (Exception e) { }
            ViewBag.facturacion = lista;

            //var pedido = db.pedido.Where(p => p.cliente.nombre == searchString);
            if (!String.IsNullOrEmpty(searchString))
            {
                pedidos = db.pedido.Include(p => p.cliente).Include(p => p.empleado).Where(p => p.cliente.nombre == searchString && p.estado != "CANCELADO").OrderByDescending(p => p.estado);
            }
            return View(pedidos.ToList());
        }

        public PartialViewResult detallePedido(string id)
        {
            var lineaPedido = db.lineaPedido.Include(l => l.pedido).Include(l => l.producto).Where(l => l.pedidoID == id);
            return PartialView(lineaPedido.ToList());
        }

        public ActionResult confirmar(string id)
        {
            try
            {
                var pedido = db.pedido.Where(p => p.pedidoID == id).First();
                pedido.estado = "CONFIRMADO";
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e) { }
            return RedirectToAction("IndexCliente");
        }

        public ActionResult rechazar(string id, string motivo)
        {
            try
            {
                var pedido = db.pedido.Where(p => p.pedidoID == id).First();
                pedido.estado = "RECHAZADO";
                pedido.descripcionEstado = motivo;
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e) { }
            return RedirectToAction("IndexCliente");
        }

        public ActionResult reprocesar(string id, string motivo)
        {
            try
            {
                var pedido = db.pedido.Where(p => p.pedidoID == id).First();
                pedido.estado = "REPROCESAR";
                pedido.descripcionEstado = motivo;
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e) { }
            return RedirectToAction("IndexCliente");
        }

        public ActionResult cancelar(string id, string motivo)
        {
            try
            {
                var pedido = db.pedido.Where(p => p.pedidoID == id).First();
                pedido.estado = "CANCELADO";
                pedido.descripcionEstado = motivo;
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();

                string correo = db.cliente.Where(p => p.clienteID == pedido.clienteID).Select(p => p.correo).First();
                enviarCorreo(pedido.pedidoID, correo, "CANCELADO");
            }
            catch (Exception e) { }
            return RedirectToAction("Index");
        }

        public void enviarCorreo(string id, string correo, string mensaje)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            NetworkCredential credentials = new NetworkCredential("tp2basestec", "nomelase");
            client.Credentials = credentials;

            MailMessage mnsj = new MailMessage();
            mnsj.Subject = "Pedido "+id+" "+mensaje;
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

        // GET: pedidos/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            pedido pedido = db.pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        public ActionResult DetailsCliente(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            pedido pedido = db.pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // GET: pedidos/Create
        public ActionResult Create()
        {
            ViewBag.clienteID = new SelectList(db.cliente, "clienteID", "nombre");
            ViewBag.empleadoID = new SelectList(db.empleado, "empleadoID", "nombre");
            return View();
        }

        // POST: pedidos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pedidoID,empleadoID,clienteID,fechaCreacion,fechaEntrega,estado,descripcionEstado")] pedido pedido)
        {
            if (ModelState.IsValid)
            {
                int num = 0;
                try
                {
                    var p = db.pedido.OrderByDescending(a => a.pedidoID).FirstOrDefault();
                    string p2 = p.pedidoID.Substring(0, p.pedidoID.IndexOf("-"));
                    num = int.Parse(p2);
                }
                catch (Exception e) { }

                string id = (num+1).ToString() + "-" + DateTime.Now.Date.Year.ToString();
                pedido.pedidoID = id;
                pedido.fechaCreacion = DateTime.Now;
                pedido.estado = "REGISTRADO";

                db.pedido.Add(pedido);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteID = new SelectList(db.cliente, "clienteID", "nombre", pedido.clienteID);
            ViewBag.empleadoID = new SelectList(db.empleado, "empleadoID", "nombre", pedido.empleadoID);
            return View(pedido);
        }

        // GET: pedidos/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pedido pedido = db.pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteID = new SelectList(db.cliente, "clienteID", "nombre", pedido.clienteID);
            ViewBag.empleadoID = new SelectList(db.empleado, "empleadoID", "nombre", pedido.empleadoID);
            return View(pedido);
        }

        // POST: pedidos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pedidoID,empleadoID,clienteID,fechaCreacion,fechaEntrega,estado,descripcionEstado")] pedido pedido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pedido).State = EntityState.Modified;
                db.SaveChanges();

                //Se verifica si el estado del pedido paso a REVISADO para enviar un correo al cliente
                if (pedido.estado == "REVISADO")
                {
                    string correo = db.cliente.Where(p => p.clienteID == pedido.clienteID).Select(p => p.correo).First();
                    enviarCorreo(pedido.pedidoID, correo, "REVISADO");
                }

                if (pedido.estado == "CANCELADO")
                {
                    string correo = db.cliente.Where(p => p.clienteID == pedido.clienteID).Select(p => p.correo).First();
                    enviarCorreo(pedido.pedidoID, correo, "CANCELADO");
                }

                return RedirectToAction("Index");
            }
            ViewBag.clienteID = new SelectList(db.cliente, "clienteID", "nombre", pedido.clienteID);
            ViewBag.empleadoID = new SelectList(db.empleado, "empleadoID", "nombre", pedido.empleadoID);

            return View(pedido);
        }

        // GET: pedidos/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pedido pedido = db.pedido.Find(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            return View(pedido);
        }

        // POST: pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var lineapedidos = db.lineaPedido.Where(p => p.pedidoID == id);
            if (lineapedidos.Count() > 0)
            {
                pedido e = db.pedido.Find(id);
                ViewData["error"] = "Este pedido tiene productos asociados";
                return View("delete", e);
            }

            pedido pedido = db.pedido.Find(id);
            db.pedido.Remove(pedido);
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

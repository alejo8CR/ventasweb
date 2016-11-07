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
    public class empleadosController : Controller
    {
        private ventasBDEntities1 db = new ventasBDEntities1();

        // GET: empleados
        public ActionResult Index()
        {
            return View(db.empleado.Where(e=>e.puesto=="EJEC").OrderBy(e=>e.nombre).ToList());
        }

        public ActionResult imprimirIndex()
        {
            return new ActionAsPdf("Index") { FileName = "empleados.pdf" };
        }

        // GET: empleados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empleado empleado = db.empleado.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }


        // GET: empleados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: empleados/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "empleadoID,nombre,apellido1,apellido2,telefono,correo,residencia,puesto,fechaNacimiento,fechaIngreso,fechaRetiro")] empleado empleado)
        {
            if (ModelState.IsValid)
            {
                empleado.fechaIngreso = DateTime.Now;
                db.empleado.Add(empleado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(empleado);
        }

        // GET: empleados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empleado empleado = db.empleado.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }

        // POST: empleados/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "empleadoID,nombre,apellido1,apellido2,telefono,correo,residencia,puesto,fechaNacimiento,fechaIngreso,fechaRetiro")] empleado empleado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(empleado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(empleado);
        }

        // GET: empleados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            empleado empleado = db.empleado.Find(id);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            return View(empleado);
        }

        // POST: empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var pedidos = db.pedido.Where(p => p.empleadoID == id);
            if (pedidos.Count() > 0)
            {
                empleado e = db.empleado.Find(id);
                ViewData["error"] = "Este empleado tiene pedidos";
                return View("delete",e);
            }

            empleado empleado = db.empleado.Find(id);
            db.empleado.Remove(empleado);
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

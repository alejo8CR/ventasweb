﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ventasP2Web.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ventasBDEntities1 : DbContext
    {
        public ventasBDEntities1()
            : base("name=ventasBDEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<cliente> cliente { get; set; }
        public virtual DbSet<cuentaPorCobrar> cuentaPorCobrar { get; set; }
        public virtual DbSet<empleado> empleado { get; set; }
        public virtual DbSet<factura> factura { get; set; }
        public virtual DbSet<lineaFactura> lineaFactura { get; set; }
        public virtual DbSet<lineaPedido> lineaPedido { get; set; }
        public virtual DbSet<metodoPago> metodoPago { get; set; }
        public virtual DbSet<pedido> pedido { get; set; }
        public virtual DbSet<producto> producto { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
    }
}
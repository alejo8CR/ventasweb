//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class cuentaPorCobrar
    {
        [DisplayName("Cuenta por cobrar")]
        public int cuentaPorCobrar1 { get; set; }
        public int facturaID { get; set; }
        [DisplayName("Total de impuestos")]
        public Nullable<double> totalImpuesto { get; set; }
        [DisplayName("Total a pagar")]
        public Nullable<double> totalAPagar { get; set; }
        [DisplayName("Total pagado")]
        public Nullable<double> totalPagado { get; set; }
    
        public virtual factura factura { get; set; }
    }
}

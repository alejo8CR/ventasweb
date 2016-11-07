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

    public partial class producto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public producto()
        {
            this.lineaPedido = new HashSet<lineaPedido>();
        }

        [Required]
        [DisplayName("SKU")]
        public int SKU { get; set; }
        public string descripcion { get; set; }
        [Required]
        public Nullable<double> costo { get; set; }
        [Required]
        [DisplayName("Precio de venta")]
        public Nullable<double> precioVenta { get; set; }
        [Required]
        public Nullable<int> stock { get; set; }
        public Nullable<double> impuesto { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<lineaPedido> lineaPedido { get; set; }
    }
}

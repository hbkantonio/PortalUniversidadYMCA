//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Universidad.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class PagoMetodo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PagoMetodo()
        {
            this.PagoDetalle = new HashSet<PagoDetalle>();
        }
    
        public int PagoMetodoId { get; set; }
        public string Descripcion { get; set; }
        public string CuentaContable { get; set; }
        public decimal Comision { get; set; }
        public bool EsVisible { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PagoDetalle> PagoDetalle { get; set; }
    }
}

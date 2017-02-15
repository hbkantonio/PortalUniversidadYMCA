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
    
    public partial class PagoConcepto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PagoConcepto()
        {
            this.AlumnoDescuento = new HashSet<AlumnoDescuento>();
            this.AlumnoReferenciaBitacora = new HashSet<AlumnoReferenciaBitacora>();
            this.Cuota = new HashSet<Cuota>();
            this.Descuento = new HashSet<Descuento>();
        }
    
        public int PagoConceptoId { get; set; }
        public int OfertaEducativaId { get; set; }
        public string Descripcion { get; set; }
        public string CuentaContable { get; set; }
        public bool EsCobrable { get; set; }
        public bool EsVisible { get; set; }
        public bool EsVariable { get; set; }
        public bool EsMultireferencia { get; set; }
        public bool EsIVA { get; set; }
        public bool EsCancelable { get; set; }
        public int EstatusId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoDescuento> AlumnoDescuento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoReferenciaBitacora> AlumnoReferenciaBitacora { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cuota> Cuota { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Descuento> Descuento { get; set; }
        public virtual Estatus Estatus { get; set; }
        public virtual OfertaEducativa OfertaEducativa { get; set; }
    }
}

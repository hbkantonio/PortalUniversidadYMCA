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
    
    public partial class Recibo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Recibo()
        {
            this.PagoParcial = new HashSet<PagoParcial>();
        }
    
        public int ReciboId { get; set; }
        public int SucursalCajaId { get; set; }
        public string Serie { get; set; }
        public string Observaciones { get; set; }
        public decimal Importe { get; set; }
        public int AlumnoId { get; set; }
        public int OfertaEducativaId { get; set; }
        public System.DateTime FechaGeneracion { get; set; }
        public System.TimeSpan HoraGeneracion { get; set; }
        public int UsuarioId { get; set; }
        public int EstatusId { get; set; }
    
        public virtual Estatus Estatus { get; set; }
        public virtual OfertaEducativa OfertaEducativa { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PagoParcial> PagoParcial { get; set; }
        public virtual SucursalCaja SucursalCaja { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual ReciboArchivo ReciboArchivo { get; set; }
        public virtual ReciboDetalle ReciboDetalle { get; set; }
    }
}

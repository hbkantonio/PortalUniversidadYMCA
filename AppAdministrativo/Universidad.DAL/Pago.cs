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
    
    public partial class Pago
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pago()
        {
            this.AlumnoReferenciaBitacora = new HashSet<AlumnoReferenciaBitacora>();
            this.PagoDescuento = new HashSet<PagoDescuento>();
            this.PagoPagare = new HashSet<PagoPagare>();
            this.PagoRecargo1 = new HashSet<PagoRecargo>();
            this.PagoParcial = new HashSet<PagoParcial>();
            this.PolizaAjuste = new HashSet<PolizaAjuste>();
            this.ReferenciaProcesada = new HashSet<ReferenciaProcesada>();
        }
    
        public int PagoId { get; set; }
        public string ReferenciaId { get; set; }
        public int AlumnoId { get; set; }
        public int Anio { get; set; }
        public int PeriodoId { get; set; }
        public int SubperiodoId { get; set; }
        public int OfertaEducativaId { get; set; }
        public System.DateTime FechaGeneracion { get; set; }
        public int CuotaId { get; set; }
        public decimal Cuota { get; set; }
        public decimal Promesa { get; set; }
        public int EstatusId { get; set; }
        public bool EsEmpresa { get; set; }
        public bool EsAnticipado { get; set; }
        public int PeriodoAnticipadoId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoReferenciaBitacora> AlumnoReferenciaBitacora { get; set; }
        public virtual Cuota Cuota1 { get; set; }
        public virtual Estatus Estatus { get; set; }
        public virtual OfertaEducativa OfertaEducativa { get; set; }
        public virtual Periodo Periodo { get; set; }
        public virtual Subperiodo Subperiodo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PagoDescuento> PagoDescuento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PagoPagare> PagoPagare { get; set; }
        public virtual PagoRecargo PagoRecargo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PagoRecargo> PagoRecargo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PagoParcial> PagoParcial { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PolizaAjuste> PolizaAjuste { get; set; }
        public virtual ReferenciadoCabecero ReferenciadoCabecero { get; set; }
        public virtual ReferenciadoCabeceroBitacora ReferenciadoCabeceroBitacora { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReferenciaProcesada> ReferenciaProcesada { get; set; }
    }
}

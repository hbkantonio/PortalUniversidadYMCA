//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class OfertaEducativa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OfertaEducativa()
        {
            this.Adeudo = new HashSet<Adeudo>();
            this.AlumnoDescuento = new HashSet<AlumnoDescuento>();
            this.AlumnoInscrito = new HashSet<AlumnoInscrito>();
            this.AlumnoInscritoBitacora = new HashSet<AlumnoInscritoBitacora>();
            this.AlumnoReingresoBitacora = new HashSet<AlumnoReingresoBitacora>();
            this.AlumnoRevision = new HashSet<AlumnoRevision>();
            this.Asignatura = new HashSet<Asignatura>();
            this.BecaDeportiva = new HashSet<BecaDeportiva>();
            this.Cuota = new HashSet<Cuota>();
            this.Descuento = new HashSet<Descuento>();
            this.GrupoAlumnoConfiguracion = new HashSet<GrupoAlumnoConfiguracion>();
            this.IdiomaGrupoAlumno = new HashSet<IdiomaGrupoAlumno>();
            this.Matricula = new HashSet<Matricula>();
            this.PagoConcepto = new HashSet<PagoConcepto>();
            this.OfertaEducativaAntecedente = new HashSet<OfertaEducativaAntecedente>();
            this.OfertaEducativaRequerimiento = new HashSet<OfertaEducativaRequerimiento>();
            this.Pago = new HashSet<Pago>();
            this.ProspectoDetalle = new HashSet<ProspectoDetalle>();
            this.Recibo = new HashSet<Recibo>();
            this.UniversidadValidacion = new HashSet<UniversidadValidacion>();
        }
    
        public int OfertaEducativaId { get; set; }
        public int OfertaEducativaTipoId { get; set; }
        public string Descripcion { get; set; }
        public string Rvoe { get; set; }
        public Nullable<System.DateTime> FechaRvoe { get; set; }
        public int SucursalId { get; set; }
        public bool RequiereDeportivo { get; set; }
        public int EstatusId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Adeudo> Adeudo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoDescuento> AlumnoDescuento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoInscrito> AlumnoInscrito { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoInscritoBitacora> AlumnoInscritoBitacora { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoReingresoBitacora> AlumnoReingresoBitacora { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoRevision> AlumnoRevision { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asignatura> Asignatura { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BecaDeportiva> BecaDeportiva { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cuota> Cuota { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Descuento> Descuento { get; set; }
        public virtual Estatus Estatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrupoAlumnoConfiguracion> GrupoAlumnoConfiguracion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IdiomaGrupoAlumno> IdiomaGrupoAlumno { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Matricula> Matricula { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PagoConcepto> PagoConcepto { get; set; }
        public virtual OfertaEducativaTipo OfertaEducativaTipo { get; set; }
        public virtual Sucursal Sucursal { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfertaEducativaAntecedente> OfertaEducativaAntecedente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfertaEducativaRequerimiento> OfertaEducativaRequerimiento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pago> Pago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProspectoDetalle> ProspectoDetalle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Recibo> Recibo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UniversidadValidacion> UniversidadValidacion { get; set; }
    }
}

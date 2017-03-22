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
    
    public partial class AlumnoDescuento
    {
        public int AlumnoDescuentoId { get; set; }
        public int AlumnoId { get; set; }
        public int OfertaEducativaId { get; set; }
        public int Anio { get; set; }
        public int PeriodoId { get; set; }
        public int DescuentoId { get; set; }
        public int PagoConceptoId { get; set; }
        public decimal Monto { get; set; }
        public int UsuarioId { get; set; }
        public string Comentario { get; set; }
        public System.DateTime FechaGeneracion { get; set; }
        public System.TimeSpan HoraGeneracion { get; set; }
        public bool EsSEP { get; set; }
        public bool EsComite { get; set; }
        public bool EsDeportiva { get; set; }
        public Nullable<System.DateTime> FechaAplicacion { get; set; }
        public int EstatusId { get; set; }
    
        public virtual Alumno Alumno { get; set; }
        public virtual Estatus Estatus { get; set; }
        public virtual OfertaEducativa OfertaEducativa { get; set; }
        public virtual PagoConcepto PagoConcepto { get; set; }
        public virtual Periodo Periodo { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual AlumnoDescuentoDocumento AlumnoDescuentoDocumento { get; set; }
        public virtual AlumnoDescuentoPendiente AlumnoDescuentoPendiente { get; set; }
    }
}
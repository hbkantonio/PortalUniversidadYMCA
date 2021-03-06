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
    
    public partial class UniversidadValidacion
    {
        public int UsuarioId { get; set; }
        public int MembresiaId { get; set; }
        public int NumeroFamiliar { get; set; }
        public int UsuarioTipoId { get; set; }
        public int CategoriaId { get; set; }
        public System.TimeSpan HoraInicio { get; set; }
        public System.TimeSpan HoraFinal { get; set; }
        public System.DateTime FechaPago { get; set; }
        public int EstatusId { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public int Inscrito { get; set; }
        public int OfertaEducativaId { get; set; }
        public int RequiereDeportivo { get; set; }
        public int Inscripcion { get; set; }
        public int Colegiatura { get; set; }
        public int Materias { get; set; }
        public int Asesorias { get; set; }
        public int ExamenMedico { get; set; }
        public decimal Adeudo { get; set; }
    
        public virtual Estatus Estatus { get; set; }
        public virtual OfertaEducativa OfertaEducativa { get; set; }
    }
}

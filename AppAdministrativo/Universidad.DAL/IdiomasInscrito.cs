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
    
    public partial class IdiomasInscrito
    {
        public int AlumnoId { get; set; }
        public int OfertaEducativaId { get; set; }
        public int Anio { get; set; }
        public int PeriodoId { get; set; }
        public System.DateTime FechaInscripcion { get; set; }
        public Nullable<int> PagoPlanId { get; set; }
        public int TurnoId { get; set; }
        public bool EsEmpresa { get; set; }
        public int UsuarioId { get; set; }
    }
}

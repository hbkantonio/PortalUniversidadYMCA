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
    
    public partial class Adeudo
    {
        public int AlumnoId { get; set; }
        public decimal ColegiaturaIdiomas { get; set; }
        public decimal Colegiatura { get; set; }
        public decimal Inscripcion { get; set; }
        public decimal Financiamiento { get; set; }
        public decimal Pagare { get; set; }
        public Nullable<int> OfertaEducativaId { get; set; }
    
        public virtual Alumno Alumno { get; set; }
        public virtual OfertaEducativa OfertaEducativa { get; set; }
    }
}

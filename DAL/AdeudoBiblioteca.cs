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
    
    public partial class AdeudoBiblioteca
    {
        public int AlumnoId { get; set; }
        public System.DateTime FechaOperacion { get; set; }
        public string Observaciones { get; set; }
        public int UsuarioId { get; set; }
    
        public virtual Alumno Alumno { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}

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
    
    public partial class UsuarioIngresosBitacora
    {
        public int UsuarioId { get; set; }
        public System.DateTime FechaIngreso { get; set; }
        public System.TimeSpan HoraIngreso { get; set; }
        public System.TimeSpan HoraSalida { get; set; }
        public string TipoIngreso { get; set; }
    
        public virtual Usuario Usuario { get; set; }
    }
}

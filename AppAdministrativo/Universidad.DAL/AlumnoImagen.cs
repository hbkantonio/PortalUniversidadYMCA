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
    
    public partial class AlumnoImagen
    {
        public int AlumnoId { get; set; }
        public byte[] Imagen { get; set; }
    
        public virtual Alumno Alumno { get; set; }
        public virtual AlumnoImagenDetalle AlumnoImagenDetalle { get; set; }
    }
}

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
    
    public partial class PersonaAutorizadaBitacora
    {
        public int PersonaAutorizadaBitacoraId { get; set; }
        public int PersonaAutorizadaId { get; set; }
        public int AlumnoId { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public int ParentescoId { get; set; }
        public bool EsAutorizada { get; set; }
        public int UsuarioId { get; set; }
        public System.DateTime Fecha { get; set; }
    }
}

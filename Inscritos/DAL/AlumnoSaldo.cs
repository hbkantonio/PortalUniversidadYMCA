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
    
    public partial class AlumnoSaldo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AlumnoSaldo()
        {
            this.AlumnoSaldoDetalle = new HashSet<AlumnoSaldoDetalle>();
            this.AlumnoSaldoGasto = new HashSet<AlumnoSaldoGasto>();
        }
    
        public int AlumnoId { get; set; }
        public decimal Saldo { get; set; }
    
        public virtual Alumno Alumno { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoSaldoDetalle> AlumnoSaldoDetalle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlumnoSaldoGasto> AlumnoSaldoGasto { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universidad.DTO.Reinscripcion
{
    public class DTOReinscripcionAcademico
    {
        public int alumnoId { get; set; }
        public int ofertaEducativaId { get; set; }
        public int anio { get; set; }
        public int periodoId { get; set; }
        public int usuarioId { get; set; }
        public bool inscripcionCompleta { get; set; }
        public int materia { get; set; }
        public int asesoria { get; set; }
        public string observaciones { get; set; }
    }
}
namespace DTO.Reinscripcion
{
    public class DTOMateriasAsesorias
    {
        public int AlumnoId { get; set; }
        public List<DTOOfertaEducativa> lstOfertas { get; set; }
        public string Nombre { get; set; }
        public List<DTOPeriodo> lstPeriodos { get; set; }
        public List<dtoCuotaReinc> Cuotas { get; set; }
        public List<dtoReferenciasReinsc> Referencias { get; set; }
        public List<dtoEstatusMA> EstatusAl { get; set; }
    }
    public class dtoEstatusMA
    {
        public int Anio { get; set; }
        public int Periodo { get; set; }
        public int OfertaEducativaId { get; set; }
        public string Estado { get; set; }
    }
    public class dtoCuotaReinc
    {
        public int CuotaId { get; set; }
        public decimal Monto {get;set;}
        public int Anio { get; set; }
        public int PeriodoId { get; set; }
        public int OfertaEducativaId { get; set; }
        public int PagoConceptoId { get; set; }
    }
    public class dtoReferenciasReinsc
    {
        public int Anio { get; set; }
        public int PeriodoId { get; set; }
        public int OfertaEducativaId { get; set; }
        public string Concepto { get; set; }
        public string Referencia { get; set; }
        public string Fecha { get; set; }
        public string Monto { get; set; }
    }
}
﻿using BLL;
using DTO;
using DTO.Reinscripcion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebServices.WS
{
    /// <summary>
    /// Descripción breve de Reinscripcion
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Reinscripcion : System.Web.Services.WebService
    {
        static CultureInfo Cultura = CultureInfo.CreateSpecificCulture("es-MX");
        [WebMethod]
        public string GenerarInscrCole(string AlumnoId, string OfertaEducativaId, string PeriodoD)
        {
            DTOPeriodo objPeriodo = BLLPeriodo.ConsultarPeriodo2(PeriodoD);

            return BLLPago.GenerarInscripcionColegiatura(int.Parse(AlumnoId), int.Parse(OfertaEducativaId));
        }
        [WebMethod]
        public List<Flujo> ConsultarTabla(string AlumnoId, string OfertaEducativaId)
        {
            return BLLPago.FlujoReinscripcion(int.Parse(AlumnoId), int.Parse(OfertaEducativaId));
        }
        [WebMethod]
        public List<string> ConsultarPagosPeriodo(string AlumnoId, string OfertaEducativaId)
        {
            return BLLPago.BuscarPagoIngles(int.Parse(AlumnoId), int.Parse(OfertaEducativaId));
        }
        [WebMethod]
        public List<DTOMes> ConsultarPagodeMes(string AlumnoId, string OfertaEducativaId)
        {
            return BLLPago.EsteMesvsSiguiente(int.Parse(AlumnoId), int.Parse(OfertaEducativaId), DateTime.Now);
        }
        [WebMethod]
        public string GenerarColegiaturaIngles(string AlumnoId, string OfertaEducativaId, string MesId)
        {
            DateTime fhoy = DateTime.Now;
            fhoy = fhoy.Month == int.Parse(MesId) ? DateTime.Now : DateTime.ParseExact("01/" + (int.Parse(MesId) < 10 ? "0" + MesId : MesId) + "/" + fhoy.Year.ToString(), "dd/MM/yyyy", Cultura);
            return BLLPago.GenerarInscripcionColegiatura(int.Parse(AlumnoId), int.Parse(OfertaEducativaId), fhoy);
        }
        [WebMethod]
        public string[] Pendiente(string AlumnoId, string OfertaEducativaId)
        {
            return BLLPago.MesPendiente(int.Parse(AlumnoId), int.Parse(OfertaEducativaId));
        }
        [WebMethod]
        public string InscribirGenerar(string AlumnoId, string OfertaEducativaId)
        {
            DTOPeriodo objP = BLLPeriodo.TraerPeriodoEntreFechas(DateTime.Now);
            string Menms = BLLPago.GenerarInscripcionColegiatura(int.Parse(AlumnoId), int.Parse(OfertaEducativaId), objP.Anio, objP.PeriodoId);
            if (Menms == "Guardado")
            {
                bool resp = false;// BLL.BLLAlumno.AplicaBecaAlumno(objBeca);
                if (resp)
                {
                    BLL.BLLSaldoAFavor.AplicacionSaldoAlumno(int.Parse(AlumnoId), true, false);
                    return "Guardado";
                }
                else { return "Fallo"; }
            }
            else
            {
                return Menms;
            }
        }
        [WebMethod]
        public DTOMateriasAsesorias AlumnoReinscripcion(string AlumnoId)
        {
            return BLLReinscripcion.TraerAlumno(int.Parse(AlumnoId));
        }
        [WebMethod]
        public bool Generar(string AlumnoId, string anio, string periodo, string oferta, string NMaterias,
                string NAsesorias, string Completa, string usuario, string Comentario)
        {
            try
            {
                BLLReinscripcion.ReinscripcionAcademico(new Universidad.DTO.Reinscripcion.DTOReinscripcionAcademico
                {
                    alumnoId = int.Parse(AlumnoId),
                    anio = int.Parse(anio),
                    asesoria = int.Parse(NAsesorias),
                    inscripcionCompleta = bool.Parse(Completa),
                    materia = int.Parse(NMaterias),
                    ofertaEducativaId = int.Parse(oferta),
                    periodoId = int.Parse(periodo),
                    usuarioId = int.Parse(usuario),
                    observaciones = Comentario
                });
                return true;
            }
            catch { return false; }
        }
    }
}

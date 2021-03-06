﻿using BLL;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace AppAdministrativos.WS
{
    /// <summary>
    /// Descripción breve de Reporte
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Reporte : System.Web.Services.WebService
    {

        [WebMethod]
        public List<DTOReporteAlumnoOferta> MostraReporteAlumnoOferta()
        {
            return BLLReportePortal.ObtenerReporteAlumnoOferta();
        }

        [WebMethod]
        public DTOFIltros MostrarCuatrimestre()
        {
            return BLLReportePortal.CargarCuatrimestre();
        }

        [WebMethod]
        public List<DTOReporteBecasCuatrimestre> MostrarReporteBecaCuatrimestre(int anio, int periodo)
        {
            return BLLReportePortal.CargaReporteBecaCuatrimestre(anio, periodo);
        }

        [WebMethod]
        public List<DTOReporteInscrito> MostrarReporteInscrito(int anio, int periodo)
        {
            return BLLReportePortal.CargaReporteInscrito(anio, periodo);
        }

        [WebMethod]
        public List<DTOReporteBecaSep> MostrarReporteBecaSep(int anio, int periodo)
        {
            return BLLReportePortal.CargaReporteBecaSep(anio, periodo);
        }


        [WebMethod]
        public List<DTOReporteInegi> MostrarReporteIneg(int anio, int periodo)
        {
            return BLLReportePortal.CargaReporteIneg(anio, periodo);
        }

        [WebMethod]
        public List<DTOReporteAlumnoReferencia> MostrarReporteAlumnoReferencia(int anio, int periodo)
        {
            return BLLReportePortal.CargaReporteAlumnoReferencia(anio, periodo);
        }
        [WebMethod]
        public DTOVoBo ReporteVoBo(int anio, int periodoid, int usuarioid)
        {
            return BLLReportePortal.ReporteVoBo(anio, periodoid,usuarioid);
        }

    }
}

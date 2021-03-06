﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using DAL;
using Herramientas;
using BLL;
using System.Data.Entity;
using System.Globalization;

namespace BLL
{
    public class BLLAlumnoPortal
    {
        static CultureInfo Cultura = CultureInfo.CreateSpecificCulture("es-MX");
        public static string InsertarAlumno(DTOAlumnoDetalle objDetalleAlumno, DTOAlumno objAlumno, List<DTOPersonaAutorizada> objPersonaAutorizada,
            DTOAlumnoInscrito objAlumnoInscrito, DTOProspectoDetalle objProspectoDetalle)
        {
            int? pagoplan = null;
            //return 0;
            using (UniversidadEntities db = new UniversidadEntities())
            {
                pagoplan = db.OfertaEducativa.
                            Where(kl => kl.OfertaEducativaId == objAlumnoInscrito.OfertaEducativaId).
                            FirstOrDefault().OfertaEducativaTipoId == 4 ? 5 : pagoplan;

                List<PersonaAutorizada> lstPersonas = new List<PersonaAutorizada>();
                foreach (DTOPersonaAutorizada objPer in objPersonaAutorizada)
                {
                    lstPersonas.Add(
                        new PersonaAutorizada
                        {
                            Nombre = objPer.Nombre,
                            Paterno = objPer.Paterno,
                            Materno = objPer.Materno,
                            Celular = objPer.Celular,
                            Telefono = objPer.Telefono,
                            Email = objPer.Email,
                            ParentescoId = objPer.ParentescoId,
                            EsAutorizada = objPer.Autoriza
                        });
                }
                try
                {
                    db.Alumno.Add(
                        new Alumno
                        {
                            FechaRegistro = DateTime.Now,
                            Nombre = objAlumno.Nombre,
                            Paterno = objAlumno.Paterno,
                            Materno = objAlumno.Materno,
                            UsuarioId = objAlumno.UsuarioId,
                            Anio = objAlumnoInscrito.Anio,
                            PeriodoId = objAlumnoInscrito.PeriodoId,
                            //UsuarioId = 7255,
                            PersonaAutorizada = lstPersonas,
                            EstatusId = 1,

                            AlumnoInscrito = new List<AlumnoInscrito>
                            {
                                new AlumnoInscrito{
                                OfertaEducativaId = objAlumnoInscrito.OfertaEducativaId,
                                Anio = objAlumnoInscrito.Anio,
                                PeriodoId = objAlumnoInscrito.PeriodoId,
                                FechaInscripcion = DateTime.Now,
                                EsEmpresa=objAlumnoInscrito.EsEmpresa,
                                //PagoPlanId=null,
                                //UsuarioId = 7255,
                                UsuarioId=objAlumno.UsuarioId,
                                TurnoId = objAlumnoInscrito.TurnoId,
                                HoraInscripcion=DateTime.Now.TimeOfDay,
                                EstatusId=1,
                                PagoPlanId = pagoplan
                                }
                            },
                            AlumnoDetalle = new AlumnoDetalle
                            {
                                GeneroId = objDetalleAlumno.GeneroId,
                                EstadoCivilId = objDetalleAlumno.EstadoCivilId,
                                FechaNacimiento = objDetalleAlumno.FechaNacimiento,
                                CURP = objDetalleAlumno.CURP.ToUpper(),
                                PaisId = objDetalleAlumno.PaisId,
                                EntidadFederativaId = objDetalleAlumno.EntidadFederativaId,
                                MunicipioId = objDetalleAlumno.MunicipioId,
                                CP = objDetalleAlumno.Cp,
                                Colonia = objDetalleAlumno.Colonia,
                                Calle = objDetalleAlumno.Calle,
                                NoExterior = objDetalleAlumno.NoExterior,
                                NoInterior = objDetalleAlumno.NoInterior,
                                TelefonoCasa = objDetalleAlumno.TelefonoCasa,
                                Celular = objDetalleAlumno.Celular,
                                Email = objDetalleAlumno.Email,
                                TelefonoOficina = objDetalleAlumno.TelefonoOficina,
                                EntidadNacimientoId = objDetalleAlumno.EntidadNacimientoId,

                            },
                            GrupoAlumnoConfiguracion = objAlumnoInscrito.EsEmpresa ?
                            new List<GrupoAlumnoConfiguracion> {
                                new GrupoAlumnoConfiguracion
                                {
                                    Anio=objAlumnoInscrito.Anio,
                                    CuotaColegiatura=db.Cuota.Where(o=>o.Anio == objAlumnoInscrito.Anio &&
                                                                          o.PeriodoId == objAlumnoInscrito.PeriodoId &&
                                                                          o.OfertaEducativaId == objAlumnoInscrito.OfertaEducativaId &&
                                                                          o.PagoConceptoId == 800).FirstOrDefault().Monto,
                                    CuotaInscripcion = db.Cuota.Where(o => o.Anio == objAlumnoInscrito.Anio &&
                                                                            o.PeriodoId == objAlumnoInscrito.PeriodoId &&
                                                                            o.OfertaEducativaId == objAlumnoInscrito.OfertaEducativaId &&
                                                                            o.PagoConceptoId == 802).FirstOrDefault().Monto,
                                    EsCuotaCongelada = false,
                                    OfertaEducativaId= objAlumnoInscrito.OfertaEducativaId,
                                    PeriodoId=objAlumnoInscrito.PeriodoId,
                                    EsEspecial=false,
                                    EstatusId= 8,
                                    EsInscripcionCongelada=false,
                                    FechaRegistro=DateTime.Now,
                                    GrupoId=null,
                                    HoraRegistro= DateTime.Now.TimeOfDay,
                                    UsuarioId = objAlumno.UsuarioId,
                                    PagoPlanId=null
                                }
                                } : null
                        });

                    db.SaveChanges();
                    if (BLLOfertaEducativaPortal.ConsultarOfertaEducativa(objAlumnoInscrito.OfertaEducativaId).Rvoe != null)
                    {
                        string MAtricula = Herramientas.Matricula.ObtenerMatricula(objAlumnoInscrito, BLLOfertaEducativaPortal.ConsultarOfertaEducativa(objAlumnoInscrito.OfertaEducativaId), db.Alumno.Local[0].AlumnoId);
                        db.Matricula.Add(new DAL.Matricula
                        {
                            AlumnoId = db.Alumno.Local[0].AlumnoId,
                            MatriculaId = MAtricula,
                            OfertaEducativaId = objAlumnoInscrito.OfertaEducativaId,
                            Anio = objAlumnoInscrito.Anio,
                            PeriodoId = objAlumnoInscrito.PeriodoId,
                            FechaAsignacion = DateTime.Now,
                            UsuarioId = objAlumno.UsuarioId
                        });

                        Alumno updateAlumno = db.Alumno.Local[0];
                        updateAlumno.MatriculaId = MAtricula;
                    }
                    else
                    {
                        Alumno updateAlumno = db.Alumno.Local[0];
                        updateAlumno.MatriculaId = "0";
                    }
                    int ofertatio = db.OfertaEducativa.Where(ok => ok.OfertaEducativaId == objAlumnoInscrito.OfertaEducativaId).FirstOrDefault().OfertaEducativaTipoId;

                    if (ofertatio != 4)
                    {
                        int ofertatios = (int)db.OfertaEducativa.Where(ok => ok.OfertaEducativaId == objAlumnoInscrito.OfertaEducativaId).FirstOrDefault().OfertaEducativaTipo.AntecedenteTipoId;
                        db.AlumnoAntecedente.Add(
                           new AlumnoAntecedente
                           {
                               AlumnoId = db.Alumno.Local[0].AlumnoId,
                               Anio = objAlumnoInscrito.Anio,
                               AntecedenteTipoId = (int)ofertatios,
                               AreaAcademicaId = objProspectoDetalle.PrepaArea,
                               EntidadFederativaId = objProspectoDetalle.PrepaPaisId == 146 ?
                                     (int)objProspectoDetalle.PrepaEntidadId : 33,
                               EsEquivalencia = (bool)objProspectoDetalle.EsEquivalencia,
                               EsTitulado = (bool)objProspectoDetalle.EsTitulado,
                               FechaRegistro = DateTime.Now,
                               MedioDifusionId = (int)objProspectoDetalle.MedioDifusionId,
                               MesId = (int)objProspectoDetalle.PrepaMes,
                               PaisId = (int)objProspectoDetalle.PrepaPaisId,
                               Procedencia = objProspectoDetalle.PrepaProcedencia,
                               Promedio = (decimal)objProspectoDetalle.PrepaPromedio,
                               UsuarioId = objAlumno.UsuarioId,
                               EscuelaEquivalencia = objProspectoDetalle.UniversidadProcedencia,
                               TitulacionMedio = objProspectoDetalle.UniversidadMotivo == null ? string.Empty : objProspectoDetalle.UniversidadMotivo
                           });
                    }

                    db.SaveChanges();

                    return db.Alumno.Local[0].AlumnoId.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

            }
        }

        public static List<DTOAlumnoLigero> ListarAlumnos()
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    List<DTOAlumnoLigero> lstAlumnos = ((from a in db.Alumno
                                                         where a.EstatusId != 3
                                                         //where a.FechaRegistro.Year >= DateTime.Now.Year - 7
                                                         select new DTOAlumnoLigero
                                                         {
                                                             AlumnoId = a.AlumnoId,
                                                             Nombre = a.Nombre + " " + a.Paterno + " " + a.Materno,
                                                             FechaRegistro = a.FechaRegistro.ToString(),
                                                             Usuario = a.Usuario.Nombre,
                                                             OfertasEducativas = a.AlumnoInscrito
                                                                                .Select(AI => AI.OfertaEducativa.Descripcion)
                                                                                    .ToList(),
                                                         }).AsNoTracking().ToList());
                    return lstAlumnos;
                }
                catch { return null; }
            }
        }

        public static DTOAlumno ObtenerAlumnoR(int AlumnoId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    DTOAlumno objAlumno = (from a in db.Alumno
                                           where a.AlumnoId == AlumnoId
                                           select new DTOAlumno
                                           {
                                               Grupo = new DTOGrupo
                                               {
                                                   Descripcion = a.GrupoAlumnoConfiguracion.Where(o => o.AlumnoId == a.AlumnoId && o.EstatusId == 1).ToList().Count > 0 ?
                                                                a.GrupoAlumnoConfiguracion.Where(o => o.AlumnoId == a.AlumnoId && o.EstatusId == 1).FirstOrDefault().Grupo.Descripcion : ""
                                               },
                                               AlumnoId = a.AlumnoId,
                                               Nombre = a.Nombre,
                                               Paterno = a.Paterno,
                                               Materno = a.Materno,
                                               FechaRegistro = a.FechaRegistro.Day + "/" + a.FechaRegistro.Month + "/" + a.FechaRegistro.Year,
                                               DTOAlumnoDetalle = new DTOAlumnoDetalle
                                               {
                                                   AlumnoId = a.AlumnoDetalle.AlumnoId,
                                                   FechaNacimiento = a.AlumnoDetalle.FechaNacimiento,
                                                   Email = a.AlumnoDetalle.Email
                                               },
                                               AlumnoInscrito = a.AlumnoInscrito.Where(a2 => a2.OfertaEducativa.OfertaEducativaTipoId != 4)
                                                                .Select(Ai => new DTOAlumnoInscrito
                                                                {
                                                                    AlumnoId = Ai.AlumnoId,
                                                                    OfertaEducativaId = Ai.OfertaEducativaId,
                                                                    OfertaEducativa = new DTOOfertaEducativa
                                                                    {
                                                                        OfertaEducativaId = Ai.OfertaEducativa.OfertaEducativaId,
                                                                        Descripcion = Ai.OfertaEducativa.Descripcion
                                                                    }
                                                                }).FirstOrDefault(),
                                               Usuario = new DTOUsuario
                                               {
                                                   UsuarioId = a.UsuarioId,
                                                   Nombre = a.Usuario.Nombre
                                               }

                                           }).AsNoTracking().FirstOrDefault();
                    //objAlumno.AlumnoInscrito.OfertaEducativa.Descripcion = objAlumno.AlumnoInscrito != null ? objAlumno.AlumnoInscrito.OfertaEducativa.Descripcion : "";
                    objAlumno.AlumnoInscrito = objAlumno.AlumnoInscrito == null ? new DTOAlumnoInscrito { OfertaEducativa = new DTOOfertaEducativa { Descripcion = "" } } : objAlumno.AlumnoInscrito;
                    objAlumno.AlumnoInscrito.EsEmpresa = db.AlumnoInscrito.Where(a => a.AlumnoId == AlumnoId).ToList().Count > 0 ?
                        db.AlumnoInscrito.Where(a => a.AlumnoId == AlumnoId).ToList().Where(a => a.EsEmpresa == true).ToList().Count > 0 ?
                        true : false : false;
                    objAlumno.lstAlumnoInscrito = db.AlumnoInscrito.Where(A => A.AlumnoId == AlumnoId && A.OfertaEducativa.OfertaEducativaTipoId != 4)
                                                .ToList().ConvertAll(new Converter<AlumnoInscrito, DTOAlumnoInscrito>(Convertidor.ToDTOAlumnoInscrito));
                    var lstAlumno = objAlumno.lstAlumnoInscrito.GroupBy(v => v.OfertaEducativaId).Select(A => A.ToList()).ToList();

                    objAlumno.lstAlumnoInscrito.Clear();
                    lstAlumno.ForEach(a =>
                    {
                        objAlumno.lstAlumnoInscrito.Add(a.FirstOrDefault());
                    });

                    return objAlumno;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static DTOAlumnoReferencias1 ObtenerReferenciasAlumno(int alumnoint, int pagoid)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {


                DTOAlumnoLigero1 alumno = new DTOAlumnoLigero1();

                var estatus = new int[] { 2 };

                if (pagoid == 0)
                {
                    alumno = db.Alumno
                               .Where(a => a.AlumnoId == alumnoint)
                               .Select(b => new DTOAlumnoLigero1
                               {
                                   AlumnoId = b.AlumnoId,
                                   Nombre = b.Nombre + " " + b.Paterno + " " + b.Materno
                               }).FirstOrDefault();

                    var referencias = db.Pago
                                        .Where(a => a.AlumnoId == alumnoint && !estatus.Contains(a.EstatusId) && a.Promesa > 0)
                                        .Select(b => new DTOAlumnoReferencias
                                        {
                                            Concepto = b.Cuota1.PagoConceptoId == 800 || b.Cuota1.PagoConceptoId == 802 ? (b.Cuota1.PagoConcepto.Descripcion +
                                                                                    " " + b.Subperiodo.Mes.Descripcion +
                                                                                    " " + b.Periodo.FechaInicial.Year.ToString())
                                                                                    : b.Cuota1.PagoConcepto.Descripcion
                                          + " " + (b.Cuota1.PagoConceptoId == 306 ?
                                          b.PagoRecargo1.FirstOrDefault().Pago.Cuota1.PagoConcepto.Descripcion
                                          + " " + b.PagoRecargo1.FirstOrDefault().Pago.Subperiodo.Mes.Descripcion + " " +
                                          b.PagoRecargo1.FirstOrDefault().Pago.Periodo.FechaInicial.Year.ToString()
                                          : ""),

                                            ReferenciaBancaria1 = b.ReferenciaId,
                                            FechaGeneracion1 = b.FechaGeneracion,
                                            UsuarioGenero = b.UsuarioId.ToString(),
                                            UsuarioTipo = b.UsuarioTipoId,
                                            Promesa = "$ " + b.Promesa,
                                            Pagado = "$ " + db.PagoParcial.Where(q => q.PagoId == b.PagoId).Select(w => w.Pago).Sum(),
                                            Estatus = b.Estatus.Descripcion == "Activo" ? "Pendiente" : b.Estatus.Descripcion,
                                            EnProcesoSolicitud = db.PagoCancelacionSolicitud.Count(f => f.PagoId == b.PagoId) > 0
                                                               && db.PagoCancelacionSolicitud.Where(c => c.PagoId == b.PagoId).OrderByDescending(f=> f.FechaSolicitud).FirstOrDefault().EstatusId == 1 ? 2
                                                               : db.PagoCancelacionSolicitud.Count(f => f.PagoId == b.PagoId) > 0 && db.PagoCancelacionSolicitud.Where(c => c.PagoId == b.PagoId).OrderByDescending(f1 => f1.FechaSolicitud).FirstOrDefault().EstatusId != 3 ? 2 : 1
                                        }).ToList();

                    List<DTOAlumnoReferencias> referencias2 = new List<DTOAlumnoReferencias>();

                    referencias.ForEach(n =>
                    {


                        int usuarioint = Int32.Parse(n.UsuarioGenero);
                        var usuario = n.UsuarioTipo == 2 ? "Alumno | " + db.Alumno.Where(e => e.AlumnoId == usuarioint).Select(r => r.Nombre + " " + r.Paterno + " " + r.Materno).FirstOrDefault()
                                                                                            : db.UsuarioTipo.Where(t => t.UsuarioTipoId == n.UsuarioTipo).Select(y => y.Descripcion).FirstOrDefault() + " | "
                                                                                            + db.Usuario.Where(u => u.UsuarioId == usuarioint).Select(i => i.Nombre + " " + i.Paterno + " " + i.Materno).FirstOrDefault();

                        referencias2.Add(new DTOAlumnoReferencias
                        {
                            Concepto = n.Concepto,
                            ReferenciaBancaria = Int32.Parse(n.ReferenciaBancaria1),
                            FechaGeneracion = n.FechaGeneracion1.ToString("dd/MM/yyyy", Cultura),
                            UsuarioGenero = usuario,
                            Promesa = n.Promesa,
                            Pagado = n.Pagado == "$ " ? "$ 0.00" : n.Pagado,
                            Estatus = n.Estatus,
                            EnProcesoSolicitud = n.EnProcesoSolicitud
                        });
                    });

                    referencias2 = referencias2.OrderByDescending(a => a.ReferenciaBancaria).ToList();

                    var objreferencias = new DTOAlumnoReferencias1 { AlumnoDatos = alumno, AlumnoReferencias = referencias2 };

                    return objreferencias;
                }
                else
                {
                    var aux = pagoid.ToString();

                    aux = aux.Remove(aux.Length - 1);

                    var id = int.Parse(aux);
                    var referencias = db.Pago
                                        .Where(a => a.PagoId == id)
                                        .Select(b => new DTOAlumnoReferencias
                                        {
                                            Concepto = b.Cuota1.PagoConceptoId == 800 || b.Cuota1.PagoConceptoId == 802 ? (b.Cuota1.PagoConcepto.Descripcion +
                                                                                    " " + b.Subperiodo.Mes.Descripcion +
                                                                                    " " + b.Periodo.FechaInicial.Year.ToString())
                                                                                    : b.Cuota1.PagoConcepto.Descripcion
                                          + " " + (b.Cuota1.PagoConceptoId == 306 ?
                                          b.PagoRecargo1.FirstOrDefault().Pago.Cuota1.PagoConcepto.Descripcion
                                          + " " + b.PagoRecargo1.FirstOrDefault().Pago.Subperiodo.Mes.Descripcion + " " +
                                          b.PagoRecargo1.FirstOrDefault().Pago.Periodo.FechaInicial.Year.ToString()
                                          : ""),
                                            ReferenciaBancaria1 = b.ReferenciaId,
                                            FechaGeneracion1 = b.FechaGeneracion,
                                            UsuarioGenero = b.UsuarioId.ToString(),
                                            UsuarioTipo = b.UsuarioTipoId,
                                            Promesa = "$ " + b.Promesa,
                                            Pagado = "$ " + db.PagoParcial.Where(q => q.PagoId == b.PagoId).Select(w => w.Pago).Sum(),
                                            Estatus = b.Estatus.Descripcion == "Activo" ? "Pendiente" : b.Estatus.Descripcion,
                                            AlumnoId = b.AlumnoId,
                                            EnProcesoSolicitud = db.PagoCancelacionSolicitud.Count(f => f.PagoId == b.PagoId) > 0
                                                               && db.PagoCancelacionSolicitud.Where(c => c.PagoId == b.PagoId).FirstOrDefault().EstatusId == 3 ? 1
                                                               : db.PagoCancelacionSolicitud.Count(f => f.PagoId == b.PagoId) > 0 && db.PagoCancelacionSolicitud.Where(c => c.PagoId == b.PagoId).FirstOrDefault().EstatusId != 3 ? 2 : 1
                                        }).ToList();



                    List<DTOAlumnoReferencias> referencias2 = new List<DTOAlumnoReferencias>();

                    referencias.ForEach(n =>
                    {

                        int usuarioint = Int32.Parse(n.UsuarioGenero);
                        var usuario = n.UsuarioTipo == 2 ? "Alumno | " + db.Alumno.Where(e => e.AlumnoId == usuarioint).Select(r => r.Nombre + " " + r.Paterno + " " + r.Materno).FirstOrDefault()
                                                                                            : db.UsuarioTipo.Where(t => t.UsuarioTipoId == n.UsuarioTipo).Select(y => y.Descripcion).FirstOrDefault() + " | "
                                                                                            + db.Usuario.Where(u => u.UsuarioId == usuarioint).Select(i => i.Nombre + " " + i.Paterno + " " + i.Materno).FirstOrDefault();

                        referencias2.Add(new DTOAlumnoReferencias
                        {
                            Concepto = n.Concepto,
                            ReferenciaBancaria = Int32.Parse(n.ReferenciaBancaria1),
                            FechaGeneracion = n.FechaGeneracion1.ToString("dd/MM/yyyy", Cultura),
                            UsuarioGenero = usuario,
                            Promesa = n.Promesa,
                            Pagado = n.Pagado == "$ " ? "$ 0.00" : n.Pagado,
                            Estatus = n.Estatus,
                            EnProcesoSolicitud = n.EnProcesoSolicitud
                        });

                    });

                    referencias2 = referencias2.OrderByDescending(a => a.ReferenciaBancaria).ToList();


                    var alumnoid = referencias.FirstOrDefault().AlumnoId;

                    alumno = db.Alumno
                               .Where(a => a.AlumnoId == alumnoid)
                               .Select(b => new DTOAlumnoLigero1
                               {
                                   AlumnoId = b.AlumnoId,
                                   Nombre = b.Nombre + " " + b.Paterno + " " + b.Materno
                               }).FirstOrDefault();

                    var objreferencias = new DTOAlumnoReferencias1 { AlumnoDatos = alumno, AlumnoReferencias = referencias2 };

                    return objreferencias;

                }




            }



        }

        public static DTOAlumno ObtenerAlumno(int AlumnoId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {

                    DTOAlumno objAlumno = (from a in db.Alumno
                                           where a.AlumnoId == AlumnoId
                                           select new DTOAlumno
                                           {
                                               AlumnoId = a.AlumnoId,
                                               Nombre = a.Nombre,
                                               Paterno = a.Paterno,
                                               Materno = a.Materno,
                                               FechaRegistro = a.FechaRegistro.Day + "/" + a.FechaRegistro.Month + "/" + a.FechaRegistro.Year,
                                               DTOAlumnoDetalle = new DTOAlumnoDetalle
                                               {
                                                   AlumnoId = a.AlumnoDetalle.AlumnoId,
                                                   FechaNacimiento = a.AlumnoDetalle.FechaNacimiento,
                                                   Email = a.AlumnoDetalle.Email
                                               },
                                               AlumnoInscrito = a.AlumnoInscrito.Select(Ai => new DTOAlumnoInscrito
                                               {
                                                   AlumnoId = Ai.AlumnoId,
                                                   OfertaEducativaId = Ai.OfertaEducativaId,
                                                   OfertaEducativa = new DTOOfertaEducativa
                                                   {
                                                       OfertaEducativaId = Ai.OfertaEducativa.OfertaEducativaId,
                                                       Descripcion = Ai.OfertaEducativa.Descripcion
                                                   }
                                               }).FirstOrDefault(),
                                               Usuario = new DTOUsuario
                                               {
                                                   UsuarioId = a.UsuarioId,
                                                   Nombre = a.Usuario.Nombre
                                               }

                                           }).AsNoTracking().FirstOrDefault();
                    objAlumno.Grupo = new DTOGrupo
                    {
                        Descripcion = db.GrupoAlumnoConfiguracion
                                           .Where(k=> k.AlumnoId==AlumnoId).ToList()?.LastOrDefault()?.Grupo?.Descripcion ?? ""
                    };
                    //objAlumno.AlumnoInscrito.OfertaEducativa.Descripcion = objAlumno.AlumnoInscrito != null ? objAlumno.AlumnoInscrito.OfertaEducativa.Descripcion : "";
                    objAlumno.AlumnoInscrito = objAlumno.AlumnoInscrito == null ? new DTOAlumnoInscrito { OfertaEducativa = new DTOOfertaEducativa { Descripcion = "" } } : objAlumno.AlumnoInscrito;
                    objAlumno.AlumnoInscrito.EsEmpresa = db.AlumnoInscrito.Where(a => a.AlumnoId == AlumnoId).ToList().Count > 0 ?
                        db.AlumnoInscrito.Where(a => a.AlumnoId == AlumnoId).ToList().Where(a => a.EsEmpresa == true).ToList().Count > 0 ?
                        true : false : false;

                    objAlumno.AlumnoInscrito.EsEspecial =
                                    db.GrupoAlumnoConfiguracion.Where(k => k.AlumnoId == AlumnoId && k.EsEspecial == true).ToList().Count > 0 ? true : false;

                    objAlumno.lstAlumnoInscrito = db.AlumnoInscrito.Where(A => A.AlumnoId == AlumnoId).ToList().ConvertAll(new Converter<AlumnoInscrito, DTOAlumnoInscrito>(Convertidor.ToDTOAlumnoInscrito));
                    var lstAlumno = objAlumno.lstAlumnoInscrito.GroupBy(v => v.OfertaEducativaId).Select(A => A.ToList()).ToList();

                    objAlumno.lstAlumnoInscrito.Clear();
                    lstAlumno.ForEach(a =>
                    {
                        objAlumno.lstAlumnoInscrito.Add(a.FirstOrDefault());
                    });

                    return objAlumno;
                }
                catch
                {
                    return null;
                }
            }
        }
        //promocion en casa
        public static DTOAlumnoPromocionCasa ConsultarAlumnoPromocionCasa(int AlumnoId, int TA)
        {
            try
            {
                using (UniversidadEntities db = new UniversidadEntities ())
                {
                    var fechaactual = DateTime.Now;
                    var periodoActual = db.Periodo.Where(p => p.FechaInicial <= fechaactual && p.FechaFinal >= fechaactual).FirstOrDefault();
                    if (TA == 1)
                    {
                        var alumno = db.Alumno.Where(a => a.AlumnoId == AlumnoId)
                                          .Select(s => new DTOAlumnoPromocionCasa
                                          {
                                              AlumnoId = s.AlumnoId,
                                              NombreC = s.Nombre + " " + s.Paterno + " " + s.Materno,
                                              OfertaEducativa = s.AlumnoInscrito.Where(w => w.Anio == periodoActual.Anio && w.PeriodoId == periodoActual.PeriodoId)
                                                                                .Select(oe => new DTOOfertaEducativa1
                                                                                {
                                                                                    ofertaEducativaId = oe.OfertaEducativaId,
                                                                                    descripcion = oe.OfertaEducativa.Descripcion
                                                                                }).ToList(),
                                              AlumnoProspecto = false

                                          }).FirstOrDefault();
                        return alumno;
                    }
                    else
                    {
                        var Estatusid = new int[] { 4, 14 };
                        var pagoconcepto = new int[] { 802,800 };

                        var periodoAnterior = db.Periodo.Where(p => p.FechaFinal < periodoActual.FechaInicial).OrderByDescending(d=> d.FechaFinal).FirstOrDefault();
                        var adeudos = db.Pago.Where(p => p.Anio == periodoAnterior.Anio
                                                    && p.PeriodoId == periodoAnterior.PeriodoId
                                                    && pagoconcepto.Contains(p.Cuota1.PagoConceptoId)
                                                    && !Estatusid.Contains(p.EstatusId)
                                                    && p.AlumnoId == AlumnoId
                                                    ).Count();

                        var alumno2 = db.AlumnoInscrito.Where(q => q.AlumnoId == AlumnoId
                                                              &&   q.Anio == periodoActual.Anio
                                                              &&   q.PeriodoId == periodoActual.PeriodoId
                                                              &&   q.OfertaEducativa.OfertaEducativaTipoId != 4)
                                                       .Select(z => new DTOAlumnoPromocionCasa
                                                       {
                                                           AlumnoIdProspecto = z.AlumnoId,
                                                           NombreCProspecto = z.Alumno.Nombre + " " +z.Alumno.Paterno + " " + z.Alumno.Materno, 
                                                           OfertaEducativaIdProspecto = z.OfertaEducativaId,
                                                           OfertaEducativaProspecto = z.OfertaEducativa.Descripcion,
                                                           AlumnoProspecto = adeudos == 0 ? true : false
                                                       }).FirstOrDefault();
                        return alumno2;
                        
                    }
                    
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static List<DTOPeriodoPromocionCasa> PeriodosPromocionCasa()
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    var fechaactual = DateTime.Now;
                    var periodos = db.Periodo.Where(p => p.FechaFinal >= fechaactual).Take(2)
                                             .Select(s => new DTOPeriodoPromocionCasa
                                             {
                                                 Descripcion = s.Descripcion,
                                                 Anio = s.Anio,
                                                 PeriodoId = s.PeriodoId,
                                                 Meses = db.Subperiodo.Where(sp => sp.PeriodoId == s.PeriodoId)
                                                                      .Select(d => new DTOMes
                                                                      {
                                                                          Descripcion = d.Mes.Descripcion,
                                                                          MesId = d.SubperiodoId
                                                                      }).ToList()
                                             }).ToList();
                    return periodos;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        public static string AplicarPromocionCasa(DTOAlumnoPromocionCasa Promocion)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    if(db.PromocionCasa.Where(a => a.AlumnoId == Promocion.AlumnoId
                                      && a.Anio == Promocion.Anio
                                      && a.PeriodoId == Promocion.PeriodoId
                                      && a.OfertaEducativaId == Promocion.OfertaEducativaIdActual)?.FirstOrDefault() != null)
                    {
                        return "Ya tiene";
                    }

                    db.PromocionCasa.Add(new PromocionCasa
                    {
                        AlumnoId = Promocion.AlumnoId,
                        OfertaEducativaId = Promocion.OfertaEducativaIdActual,
                        AlumnoIdProspecto = Promocion.AlumnoIdProspecto,
                        Anio = Promocion.Anio,
                        PeriodoId = Promocion.PeriodoId,
                        SubPeriodoId = Promocion.SubPeriodoId,
                        Monto = Promocion.Monto,
                        FechaGeneracion = DateTime.Now,
                        HoraGeneracion = DateTime.Now.TimeOfDay,
                        EstatusId =  1
                    });

                    //ver si hay referecias generadas de ese subperido
                    var pago = db.Pago.Where(p => p.AlumnoId == Promocion.AlumnoId
                                                  && p.OfertaEducativaId == Promocion.OfertaEducativaIdActual
                                                  && p.Anio == Promocion.Anio
                                                  && p.PeriodoId == Promocion.PeriodoId
                                                  && p.SubperiodoId == Promocion.SubPeriodoId
                                                  && p.Cuota1.PagoConceptoId == 800)?.FirstOrDefault() ?? null;

                    if (pago != null)
                    {
                        var Estatusid = new int[] { 4, 14 };

                        //obtener descuentoId de Promocion en casa
                        var descuentoid = db.Descuento.Where(d => d.PagoConceptoId == 800
                                                             && d.OfertaEducativaId == Promocion.OfertaEducativaIdActual
                                                             && d.Descripcion.Contains("Promoción en Casa")).FirstOrDefault().DescuentoId;


                        if (Estatusid.Contains(pago.EstatusId)) /// pagado
                        {
                            if (pago.Promesa >= Promocion.Monto)
                            {

                                pago.Promesa = pago.Promesa - Promocion.Monto;

                                // generar pagodescuento 
                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = pago.PagoId,
                                    DescuentoId = descuentoid,
                                    Monto = Promocion.Monto
                                });

                                //obtener lista pago parcial
                                var lstPP = db.PagoParcial.Where(pp => pp.PagoId == pago.PagoId && pp.EstatusId == 4).ToList();

                                var auxMonto = Promocion.Monto;
                                lstPP.ForEach(n =>
                                {
                                    if (auxMonto > 0)
                                    {
                                        if (n.Pago <= auxMonto)
                                        {
                                            n.ReferenciaProcesada.Restante = n.ReferenciaProcesada.Restante + n.Pago;
                                            n.ReferenciaProcesada.SeGasto = n.ReferenciaProcesada.Restante > 0 ? false : true;
                                            auxMonto = auxMonto - n.Pago;
                                            n.Pago = 0;
                                            n.EstatusId = 2;
                                        }
                                        else
                                        {
                                            n.ReferenciaProcesada.Restante = n.ReferenciaProcesada.Restante + (decimal)auxMonto;
                                            n.ReferenciaProcesada.SeGasto = n.ReferenciaProcesada.Restante > 0 ? false : true;
                                            n.Pago = n.Pago - (decimal)auxMonto;
                                        }
                                    }//if (promocion.Monto < 0)


                                });


                            }// if (pago.Promesa >= promocion.Monto)

                        }
                        else if (pago.EstatusId == 1)  // sin pagar
                        {
                            if (pago.Promesa >= Promocion.Monto)
                            {

                                if (pago.Restante < pago.Promesa)// los que ya tiene pagos en la referencia
                                {
                                    pago.Promesa = pago.Promesa - Promocion.Monto;

                                    // generar pagodescuento 
                                    db.PagoDescuento.Add(new PagoDescuento
                                    {
                                        PagoId = pago.PagoId,
                                        DescuentoId = descuentoid,
                                        Monto = Promocion.Monto
                                    });

                                    ///verificar si el restante es mayoe al descuento
                                    if (pago.Restante >= Promocion.Monto)
                                    {
                                        pago.Restante = pago.Restante - Promocion.Monto;

                                    } else
                                    {
                                        var auxMonto =   Promocion.Monto - pago.Restante ;
                                        pago.Restante = 0;

                                        //obtener lista pago parcial
                                        var lstPP = db.PagoParcial.Where(pp => pp.PagoId == pago.PagoId && pp.EstatusId == 4).ToList();

                                        
                                        lstPP.ForEach(n =>
                                        {
                                            if (auxMonto > 0)
                                            {
                                                if (n.Pago <= auxMonto)
                                                {
                                                    n.ReferenciaProcesada.Restante = n.ReferenciaProcesada.Restante + n.Pago;
                                                    n.ReferenciaProcesada.SeGasto = n.ReferenciaProcesada.Restante > 0 ? false : true;
                                                    auxMonto = auxMonto - n.Pago;
                                                    n.Pago = 0;
                                                    n.EstatusId = 2;
                                                }
                                                else
                                                {
                                                    n.ReferenciaProcesada.Restante = n.ReferenciaProcesada.Restante + (decimal)auxMonto;
                                                    n.ReferenciaProcesada.SeGasto = n.ReferenciaProcesada.Restante > 0 ? false : true;
                                                    n.Pago = n.Pago - (decimal)auxMonto;
                                                }
                                            }//if (promocion.Monto < 0)


                                        });//fin foreach
                                        

                                    }// if (pago.Restante >= promocion.Monto)
                                    
                                }else
                                {
                                    pago.Promesa = pago.Promesa - Promocion.Monto;
                                    pago.Restante = pago.Restante - Promocion.Monto;
                                }//  if (pago.Restante < pago.Promesa )

                                if (pago.Restante == 0)
                                {
                                    pago.EstatusId = 4;
                                }

                            }// if (pago.Promesa >= promocion.Monto)

                        }
                        db.SaveChanges();

                        var promocion = db.PromocionCasa.Where(a => a.AlumnoId == Promocion.AlumnoId
                                    && a.Anio == Promocion.Anio
                                    && a.PeriodoId == Promocion.PeriodoId
                                    && a.OfertaEducativaId == Promocion.OfertaEducativaIdActual)?.FirstOrDefault() ?? null;

                        promocion.FechaAplicacion = DateTime.Now;
                        promocion.HoraAplicacion = DateTime.Now.TimeOfDay;
                        promocion.PagoId = pago.PagoId;
                        promocion.EstatusId = 7;

                        db.SaveChanges();

                        return "Aplicada";
                    }//if (pago != null )
                    else
                    {
                        db.SaveChanges();
                        return "Pendiente";
                    }

                    
                }
                catch (Exception)
                {
                    return "Error";
                }
            }
        }

        public static void AplicaPromocionCasa2(int AlumnoId, int Anio, int PeriodoId, int OfertaEducativaId)
        {
            try
            {
                using (UniversidadEntities db = new UniversidadEntities())
                {
                    var Estatusid = new int[] { 4, 14 };

                    // ver si tiene promocion en casa 
                    var promocion = db.PromocionCasa.Where(a => a.AlumnoId == AlumnoId
                                         && a.Anio == Anio
                                         && a.PeriodoId == PeriodoId
                                         && a.OfertaEducativaId == OfertaEducativaId
                                         && a.EstatusId == 1)?.FirstOrDefault() ?? null;

                    if (promocion != null)
                    {

                        //ver si hay referecias generadas de ese subperido
                        var pago = db.Pago.Where(p => p.AlumnoId == promocion.AlumnoId
                                                      && p.OfertaEducativaId == promocion.OfertaEducativaId
                                                      && p.Anio == promocion.Anio
                                                      && p.PeriodoId == promocion.PeriodoId
                                                      && p.SubperiodoId == promocion.SubPeriodoId
                                                      && p.Cuota1.PagoConceptoId == 800)?.FirstOrDefault() ?? null;

                        if (pago != null)
                        {
                            //obtener descuentoId de Promocion en casa
                            var descuentoid = db.Descuento.Where(d => d.PagoConceptoId == 800
                                                                 && d.OfertaEducativaId == promocion.OfertaEducativaId
                                                                 && d.Descripcion.Contains("Promoción en Casa")).FirstOrDefault().DescuentoId;


                            if (Estatusid.Contains(pago.EstatusId)) /// pagado
                            {
                                if (pago.Promesa >= promocion.Monto)
                                {

                                    pago.Promesa = pago.Promesa - (decimal)promocion.Monto;

                                    // generar pagodescuento 
                                    db.PagoDescuento.Add(new PagoDescuento
                                    {
                                        PagoId = pago.PagoId,
                                        DescuentoId = descuentoid,
                                        Monto = (decimal)promocion.Monto
                                    });

                                    //obtener lista pago parcial
                                    var lstPP = db.PagoParcial.Where(pp => pp.PagoId == pago.PagoId && pp.EstatusId == 4).ToList();

                                    var auxMonto = promocion.Monto;
                                    lstPP.ForEach(n =>
                                    {
                                        if (auxMonto > 0)
                                        {
                                            if (n.Pago <= auxMonto)
                                            {
                                                n.ReferenciaProcesada.Restante = n.ReferenciaProcesada.Restante + n.Pago;
                                                n.ReferenciaProcesada.SeGasto = n.ReferenciaProcesada.Restante > 0 ? false : true;
                                                auxMonto = auxMonto - n.Pago;
                                                n.Pago = 0;
                                                n.EstatusId = 2;
                                            }
                                            else
                                            {
                                                n.ReferenciaProcesada.Restante = n.ReferenciaProcesada.Restante + (decimal)auxMonto;
                                                n.ReferenciaProcesada.SeGasto = n.ReferenciaProcesada.Restante > 0 ? false : true;
                                                n.Pago = n.Pago - (decimal)auxMonto;
                                            }
                                        }//if (promocion.Monto < 0)


                                    });


                                }// if (pago.Promesa >= promocion.Monto)

                            }
                            else if (pago.EstatusId == 1)  // sin pagar
                            {
                                if (pago.Promesa >= promocion.Monto)
                                {

                                    if (pago.Restante < pago.Promesa)// los que ya tiene pagos en la referencia
                                    {
                                        pago.Promesa = pago.Promesa - (decimal)promocion.Monto;

                                        // generar pagodescuento 
                                        db.PagoDescuento.Add(new PagoDescuento
                                        {
                                            PagoId = pago.PagoId,
                                            DescuentoId = descuentoid,
                                            Monto = (decimal)promocion.Monto
                                        });

                                        ///verificar si el restante es mayoe al descuento
                                        if (pago.Restante >= promocion.Monto)
                                        {
                                            pago.Restante = pago.Restante - (decimal)promocion.Monto;

                                        }
                                        else
                                        {
                                            var auxMonto = (decimal)promocion.Monto - pago.Restante;
                                            pago.Restante = 0;

                                            //obtener lista pago parcial
                                            var lstPP = db.PagoParcial.Where(pp => pp.PagoId == pago.PagoId && pp.EstatusId == 4).ToList();


                                            lstPP.ForEach(n =>
                                            {
                                                if (auxMonto > 0)
                                                {
                                                    if (n.Pago <= auxMonto)
                                                    {
                                                        n.ReferenciaProcesada.Restante = n.ReferenciaProcesada.Restante + n.Pago;
                                                        n.ReferenciaProcesada.SeGasto = n.ReferenciaProcesada.Restante > 0 ? false : true;
                                                        auxMonto = auxMonto - n.Pago;
                                                        n.Pago = 0;
                                                        n.EstatusId = 2;
                                                    }
                                                    else
                                                    {
                                                        n.ReferenciaProcesada.Restante = n.ReferenciaProcesada.Restante + auxMonto;
                                                        n.ReferenciaProcesada.SeGasto = n.ReferenciaProcesada.Restante > 0 ? false : true;
                                                        n.Pago = n.Pago - auxMonto;
                                                    }
                                                }//if (promocion.Monto < 0)


                                            });//fin foreach


                                        }// if (pago.Restante >= promocion.Monto)

                                    }
                                    else
                                    {
                                        pago.Promesa = pago.Promesa - (decimal)promocion.Monto;
                                        pago.Restante = pago.Restante - (decimal)promocion.Monto;

                                        // generar pagodescuento 
                                        db.PagoDescuento.Add(new PagoDescuento
                                        {
                                            PagoId = pago.PagoId,
                                            DescuentoId = descuentoid,
                                            Monto = (decimal)promocion.Monto
                                        });

                                    }//  if (pago.Restante < pago.Promesa )

                                    if (pago.Restante == 0)
                                    {
                                        pago.EstatusId = 4;
                                    }

                                }// if (pago.Promesa >= promocion.Monto)

                            }
                            db.SaveChanges();

                            promocion.FechaAplicacion = DateTime.Now;
                            promocion.HoraAplicacion = DateTime.Now.TimeOfDay;
                            promocion.PagoId = pago.PagoId;
                            promocion.EstatusId = 7;

                            db.SaveChanges();

                        }//if (pago != null )

                    }// if (promocion > 0 )
                    
                }// fin using
            }
            catch (Exception)
            {

                throw;
            }
        }
        //promocion en casa

        public static DTOAlumno ObtenerAlumno1(int AlumnoId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    DTOAlumno objAlumno = (from a in db.Alumno
                                           where a.AlumnoId == AlumnoId
                                           select new DTOAlumno
                                           {
                                               AlumnoId = a.AlumnoId,
                                               Nombre = a.Nombre,
                                               Paterno = a.Paterno,
                                               Materno = a.Materno,
                                               FechaRegistro = a.FechaRegistro.Day + "/" + a.FechaRegistro.Month + "/" + a.FechaRegistro.Year,
                                               DTOAlumnoDetalle = new DTOAlumnoDetalle
                                               {
                                                   AlumnoId = a.AlumnoDetalle.AlumnoId,
                                                   FechaNacimiento = a.AlumnoDetalle.FechaNacimiento,
                                                   Email = a.AlumnoDetalle.Email
                                               },
                                               Usuario = new DTOUsuario
                                               {
                                                   UsuarioId = a.UsuarioId,
                                                   Nombre = a.Usuario.Nombre
                                               }
                                           }).AsNoTracking().FirstOrDefault();
                    return objAlumno;
                }
                catch
                {
                    return null;
                }
            }
        }
        public static List<DTOAlumnoOfertas> ObtenerOfertasAlumno(int v)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    List<DTOAlumnoOfertas> lstOFerta = db.AlumnoInscrito.Where(a => a.AlumnoId == v)
                        .ToList().OrderByDescending(s => s.Anio).ThenBy(x => x.PeriodoId)
                        .Select(a => new DTOAlumnoOfertas
                        {
                            OfertaEducativaId = a.OfertaEducativaId,
                            Descripcion = a.Anio + "-" + a.PeriodoId + " " + a.OfertaEducativa.Descripcion
                        }).ToList();


                    return lstOFerta;
                }
                catch
                {
                    return null;
                }
            }
        }
        public static DTOAlumno ObtenerAlumnoCompleto(int AlumnoId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    Alumno objAlB = db.Alumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault();

                    objAlB.AlumnoInscrito = new List<AlumnoInscrito>(objAlB.AlumnoInscrito
                        .OrderBy(o => o.Anio).ThenBy(o=> o.PeriodoId).Reverse());
                    

                    DTOAlumno objAlumno = new DTOAlumno
                    {
                        AlumnoId = AlumnoId,
                        Nombre = objAlB.Nombre,
                        Paterno = objAlB.Paterno,
                        Materno = objAlB.Materno,
                        Matricula = objAlB.MatriculaId,
                        DTOAlumnoDetalle = new DTOAlumnoDetalle
                        {
                            EstadoCivilId = objAlB.AlumnoDetalle.EstadoCivilId,
                            Celular = (objAlB.AlumnoDetalle.Celular.Trim()).Replace("-",""),
                            TelefonoCasa = (objAlB.AlumnoDetalle.TelefonoCasa.Trim()).Replace("-",""),
                            FechaNacimiento = objAlB.AlumnoDetalle.FechaNacimiento,
                            FechaNacimientoC = objAlB.AlumnoDetalle.FechaNacimiento.ToString("dd-MM-yyyy", Cultura),
                            GeneroId = objAlB.AlumnoDetalle.GeneroId,
                            CURP = objAlB.AlumnoDetalle.CURP,
                            Email = objAlB.AlumnoDetalle.Email.Trim(),
                            Calle = objAlB.AlumnoDetalle.Calle.Trim(),
                            NoExterior = objAlB.AlumnoDetalle.NoExterior,
                            NoInterior = objAlB.AlumnoDetalle.NoInterior,
                            Cp = objAlB.AlumnoDetalle.CP.Trim(),
                            Colonia = objAlB.AlumnoDetalle.Colonia.Trim(),
                            EntidadFederativaId = objAlB.AlumnoDetalle.EntidadFederativaId,
                            MunicipioId = objAlB.AlumnoDetalle.MunicipioId,
                            PaisId = objAlB.AlumnoDetalle.PaisId,
                            EntidadNacimientoId = objAlB.AlumnoDetalle.EntidadNacimientoId
                        },
                        lstOfertas = objAlB.AlumnoInscrito                                                
                        .Select(s =>
                         new DTOAlumnoOfertas
                         {
                             Descripcion = s.Anio + " - " + s.PeriodoId + " " + s.OfertaEducativa.Descripcion,
                             OfertaEducativaId = s.OfertaEducativaId,
                             OfertaEducativaTipoId = s.OfertaEducativa.OfertaEducativaTipoId
                         }).ToList(),
                        Antecendentes = db.AlumnoAntecedente
                                        .Where(at => at.AlumnoId == AlumnoId)
                                        .Select(i => new DTOAlumnoAntecendente
                                        {
                                            AlumnoId = i.AlumnoId,
                                            Anio = i.Anio,
                                            AntecedenteTipoId = i.AntecedenteTipoId,
                                            AreaAcademicaId = i.AreaAcademicaId,
                                            EntidadFederativaId = i.EntidadFederativaId,
                                            EscuelaEquivalencia = i.EscuelaEquivalencia,
                                            EsTitulado = i.EsTitulado,
                                            EsEquivalencia = i.EsEquivalencia,
                                            FechaRegistro = i.FechaRegistro,
                                            MedioDifusionId = i.MedioDifusionId,
                                            MesId = i.MesId,
                                            PaisId = i.PaisId,
                                            Procedencia = i.Procedencia,
                                            Promedio = i.Promedio,
                                            TitulacionMedio = i.TitulacionMedio,
                                            UsuarioId = i.UsuarioId
                                        }).ToList()
                    };

                    List<DTOAlumnoOfertas> lstBitacora = db.AlumnoInscritoBitacora
                                            .OrderByDescending(o => new { o.Anio, o.PeriodoId })
                                            .ToList()
                                            .Where(i => i.AlumnoId == AlumnoId)                                            
                                            .Select(d => new DTOAlumnoOfertas
                                            {
                                                Descripcion = d.Anio + " - " + d.PeriodoId + " " + d.OfertaEducativa.Descripcion,
                                                OfertaEducativaId = d.OfertaEducativaId,
                                                OfertaEducativaTipoId = d.OfertaEducativa.OfertaEducativaTipoId
                                            }).ToList();
                    if (lstBitacora.Count > 0)
                    {
                        lstBitacora.ForEach(i =>
                        {
                            if (objAlumno.lstOfertas.Where(s => s.OfertaEducativaId == i.OfertaEducativaId).ToList().Count == 0)
                            {
                                objAlumno.lstOfertas.Add(i);
                            }
                        });
                    }


                    objAlumno.DTOPersonaAutorizada = (from d in objAlB.PersonaAutorizada
                                                      select new DTOPersonaAutorizada
                                                      {
                                                          Nombre = d.Nombre,
                                                          Paterno = d.Paterno,
                                                          Materno = d.Materno,
                                                          ParentescoId = d.ParentescoId,
                                                          Email = d.Email,
                                                          Celular = d.Celular,
                                                          Telefono = d.Telefono,
                                                          Autoriza = d.EsAutorizada
                                                      }).ToList();

                    return objAlumno;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static DTOAlumno ObenerDatosAlumnoActualiza(int AlumnoId)
        {
            using(UniversidadEntities db = new UniversidadEntities())
            {
                try
                {

                if (db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).Count() > 0)
                {
                    Alumno objAlB = db.Alumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault();
                    AlumnoDetalleAlumno objAlB2 = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault();

                    DTOAlumno objAlumno = new DTOAlumno
                    {
                        AlumnoId = AlumnoId,
                        Nombre = objAlB.Nombre,
                        Paterno = objAlB.Paterno,
                        Materno = objAlB.Materno,
                        DTOAlumnoDetalle = new DTOAlumnoDetalle
                        {
                            EstadoCivilId = objAlB2.EstadoCivilId,
                            Celular = objAlB2.Celular,
                            TelefonoCasa = objAlB2.TelefonoCasa,
                            FechaNacimiento = objAlB.AlumnoDetalle.FechaNacimiento,
                            FechaNacimientoC = objAlB.AlumnoDetalle.FechaNacimiento.ToString("dd-MM-yyyy", Cultura),
                            GeneroId = objAlB.AlumnoDetalle.GeneroId,
                            CURP = objAlB.AlumnoDetalle.CURP,
                            Email = objAlB2.Email,
                            Calle = objAlB2.Calle,
                            NoExterior = objAlB2.NoExterior,
                            NoInterior = objAlB2.NoInterior,
                            Cp = objAlB2.CP,
                            Colonia = objAlB2.Colonia,
                            EntidadFederativaId = objAlB2.EntidadFederativaId,
                            MunicipioId = objAlB2.MunicipioId,
                            PaisId = objAlB.AlumnoDetalle.PaisId,
                            EntidadNacimientoId = objAlB.AlumnoDetalle.EntidadNacimientoId
                        }
                    };

                    return objAlumno;

                }
                else {
                  
                        Alumno objAlB = db.Alumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault();


                        DTOAlumno objAlumno = new DTOAlumno
                        {
                            AlumnoId = AlumnoId,
                            Nombre = objAlB.Nombre,
                            Paterno = objAlB.Paterno,
                            Materno = objAlB.Materno,
                            DTOAlumnoDetalle = new DTOAlumnoDetalle
                            {
                                EstadoCivilId = objAlB.AlumnoDetalle.EstadoCivilId,
                                Celular = (objAlB.AlumnoDetalle.Celular.Trim()).Replace("-", ""),
                                TelefonoCasa = (objAlB.AlumnoDetalle.TelefonoCasa.Trim()).Replace("-", ""),
                                FechaNacimiento = objAlB.AlumnoDetalle.FechaNacimiento,
                                FechaNacimientoC = objAlB.AlumnoDetalle.FechaNacimiento.ToString("dd-MM-yyyy", Cultura),
                                GeneroId = objAlB.AlumnoDetalle.GeneroId,
                                CURP = objAlB.AlumnoDetalle.CURP,
                                Email = objAlB.AlumnoDetalle.Email.Trim(),
                                Calle = objAlB.AlumnoDetalle.Calle.Trim(),
                                NoExterior = objAlB.AlumnoDetalle.NoExterior,
                                NoInterior = objAlB.AlumnoDetalle.NoInterior,
                                Cp = objAlB.AlumnoDetalle.CP.Trim(),
                                Colonia = objAlB.AlumnoDetalle.Colonia.Trim(),
                                EntidadFederativaId = objAlB.AlumnoDetalle.EntidadFederativaId,
                                MunicipioId = objAlB.AlumnoDetalle.MunicipioId,
                                PaisId = objAlB.AlumnoDetalle.PaisId,
                                EntidadNacimientoId = objAlB.AlumnoDetalle.EntidadNacimientoId
                            }
                        };

                        return objAlumno;
                    }            
                }
                
                    catch
                    {
                        return null;
                    } 
            }
        }

        public static DTOAlumno ObenerDatosAlumnoCordinador(int AlumnoId)
        {
               using(UniversidadEntities db = new UniversidadEntities())
            {
                try
                {

                if (db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).Count() > 0)
                {
                    Alumno objAlB = db.Alumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault();
                    AlumnoDetalleCoordinador objAlB2 = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault();

                    DTOAlumno objAlumno = new DTOAlumno
                    {
                        AlumnoId = AlumnoId,
                        Nombre = objAlB.Nombre,
                        Paterno = objAlB.Paterno,
                        Materno = objAlB.Materno,
                        DTOAlumnoDetalle = new DTOAlumnoDetalle
                        {
                            EstadoCivilId = objAlB2.EstadoCivilId,
                            Celular = objAlB2.Celular,
                            TelefonoCasa = objAlB2.TelefonoCasa,
                            FechaNacimiento = objAlB.AlumnoDetalle.FechaNacimiento,
                            FechaNacimientoC = objAlB.AlumnoDetalle.FechaNacimiento.ToString("dd-MM-yyyy", Cultura),
                            GeneroId = objAlB.AlumnoDetalle.GeneroId,
                            CURP = objAlB.AlumnoDetalle.CURP,
                            Email = objAlB2.Email,
                            Calle = objAlB2.Calle,
                            NoExterior = objAlB2.NoExterior,
                            NoInterior = objAlB2.NoInterior,
                            Cp = objAlB2.CP,
                            Colonia = objAlB2.Colonia,
                            EntidadFederativaId = objAlB2.EntidadFederativaId,
                            MunicipioId = objAlB2.MunicipioId,
                            PaisId = objAlB.AlumnoDetalle.PaisId,
                            EntidadNacimientoId = objAlB.AlumnoDetalle.EntidadNacimientoId
                        }
                    };

                    return objAlumno;

                }
                else {
                  
                        Alumno objAlB = db.Alumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault();


                        DTOAlumno objAlumno = new DTOAlumno
                        {
                            AlumnoId = AlumnoId,
                            Nombre = objAlB.Nombre,
                            Paterno = objAlB.Paterno,
                            Materno = objAlB.Materno,
                            DTOAlumnoDetalle = new DTOAlumnoDetalle
                            {
                                EstadoCivilId = objAlB.AlumnoDetalle.EstadoCivilId,
                                Celular = (objAlB.AlumnoDetalle.Celular.Trim()).Replace("-", ""),
                                TelefonoCasa = (objAlB.AlumnoDetalle.TelefonoCasa.Trim()).Replace("-", ""),
                                FechaNacimiento = objAlB.AlumnoDetalle.FechaNacimiento,
                                FechaNacimientoC = objAlB.AlumnoDetalle.FechaNacimiento.ToString("dd-MM-yyyy", Cultura),
                                GeneroId = objAlB.AlumnoDetalle.GeneroId,
                                CURP = objAlB.AlumnoDetalle.CURP,
                                Email = objAlB.AlumnoDetalle.Email.Trim(),
                                Calle = objAlB.AlumnoDetalle.Calle.Trim(),
                                NoExterior = objAlB.AlumnoDetalle.NoExterior,
                                NoInterior = objAlB.AlumnoDetalle.NoInterior,
                                Cp = objAlB.AlumnoDetalle.CP.Trim(),
                                Colonia = objAlB.AlumnoDetalle.Colonia.Trim(),
                                EntidadFederativaId = objAlB.AlumnoDetalle.EntidadFederativaId,
                                MunicipioId = objAlB.AlumnoDetalle.MunicipioId,
                                PaisId = objAlB.AlumnoDetalle.PaisId,
                                EntidadNacimientoId = objAlB.AlumnoDetalle.EntidadNacimientoId
                            }
                        };

                        return objAlumno;
                    }            
                }
                
                    catch
                    {
                        return null;
                    } 
            }
        }

        public static DTOAlumnoDatos ObenerDatosAlumnoTodos(int AlumnoId)
        {
            using (UniversidadEntities db= new UniversidadEntities())
            {
                try
                {

                    var datos = db.Alumno.Where(a => a.AlumnoId == AlumnoId)
                                        .Select(b => new DTOAlumnoDatos
                                        {
                                            AlumnoId = b.AlumnoId,
                                            Nombre = b.Nombre,
                                            Paterno = b.Paterno,
                                            Materno = b.Materno,
                                            FechaNacimiento = b.AlumnoDetalle.FechaNacimiento,
                                            GeneroId = b.AlumnoDetalle.GeneroId,
                                            CURP = b.AlumnoDetalle.CURP,
                                            PaisId = b.AlumnoDetalle.PaisId,
                                            EntidadNacimientoId = b.AlumnoDetalle.EntidadNacimientoId,
                                        }).FirstOrDefault();

                    datos.FechaNacimientoC = datos.FechaNacimiento.ToString("dd/MM/yyyy", Cultura);

                    datos.DatosContacto = new List<DTOAlumnoDatos2>();

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Estado Civil",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.EstadoCivil.Descripcion.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.EstadoCivil.Descripcion.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.EstadoCivil.Descripcion.ToString() ?? "",
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Correo Electrónico",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Email.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Email.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Email.ToString() ?? ""
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Teléfono Celular",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Celular.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Celular.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Celular.ToString() ?? ""
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Teléfono Casa",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.TelefonoCasa.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.TelefonoCasa.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.TelefonoCasa.ToString() ?? ""
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Calle",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Calle.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Calle.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Calle.ToString() ?? ""
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Número Exterior",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.NoExterior.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.NoExterior.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.NoExterior.ToString() ?? ""
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Numero Interior",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.NoInterior.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.NoInterior.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.NoInterior.ToString() ?? ""
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Código Postal",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.CP.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.CP.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.CP.ToString() ?? ""
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Colonia",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Colonia.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Colonia.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Colonia.ToString() ?? ""
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Estado",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.EntidadFederativa.Descripcion.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.EntidadFederativa.Descripcion.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.EntidadFederativa.Descripcion ?? ""
                    }
                    );

                    datos.DatosContacto.Add(new DTOAlumnoDatos2
                    {
                        Dato = "Delegación | Municipio",
                        Alumno = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Municipio.Descripcion.ToString() ?? "",
                        Coordinador = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Municipio.Descripcion.ToString() ?? "",
                        ServiciosEscolares = db.AlumnoDetalle.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault()?.Municipio.Descripcion ?? ""
                    }
                    );

                    return datos;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        public static bool UpdateAlumnoDatos(DTOAlumnoDetalle AlumnoDatos)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    var alumnoid = AlumnoDatos.AlumnoId;

                    if (db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == alumnoid).Count() > 0)
                    {
                        var AlumnoActualizaDatos = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == alumnoid).FirstOrDefault();

                        AlumnoActualizaDatos.EstadoCivilId = AlumnoDatos.EstadoCivilId;
                        AlumnoActualizaDatos.EntidadFederativaId = AlumnoDatos.EntidadFederativaId;
                        AlumnoActualizaDatos.MunicipioId = AlumnoDatos.MunicipioId;
                        AlumnoActualizaDatos.CP = AlumnoDatos.Cp;
                        AlumnoActualizaDatos.Colonia = AlumnoDatos.Colonia;
                        AlumnoActualizaDatos.Calle = AlumnoDatos.Calle;
                        AlumnoActualizaDatos.NoExterior = AlumnoDatos.NoExterior;
                        AlumnoActualizaDatos.NoInterior = AlumnoDatos.NoInterior;
                        AlumnoActualizaDatos.TelefonoCasa = AlumnoDatos.TelefonoCasa;
                        AlumnoActualizaDatos.Celular = AlumnoDatos.Celular;
                        AlumnoActualizaDatos.Email = AlumnoDatos.Email;
                        AlumnoActualizaDatos.Fecha = DateTime.Now;
                    }
                    else 
                    {
                        db.AlumnoDetalleAlumno.Add(new AlumnoDetalleAlumno 
                        {
                            AlumnoId = AlumnoDatos.AlumnoId,
                            EstadoCivilId = AlumnoDatos.EstadoCivilId,
                            EntidadFederativaId = AlumnoDatos.EntidadFederativaId,
                            MunicipioId = AlumnoDatos.MunicipioId,
                            CP = AlumnoDatos.Cp,
                            Colonia = AlumnoDatos.Colonia,
                            Calle = AlumnoDatos.Calle,
                            NoExterior = AlumnoDatos.NoExterior,
                            NoInterior = AlumnoDatos.NoInterior,
                            TelefonoCasa = AlumnoDatos.TelefonoCasa,
                            Celular = AlumnoDatos.Celular,
                            Email = AlumnoDatos.Email,
                            Fecha = DateTime.Now
                        });
                    }
                    db.SaveChanges();
                    return true;
                }

                catch (Exception)
                {
                    return false;
                }
            }//using
                
        }

        public static bool UpdateAlumnoDatosCoordinador(DTOAlumnoDetalle AlumnoDatos)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    var alumnoid = AlumnoDatos.AlumnoId;

                    if (db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == alumnoid).Count() > 0)
                    {
                        var AlumnoActualizaDatos = db.AlumnoDetalleCoordinador.Where(a => a.AlumnoId == alumnoid).FirstOrDefault();

                        AlumnoActualizaDatos.EstadoCivilId = AlumnoDatos.EstadoCivilId;
                        AlumnoActualizaDatos.EntidadFederativaId = AlumnoDatos.EntidadFederativaId;
                        AlumnoActualizaDatos.MunicipioId = AlumnoDatos.MunicipioId;
                        AlumnoActualizaDatos.CP = AlumnoDatos.Cp;
                        AlumnoActualizaDatos.Colonia = AlumnoDatos.Colonia;
                        AlumnoActualizaDatos.Calle = AlumnoDatos.Calle;
                        AlumnoActualizaDatos.NoExterior = AlumnoDatos.NoExterior;
                        AlumnoActualizaDatos.NoInterior = AlumnoDatos.NoInterior;
                        AlumnoActualizaDatos.TelefonoCasa = AlumnoDatos.TelefonoCasa;
                        AlumnoActualizaDatos.Celular = AlumnoDatos.Celular;
                        AlumnoActualizaDatos.Email = AlumnoDatos.Email;
                        AlumnoActualizaDatos.Fecha = DateTime.Now;
                        AlumnoActualizaDatos.UsuarioId = AlumnoDatos.UsuarioId;
                    }
                    else
                    {
                        db.AlumnoDetalleCoordinador.Add(new AlumnoDetalleCoordinador
                        {
                            AlumnoId = AlumnoDatos.AlumnoId,
                            EstadoCivilId = AlumnoDatos.EstadoCivilId,
                            EntidadFederativaId = AlumnoDatos.EntidadFederativaId,
                            MunicipioId = AlumnoDatos.MunicipioId,
                            CP = AlumnoDatos.Cp,
                            Colonia = AlumnoDatos.Colonia,
                            Calle = AlumnoDatos.Calle,
                            NoExterior = AlumnoDatos.NoExterior,
                            NoInterior = AlumnoDatos.NoInterior,
                            TelefonoCasa = AlumnoDatos.TelefonoCasa,
                            Celular = AlumnoDatos.Celular,
                            Email = AlumnoDatos.Email,
                            Fecha = DateTime.Now,
                            UsuarioId = AlumnoDatos.UsuarioId

                        });
                    }
                    db.SaveChanges();
                    return true;
                }

                catch (Exception)
                {
                    return false;
                }
            }//using
        }

        public static bool VerificaAlumnoDatos(int AlumnoId)
        {
             using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    if (db.AlumnoDetalleAlumno.Where(w => w.AlumnoId == AlumnoId).Count() > 0)
                    {

                        var fechaActual = DateTime.Now;
                        var Periodo = db.Periodo.Where(p => p.FechaInicial <= fechaActual
                                                                             && fechaActual <= p.FechaFinal).FirstOrDefault();
                        DateTime fechaActualizo = db.AlumnoDetalleAlumno.Where(a => a.AlumnoId == AlumnoId).FirstOrDefault().Fecha;

                        if ((Periodo.FechaInicial <= fechaActualizo && fechaActualizo <= Periodo.FechaFinal))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }
                    else
                    {
                        return true;
                    }

                }
                catch (Exception)
                {
                    
                    return false;
                }
             }
                
        }

        public static bool VerificaAlumnoEncuesta(int AlumnoId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    if (db.Respuesta.Where(w => w.AlumnoId == AlumnoId).Count() > 0)
                    {

                        var fechaActual = DateTime.Now;
                        var Periodo = db.Periodo.Where(p => p.FechaInicial <= fechaActual
                                                                             && fechaActual <= p.FechaFinal).FirstOrDefault();
                        DateTime? fechaEncuesta = db.Respuesta.Where(a => a.AlumnoId == AlumnoId).OrderByDescending(b=> b.FechaGeneracion).FirstOrDefault().FechaGeneracion;

                        if ((Periodo.FechaInicial <= fechaEncuesta && fechaEncuesta <= Periodo.FechaFinal))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }
                    else
                    {
                        return true;
                    }

                }
                catch (Exception)
                {

                    return false;
                }
            }

        }

        public static List<DTOPreguntas> PreguntasPortal()
        {
            using (UniversidadEntities db=new UniversidadEntities ())
            {
                var fechaActual = DateTime.Now;
                var Periodo = db.Periodo.Where(p => p.FechaInicial <= fechaActual
                                                                     && fechaActual <= p.FechaFinal).FirstOrDefault();

                var preguntas = db.Pregunta.Where(a => a.Anio == Periodo.Anio && a.PeriodoId == Periodo.PeriodoId)
                                           .Select(b => new DTOPreguntas
                                           {
                                               PreguntaId = b.PreguntaId,
                                               Descripcion = b.Descripcion,
                                               PreguntaTipoId = (int)b.PreguntaTipoId,
                                               SupPregunta = b.SubPregunta,
                                               Opciones = db.PreguntaTipoValores.Where(c=> c.PreguntaTipoId == b.PreguntaTipoId)
                                                                                . Select(d=> new DTOOpciones
                                                                                {
                                                                                    PreguntaTipoValoresId = d.PreguntaTipoValoresId,
                                                                                    PreguntaTipoId= (int) d.PreguntaTipoId,
                                                                                    Descripcion = d.Descripcion
                                                                                }).ToList()
                                           }).ToList();

                return preguntas;
            }
        }

        public static bool GuardarRespuestas(DTORespuestas RespuesasEncuesta)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                //try
                //{
                    var respuestas = RespuesasEncuesta.Respuestas;

                    respuestas.ForEach(n=> 
                    {
                        db.Respuesta.Add(new Respuesta
                        {
                            AlumnoId = n.AlumnoId,
                            PreguntaId = n.Pregunta,
                            FechaGeneracion = DateTime.Now,
                            HoraGeneracion = DateTime.Now.TimeOfDay,
                            PreguntaTipoValoresId =  n.Respuesta,
                            Comentario = n.Comentario
                        });
                    }
                        );
                    db.SaveChanges();
                    return true;
                //}
                //catch (Exception)
                //{

                   // return false;
                //}
                
            }
        }



        public static List<ReferenciasPagadas> ReferenciasConsulta(string Dato, int TipoBusqueda)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {

                    List< ReferenciasPagadas>  referencia= new List <ReferenciasPagadas>();
                    if (TipoBusqueda == 1)
                    {
                        var alumnoid = int.Parse(Dato);

                         referencia = (
                            from a in db.ReferenciaProcesada
                            //join b in db.PagoParcial on a.ReferenciaProcesadaId equals b.ReferenciaProcesadaId
                            where a.ReferenciaTipoId == 1
                               && a.EstatusId == 1
                               //&& b.EstatusId == 4
                               && a.AlumnoId == alumnoid
                            select new ReferenciasPagadas
                            {
                                AlumnoId = a.AlumnoId,
                                ReferenciaId = a.ReferenciaId,
                                FechaPagoD = a.FechaPago,
                                MontoPagado = a.Importe.ToString(),
                                MontoReferencia = a.Importe.ToString(),
                                Saldo = a.Restante.ToString()
                            }

                            ).ToList();

                    }
                    else if (TipoBusqueda == 2)
                    {
                        var referenciaid = Dato;

                        referencia = (
                      from a in db.ReferenciaProcesada
                      //join b in db.PagoParcial on a.ReferenciaProcesadaId equals b.ReferenciaProcesadaId
                      where a.ReferenciaTipoId == 1
                         && a.EstatusId == 1
                        // && b.EstatusId == 4
                         && a.ReferenciaId.Contains(referenciaid)
                      select new ReferenciasPagadas
                      {
                          AlumnoId = a.AlumnoId,
                          ReferenciaId = a.ReferenciaId,
                          FechaPagoD = a.FechaPago,
                          MontoPagado = a.Importe.ToString(),
                          MontoReferencia = a.Importe.ToString(),
                          Saldo = a.Restante.ToString()
                      }

                      ).ToList();

                    }
                    else if (TipoBusqueda == 3)
                    {
                        decimal importe = decimal.Parse(Dato);

                        referencia = (
                           from a in db.ReferenciaProcesada
                           //join b in db.PagoParcial on a.ReferenciaProcesadaId equals b.ReferenciaProcesadaId
                           where a.ReferenciaTipoId == 1
                              && a.EstatusId == 1
                              //&& b.EstatusId == 4
                              && a.Importe == importe
                           select new ReferenciasPagadas
                           {
                               AlumnoId = a.AlumnoId,
                               ReferenciaId = a.ReferenciaId,
                               FechaPagoD = a.FechaPago,
                               MontoPagado = a.Importe.ToString(),
                               MontoReferencia = a.Importe.ToString(),
                               Saldo = a.Restante.ToString()
                           }

                           ).ToList();
                    }
                    else if (TipoBusqueda == 4)
                    {
                        DateTime fechapago =DateTime.Parse(Dato);

                        referencia = (
                           from a in db.ReferenciaProcesada
                          // join b in db.PagoParcial on a.ReferenciaProcesadaId equals b.ReferenciaProcesadaId
                           where a.ReferenciaTipoId == 1
                              && a.EstatusId == 1
                              //&& b.EstatusId == 4
                              && a.FechaPago== fechapago
                           select new ReferenciasPagadas
                           {
                               AlumnoId = a.AlumnoId,
                               ReferenciaId = a.ReferenciaId,
                               FechaPagoD = a.FechaPago,
                               MontoPagado = a.Importe.ToString(),
                               MontoReferencia = a.Importe.ToString(),
                               Saldo = a.Restante.ToString()
                           }

                           ).ToList();
                    }

                    referencia = (from a in referencia
                                  join b in db.Alumno on a.AlumnoId equals b.AlumnoId
                                  select new ReferenciasPagadas
                                  {
                                      AlumnoId = a.AlumnoId,
                                      Nombre = b.Nombre + " " + b.Paterno + " " + b.Materno,
                                      ReferenciaId = a.ReferenciaId,
                                      FechaPago = a.FechaPagoD.ToString("dd/MM/yyyy",Cultura),
                                      MontoPagado = a.MontoPagado,
                                      MontoReferencia = a.MontoReferencia,
                                      Saldo = a.Saldo
                                  }
                                 ).ToList();
                        
                    return referencia;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }




        public static List<DTOAlumno> ListarAlumnos(string Nombre, string Paterno, string Materno)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    string Filtro = (Nombre + " " + Paterno + " " + Materno).Trim();
                    List<DTOAlumno> lstAlumnos = (from a in db.Alumno
                                                  where (a.Nombre + " " + a.Paterno + " " + a.Materno).Contains(Filtro)
                                                  select new DTOAlumno
                                                  {
                                                      AlumnoId = a.AlumnoId,
                                                      Nombre = a.Nombre + " " + a.Paterno + " " + a.Materno,
                                                      FechaRegistro = (a.FechaRegistro.Day < 10 ? "0" + a.FechaRegistro.Day : "" + a.FechaRegistro.Day)
                                                      + "/" + (a.FechaRegistro.Month < 10 ? "0" + a.FechaRegistro.Month : "" + a.FechaRegistro.Month)
                                                      + "/" + a.FechaRegistro.Year,
                                                      DTOAlumnoDetalle = new DTOAlumnoDetalle
                                                      {
                                                          AlumnoId = a.AlumnoDetalle.AlumnoId,
                                                          FechaNacimientoC = (a.AlumnoDetalle.FechaNacimiento.Day < 10 ? "0" + a.AlumnoDetalle.FechaNacimiento.Day : "" + a.AlumnoDetalle.FechaNacimiento.Day)
                                                            + "/" + (a.AlumnoDetalle.FechaNacimiento.Month < 10 ? "0" + a.AlumnoDetalle.FechaNacimiento.Month : "" + a.AlumnoDetalle.FechaNacimiento.Month)
                                                            + "/" + a.AlumnoDetalle.FechaNacimiento.Year,
                                                      },
                                                      AlumnoInscrito = (from b in db.AlumnoInscrito
                                                                        where a.AlumnoId == b.AlumnoId
                                                                        select new DTOAlumnoInscrito
                                                                        {
                                                                            AlumnoId = b.AlumnoId,
                                                                            OfertaEducativaId = b.OfertaEducativaId,
                                                                            OfertaEducativa = new DTOOfertaEducativa
                                                                            {
                                                                                OfertaEducativaId = b.OfertaEducativaId,
                                                                                Descripcion = b.OfertaEducativa.Descripcion
                                                                            }
                                                                        }).FirstOrDefault(),
                                                      Usuario = (from f in db.Usuario
                                                                 where f.UsuarioId == a.UsuarioId
                                                                 select new DTOUsuario
                                                                 {
                                                                     UsuarioId = f.UsuarioId,
                                                                     Nombre = f.Nombre
                                                                 }).FirstOrDefault()
                                                  }).ToList();
                    lstAlumnos.ForEach(delegate (DTOAlumno objAlumno)
                    {
                        if (objAlumno.AlumnoInscrito == null)
                        {
                            objAlumno.AlumnoInscrito = new DTOAlumnoInscrito
                            {
                                OfertaEducativaId = 0,
                                OfertaEducativa = new DTOOfertaEducativa
                                {
                                    OfertaEducativaId = 0,
                                    Descripcion = ""
                                }
                            };
                        }
                    });
                    //List<DTOAlumno> lstAlumnos2 = lstAlumnos.FindAll(X => X.Paterno.Contains(Paterno));
                    //lstAlumnos = lstAlumnos2.FindAll(X => X.Materno.Contains(Materno));
                    return lstAlumnos;
                }
                catch { return null; }
            }
        }
        private static String SinAcentos(String Texto)
        {
            String Resultado;
            byte[] Arreglo;
            Arreglo = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(Texto);
            Resultado = System.Text.Encoding.UTF8.GetString(Arreglo);
            return Resultado;
        }
        public static List<DTOAlumno> AlumnosEmpresa(int grupoid)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                //DTOPeriodo objPeriodo = BLLPeriodo.TraerPeriodoEntreFechas(DateTime.Now);
                DTOGrupo objGrupo = BLLGrupo.ObtenerGrupo(grupoid);
                List<DTOAlumno> lstAlIns = (from a in db.Alumno
                                            join ai in db.AlumnoInscrito on new { a.AlumnoId } equals new { ai.AlumnoId }
                                            where ai.EsEmpresa == true && a.GrupoAlumnoConfiguracion.Where(o => o.EstatusId == 1).LastOrDefault().Grupo == null && ai.PagoPlanId == null && ai.OfertaEducativaId == objGrupo.OfertaEducativaId
                                            //ai.PeriodoId == objPeriodo.PeriodoId
                                            select new DTOAlumno
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Nombre = a.Nombre + " " + a.Paterno + " " + a.Materno,
                                                Paterno = a.Paterno,
                                                Materno = a.Materno,
                                                FechaRegistro = a.FechaRegistro.ToString(),
                                                UsuarioId = a.UsuarioId,
                                                //Grupo=a.Grupo
                                                AlumnoInscrito = new DTOAlumnoInscrito
                                                {
                                                    AlumnoId = ai.AlumnoId,
                                                    Anio = ai.Anio,
                                                    EsEmpresa = ai.EsEmpresa,
                                                    FechaInscripcion = ai.FechaInscripcion,
                                                    OfertaEducativaId = ai.OfertaEducativaId,
                                                    PagoPlanId = ai.PagoPlanId,
                                                    PeriodoId = ai.PeriodoId,
                                                    TurnoId = ai.TurnoId,
                                                    UsuarioId = ai.UsuarioId,
                                                    OfertaEducativa = new DTOOfertaEducativa
                                                    {
                                                        OfertaEducativaId = ai.OfertaEducativa.OfertaEducativaId,
                                                        OfertaEducativaTipoId = ai.OfertaEducativa.OfertaEducativaTipoId,
                                                        Descripcion = ai.OfertaEducativa.Descripcion,
                                                        Rvoe = ai.OfertaEducativa.Rvoe
                                                    }
                                                },
                                                Usuario = new DTOUsuario
                                                {
                                                    EstatusId = a.Usuario.EstatusId,
                                                    Materno = a.Usuario.Materno,
                                                    Nombre = a.Usuario.Nombre,
                                                    Paterno = a.Paterno
                                                }
                                            }).ToList();
                //lstAlIns=lstAlIns.Select(X=>X.g)
                return lstAlIns;
            }
        }
        public static List<DTOAlumno> AlumnosdeEmpresa(int GrupoId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                DTOGrupo objGrupo = BLLGrupo.ObtenerGrupo(GrupoId);
                List<DTOAlumno> lstAlIns = (from a in db.Alumno
                                            join ai in db.AlumnoInscrito on new { a.AlumnoId } equals new { ai.AlumnoId }
                                            where ai.EsEmpresa == true && a.GrupoAlumnoConfiguracion.Where(o => o.EstatusId == 1).LastOrDefault().GrupoId == GrupoId
                                            select new DTOAlumno
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Nombre = a.Nombre + " " + a.Paterno + " " + a.Materno,
                                                Paterno = a.Paterno,
                                                Materno = a.Materno,
                                                FechaRegistro = a.FechaRegistro.ToString(),
                                                UsuarioId = a.UsuarioId,
                                                //Grupo=a.Grupo
                                                AlumnoInscrito = new DTOAlumnoInscrito
                                                {
                                                    AlumnoId = ai.AlumnoId,
                                                    Anio = ai.Anio,
                                                    EsEmpresa = ai.EsEmpresa,
                                                    FechaInscripcion = ai.FechaInscripcion,
                                                    OfertaEducativaId = ai.OfertaEducativaId,
                                                    PagoPlanId = ai.PagoPlanId,
                                                    PeriodoId = ai.PeriodoId,
                                                    TurnoId = ai.TurnoId,
                                                    UsuarioId = ai.UsuarioId,
                                                    OfertaEducativa = new DTOOfertaEducativa
                                                    {
                                                        OfertaEducativaId = ai.OfertaEducativa.OfertaEducativaId,
                                                        OfertaEducativaTipoId = ai.OfertaEducativa.OfertaEducativaTipoId,
                                                        Descripcion = ai.OfertaEducativa.Descripcion,
                                                        Rvoe = ai.OfertaEducativa.Rvoe
                                                    }
                                                },
                                                Usuario = new DTOUsuario
                                                {
                                                    EstatusId = a.Usuario.EstatusId,
                                                    Materno = a.Usuario.Materno,
                                                    Nombre = a.Usuario.Nombre,
                                                    Paterno = a.Paterno
                                                }
                                            }).ToList();
                //lstAlIns=lstAlIns.Select(X=>X.g)
                return lstAlIns;
            }
        }

        public static List<DTOAlumno> ListarAlumnosBeca(Boolean Ingles)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                DTOPeriodo objPeriodo = BLLPeriodoPortal.TraerPeriodoEntreFechas(DateTime.Now);
                db.Configuration.LazyLoadingEnabled = false;
                List<DTOAlumno> lstAlumnos = Ingles != true ? ((from a in db.Alumno
                                                                orderby a.AlumnoId
                                                                select new DTOAlumno
                                                                {
                                                                    AlumnoId = a.AlumnoId,
                                                                    Nombre = a.Nombre + " " + a.Paterno + " " + a.Materno,
                                                                    FechaRegistro = a.FechaRegistro.ToString(),
                                                                    AlumnoInscrito2 = new DTOAlumnoInscrito2 { Descripcion = "" },
                                                                    EstatusDescuento = (from AD in db.AlumnoDescuento
                                                                                        where AD.AlumnoId == a.AlumnoId && AD.Anio == objPeriodo.Anio && AD.PeriodoId == objPeriodo.PeriodoId
                                                                                        select a.AlumnoId).ToList().Count > 0 ? "Ya Capturado" : "Pendiente",
                                                                    lstAlumnoInscrito = (from b in db.AlumnoInscrito
                                                                                         where a.AlumnoId == b.AlumnoId && b.OfertaEducativa.OfertaEducativaTipoId != 4
                                                                                         select new DTOAlumnoInscrito
                                                                                         {
                                                                                             OfertaEducativa = new DTOOfertaEducativa
                                                                                             {
                                                                                                 Descripcion = b.OfertaEducativa.Descripcion
                                                                                             }
                                                                                         }).ToList()
                                                                }).AsNoTracking().ToList()) :
                                               ((from a in db.Alumno
                                                 join AI in db.AlumnoInscrito on a.AlumnoId equals AI.AlumnoId
                                                 orderby a.AlumnoId
                                                 select new DTOAlumno
                                                 {
                                                     AlumnoId = a.AlumnoId,
                                                     Nombre = a.Nombre + " " + a.Paterno + " " + a.Materno,
                                                     FechaRegistro = a.FechaRegistro.ToString(),
                                                     AlumnoInscrito2 = new DTOAlumnoInscrito2 { Descripcion = "" },
                                                     EstatusDescuento = (from AD in db.AlumnoDescuento
                                                                         where AD.AlumnoId == a.AlumnoId && AD.Anio == objPeriodo.Anio && AD.PeriodoId == objPeriodo.PeriodoId
                                                                         select a.AlumnoId).ToList().Count > 0 ? "Ya Capturado" : "Pendiente",
                                                     lstAlumnoInscrito = (from b in db.AlumnoInscrito
                                                                          where a.AlumnoId == b.AlumnoId && b.OfertaEducativa.OfertaEducativaTipoId == 4
                                                                          select new DTOAlumnoInscrito
                                                                          {
                                                                              OfertaEducativa = new DTOOfertaEducativa
                                                                              {
                                                                                  Descripcion = b.OfertaEducativa.Descripcion
                                                                              }
                                                                          }).ToList()
                                                 }).AsNoTracking().ToList());

                lstAlumnos = Ingles != true ? lstAlumnos.Where(P => P.lstAlumnoInscrito.Count > 0).ToList() : lstAlumnos;
                lstAlumnos.ForEach(Alumno =>
                {
                    if (Alumno.lstAlumnoInscrito != null && Alumno.lstAlumnoInscrito.Count > 0)
                    {
                        Alumno.lstAlumnoInscrito.ForEach(Oferta =>
                        {
                            Alumno.AlumnoInscrito2.Descripcion += Oferta.OfertaEducativa.Descripcion + "/";
                        });
                        string xs = Alumno.AlumnoInscrito2.Descripcion;
                        xs = xs.Substring(0, xs.Length - 1);
                        Alumno.AlumnoInscrito2.Descripcion = xs;
                    }
                });
                return lstAlumnos;
            }
        }
        public static List<DTOAlumno> BuscarAlumno(string filtro)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    List<DTOAlumno> lstAlumnos = (from a in db.Alumno
                                                  where (a.Nombre + " " + a.Paterno + " " + a.Materno).Contains(filtro)
                                                  select new DTOAlumno
                                                  {
                                                      AlumnoId = a.AlumnoId,
                                                      Nombre = a.Nombre + " " + a.Paterno + " " + a.Materno,
                                                      Chocolates = a.AdeudoChocolates != null ? true : false,
                                                      //Biblioteca = a.AdeudoBiblioteca != null ? true : false
                                                  }
                            ).AsNoTracking().ToList();
                    return lstAlumnos;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static DTOAlumnoPermitido1 ObtenerAlumno2(int AlumnoId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    DateTime fHoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    if (fHoy.Month == 4 || fHoy.Month == 8 || fHoy.Month == 12)
                    { fHoy.AddMonths(1); }

                    Periodo objPer = db.Periodo.Where(p => fHoy >= p.FechaInicial && fHoy <= p.FechaFinal).FirstOrDefault();


                    if (db.AlumnoPermitido.Where(A => A.AlumnoId == AlumnoId && A.Anio == objPer.Anio && A.PeriodoId == objPer.PeriodoId).ToList().Count > 0)
                    {
                        return new DTOAlumnoPermitido1
                        {
                            AlumnoId = 0,
                            Nombre = "El Alumno ya esta liberado",
                            lstBitacora = BLLAlumnoPermitido.RegistrosdeAlumno(AlumnoId)
                        };
                    }
                    else
                    {
                        DTOAlumnoPermitido1 objAlumno = (from a in db.Alumno
                                                         where a.AlumnoId == AlumnoId
                                                         select new DTOAlumnoPermitido1
                                                         {
                                                             AlumnoId = a.AlumnoId,
                                                             Nombre = a.Nombre,
                                                             Paterno = a.Paterno,
                                                             Materno = a.Materno
                                                         }).AsNoTracking().FirstOrDefault();
                        objAlumno.lstBitacora = BLLAlumnoPermitido.RegistrosdeAlumno(AlumnoId);
                        return objAlumno;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
        private class AlumnoOferta1
        {
            public int AlumnoId { get; set; }
            public int OfertaEducativaId { get; set; }
        }
        //public static List<DTOAlumnoBecas> ListaAlumnosActuales()
        //{
        //    using (UniversidadEntities db = new UniversidadEntities())
        //    {
        //        try
        //        {
        //            List<DTOAlumnoBecas> lstAlumnos = null;
        //            DateTime fHoy = DateTime.Now;
        //            DTOPeriodo objPeriodo = BLLPeriodo.TraerPeriodoEntreFechas(fHoy);

        //            //var lslAlumnoidOferta = db.Pago.Where(P => P.Anio == objPeriodo.Anio && P.PeriodoId == objPeriodo.PeriodoId &&
        //            //    (P.Cuota1.PagoConceptoId == 800 || P.Cuota1.PagoConceptoId == 802)).Distinct().Select(a => new { a.AlumnoId, a.OfertaEducativaId }).Distinct().ToList();

        //            //List<AlumnoOferta1> lstAlumnoIdOfertaId = lslAlumnoidOferta.Select(L => new AlumnoOferta1 { AlumnoId = L.AlumnoId, OfertaEducativaId = L.OfertaEducativaId }).ToList();
        //            //List<AlumnoInscrito> lstAlumnosInscritos = null;
        //            List<AlumnoInscrito> listAlDB =
        //           (from a in db.AlumnoInscritoDetalle
        //            join b in db.AlumnoInscrito on new { a.AlumnoId, a.OfertaEducativaId, a.Anio, a.PeriodoId } equals new { b.AlumnoId, b.OfertaEducativaId, b.Anio, b.PeriodoId }
        //            join p in db.Pago on new { a.AlumnoId, a.OfertaEducativaId, a.PeriodoId, b.Anio } equals new { p.AlumnoId, p.OfertaEducativaId, p.PeriodoId, p.Anio }
        //            where a.Anio == 2016 && a.PeriodoId == 3
        //            select b).AsNoTracking().Distinct().ToList().OrderBy(P => P.AlumnoId).ToList();

        //            lstAlumnos = (from a in db.AlumnoInscrito
        //                          join p in db.Pago on new { a.AlumnoId, a.OfertaEducativaId } equals new { p.AlumnoId, p.OfertaEducativaId }
        //                          join ad in db.AlumnoDescuento on
        //                          new { a.AlumnoId, a.OfertaEducativaId, objPeriodo.Anio, objPeriodo.PeriodoId } equals new { ad.AlumnoId, ad.OfertaEducativaId, ad.Anio, ad.PeriodoId }
        //                          where p.Anio == objPeriodo.Anio && p.PeriodoId == objPeriodo.PeriodoId &&
        //                           (p.Cuota1.PagoConceptoId == 800 || p.Cuota1.PagoConceptoId == 802) &&
        //                           (a.Alumno.Anio != objPeriodo.Anio || a.Alumno.PeriodoId != objPeriodo.PeriodoId)
        //                          orderby a.AlumnoId
        //                          select new DTOAlumnoBecas
        //                          {
        //                              AlumnoId = "" + a.AlumnoId,
        //                              OfertaEducativaId = a.OfertaEducativaId,
        //                              OfertaEducativa = a.OfertaEducativa.Descripcion,
        //                              FechaReinscripcion = (DateTime)ad.FechaAplicacion,
        //                              Nombre = a.Alumno.Nombre + " " + a.Alumno.Paterno + " " + a.Alumno.Materno,
        //                              BecaSep = "" + (from a1 in a.Alumno.AlumnoDescuento
        //                                              join d in db.Descuento on a1.DescuentoId equals d.DescuentoId
        //                                              where a1.AlumnoId == a.AlumnoId && a1.Anio == a.Anio && a1.PeriodoId == a.PeriodoId &&
        //                                              a1.OfertaEducativaId == a.OfertaEducativaId && a1.EstatusId == 2 && a1.PagoConceptoId == 800
        //                                              && d.Descripcion == "Beca SEP"
        //                                              select a1).FirstOrDefault().Monto,
        //                              BecaAcademica = "" + (from a1 in a.Alumno.AlumnoDescuento
        //                                                    join d in db.Descuento on a1.DescuentoId equals d.DescuentoId
        //                                                    where a1.AlumnoId == a.AlumnoId && a1.Anio == a.Anio && a1.PeriodoId == a.PeriodoId &&
        //                                                    a1.OfertaEducativaId == a.OfertaEducativaId && a1.EstatusId == 2 && a1.PagoConceptoId == 800
        //                                                    && d.Descripcion == "Beca Académica"
        //                                                    select a1).FirstOrDefault().Monto,
        //                              Usuario = "" + (from a1 in a.Alumno.AlumnoDescuento
        //                                              where a1.AlumnoId == a.AlumnoId && a1.Anio == a.Anio && a1.PeriodoId == a.PeriodoId &&
        //                                                a1.OfertaEducativaId == a.OfertaEducativaId && a1.EstatusId == 2 && a1.PagoConceptoId == 800
        //                                              select a1.Usuario.Nombre).FirstOrDefault(),
        //                          }).AsNoTracking().Distinct().ToList().OrderBy(P => P.AlumnoId).ToList();
        //            //lstAlumnos.AddRange(
        //            //    (from a in db.AlumnoInscritoBeca
        //            //        join p in db.Pago on new { a.AlumnoId, a.OfertaEducativaId } equals new { p.AlumnoId, p.OfertaEducativaId }
        //            //         where lstAlumnos.Where(a1=> int.Parse(a1.AlumnoId)==a.AlumnoId ).ToList().Count>0
        //            //         select
        //            //    );
        //            listAlDB.ForEach(a =>
        //            {

        //                if (lstAlumnos.FindIndex(d => int.Parse(d.AlumnoId) == a.AlumnoId) != -1)
        //                {
        //                    DTOAlumnoBecas obalumnoadd = new DTOAlumnoBecas();

        //                    AlumnoInscritoBeca onAlIns = a.Where(s =>
        //                        s.OfertaEducativaId == a.OfertaEducativaId
        //                        && s.PeriodoId == a.PeriodoId
        //                        && s.Anio == a.Anio).ToList().FirstOrDefault();

        //                    obalumnoadd.AlumnoId = "" + a.AlumnoId;
        //                    obalumnoadd.OfertaEducativaId = a.OfertaEducativaId;
        //                    obalumnoadd.OfertaEducativa = a.OfertaEducativa.Descripcion;
        //                    obalumnoadd.FechaReinscripcion = onAlIns != null ? onAlIns.FechaAplicacion : a.FechaInscripcion;
        //                    obalumnoadd.Nombre = a.Alumno.Nombre + " " + a.Alumno.Paterno + " " + a.Alumno.Materno;
        //                    obalumnoadd.BecaSep = "";
        //                    obalumnoadd.BecaAcademica = "";
        //                    obalumnoadd.Usuario = onAlIns != null ? (from f in db.Usuario
        //                                                             where f.UsuarioId == onAlIns.UsuarioId
        //                                                             select f.Nombre).FirstOrDefault() : a.Usuario.Nombre;

        //                    lstAlumnos.Add(obalumnoadd);
        //                }
        //            });
        //            lstAlumnos.ForEach(delegate (DTO.Alumno.Beca.s objAlumno)
        //            {
        //                int indice = -1, indiceInicial = -1;
        //                indiceInicial = lstAlumnos.FindIndex(P => P.AlumnoId == objAlumno.AlumnoId && P.OfertaEducativaId == objAlumno.OfertaEducativaId);
        //                indice = lstAlumnos.FindIndex(indiceInicial + 1, P => P.AlumnoId == objAlumno.AlumnoId && P.OfertaEducativaId == objAlumno.OfertaEducativaId);
        //                objAlumno.FechaReinscripcionS = objAlumno.FechaReinscripcion.ToString("dd/MM/yyyy", Cultura);
        //                if (indice > -1)
        //                {
        //                    lstAlumnos[indice].FechaReinscripcionS = objAlumno.FechaReinscripcion.ToString("dd/MM/yyyy", Cultura);
        //                    lstAlumnos.RemoveAt(indiceInicial);
        //                }
        //            });

        //            lstAlumnos = lstAlumnos.OrderBy(P => P.AlumnoId).ToList();
        //            return lstAlumnos;
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }
        //}
        public static bool BuscarAlumnoReinscribir(int AlumnoId, int OfertaEducativa, int periodo, int anio)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                AlumnoInscrito alIn = db.AlumnoInscrito.Where(a => a.Anio == anio && a.PeriodoId == periodo &&
                AlumnoId == a.AlumnoId && a.OfertaEducativaId == OfertaEducativa).FirstOrDefault();

                if (alIn == null)
                {
                    try
                    {
                        AlumnoInscrito alIn2 = db.AlumnoInscrito.Where(a => AlumnoId == a.AlumnoId && a.OfertaEducativaId == OfertaEducativa).FirstOrDefault();
                        db.AlumnoInscritoBitacora.Add(new AlumnoInscritoBitacora
                        {
                            AlumnoId = alIn2.AlumnoId,
                            OfertaEducativaId = alIn2.OfertaEducativaId,
                            Anio = alIn2.Anio,
                            PeriodoId = alIn2.PeriodoId,
                            FechaInscripcion = alIn2.FechaInscripcion,
                            PagoPlanId = (int)alIn2.PagoPlanId,
                            TurnoId = alIn2.TurnoId,
                            UsuarioId = alIn2.UsuarioId,
                            EsEmpresa = alIn2.EsEmpresa,
                            HoraInscripcion = DateTime.Now.TimeOfDay,
                        });

                        db.AlumnoInscrito.Add(new AlumnoInscrito
                        {
                            AlumnoId = AlumnoId,
                            OfertaEducativaId = (int)alIn2.OfertaEducativaId,
                            Anio = anio,
                            PeriodoId = periodo,
                            FechaInscripcion = DateTime.Now,
                            PagoPlanId = alIn2.PagoPlanId,
                            TurnoId = alIn2.TurnoId,
                            EsEmpresa = alIn2.EsEmpresa,
                            UsuarioId = alIn2.UsuarioId,
                            EstatusId = alIn2.EstatusId,
                            HoraInscripcion = DateTime.Now.TimeOfDay,
                        });

                        db.AlumnoInscrito.Remove(alIn2);
                        db.SaveChanges();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }

                }
                return true;
            }
        }
        public static AlumnoPagos BuscarAlumno(int AlumnoId, int OfertaEducativaId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    AlumnoPagos objRegresa = null;
                    DTOPeriodo objPeriodoActual = BLLPeriodoPortal.TraerPeriodoEntreFechas(DateTime.Now);
                    Alumno objAlumnoDB = db.AlumnoInscrito.Where(A => A.OfertaEducativaId == OfertaEducativaId && A.AlumnoId == AlumnoId).FirstOrDefault().Alumno;
                    List<Pago> lstPagosAlumno = new List<Pago>();
                    if (objAlumnoDB == null)
                    {
                        return new AlumnoPagos
                        {
                            AlumnoId = "-3"
                        };
                    }
                    Grupo objDefault = new Grupo
                    {
                        Descripcion = ""
                    };
                     
                    List<AlumnoInscrito> objcom = objAlumnoDB.AlumnoInscrito.Where(ds => ds.Anio == objPeriodoActual.Anio
                         && ds.PeriodoId == objPeriodoActual.PeriodoId && ds.OfertaEducativaId == OfertaEducativaId).ToList();
                    List<Pago> lstPagos = db.Pago.Where(i => i.AlumnoId == AlumnoId
                                                 && i.OfertaEducativaId == OfertaEducativaId
                                                 && i.Anio == objPeriodoActual.Anio
                                                 && i.PeriodoId == objPeriodoActual.PeriodoId
                                                 && i.EstatusId != 2
                                                 && (i.Cuota1.PagoConceptoId == 800
                                                 || i.Cuota1.PagoConceptoId == 802
                                                 || i.Cuota1.PagoConceptoId == 304
                                                 || i.Cuota1.PagoConceptoId == 320
                                                 || i.Cuota1.PagoConceptoId == 15)).ToList();
                    lstPagosAlumno.AddRange(lstPagos);

                    #region Inscrito
                    if (objcom.Count > 0)
                    {

                        List<AlumnoDescuento> listDesc = db.AlumnoDescuento.Where(o => o.AlumnoId == AlumnoId
                                                                       && o.OfertaEducativaId == OfertaEducativaId
                                                                       && o.Anio == objPeriodoActual.Anio
                                                                       && o.PeriodoId == objPeriodoActual.PeriodoId
                                                                       && o.PagoConceptoId == 800
                                                                       && o.EstatusId == 2).ToList();

                        //Empresa
                        if (objcom.Where(s => s.EsEmpresa == true).ToList().Count > 0)
                        {

                            objRegresa = new AlumnoPagos
                            {
                                AlumnoId = lstPagos.Where(o => o.Cuota1.PagoConceptoId == 15
                                                       || o.Cuota1.PagoConceptoId == 304
                                                       || o.Cuota1.PagoConceptoId == 320).ToList().Count > 0 ? "-5" : "-1",
                                Nombre = objAlumnoDB.Nombre + " " + objAlumnoDB.Paterno + " " + objAlumnoDB.Materno,
                                OfertasEducativas = (from b in db.OfertaEducativa
                                                     where b.OfertaEducativaId == OfertaEducativaId
                                                     select new DTOOfertaEducativa
                                                     {
                                                         OfertaEducativaId = b.OfertaEducativaId,
                                                         OfertaEducativaTipoId = b.OfertaEducativaTipoId,
                                                         Descripcion = b.Descripcion
                                                     }).ToList(),
                                PeriodoD = objPeriodoActual.Anio + " " + objPeriodoActual.PeriodoId,
                                PeriodoId = " " + objPeriodoActual.PeriodoId,
                                Anio = " " + objPeriodoActual.Anio,
                                Inscrito = true,
                                Academica = listDesc.Count > 0 ? listDesc.FirstOrDefault().Monto > 0 ?
                                            listDesc.FirstOrDefault().EsSEP == false && listDesc.FirstOrDefault().EsComite == false ?
                                            true : false : false : false,
                                Comite = listDesc.Count > 0 ? listDesc.FirstOrDefault().Monto > 0 ?
                                           listDesc.FirstOrDefault().EsComite == true ? true
                                           : false : false : false,
                                SEP = listDesc.Count > 0 ? listDesc.FirstOrDefault().Monto > 0 ?
                                           listDesc.FirstOrDefault().EsSEP == true ? true
                                           : false : false : false,
                                lstPagos = (from a in lstPagos
                                            select new PagosAlumnos
                                            {
                                                SubPeriodo = a.SubperiodoId,
                                                Concepto = a.Cuota1.PagoConcepto.Descripcion,
                                                PagoId = "" + a.PagoId,
                                                ReferenciaId = "" + int.Parse(a.ReferenciaId)
                                            }).ToList(),
                                Materias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 304
                                                          || I.Cuota1.PagoConceptoId == 320).ToList().Count,
                                NuevoIngreso = (objAlumnoDB.Anio == objPeriodoActual.Anio && objAlumnoDB.PeriodoId == objPeriodoActual.PeriodoId) ? true : false,
                                Asesorias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 15)
                                                            .ToList().Count,
                                Completa = lstPagos.Where(p =>
                                                      p.Cuota1.PagoConceptoId == 802
                                                      && p.Cuota1.PagoConceptoId == 800).ToList().Count == 5 ? true : false,
                                EsEmpresa = true,
                                
                                EsEspecial = objcom.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion?.FirstOrDefault()?.EsEspecial ?? false,

                                Grupo = (objcom.FirstOrDefault().Alumno?.GrupoAlumnoConfiguracion).Where(o => o.OfertaEducativaId == OfertaEducativaId).FirstOrDefault()?.Grupo.Descripcion ?? ""
                                //objcom.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion.Count>0 ?
                                // objcom.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion.Where(k => k.OfertaEducativaId == OfertaEducativaId).ToList().Count > 0 ?
                                //                objcom.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion.Where(k => k.OfertaEducativaId == OfertaEducativaId).ToList().First().Grupo?.Descripcion ?? ""
                                //                : "" : ""
                            };
                        }
                        //No es EMpresa
                        else
                        {
                            objRegresa = new AlumnoPagos
                            {
                                AlumnoId = lstPagos.Where(o => o.Cuota1.PagoConceptoId == 15
                                                       || o.Cuota1.PagoConceptoId == 304
                                                       || o.Cuota1.PagoConceptoId == 320).ToList().Count > 0 ? "-5" : objAlumnoDB.AlumnoId.ToString(),
                                Nombre = objAlumnoDB.Nombre + " " + objAlumnoDB.Paterno + " " + objAlumnoDB.Materno,
                                OfertasEducativas = (from b in db.OfertaEducativa
                                                     where b.OfertaEducativaId == OfertaEducativaId
                                                     select new DTOOfertaEducativa
                                                     {
                                                         OfertaEducativaId = b.OfertaEducativaId,
                                                         OfertaEducativaTipoId = b.OfertaEducativaTipoId,
                                                         Descripcion = b.Descripcion
                                                     }).ToList(),
                                PeriodoD = objPeriodoActual.Anio + " " + objPeriodoActual.PeriodoId,
                                PeriodoId = " " + objPeriodoActual.PeriodoId,
                                Anio = " " + objPeriodoActual.Anio,
                                Inscrito = true,
                                Academica = listDesc.Count > 0 ?
                                            listDesc.FirstOrDefault().Monto > 0 ?
                                            listDesc.FirstOrDefault().EsSEP == false && listDesc.FirstOrDefault().EsComite == false ?
                                            true : false : false : false,
                                Comite = listDesc.Count > 0 ?
                                            listDesc.FirstOrDefault().Monto > 0 ?
                                           listDesc.FirstOrDefault().EsComite == true ? true
                                           : false : false : false,
                                SEP = listDesc.Count > 0 ? listDesc.FirstOrDefault().Monto > 0 ?
                                           listDesc.FirstOrDefault().EsSEP == true ? true
                                           : false : false : false,
                                lstPagos = (from a in lstPagos
                                            select new PagosAlumnos
                                            {
                                                SubPeriodo = a.SubperiodoId,
                                                Concepto = a.Cuota1.PagoConcepto.Descripcion,
                                                PagoId = "" + a.PagoId,
                                                ReferenciaId = "" + int.Parse(a.ReferenciaId)
                                            }).ToList(),
                                Materias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 304
                                                          || I.Cuota1.PagoConceptoId == 320).ToList().Count,
                                NuevoIngreso = (objAlumnoDB.Anio == objPeriodoActual.Anio && objAlumnoDB.PeriodoId == objPeriodoActual.PeriodoId) ? true : false,
                                Asesorias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 15)
                                                            .ToList().Count,
                                Completa = lstPagos.Where(p =>
                                                      p.Cuota1.PagoConceptoId == 802
                                                      && p.Cuota1.PagoConceptoId == 800).ToList().Count == 5 ? true : false,
                                EsEspecial = objcom.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion?.FirstOrDefault()?.EsEspecial ?? false,

                                Grupo = (objcom.FirstOrDefault().Alumno?.GrupoAlumnoConfiguracion).Where(o => o.OfertaEducativaId == OfertaEducativaId).FirstOrDefault()?.Grupo.Descripcion ?? ""
                            };
                        }
                    }
                    #endregion
                    #region No Inscrito
                    else
                    {
                        #region Solo Primeros Pagos
                        if (lstPagos.Where(o => o.Cuota1.PagoConceptoId == 800 ||
                                            o.Cuota1.PagoConceptoId == 802).ToList().Count == 2)
                        {
                            //Empresa
                            #region Empresa
                            //if (objcom.Where(s => s.EsEmpresa == true).ToList().Count > 0)
                            if (db.AlumnoInscrito.Where(s => s.AlumnoId == AlumnoId
                                                            && s.EsEmpresa == true
                                                            && s.OfertaEducativaId == OfertaEducativaId).ToList().Count > 0)
                            {
                                var objAlumnoNI = db.AlumnoInscrito.Where(s => s.AlumnoId == AlumnoId
                                                              && s.EsEmpresa == true
                                                              && s.OfertaEducativaId == OfertaEducativaId).ToList();
                                objRegresa = new AlumnoPagos
                                {
                                    AlumnoId = lstPagos.Where(o => o.Cuota1.PagoConceptoId == 15
                                                      || o.Cuota1.PagoConceptoId == 304
                                                      || o.Cuota1.PagoConceptoId == 320).ToList().Count > 0 ? "-5" : "-1",
                                    Nombre = objAlumnoDB.Nombre + " " + objAlumnoDB.Paterno + " " + objAlumnoDB.Materno,
                                    OfertasEducativas = (from b in db.OfertaEducativa
                                                         where b.OfertaEducativaId == OfertaEducativaId
                                                         select new DTOOfertaEducativa
                                                         {
                                                             OfertaEducativaId = b.OfertaEducativaId,
                                                             OfertaEducativaTipoId = b.OfertaEducativaTipoId,
                                                             Descripcion = b.Descripcion
                                                         }).ToList(),
                                    PeriodoD = objPeriodoActual.Anio + " " + objPeriodoActual.PeriodoId,
                                    PeriodoId = " " + objPeriodoActual.PeriodoId,
                                    Anio = " " + objPeriodoActual.Anio,
                                    Inscrito = false,
                                    Academica = false,
                                    Comite = false,
                                    SEP = false,
                                    EsEmpresa=true,
                                    lstPagos = (from a in lstPagos
                                                select new PagosAlumnos
                                                {
                                                    SubPeriodo = a.SubperiodoId,
                                                    Concepto = a.Cuota1.PagoConcepto.Descripcion,
                                                    PagoId = "" + a.PagoId,
                                                    ReferenciaId = "" + int.Parse(a.ReferenciaId)
                                                }).ToList(),
                                    Materias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 304
                                                              || I.Cuota1.PagoConceptoId == 320).ToList().Count,
                                    NuevoIngreso = (objAlumnoDB.Anio == objPeriodoActual.Anio && objAlumnoDB.PeriodoId == objPeriodoActual.PeriodoId) ? true : false,
                                    Asesorias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 15)
                                                            .ToList().Count,
                                    EsEspecial = objAlumnoNI.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion?.FirstOrDefault()?.EsEspecial ?? false,

                                    Grupo = (objAlumnoNI.FirstOrDefault().Alumno?.GrupoAlumnoConfiguracion).Where(o => o.OfertaEducativaId == OfertaEducativaId).FirstOrDefault()?.Grupo.Descripcion ?? ""
                                };
                            }
                            #endregion
                            #region No Empresa
                            //No Es Empresa
                            else
                            {
                                var objAlumnoNI = db.AlumnoInscrito.Where(s => s.AlumnoId == AlumnoId
                                                              && s.OfertaEducativaId == OfertaEducativaId).ToList();

                                objRegresa = new AlumnoPagos();

                                objRegresa.AlumnoId = lstPagos.Where(o => o.Cuota1.PagoConceptoId == 15
                                                      || o.Cuota1.PagoConceptoId == 304
                                                      || o.Cuota1.PagoConceptoId == 320).ToList().Count > 0 ? "-5" : "-2";
                                objRegresa.Nombre = objAlumnoDB.Nombre + " " + objAlumnoDB.Paterno + " " + objAlumnoDB.Materno;
                                objRegresa.OfertasEducativas = (from b in db.OfertaEducativa
                                                                where b.OfertaEducativaId == OfertaEducativaId
                                                                select new DTOOfertaEducativa
                                                                {
                                                                    OfertaEducativaId = b.OfertaEducativaId,
                                                                    OfertaEducativaTipoId = b.OfertaEducativaTipoId,
                                                                    Descripcion = b.Descripcion
                                                                }).ToList();
                                objRegresa.PeriodoD = objPeriodoActual.Anio + " " + objPeriodoActual.PeriodoId;
                                objRegresa.PeriodoId = " " + objPeriodoActual.PeriodoId;
                                objRegresa.Anio = " " + objPeriodoActual.Anio;
                                objRegresa.Inscrito = false;
                                objRegresa.Academica = false;
                                objRegresa.Comite = false;
                                objRegresa.SEP = false;
                                objRegresa.lstPagos = (from a in lstPagos
                                                       select new PagosAlumnos
                                                       {
                                                           SubPeriodo = a.SubperiodoId,
                                                           Concepto = a.Cuota1.PagoConcepto.Descripcion,
                                                           PagoId = "" + a.PagoId,
                                                           ReferenciaId = "" + int.Parse(a.ReferenciaId)
                                                       }).ToList();
                                objRegresa.Materias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 304
                                                      || I.Cuota1.PagoConceptoId == 320).ToList().Count;
                                objRegresa.NuevoIngreso = (objAlumnoDB.Anio == objPeriodoActual.Anio && objAlumnoDB.PeriodoId == objPeriodoActual.PeriodoId) ? true : false;
                                objRegresa.Asesorias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 15)
                                                        .ToList().Count;
                                objRegresa.EsEspecial = objAlumnoNI.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion?.FirstOrDefault()?.EsEspecial ?? false;

                                objRegresa.Grupo = (objAlumnoNI.FirstOrDefault().Alumno?.GrupoAlumnoConfiguracion).Where(o => o.OfertaEducativaId == OfertaEducativaId).FirstOrDefault()?.Grupo.Descripcion ?? "";

                            }
                            #endregion
                        }
                        #endregion
                        #region No tiene Pagos
                        else if (lstPagos.Where(o => o.Cuota1.PagoConceptoId == 800
                                                    || o.Cuota1.PagoConceptoId == 802).ToList().Count == 0)
                        {
                            if (objcom.Count == 0)
                            {
                                objcom.Add(db.AlumnoInscrito.Where(k => k.AlumnoId == AlumnoId && k.OfertaEducativaId == OfertaEducativaId).FirstOrDefault());
                            }
                            List<Pago> lstPagos2 = db.Pago.Where(i => i.AlumnoId == AlumnoId
                                                 && i.OfertaEducativaId == OfertaEducativaId
                                                 && i.EstatusId != 2
                                                 && (i.Cuota1.PagoConceptoId == 800
                                                 || i.Cuota1.PagoConceptoId == 802
                                                 || i.Cuota1.PagoConceptoId == 304
                                                 || i.Cuota1.PagoConceptoId == 320
                                                 || i.Cuota1.PagoConceptoId == 15)).ToList();
                            lstPagosAlumno.AddRange(lstPagos2);

                            //Empresa
                            if (objcom.Where(s => s.EsEmpresa == true).ToList().Count > 0)
                            {
                                objRegresa = new AlumnoPagos
                                {
                                    AlumnoId = lstPagos.Where(o => o.Cuota1.PagoConceptoId == 15
                                                       || o.Cuota1.PagoConceptoId == 304
                                                       || o.Cuota1.PagoConceptoId == 320).ToList().Count > 0 ? "-5" : "-21",
                                    Nombre = objAlumnoDB.Nombre + " " + objAlumnoDB.Paterno + " " + objAlumnoDB.Materno,
                                    OfertasEducativas = (from b in db.OfertaEducativa
                                                         where b.OfertaEducativaId == OfertaEducativaId
                                                         select new DTOOfertaEducativa
                                                         {
                                                             OfertaEducativaId = b.OfertaEducativaId,
                                                             OfertaEducativaTipoId = b.OfertaEducativaTipoId,
                                                             Descripcion = b.Descripcion
                                                         }).ToList(),
                                    PeriodoD = objPeriodoActual.Anio + " " + objPeriodoActual.PeriodoId,
                                    PeriodoId = " " + objPeriodoActual.PeriodoId,
                                    Anio = " " + objPeriodoActual.Anio,
                                    Inscrito = false,
                                    Academica = false,
                                    Comite = false,
                                    SEP = false,
                                    EsEmpresa=true,
                                    lstPagos = (from a in lstPagos2
                                                select new PagosAlumnos
                                                {
                                                    SubPeriodo = a.SubperiodoId,
                                                    Concepto = a.Cuota1.PagoConcepto.Descripcion,
                                                    PagoId = "" + a.PagoId,
                                                    ReferenciaId = "" + int.Parse(a.ReferenciaId)
                                                }).ToList(),
                                    Materias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 304
                                                          || I.Cuota1.PagoConceptoId == 320).ToList().Count,
                                    NuevoIngreso = (objAlumnoDB.Anio == objPeriodoActual.Anio && objAlumnoDB.PeriodoId == objPeriodoActual.PeriodoId) ? true : false,
                                Asesorias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 15)
                                                            .ToList().Count,
                                    EsEspecial = objcom.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion?.FirstOrDefault()?.EsEspecial ?? false,

                                    Grupo = (objcom.FirstOrDefault().Alumno?.GrupoAlumnoConfiguracion).Where(o => o.OfertaEducativaId == OfertaEducativaId).FirstOrDefault()?.Grupo.Descripcion ?? ""
                                };
                            }
                            //Normal 
                            else
                            {

                                objRegresa = new AlumnoPagos
                                {
                                    AlumnoId = lstPagos.Where(o => o.Cuota1.PagoConceptoId == 15
                                                      || o.Cuota1.PagoConceptoId == 304
                                                      || o.Cuota1.PagoConceptoId == 320).ToList().Count > 0 ? "-5" : "-4",
                                    Nombre = objAlumnoDB.Nombre + " " + objAlumnoDB.Paterno + " " + objAlumnoDB.Materno,
                                    OfertasEducativas = (from b in db.OfertaEducativa
                                                         where b.OfertaEducativaId == OfertaEducativaId
                                                         select new DTOOfertaEducativa
                                                         {
                                                             OfertaEducativaId = b.OfertaEducativaId,
                                                             OfertaEducativaTipoId = b.OfertaEducativaTipoId,
                                                             Descripcion = b.Descripcion
                                                         }).ToList(),
                                    PeriodoD = objPeriodoActual.Anio + " " + objPeriodoActual.PeriodoId,
                                    PeriodoId = " " + objPeriodoActual.PeriodoId,
                                    Anio = " " + objPeriodoActual.Anio,
                                    Inscrito = false,
                                    Academica = false,
                                    Comite = false,
                                    SEP = false,
                                    lstPagos = (from a in lstPagos2
                                                select new PagosAlumnos
                                                {
                                                    SubPeriodo = a.SubperiodoId,
                                                    Concepto = a.Cuota1.PagoConcepto.Descripcion,
                                                    PagoId = "" + a.PagoId,
                                                    ReferenciaId = "" + int.Parse(a.ReferenciaId)
                                                }).ToList(),
                                    Materias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 304
                                                          || I.Cuota1.PagoConceptoId == 320).ToList().Count,
                                    Asesorias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 15)
                                                            .ToList().Count,
                                    NuevoIngreso = (objAlumnoDB.Anio == objPeriodoActual.Anio && objAlumnoDB.PeriodoId == objPeriodoActual.PeriodoId) ? true : false,
                                    EsEspecial = objcom.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion?.FirstOrDefault()?.EsEspecial ?? false,

                                    Grupo = (objcom.FirstOrDefault().Alumno?.GrupoAlumnoConfiguracion).Where(o => o.OfertaEducativaId == OfertaEducativaId).FirstOrDefault()?.Grupo.Descripcion ?? ""
                                };
                            }
                        }
                        #endregion
                        #region Tiene Pagos pero no Inscrito
                        else if (lstPagos.Where(o => o.Cuota1.PagoConceptoId == 800 ||
                                           o.Cuota1.PagoConceptoId == 802).ToList().Count >= 1)
                        {
                            var objAlumnoNI = db.AlumnoInscrito.Where(s => s.AlumnoId == AlumnoId
                                                             && s.OfertaEducativaId == OfertaEducativaId).ToList();
                            objRegresa = new AlumnoPagos
                            {
                                AlumnoId = "-5",
                                Nombre = objAlumnoDB.Nombre + " " + objAlumnoDB.Paterno + " " + objAlumnoDB.Materno,
                                OfertasEducativas = (from b in db.OfertaEducativa
                                                     where b.OfertaEducativaId == OfertaEducativaId
                                                     select new DTOOfertaEducativa
                                                     {
                                                         OfertaEducativaId = b.OfertaEducativaId,
                                                         OfertaEducativaTipoId = b.OfertaEducativaTipoId,
                                                         Descripcion = b.Descripcion
                                                     }).ToList(),
                                PeriodoD = objPeriodoActual.Anio + " " + objPeriodoActual.PeriodoId,
                                PeriodoId = " " + objPeriodoActual.PeriodoId,
                                Anio = " " + objPeriodoActual.Anio,
                                Inscrito = false,
                                Academica = false,
                                Comite = false,
                                SEP = false,
                                lstPagos = (from a in lstPagos
                                            select new PagosAlumnos
                                            {
                                                SubPeriodo = a.SubperiodoId,
                                                Concepto = a.Cuota1.PagoConcepto.Descripcion,
                                                PagoId = "" + a.PagoId,
                                                ReferenciaId = "" + int.Parse(a.ReferenciaId)
                                            }).ToList(),
                                Materias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 304
                                                      || I.Cuota1.PagoConceptoId == 320).ToList().Count,
                                NuevoIngreso = (objAlumnoDB.Anio == objPeriodoActual.Anio && objAlumnoDB.PeriodoId == objPeriodoActual.PeriodoId) ? true : false,
                                Asesorias = lstPagos.Where(I => I.Cuota1.PagoConceptoId == 15)
                                                           .ToList().Count,
                                Completa = lstPagos.Where(p =>
                                                      p.Cuota1.PagoConceptoId == 802
                                                      && p.Cuota1.PagoConceptoId == 800).ToList().Count == 5 ? true : false,
                                EsEmpresa = objAlumnoNI.FirstOrDefault()?.EsEmpresa ?? false,
                                EsEspecial = objAlumnoNI.FirstOrDefault().Alumno.GrupoAlumnoConfiguracion?.FirstOrDefault()?.EsEspecial ?? false,

                                Grupo = (objAlumnoNI.FirstOrDefault().Alumno?.GrupoAlumnoConfiguracion).Where(o => o.OfertaEducativaId == OfertaEducativaId).FirstOrDefault()?.Grupo.Descripcion ?? ""
                            };
                        }
                        #endregion
                    }
                    #endregion


                    List<AlumnoDescuento> lstAlumnoDescuento = objAlumnoDB.AlumnoDescuento.Where(P =>
                                                                        P.OfertaEducativaId == OfertaEducativaId
                                                                        && P.Anio == objPeriodoActual.Anio
                                                                        && P.PeriodoId == objPeriodoActual.PeriodoId
                                                                        && P.EstatusId==2
                                                                        && (P.PagoConceptoId == 800
                                                                            || P.PagoConceptoId == 802
                                                                            || P.PagoConceptoId == 15
                                                                            || P.PagoConceptoId == 304
                                                                            || P.PagoConceptoId == 320)).ToList();
                    objRegresa.lstPagos.ForEach(objPago =>
                    {
                        #region Buscar Descuentos
                        Pago objPAgoDB = lstPagosAlumno.Where(a => a.PagoId == int.Parse(objPago.PagoId)).FirstOrDefault();
                        if (objPAgoDB.Cuota1.PagoConceptoId == 802)
                        {
                            if (lstAlumnoDescuento.Where(s => s.EsSEP == true && s.PagoConceptoId == 802).ToList().Count > 0)
                            {
                                objPago.BecaSEP = lstAlumnoDescuento.Where(s => s.EsSEP == true && s.PagoConceptoId == 802).ToList().FirstOrDefault().Monto.ToString() + "%";

                                objPago.BecaSEPD = lstAlumnoDescuento.Where(s => s.EsSEP == true && s.PagoConceptoId == 802).ToList().FirstOrDefault().Monto;

                                objPago.CargoD = (objPAgoDB.Cuota - objPAgoDB.PagoDescuento.Where(P => P.Descuento.Descripcion == "Pago Anticipado").ToList()
                                    .Sum(P => P.Monto));
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);

                                objPago.BecaAcademica = "0";
                                objPago.BecaAcademicaD = 0;
                            }
                            else
                            {
                                objPago.BecaSEP = "0";
                                objPago.BecaSEPD = 0;
                                objPago.BecaAcademica = "0";
                                objPago.BecaAcademicaD = 0;
                                objPago.CargoD = objPAgoDB.Promesa;
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);
                            }
                        }
                        else if (lstPagosAlumno.Where(a => a.PagoId == int.Parse(objPago.PagoId)).FirstOrDefault().Cuota1.PagoConceptoId == 800)
                        {
                            if (lstAlumnoDescuento.Where(s => s.EsSEP == true && s.PagoConceptoId == 800).ToList().Count > 0)
                            {
                                objPago.BecaSEP = lstAlumnoDescuento.Where(s => s.EsSEP == true && s.PagoConceptoId == 800).ToList().FirstOrDefault().Monto.ToString() + "%";

                                objPago.BecaSEPD = lstAlumnoDescuento.Where(s => s.EsSEP == true && s.PagoConceptoId == 800).ToList().FirstOrDefault().Monto;

                                objPago.CargoD = (objPAgoDB.Cuota - objPAgoDB.PagoDescuento.Where(P => P.Descuento.Descripcion == "Pago Anticipado").ToList()
                                    .Sum(P => P.Monto));
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);

                                objPago.BecaAcademica = "0";
                                objPago.BecaAcademicaD = 0;
                            }
                            else if (lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca Académica"
                              && s.PagoConceptoId == 800).ToList().Count > 0)
                            {
                                objPago.BecaSEP = "0";

                                objPago.BecaSEPD = 0;

                                objPago.CargoD = (objPAgoDB.Cuota - objPAgoDB.PagoDescuento.Where(P => P.Descuento.Descripcion == "Pago Anticipado").ToList()
                                    .Sum(P => P.Monto));
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);

                                objPago.BecaAcademica = lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca Académica"
                               && s.PagoConceptoId == 800).ToList().FirstOrDefault().Monto.ToString() + "%";

                                objPago.BecaAcademicaD = lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca Académica"
                                && s.PagoConceptoId == 800).ToList().FirstOrDefault().Monto;
                            }
                            else
                            {
                                objPago.BecaSEP = "0";
                                objPago.BecaSEPD = 0;
                                objPago.BecaAcademica = "0";
                                objPago.BecaAcademicaD = 0;
                                objPago.CargoD = objPAgoDB.Promesa;
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);
                            }
                        }
                        #endregion
                        decimal total;
                        objPago.BecaAcademica = objPago.BecaAcademicaD.ToString() + "%";
                        objPago.BecaSEP = objPago.BecaSEPD.ToString() + "%";
                        total = objPago.BecaAcademicaD != 0 ?
                            objPago.CargoD * (objPago.BecaAcademicaD / 100)
                            : objPago.BecaSEPD != 0 ?
                            objPago.CargoD * (objPago.BecaSEPD / 100)
                            : objPago.CargoD;
                        total = Math.Round(total);
                        objPago.TotalPagar = total.ToString("C", Cultura);
                    });

                    objRegresa.Revision = objRegresa.NuevoIngreso ? true : (db.AlumnoRevision
                                            .Where(o =>
                                                o.AlumnoId == AlumnoId
                                                && o.OfertaEducativaId == OfertaEducativaId
                                                && o.Anio == objPeriodoActual.Anio
                                                && o.PeriodoId == objPeriodoActual.PeriodoId)
                                                .ToList()
                                                .Count > 0 ? true : false);
                    return objRegresa;
                }
                catch (Exception e)
                {
                    AlumnoPagos objP = new AlumnoPagos();
                    objP.Nombre = e.Message;
                    return objP;
                }
            }
        }
        public static AlumnoPagos BuscarAlumno(int AlumnoId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    DTOPeriodo objPeriodoActual = BLLPeriodoPortal.TraerPeriodoEntreFechas(DateTime.Now);
                    Alumno objAlumnoDB = db.Alumno.Where(A => A.AlumnoId == AlumnoId).FirstOrDefault();
                    List<Pago> lstPagosAlumno = new List<Pago>();
                    if (objAlumnoDB == null)
                    {
                        return new AlumnoPagos
                        {
                            AlumnoId = "-3"
                        };
                    }
                    int OFerta = 0;
                    if (objAlumnoDB.AlumnoInscrito.Where(a => a.Anio == objPeriodoActual.Anio && a.PeriodoId == objPeriodoActual.PeriodoId).ToList().Count > 0)
                    {
                        objAlumnoDB.AlumnoInscrito.Where(a => a.Anio == objPeriodoActual.Anio &&
                            a.PeriodoId == objPeriodoActual.PeriodoId).ToList().ForEach(a =>
                            {
                                lstPagosAlumno.AddRange(db.Pago.Where(P => P.AlumnoId == AlumnoId && P.Anio == objPeriodoActual.Anio
                                    && P.PeriodoId == objPeriodoActual.PeriodoId
                                    && (P.EstatusId == 1 || P.EstatusId == 4 || P.EstatusId == 13 || P.EstatusId == 14)
                                    && (P.Cuota1.PagoConceptoId == 800 || P.Cuota1.PagoConceptoId == 802)
                                    && a.OfertaEducativaId == P.OfertaEducativaId).ToList());
                            });
                    }

                    if (lstPagosAlumno.Count == 0)
                    {
                        string EstatusAlumno = "";
                        if (objAlumnoDB.AlumnoInscrito.Where(AI => AI.OfertaEducativaId == objAlumnoDB.AlumnoInscrito.ToList().FirstOrDefault().OfertaEducativaId).FirstOrDefault().EsEmpresa == true)
                        {
                            EstatusAlumno = "-21";

                        }
                        else
                        {
                            EstatusAlumno = "-2";
                        }
                        EstatusAlumno = "";
                    }
                    OFerta = (int)lstPagosAlumno.FirstOrDefault().OfertaEducativaId;
                    if (OFerta == 0)
                    {
                        return new AlumnoPagos
                        {
                            AlumnoId = "-4"
                        };
                    }
                    AlumnoPagos objAlumno = new AlumnoPagos
                    {
                        AlumnoId = "" + objAlumnoDB.AlumnoId,
                        Nombre = objAlumnoDB.Nombre + " " + objAlumnoDB.Paterno + " " + objAlumnoDB.Materno,
                        OfertasEducativas = (from a in objAlumnoDB.AlumnoInscrito
                                             join b in lstPagosAlumno on a.OfertaEducativaId equals b.OfertaEducativaId
                                             select new DTOOfertaEducativa
                                             {
                                                 OfertaEducativaId = a.OfertaEducativa.OfertaEducativaId,
                                                 OfertaEducativaTipoId = a.OfertaEducativa.OfertaEducativaTipoId,
                                                 Descripcion = a.OfertaEducativa.Descripcion,
                                                 Rvoe = a.OfertaEducativa.Rvoe
                                             }).ToList()
                    };
                    objAlumno.PeriodoD = (from a in objAlumnoDB.AlumnoInscrito
                                          join b in lstPagosAlumno on a.OfertaEducativaId equals b.OfertaEducativaId
                                          select b.Anio + " " + b.PeriodoId).FirstOrDefault();
                    objAlumno.PeriodoId = " " + (from a in objAlumnoDB.AlumnoInscrito
                                                 join b in lstPagosAlumno on a.OfertaEducativaId equals b.OfertaEducativaId
                                                 select b.PeriodoId).FirstOrDefault();
                    objAlumno.Anio = " " + (from a in objAlumnoDB.AlumnoInscrito
                                            join b in lstPagosAlumno on a.OfertaEducativaId equals b.OfertaEducativaId
                                            select b.Anio).FirstOrDefault();
                    objAlumno.lstPagos = (from a in objAlumnoDB.AlumnoInscrito
                                          join b in lstPagosAlumno on a.OfertaEducativaId equals b.OfertaEducativaId
                                          select new PagosAlumnos
                                          {
                                              SubPeriodo = b.SubperiodoId,
                                              Concepto = b.Cuota1.PagoConcepto.Descripcion,
                                              PagoId = "" + b.PagoId,
                                              ReferenciaId = "" + int.Parse(b.ReferenciaId),
                                              //TotalPagar = b.Cuota.ToString("C", Cultura),
                                          }).ToList();
                    List<AlumnoDescuento> lstAlumnoDescuento = objAlumnoDB.AlumnoDescuento.Where(P => P.OfertaEducativaId == OFerta && P.Anio == objPeriodoActual.Anio &&
                        P.PeriodoId == objPeriodoActual.PeriodoId && (P.PagoConceptoId == 800 || P.PagoConceptoId == 802)).ToList();

                    objAlumno.lstPagos.ForEach(delegate (PagosAlumnos objPago)
                    {
                        #region Buscar Descuentos
                        Pago objPAgoDB = lstPagosAlumno.Where(a => a.PagoId == int.Parse(objPago.PagoId)).FirstOrDefault();
                        if (objPAgoDB.Cuota1.PagoConceptoId == 802)
                        {
                            if (lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca SEP"
                                && s.PagoConceptoId == 802).ToList().Count > 0)
                            {
                                objPago.BecaSEP = lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca SEP"
                                && s.PagoConceptoId == 802).ToList().FirstOrDefault().Monto.ToString() + "%";

                                objPago.BecaSEPD = lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca SEP"
                                && s.PagoConceptoId == 802).ToList().FirstOrDefault().Monto;

                                objPago.CargoD = (objPAgoDB.Cuota - objPAgoDB.PagoDescuento.Where(P => P.Descuento.Descripcion == "Pago Anticipado").ToList()
                                    .Sum(P => P.Monto));
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);

                                objPago.BecaAcademica = "0";
                                objPago.BecaAcademicaD = 0;
                            }
                            else
                            {
                                objPago.BecaSEP = "0";
                                objPago.BecaSEPD = 0;
                                objPago.BecaAcademica = "0";
                                objPago.BecaAcademicaD = 0;
                                objPago.CargoD = objPAgoDB.Promesa;
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);
                            }
                        }
                        else if (lstPagosAlumno.Where(a => a.PagoId == int.Parse(objPago.PagoId)).FirstOrDefault().Cuota1.PagoConceptoId == 800)
                        {
                            if (lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca SEP"
                               && s.PagoConceptoId == 800).ToList().Count > 0)
                            {
                                objPago.BecaSEP = lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca SEP"
                                && s.PagoConceptoId == 800).ToList().FirstOrDefault().Monto.ToString() + "%";

                                objPago.BecaSEPD = lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca SEP"
                                && s.PagoConceptoId == 800).ToList().FirstOrDefault().Monto;

                                objPago.CargoD = (objPAgoDB.Cuota - objPAgoDB.PagoDescuento.Where(P => P.Descuento.Descripcion == "Pago Anticipado").ToList()
                                    .Sum(P => P.Monto));
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);

                                objPago.BecaAcademica = "0";
                                objPago.BecaAcademicaD = 0;
                            }
                            else if (lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca Académica"
                              && s.PagoConceptoId == 800).ToList().Count > 0)
                            {
                                objPago.BecaSEP = "0";

                                objPago.BecaSEPD = 0;

                                objPago.CargoD = (objPAgoDB.Cuota - objPAgoDB.PagoDescuento.Where(P => P.Descuento.Descripcion == "Pago Anticipado").ToList()
                                    .Sum(P => P.Monto));
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);

                                objPago.BecaAcademica = lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca Académica"
                               && s.PagoConceptoId == 800).ToList().FirstOrDefault().Monto.ToString() + "%";

                                objPago.BecaAcademicaD = lstAlumnoDescuento.Where(s => db.Descuento.Where(d => d.DescuentoId == s.DescuentoId).FirstOrDefault().Descripcion == "Beca Académica"
                                && s.PagoConceptoId == 800).ToList().FirstOrDefault().Monto;
                            }
                            else
                            {
                                objPago.BecaSEP = "0";
                                objPago.BecaSEPD = 0;
                                objPago.BecaAcademica = "0";
                                objPago.BecaAcademicaD = 0;
                                objPago.CargoD = objPAgoDB.Promesa;
                                objPago.Cargo = objPago.CargoD.ToString("C", Cultura);
                            }
                        }
                        #endregion
                        decimal total;
                        objPago.BecaAcademica = objPago.BecaAcademicaD.ToString() + "%";
                        objPago.BecaSEP = objPago.BecaSEPD.ToString() + "%";
                        total = objPago.BecaAcademicaD != 0 ?
                            objPago.CargoD * (objPago.BecaAcademicaD / 100)
                            : objPago.BecaSEPD != 0 ?
                            objPago.CargoD * (objPago.BecaSEPD / 100)
                            : objPago.CargoD;
                        total = Math.Round(total);
                        objPago.TotalPagar = total.ToString("C", Cultura);
                    });

                    if (lstPagosAlumno.Count >= 5)
                    {
                        int Oferta = lstPagosAlumno[0].OfertaEducativaId;
                        int inscrito = db.AlumnoInscritoBeca.Where(I => I.AlumnoId == AlumnoId && I.OfertaEducativaId == OFerta
                            && I.Anio == objPeriodoActual.Anio && I.PeriodoId == objPeriodoActual.PeriodoId).ToList().Count > 0 ?
                            db.AlumnoInscritoBeca.Where(I => I.AlumnoId == AlumnoId && I.OfertaEducativaId == OFerta
                            && I.Anio == objPeriodoActual.Anio && I.PeriodoId == objPeriodoActual.PeriodoId).ToList().Count : 0;

                        AlumnoDescuento objdesc = (from D in db.AlumnoDescuento
                                                   join a in db.Descuento on D.DescuentoId equals a.DescuentoId
                                                   where D.AlumnoId == AlumnoId && D.OfertaEducativaId == Oferta
                                                     && D.Anio == objPeriodoActual.Anio &&
                                                     D.PeriodoId == objPeriodoActual.PeriodoId
                                                     && D.EstatusId == 2 && (a.Descripcion == "Beca SEP"
                                                     || a.Descripcion == "Beca Académica")
                                                   select D).FirstOrDefault();

                        if (objdesc != null || inscrito > 0)
                        {
                            objAlumno.Inscrito = true;
                        }
                        else
                        {
                            List<PagosAlumnos> lstP = objAlumno.lstPagos.Where(P => P.SubPeriodo == 1).ToList();
                            objAlumno.lstPagos = lstP;
                        }
                    }
                    if (objAlumnoDB.AlumnoInscrito.Where(AI => AI.OfertaEducativaId == objAlumno.OfertasEducativas.FirstOrDefault().OfertaEducativaId).FirstOrDefault().EsEmpresa == true)
                    {
                        objAlumno.AlumnoId = "-1";
                        return objAlumno;
                    }
                    return objAlumno;
                }
                catch (Exception e)
                {
                    AlumnoPagos objP = new AlumnoPagos();
                    objP.Nombre = e.Message;
                    return objP;
                }
            }
        }

        public static void AplicaBecaAlumno1(DTO.Alumno.Beca.DTOAlumnoBeca AlumnoBeca)
        {
            List<DAL.Pago> PagosPendientes = new List<Pago>();
            List<Pago> Cabecero = new List<Pago>();
            DAL.AlumnoDescuento DescuentoColegiatura = new AlumnoDescuento();
            DAL.AlumnoDescuento DescuentoInscripcion = new AlumnoDescuento();
            List<DTO.Varios.DTOEstatus> EstadosActivos = BLL.BLLVarios.EstatusActivos();
            int[] Estatus = { 1, 4, 13, 14 };
            int[] Subperiodos = { 1, 2, 3, 4 };
            List<DTO.ReciboDatos> recibos = new List<ReciboDatos>();
            List<DAL.Recibo> Recibos = new List<DAL.Recibo>();
            int descuentoIdColegiatura, descuentoIdInscripcion;
            DTO.Usuario.DTOUsuario Usuario;

            using (UniversidadEntities db = new UniversidadEntities())
            {
                Usuario = (from a in db.Usuario
                           where a.UsuarioId == AlumnoBeca.usuarioId
                           select new DTO.Usuario.DTOUsuario
                           {
                               usuarioId = a.UsuarioId,
                               usuarioTipoId = a.UsuarioTipoId
                           }).AsNoTracking().FirstOrDefault();

                #region Pagos Pendientes

                PagosPendientes.AddRange((from a in db.Pago
                                          where (a.Cuota1.PagoConceptoId == 800 || (AlumnoBeca.esSEP ? a.Cuota1.PagoConceptoId == 802 : a.Cuota1.PagoConceptoId == 800))
                                             && Estatus.Contains(a.EstatusId)
                                             && (a.Anio == AlumnoBeca.anio && a.PeriodoId == AlumnoBeca.periodoId)
                                             && a.Promesa > 0
                                             && a.AlumnoId == AlumnoBeca.alumnoId
                                          select a).ToList());

                #endregion Pagos Pendientes

                #region Generación de Cargos

                var Colegiaturas = (from a in db.Pago
                                    join b in db.Cuota on a.CuotaId equals b.CuotaId
                                    where a.AlumnoId == AlumnoBeca.alumnoId
                                    && Estatus.Contains(a.EstatusId)
                                    && b.PagoConceptoId == 800
                                    && a.Anio == AlumnoBeca.anio
                                    && a.PeriodoId == AlumnoBeca.periodoId
                                    && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                    select a).AsNoTracking().ToList();

                var Inscripcion = (from a in db.Pago
                                   join b in db.Cuota on a.CuotaId equals b.CuotaId
                                   where a.AlumnoId == AlumnoBeca.alumnoId
                                   && Estatus.Contains(a.EstatusId)
                                   && b.PagoConceptoId == 802
                                   && a.Anio == AlumnoBeca.anio
                                   && a.PeriodoId == AlumnoBeca.periodoId
                                   && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                   select a).AsNoTracking().ToList();

                var CuotaColegiatura = (from a in db.Cuota
                                        where a.Anio == AlumnoBeca.anio
                                        && a.PeriodoId == AlumnoBeca.periodoId
                                        && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                        && a.PagoConceptoId == 800
                                        select a).AsNoTracking().FirstOrDefault();

                var CuotaInscripcion = (from a in db.Cuota
                                        where a.Anio == AlumnoBeca.anio
                                        && a.PeriodoId == AlumnoBeca.periodoId
                                        && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                        && a.PagoConceptoId == 802
                                        select a).AsNoTracking().FirstOrDefault();

                for (int i = 1; i <= 4; i++)

                    if (Colegiaturas.Count(n => n.SubperiodoId == i) == 0)
                        Cabecero.Add(new Pago
                        {
                            ReferenciaId = "",
                            AlumnoId = AlumnoBeca.alumnoId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            SubperiodoId = i,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            FechaGeneracion = DateTime.Now,
                            HoraGeneracion = DateTime.Now.TimeOfDay,
                            CuotaId = CuotaColegiatura.CuotaId,
                            Cuota = CuotaColegiatura.Monto,
                            Promesa = CuotaColegiatura.Monto,
                            Restante = CuotaColegiatura.Monto,
                            EstatusId = 1,
                            EsEmpresa = false,
                            EsAnticipado = false,
                            UsuarioId = Usuario.usuarioId,
                            UsuarioTipoId = Usuario.usuarioTipoId,
                            PeriodoAnticipadoId = 0
                        });

                if (Inscripcion == null)
                    Cabecero.Add(new Pago
                    {
                        ReferenciaId = "",
                        AlumnoId = AlumnoBeca.alumnoId,
                        Anio = AlumnoBeca.anio,
                        PeriodoId = AlumnoBeca.periodoId,
                        SubperiodoId = 1,
                        OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                        FechaGeneracion = DateTime.Now,
                        HoraGeneracion = DateTime.Now.TimeOfDay,
                        CuotaId = CuotaInscripcion.CuotaId,
                        Cuota = CuotaInscripcion.Monto,
                        Promesa = CuotaInscripcion.Monto,
                        Restante = CuotaInscripcion.Monto,
                        EstatusId = 1,
                        EsEmpresa = false,
                        EsAnticipado = false,
                        UsuarioId = Usuario.usuarioId,
                        UsuarioTipoId = Usuario.usuarioTipoId,
                        PeriodoAnticipadoId = 0
                    });

                if (Cabecero.Count > 0)
                {

                    Cabecero.ForEach(n =>
                    {
                        db.Pago.Add(n);
                    });

                    db.SaveChanges();

                    Cabecero.ForEach(n =>
                    {
                        n.ReferenciaId = db.spGeneraReferencia(n.PagoId).FirstOrDefault();
                    });

                    db.SaveChanges();
                }

                #endregion Generación de Cargos

                #region Verificar Descuentos

                DescuentoColegiatura = (from a in db.AlumnoDescuento
                                        join b in db.Descuento on a.DescuentoId equals b.DescuentoId
                                        where a.AlumnoId == AlumnoBeca.alumnoId
                                        && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                        && a.Anio == AlumnoBeca.anio
                                        && a.PeriodoId == AlumnoBeca.periodoId
                                        && a.PagoConceptoId == 800
                                        && (b.Descripcion == "Descuento en colegiatura" || b.Descripcion == "Beca Académica" || b.Descripcion == "Beca SEP")
                                        select a).ToList().FirstOrDefault();

                descuentoIdColegiatura = AlumnoBeca.esSEP
                        ? (from a in db.Descuento.AsNoTracking()
                           where a.PagoConceptoId == 800
                           && a.Descripcion == "Beca SEP"
                           && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                           select a.DescuentoId).FirstOrDefault()
                        : (from a in db.Descuento.AsNoTracking()
                           where a.PagoConceptoId == 800
                           && a.Descripcion == "Beca Académica"
                           && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                           select a.DescuentoId).FirstOrDefault();

                descuentoIdColegiatura = AlumnoBeca.esComite ? DescuentoColegiatura.DescuentoId : descuentoIdColegiatura;

                DescuentoInscripcion = (from a in db.AlumnoDescuento
                                        join b in db.Descuento on a.DescuentoId equals b.DescuentoId
                                        where a.AlumnoId == AlumnoBeca.alumnoId
                                              && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                              && a.Anio == AlumnoBeca.anio
                                              && a.PeriodoId == AlumnoBeca.periodoId
                                              && a.PagoConceptoId == 802
                                              && (b.Descripcion == "Descuento en inscripción" || b.Descripcion == "Beca SEP")
                                        select a).ToList().FirstOrDefault();

                descuentoIdInscripcion = AlumnoBeca.esSEP
                        ? (from a in db.Descuento.AsNoTracking()
                           where a.PagoConceptoId == 802
                           && a.Descripcion == "Beca SEP"
                           && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                           select a.DescuentoId).FirstOrDefault()
                        : 0;

                descuentoIdInscripcion = AlumnoBeca.esComite ? DescuentoInscripcion.DescuentoId : descuentoIdInscripcion;

                #endregion Verificar Descuentos

                #region Pagos Pendientes

                if (Cabecero.Count > 0)
                {
                    PagosPendientes.Clear();

                    PagosPendientes.AddRange((from a in db.Pago
                                              where (a.Cuota1.PagoConceptoId == 800 || (AlumnoBeca.esSEP ? a.Cuota1.PagoConceptoId == 802 : a.Cuota1.PagoConceptoId == 800))
                                                 && Estatus.Contains(a.EstatusId)
                                                 && (a.Anio == AlumnoBeca.anio && a.PeriodoId == AlumnoBeca.periodoId)
                                                 && a.Promesa > 0
                                                 && a.AlumnoId == AlumnoBeca.alumnoId
                                              select a).ToList());
                }

                #endregion Pagos Pendientes

                PagosPendientes.ForEach(n =>
                {
                    #region Colegiatura

                    if (n.Cuota1.PagoConceptoId == 800)
                    {
                        #region Existe Descuento

                        if (DescuentoColegiatura != null)
                        {
                            #region Calculos

                            decimal diferencia = n.Promesa;
                            decimal importe = 0;
                            decimal saldo = 0;
                            decimal sumaAnterior = 0;
                            decimal montoDescuento = 0;

                            PagoDescuento Descuento = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && s.DescuentoId == DescuentoColegiatura.DescuentoId).FirstOrDefault();
                            sumaAnterior = n.Promesa + (Descuento != null ? Descuento.Monto : 0);
                            n.Promesa = n.Promesa + (Descuento != null ? Descuento.Monto : 0) - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                            montoDescuento = sumaAnterior - Math.Round(n.Promesa, 2);

                            db.PagoDescuento.Add(new PagoDescuento
                            {
                                PagoId = n.PagoId,
                                DescuentoId = descuentoIdColegiatura,
                                Monto = montoDescuento
                            });

                            n.Promesa = sumaAnterior - montoDescuento;
                            importe = diferencia - n.Promesa;

                            if (importe >= 0)
                                n.Restante = n.Restante - importe;
                            else if (importe < 0)
                                n.Restante = n.Restante + Math.Abs(importe);

                            if (n.Restante <= 0)
                            {
                                saldo = Math.Abs(n.Restante);
                                n.Restante = 0;
                            }

                            if (n.Restante > 0)
                                n.EstatusId = 1;
                            else if (n.Restante == 0)
                                n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                            #endregion Calculos

                            #region Saldo a Favor

                            if (saldo > 0)
                            {
                                #region Referenciados

                                var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                Referenciados.ForEach(s =>
                                {

                                    if (saldo > 0)
                                    {
                                        #region PagoParcial Bitacora

                                        db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                        {
                                            PagoParcialId = s.PagoParcialId,
                                            PagoId = s.PagoId,
                                            SucursalCajaId = s.SucursalCajaId,
                                            ReciboId = s.ReciboId,
                                            Pago = s.Pago,
                                            FechaPago = s.FechaPago,
                                            HoraPago = s.HoraPago,
                                            EstatusId = s.EstatusId,
                                            TieneMovimientos = s.TieneMovimientos,
                                            PagoTipoId = s.PagoTipoId,
                                            ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                            FechaBitacora = DateTime.Now.Date,
                                            HoraBitacora = DateTime.Now.TimeOfDay,
                                            UsuarioId = Usuario.usuarioId
                                        });

                                        #endregion PagoParcial Bitacora

                                        if (saldo <= s.Pago)
                                        {
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                            s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                            s.Pago = s.Pago - saldo;
                                            saldo = 0;
                                        }

                                        else if (saldo > s.Pago)
                                        {
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                            s.PagoDetalle.FirstOrDefault().Importe = 0;
                                            saldo = saldo - s.Pago;
                                            s.Pago = 0;
                                            s.EstatusId = 2;
                                        }
                                    }
                                });

                                #endregion Referenciados

                                #region Caja

                                var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                Caja.ForEach(s =>
                                {
                                    if (saldo > 0)
                                    {
                                        #region PagoParcial Bitacora

                                        db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                        {
                                            PagoParcialId = s.PagoParcialId,
                                            PagoId = s.PagoId,
                                            SucursalCajaId = s.SucursalCajaId,
                                            ReciboId = s.ReciboId,
                                            Pago = s.Pago,
                                            FechaPago = s.FechaPago,
                                            HoraPago = s.HoraPago,
                                            EstatusId = s.EstatusId,
                                            TieneMovimientos = s.TieneMovimientos,
                                            PagoTipoId = s.PagoTipoId,
                                            ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                            FechaBitacora = DateTime.Now.Date,
                                            HoraBitacora = DateTime.Now.TimeOfDay,
                                            UsuarioId = Usuario.usuarioId
                                        });

                                        #endregion PagoParcial Bitacora

                                        if (saldo <= s.Pago)
                                        {
                                            /* Recibo */
                                            recibos.Add(new ReciboDatos
                                            {
                                                reciboId = s.ReciboId,
                                                sucursalCajaId = s.SucursalCajaId,
                                                importe = saldo
                                            });

                                            var Detalles = s.PagoDetalle.ToList();
                                            decimal saldoDetalles = saldo;

                                            Detalles.ForEach(a =>
                                            {
                                                if (saldoDetalles > 0)
                                                {
                                                    if (saldoDetalles <= a.Importe)
                                                    {
                                                        a.Importe = a.Importe - saldoDetalles;
                                                        saldoDetalles = 0;
                                                    }

                                                    else if (saldoDetalles > a.Importe)
                                                    {
                                                        saldoDetalles = saldoDetalles - a.Importe;
                                                        a.Importe = 0;
                                                    }
                                                }
                                            });

                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                            s.Pago = s.Pago - saldo;
                                            saldo = 0;
                                        }

                                        else if (saldo > s.Pago)
                                        {
                                            /* Recibo */
                                            recibos.Add(new ReciboDatos
                                            {
                                                reciboId = s.ReciboId,
                                                sucursalCajaId = s.SucursalCajaId,
                                                importe = s.Pago
                                            });

                                            //Duda
                                            s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                            saldo = saldo - s.Pago;
                                            s.Pago = 0;
                                            s.EstatusId = 2;
                                        }
                                    }
                                });

                                if (recibos.Count > 0)
                                {
                                    var RecibosTotal = (from consulta in
                                                            (from a in recibos
                                                             select new { a })
                                                        group consulta by new
                                                        {
                                                            consulta.a.reciboId,
                                                            consulta.a.sucursalCajaId
                                                        } into g

                                                        select new DTO.ReciboDatos
                                                        {
                                                            reciboId = g.Key.reciboId,
                                                            sucursalCajaId = g.Key.sucursalCajaId,
                                                            importe = g.Sum(a => a.a.importe)
                                                        }).ToList();

                                    RecibosTotal.ForEach(a =>
                                    {
                                        Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                    });

                                    Recibos.ForEach(a =>
                                    {
                                        a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                    });
                                }

                                #endregion Caja
                            }

                            #endregion Saldo a Favor

                            if (Descuento != null)
                                db.PagoDescuento.Remove(Descuento);
                        }

                        #endregion Existe Descuento

                        #region No Existe Descuento

                        else if (DescuentoColegiatura == null)
                        {
                            #region Calculos

                            decimal diferencia = n.Promesa;
                            decimal importe = 0;
                            decimal saldo = 0;
                            decimal sumaAnterior = 0;
                            decimal montoDescuento = 0;


                            sumaAnterior = n.Promesa;
                            n.Promesa = n.Promesa - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                            montoDescuento = sumaAnterior - Math.Round(n.Promesa, 2);

                            db.PagoDescuento.Add(new PagoDescuento
                            {
                                PagoId = n.PagoId,
                                DescuentoId = descuentoIdColegiatura,
                                Monto = montoDescuento
                            });

                            n.Promesa = sumaAnterior - montoDescuento;
                            importe = diferencia - n.Promesa;

                            if (importe >= 0)
                                n.Restante = n.Restante - importe;
                            else if (importe < 0)
                                n.Restante = n.Restante + Math.Abs(importe);

                            if (n.Restante <= 0)
                            {
                                saldo = Math.Abs(n.Restante);
                                n.Restante = 0;
                            }

                            if (n.Restante > 0)
                                n.EstatusId = 1;
                            else if (n.Restante == 0)
                                n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                            #endregion Calculos

                            #region Saldo a Favor

                            if (saldo > 0)
                            {
                                #region Referenciados

                                var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                Referenciados.ForEach(s =>
                                {

                                    if (saldo > 0)
                                    {
                                        #region PagoParcial Bitacora

                                        db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                        {
                                            PagoParcialId = s.PagoParcialId,
                                            PagoId = s.PagoId,
                                            SucursalCajaId = s.SucursalCajaId,
                                            ReciboId = s.ReciboId,
                                            Pago = s.Pago,
                                            FechaPago = s.FechaPago,
                                            HoraPago = s.HoraPago,
                                            EstatusId = s.EstatusId,
                                            TieneMovimientos = s.TieneMovimientos,
                                            PagoTipoId = s.PagoTipoId,
                                            ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                            FechaBitacora = DateTime.Now.Date,
                                            HoraBitacora = DateTime.Now.TimeOfDay,
                                            UsuarioId = Usuario.usuarioId
                                        });

                                        #endregion PagoParcial Bitacora

                                        if (saldo <= s.Pago)
                                        {
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                            s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                            s.Pago = s.Pago - saldo;
                                            saldo = 0;
                                        }

                                        else if (saldo > s.Pago)
                                        {
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                            s.PagoDetalle.FirstOrDefault().Importe = 0;
                                            saldo = saldo - s.Pago;
                                            s.Pago = 0;
                                            s.EstatusId = 2;
                                        }
                                    }
                                });

                                #endregion Referenciados

                                #region Caja

                                var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                Caja.ForEach(s =>
                                {
                                    if (saldo > 0)
                                    {
                                        #region PagoParcial Bitacora

                                        db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                        {
                                            PagoParcialId = s.PagoParcialId,
                                            PagoId = s.PagoId,
                                            SucursalCajaId = s.SucursalCajaId,
                                            ReciboId = s.ReciboId,
                                            Pago = s.Pago,
                                            FechaPago = s.FechaPago,
                                            HoraPago = s.HoraPago,
                                            EstatusId = s.EstatusId,
                                            TieneMovimientos = s.TieneMovimientos,
                                            PagoTipoId = s.PagoTipoId,
                                            ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                            FechaBitacora = DateTime.Now.Date,
                                            HoraBitacora = DateTime.Now.TimeOfDay,
                                            UsuarioId = Usuario.usuarioId
                                        });

                                        #endregion PagoParcial Bitacora

                                        if (saldo <= s.Pago)
                                        {
                                            /* Recibo */
                                            recibos.Add(new ReciboDatos
                                            {
                                                reciboId = s.ReciboId,
                                                sucursalCajaId = s.SucursalCajaId,
                                                importe = saldo
                                            });

                                            var Detalles = s.PagoDetalle.ToList();
                                            decimal saldoDetalles = saldo;

                                            Detalles.ForEach(a =>
                                            {
                                                if (saldoDetalles > 0)
                                                {
                                                    if (saldoDetalles <= a.Importe)
                                                    {
                                                        a.Importe = a.Importe - saldoDetalles;
                                                        saldoDetalles = 0;
                                                    }

                                                    else if (saldoDetalles > a.Importe)
                                                    {
                                                        saldoDetalles = saldoDetalles - a.Importe;
                                                        a.Importe = 0;
                                                    }
                                                }
                                            });

                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                            s.Pago = s.Pago - saldo;
                                            saldo = 0;
                                        }

                                        else if (saldo > s.Pago)
                                        {
                                            /* Recibo */
                                            recibos.Add(new ReciboDatos
                                            {
                                                reciboId = s.ReciboId,
                                                sucursalCajaId = s.SucursalCajaId,
                                                importe = s.Pago
                                            });

                                            //Duda
                                            s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                            saldo = saldo - s.Pago;
                                            s.Pago = 0;
                                            s.EstatusId = 2;
                                        }
                                    }
                                });

                                if (recibos.Count > 0)
                                {
                                    var RecibosTotal = (from consulta in
                                                            (from a in recibos
                                                             select new { a })
                                                        group consulta by new
                                                        {
                                                            consulta.a.reciboId,
                                                            consulta.a.sucursalCajaId
                                                        } into g

                                                        select new DTO.ReciboDatos
                                                        {
                                                            reciboId = g.Key.reciboId,
                                                            sucursalCajaId = g.Key.sucursalCajaId,
                                                            importe = g.Sum(a => a.a.importe)
                                                        }).ToList();

                                    RecibosTotal.ForEach(a =>
                                    {
                                        Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                    });

                                    Recibos.ForEach(a =>
                                    {
                                        a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                    });
                                }

                                #endregion Caja
                            }

                            #endregion Saldo a Favor
                        }

                        #endregion No Existe Descuento

                        db.SaveChanges();
                    }

                    #endregion Colegiatura

                    #region Inscripción

                    else if (n.Cuota1.PagoConceptoId == 802)
                    {
                        #region Existe Descuento

                        if (DescuentoInscripcion != null)
                        {
                            #region Calculos

                            decimal diferencia = n.Promesa;
                            decimal importe = 0;
                            decimal saldo = 0;
                            decimal sumaAnterior = 0;
                            decimal montoDescuento = 0;

                            PagoDescuento Descuento = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && s.DescuentoId == DescuentoColegiatura.DescuentoId).FirstOrDefault();
                            sumaAnterior = n.Promesa + (Descuento != null ? Descuento.Monto : 0);
                            n.Promesa = n.Promesa + (Descuento != null ? Descuento.Monto : 0) - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                            montoDescuento = sumaAnterior - Math.Round(n.Promesa, 2);

                            db.PagoDescuento.Add(new PagoDescuento
                            {
                                PagoId = n.PagoId,
                                DescuentoId = descuentoIdInscripcion,
                                Monto = montoDescuento
                            });

                            n.Promesa = sumaAnterior - montoDescuento;
                            importe = diferencia - n.Promesa;

                            if (importe >= 0)
                                n.Restante = n.Restante - importe;
                            else if (importe < 0)
                                n.Restante = n.Restante + Math.Abs(importe);

                            if (n.Restante <= 0)
                            {
                                saldo = Math.Abs(n.Restante);
                                n.Restante = 0;
                            }

                            if (n.Restante > 0)
                                n.EstatusId = 1;
                            else if (n.Restante == 0)
                                n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                            #endregion Calculos

                            #region Saldo a Favor

                            if (saldo > 0)
                            {
                                #region Referenciados

                                var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                Referenciados.ForEach(s =>
                                {
                                    if (saldo > 0)
                                    {
                                        #region PagoParcial Bitacora

                                        db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                        {
                                            PagoParcialId = s.PagoParcialId,
                                            PagoId = s.PagoId,
                                            SucursalCajaId = s.SucursalCajaId,
                                            ReciboId = s.ReciboId,
                                            Pago = s.Pago,
                                            FechaPago = s.FechaPago,
                                            HoraPago = s.HoraPago,
                                            EstatusId = s.EstatusId,
                                            TieneMovimientos = s.TieneMovimientos,
                                            PagoTipoId = s.PagoTipoId,
                                            ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                            FechaBitacora = DateTime.Now.Date,
                                            HoraBitacora = DateTime.Now.TimeOfDay,
                                            UsuarioId = Usuario.usuarioId
                                        });

                                        #endregion PagoParcial Bitacora

                                        if (saldo <= s.Pago)
                                        {
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                            s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                            s.Pago = s.Pago - saldo;
                                            saldo = 0;
                                        }

                                        else if (saldo > s.Pago)
                                        {
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                            s.PagoDetalle.FirstOrDefault().Importe = 0;
                                            saldo = saldo - s.Pago;
                                            s.Pago = 0;
                                            s.EstatusId = 2;
                                        }
                                    }
                                });

                                #endregion Referenciados

                                #region Caja

                                var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                Caja.ForEach(s =>
                                {
                                    if (saldo > 0)
                                    {
                                        #region PagoParcial Bitacora

                                        db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                        {
                                            PagoParcialId = s.PagoParcialId,
                                            PagoId = s.PagoId,
                                            SucursalCajaId = s.SucursalCajaId,
                                            ReciboId = s.ReciboId,
                                            Pago = s.Pago,
                                            FechaPago = s.FechaPago,
                                            HoraPago = s.HoraPago,
                                            EstatusId = s.EstatusId,
                                            TieneMovimientos = s.TieneMovimientos,
                                            PagoTipoId = s.PagoTipoId,
                                            ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                            FechaBitacora = DateTime.Now.Date,
                                            HoraBitacora = DateTime.Now.TimeOfDay,
                                            UsuarioId = Usuario.usuarioId
                                        });

                                        #endregion PagoParcial Bitacora

                                        if (saldo <= s.Pago)
                                        {
                                            /* Recibo */
                                            recibos.Add(new ReciboDatos
                                            {
                                                reciboId = s.ReciboId,
                                                sucursalCajaId = s.SucursalCajaId,
                                                importe = saldo
                                            });

                                            var Detalles = s.PagoDetalle.ToList();
                                            decimal saldoDetalles = saldo;

                                            Detalles.ForEach(a =>
                                            {
                                                if (saldoDetalles > 0)
                                                {
                                                    if (saldoDetalles <= a.Importe)
                                                    {
                                                        a.Importe = a.Importe - saldoDetalles;
                                                        saldoDetalles = 0;
                                                    }

                                                    else if (saldoDetalles > a.Importe)
                                                    {
                                                        saldoDetalles = saldoDetalles - a.Importe;
                                                        a.Importe = 0;
                                                    }
                                                }
                                            });

                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                            s.Pago = s.Pago - saldo;
                                            saldo = 0;
                                        }

                                        else if (saldo > s.Pago)
                                        {
                                            /* Recibo */
                                            recibos.Add(new ReciboDatos
                                            {
                                                reciboId = s.ReciboId,
                                                sucursalCajaId = s.SucursalCajaId,
                                                importe = s.Pago
                                            });

                                            //Duda

                                            s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                            saldo = saldo - s.Pago;
                                            s.Pago = 0;
                                            s.EstatusId = 2;
                                        }
                                    }
                                });

                                if (recibos.Count > 0)
                                {
                                    var RecibosTotal = (from consulta in
                                                            (from a in recibos
                                                             select new { a })
                                                        group consulta by new
                                                        {
                                                            consulta.a.reciboId,
                                                            consulta.a.sucursalCajaId
                                                        } into g

                                                        select new DTO.ReciboDatos
                                                        {
                                                            reciboId = g.Key.reciboId,
                                                            sucursalCajaId = g.Key.sucursalCajaId,
                                                            importe = g.Sum(a => a.a.importe)
                                                        }).ToList();

                                    RecibosTotal.ForEach(a =>
                                    {
                                        Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                    });

                                    Recibos.ForEach(a =>
                                    {
                                        a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                    });
                                }

                                #endregion Caja
                            }

                            #endregion Saldo a Favor

                            if (Descuento != null)
                                db.PagoDescuento.Remove(Descuento);
                        }

                        #endregion Existe Descuento

                        #region No Existe Descuento

                        else if (DescuentoInscripcion == null)
                        {
                            #region Calculos

                            decimal diferencia = n.Promesa;
                            decimal importe = 0;
                            decimal saldo = 0;
                            decimal sumaAnterior = 0;
                            decimal montoDescuento = 0;

                            sumaAnterior = n.Promesa;
                            n.Promesa = n.Promesa - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                            montoDescuento = sumaAnterior - Math.Round(n.Promesa, 2);

                            db.PagoDescuento.Add(new PagoDescuento
                            {
                                PagoId = n.PagoId,
                                DescuentoId = descuentoIdInscripcion,
                                Monto = montoDescuento
                            });

                            n.Promesa = sumaAnterior - montoDescuento;
                            importe = diferencia - n.Promesa;

                            if (importe >= 0)
                                n.Restante = n.Restante - importe;
                            else if (importe < 0)
                                n.Restante = n.Restante + Math.Abs(importe);

                            if (n.Restante <= 0)
                            {
                                saldo = Math.Abs(n.Restante);
                                n.Restante = 0;
                            }

                            if (n.Restante > 0)
                                n.EstatusId = 1;
                            else if (n.Restante == 0)
                                n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                            #endregion Calculos

                            #region Saldo a Favor

                            if (saldo > 0)
                            {
                                #region Referenciados

                                var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                Referenciados.ForEach(s =>
                                {
                                    if (saldo > 0)
                                    {
                                        #region PagoParcial Bitacora

                                        db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                        {
                                            PagoParcialId = s.PagoParcialId,
                                            PagoId = s.PagoId,
                                            SucursalCajaId = s.SucursalCajaId,
                                            ReciboId = s.ReciboId,
                                            Pago = s.Pago,
                                            FechaPago = s.FechaPago,
                                            HoraPago = s.HoraPago,
                                            EstatusId = s.EstatusId,
                                            TieneMovimientos = s.TieneMovimientos,
                                            PagoTipoId = s.PagoTipoId,
                                            ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                            FechaBitacora = DateTime.Now.Date,
                                            HoraBitacora = DateTime.Now.TimeOfDay,
                                            UsuarioId = Usuario.usuarioId
                                        });

                                        #endregion PagoParcial Bitacora

                                        if (saldo <= s.Pago)
                                        {
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                            s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                            s.Pago = s.Pago - saldo;
                                            saldo = 0;
                                        }

                                        else if (saldo > s.Pago)
                                        {
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                            s.PagoDetalle.FirstOrDefault().Importe = 0;
                                            saldo = saldo - s.Pago;
                                            s.Pago = 0;
                                            s.EstatusId = 2;
                                        }
                                    }
                                });

                                #endregion Referenciados

                                #region Caja

                                var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                Caja.ForEach(s =>
                                {
                                    if (saldo > 0)
                                    {
                                        #region PagoParcial Bitacora

                                        db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                        {
                                            PagoParcialId = s.PagoParcialId,
                                            PagoId = s.PagoId,
                                            SucursalCajaId = s.SucursalCajaId,
                                            ReciboId = s.ReciboId,
                                            Pago = s.Pago,
                                            FechaPago = s.FechaPago,
                                            HoraPago = s.HoraPago,
                                            EstatusId = s.EstatusId,
                                            TieneMovimientos = s.TieneMovimientos,
                                            PagoTipoId = s.PagoTipoId,
                                            ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                            FechaBitacora = DateTime.Now.Date,
                                            HoraBitacora = DateTime.Now.TimeOfDay,
                                            UsuarioId = Usuario.usuarioId
                                        });

                                        #endregion PagoParcial Bitacora

                                        if (saldo <= s.Pago)
                                        {
                                            /* Recibo */
                                            recibos.Add(new ReciboDatos
                                            {
                                                reciboId = s.ReciboId,
                                                sucursalCajaId = s.SucursalCajaId,
                                                importe = saldo
                                            });

                                            var Detalles = s.PagoDetalle.ToList();
                                            decimal saldoDetalles = saldo;

                                            Detalles.ForEach(a =>
                                            {
                                                if (saldoDetalles > 0)
                                                {
                                                    if (saldoDetalles <= a.Importe)
                                                    {
                                                        a.Importe = a.Importe - saldoDetalles;
                                                        saldoDetalles = 0;
                                                    }

                                                    else if (saldoDetalles > a.Importe)
                                                    {
                                                        saldoDetalles = saldoDetalles - a.Importe;
                                                        a.Importe = 0;
                                                    }
                                                }
                                            });

                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                            s.Pago = s.Pago - saldo;
                                            saldo = 0;
                                        }

                                        else if (saldo > s.Pago)
                                        {
                                            /* Recibo */
                                            recibos.Add(new ReciboDatos
                                            {
                                                reciboId = s.ReciboId,
                                                sucursalCajaId = s.SucursalCajaId,
                                                importe = s.Pago
                                            });

                                            //Duda
                                            s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                            s.ReferenciaProcesada.SeGasto = false;
                                            s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                            saldo = saldo - s.Pago;
                                            s.Pago = 0;
                                            s.EstatusId = 2;
                                        }
                                    }
                                });

                                if (recibos.Count > 0)
                                {
                                    var RecibosTotal = (from consulta in
                                                            (from a in recibos
                                                             select new { a })
                                                        group consulta by new
                                                        {
                                                            consulta.a.reciboId,
                                                            consulta.a.sucursalCajaId
                                                        } into g

                                                        select new DTO.ReciboDatos
                                                        {
                                                            reciboId = g.Key.reciboId,
                                                            sucursalCajaId = g.Key.sucursalCajaId,
                                                            importe = g.Sum(a => a.a.importe)
                                                        }).ToList();

                                    RecibosTotal.ForEach(a =>
                                    {
                                        Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                    });

                                    Recibos.ForEach(a =>
                                    {
                                        a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                    });
                                }

                                #endregion Caja
                            }

                            #endregion Saldo a Favor
                        }

                        #endregion No Existe Descuento

                        db.SaveChanges();
                    }

                    #endregion Inscripción
                });

                #region Actualizar Descuento Colegiatura

                #region Existe Descuento

                if (DescuentoColegiatura != null)
                {
                    DescuentoBitacora(DescuentoColegiatura);

                    DescuentoColegiatura.Monto = AlumnoBeca.porcentajeBeca;
                    DescuentoColegiatura.DescuentoId = descuentoIdColegiatura;
                    DescuentoColegiatura.UsuarioId = Usuario.usuarioId;
                    DescuentoColegiatura.FechaGeneracion = DateTime.Now;
                    DescuentoColegiatura.FechaAplicacion = DateTime.Now;
                    DescuentoColegiatura.EsComite = AlumnoBeca.esComite;
                    DescuentoColegiatura.EsSEP = AlumnoBeca.esComite ? (DescuentoColegiatura.EsSEP ? true : false) : (AlumnoBeca.esSEP ? true : false);
                    DescuentoColegiatura.EsDeportiva = false;
                }

                #endregion Existe Descuento

                #region No Existe Descuento

                else if (DescuentoColegiatura == null)
                {
                    db.AlumnoDescuento.Add(new AlumnoDescuento
                    {
                        AlumnoId = AlumnoBeca.alumnoId,
                        OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                        Anio = AlumnoBeca.anio,
                        PeriodoId = AlumnoBeca.periodoId,
                        PagoConceptoId = 800,
                        Monto = AlumnoBeca.porcentajeBeca,
                        UsuarioId = Usuario.usuarioId,
                        Comentario = "",
                        FechaGeneracion = DateTime.Now,
                        FechaAplicacion = DateTime.Now,
                        EstatusId = 2,
                        DescuentoId = descuentoIdColegiatura,
                        EsDeportiva = false,
                        EsComite = AlumnoBeca.esComite,
                        EsSEP = AlumnoBeca.esSEP
                    });
                }

                #endregion No Existe Descuento

                #endregion Actualizar Descuento Colegiatura

                #region Actualizar Descuento Inscripcion

                if (AlumnoBeca.esSEP)
                {
                    #region Existe Descuento

                    if (DescuentoInscripcion != null)
                    {
                        DescuentoBitacora(DescuentoInscripcion);

                        DescuentoInscripcion.Monto = AlumnoBeca.porcentajeBeca;
                        DescuentoInscripcion.DescuentoId = descuentoIdInscripcion;
                        DescuentoInscripcion.UsuarioId = Usuario.usuarioId;
                        DescuentoInscripcion.FechaGeneracion = DateTime.Now;
                        DescuentoInscripcion.FechaAplicacion = DateTime.Now;
                        DescuentoInscripcion.EsComite = AlumnoBeca.esComite;
                        DescuentoInscripcion.EsSEP = AlumnoBeca.esComite ? (DescuentoInscripcion.EsSEP ? true : false) : (AlumnoBeca.esSEP ? true : false);
                        DescuentoInscripcion.EsDeportiva = false;
                    }

                    #endregion Existe Descuento

                    #region No Existe Descuento

                    else if (DescuentoInscripcion == null)
                    {
                        db.AlumnoDescuento.Add(new AlumnoDescuento
                        {
                            AlumnoId = AlumnoBeca.alumnoId,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            PagoConceptoId = 802,
                            Monto = AlumnoBeca.porcentajeBeca,
                            UsuarioId = Usuario.usuarioId,
                            Comentario = "",
                            FechaGeneracion = DateTime.Now,
                            FechaAplicacion = DateTime.Now,
                            EstatusId = 2,
                            DescuentoId = descuentoIdInscripcion,
                            EsDeportiva = false,
                            EsComite = AlumnoBeca.esComite,
                            EsSEP = AlumnoBeca.esSEP
                        });
                    }

                    #endregion No Existe Descuento
                }

                #endregion Actualizar Descuento Inscripcion

                db.SaveChanges();

                Inscribir(AlumnoBeca, Usuario);
            }
        }

        public static bool AplicaBecaAlumno2(DTO.Alumno.Beca.DTOAlumnoBeca AlumnoBeca)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                List<DAL.Pago> PagosPendientes = new List<Pago>();
                DAL.AlumnoDescuento DescuentoColegiatura = new AlumnoDescuento();
                DAL.AlumnoDescuento DescuentoInscripcion = new AlumnoDescuento();
                List<Pago> Cabecero = new List<Pago>();
                Cuota CQ = new Cuota();
                Cuota CQI = new Cuota();

                #region Pagos Pendientes

                PagosPendientes.AddRange(
                    (from a in db.Pago

                     where (a.Cuota1.PagoConceptoId == 800 ||
                            a.Cuota1.PagoConceptoId == 802)
                           && (a.EstatusId == 1 || a.EstatusId == 13 || a.EstatusId == 14 || a.EstatusId == 4)
                           && (a.Anio == AlumnoBeca.anio && a.PeriodoId == AlumnoBeca.periodoId)
                           && (a.Promesa > 0)
                           && a.AlumnoId == AlumnoBeca.alumnoId
                     select a).ToList());

                #endregion Pagos Pendientes


                try
                {
                    #region Verificar Descuentos

                    DescuentoColegiatura = (from a in db.AlumnoDescuento
                                            join b in db.Descuento on a.DescuentoId equals b.DescuentoId
                                            where a.AlumnoId == AlumnoBeca.alumnoId
                                            && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                            && a.Anio == AlumnoBeca.anio
                                            && a.PeriodoId == AlumnoBeca.periodoId
                                            && a.PagoConceptoId == 800
                                            && b.Descripcion != "Beca deportiva"
                                            select a).ToList().FirstOrDefault();

                    DescuentoInscripcion = (db.AlumnoDescuento
                        .Where(a => a.AlumnoId == AlumnoBeca.alumnoId
                                    && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                    && a.Anio == AlumnoBeca.anio
                                    && a.PeriodoId == AlumnoBeca.periodoId
                                    && a.PagoConceptoId == 802).FirstOrDefault());

                    #endregion Verificar Descuentos

                    PagosPendientes.ForEach(a =>
                    {
                        #region Colegiatura
                        if (a.Cuota1.PagoConceptoId == 800)
                        {
                            #region Tiene AlumnoDescuento
                            if (DescuentoColegiatura != null)
                            {
                                PagoDescuento Descuento = db.PagoDescuento
                                        .Where(n => n.PagoId == a.PagoId && n.DescuentoId == DescuentoColegiatura.DescuentoId).FirstOrDefault();

                                if (Descuento != null)
                                {
                                    var PromesaAnterior = a.Promesa;
                                    a.Promesa = (a.Promesa + Descuento.Monto);

                                    db.PagoDescuento.Add(new PagoDescuento
                                    {
                                        PagoId = a.PagoId,
                                        DescuentoId = (from z in db.Descuento
                                                       where z.PagoConceptoId == a.Cuota1.PagoConceptoId
                                                            && z.Descripcion == "Descuento en colegiatura"
                                                            && z.OfertaEducativaId == a.OfertaEducativaId
                                                       select z.DescuentoId).FirstOrDefault(),
                                        Monto = (a.Promesa) * (AlumnoBeca.porcentajeBeca / 100)
                                    });

                                    Descuento.Monto = a.Promesa * (AlumnoBeca.porcentajeBeca / 100);
                                    var PromesaNueva = a.Promesa - Descuento.Monto;

                                    a.Promesa = (a.EstatusId == 4 || a.EstatusId == 14) ? a.Promesa : a.Promesa = a.Promesa - Descuento.Monto;

                                    #region Estatus 4, 14 - Pagos > Nueva Promesa
                                    if (a.EstatusId == 4 || a.EstatusId == 14)
                                    {
                                        var Valor = (PromesaAnterior > PromesaNueva)
                                            ? (PromesaAnterior - PromesaNueva) //Mayor la primera, Sobra 
                                            : (PromesaAnterior == PromesaNueva)
                                                ? 0   //Iguales, no pasa nada
                                                : -1; //Menor; aumentar promesa de pago y cambiar EstatusId

                                        //Menor la nueva, nos debe
                                        if (Valor == -1)
                                        {
                                            a.EstatusId = 1;
                                            a.Promesa = PromesaNueva;
                                        }

                                        #region Saldo a Favor

                                        if (db.AlumnoInscritoBeca.Where(x => x.AlumnoId == AlumnoBeca.alumnoId
                                                                        && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                                        && x.Anio == AlumnoBeca.anio
                                                                        && x.PeriodoId == AlumnoBeca.periodoId
                                                                        ).ToList().Count == 0 && Valor > 0)
                                        {
                                            int alumnoSaldo = db.AlumnoSaldo.Count(al => al.AlumnoId == a.AlumnoId);

                                            if (alumnoSaldo > 0)
                                            {
                                                var Saldo = db.AlumnoSaldo.Where(al => al.AlumnoId == a.AlumnoId).FirstOrDefault();
                                                Saldo.Saldo = Saldo.Saldo + Valor;
                                                Saldo.AlumnoSaldoDetalle.Add(new DAL.AlumnoSaldoDetalle
                                                {
                                                    AlumnoId = a.AlumnoId,
                                                    Importe = Valor,
                                                    Rubro = a.Cuota1.PagoConceptoId,
                                                    ReferenciaId = a.ReferenciaId,
                                                    FechaPago = DateTime.Now,
                                                    FechaAplicacion = DateTime.Now,
                                                    HoraAplicacion = DateTime.Now.TimeOfDay
                                                });
                                            }

                                            else
                                            {
                                                db.AlumnoSaldo.Add(new DAL.AlumnoSaldo
                                                {
                                                    AlumnoId = a.AlumnoId,
                                                    Saldo = Valor,
                                                    AlumnoSaldoDetalle = new List<DAL.AlumnoSaldoDetalle>{new DAL.AlumnoSaldoDetalle{
                                                        AlumnoId = a.AlumnoId,
                                                        Importe = Valor,
                                                        Rubro = a.Cuota1.PagoConceptoId,
                                                        ReferenciaId = a.ReferenciaId,
                                                        FechaPago = DateTime.Now,
                                                        FechaAplicacion = DateTime.Now,
                                                        HoraAplicacion = DateTime.Now.TimeOfDay
                                                    }}
                                                });
                                            }
                                        }
                                        #endregion Saldo a Favor

                                    }
                                    #endregion Estatus 4, 14 - Pagos > Nueva Promesa

                                    #region Estatus 1, 13 - Pagos > Nueva Promesa
                                    if (a.EstatusId == 1 || a.EstatusId == 13)
                                    {
                                        var refcabecero = (db.ReferenciadoCabecero.Count(x => x.PagoId == a.PagoId) > 0 ? db.ReferenciadoCabecero.Where(x => x.PagoId == a.PagoId && x.EstatusId == 8).FirstOrDefault().ImporteTotal : 0);
                                        var parciales = (db.PagoParcial.Count(x => x.PagoId == a.PagoId)) > 0 ? db.PagoParcial.Where(x => x.PagoId == a.PagoId).Sum(x => x.Pago) : 0;

                                        if ((refcabecero + parciales) > PromesaNueva)
                                        {
                                            var Valor = (refcabecero + parciales) - PromesaNueva;
                                            a.EstatusId = 4;

                                            #region Saldo a Favor

                                            int alumnoSaldo = db.AlumnoSaldo.Count(al => al.AlumnoId == a.AlumnoId);

                                            if (alumnoSaldo > 0)
                                            {
                                                var Saldo = db.AlumnoSaldo.Where(al => al.AlumnoId == a.AlumnoId).FirstOrDefault();
                                                Saldo.Saldo = Saldo.Saldo + Valor;
                                                Saldo.AlumnoSaldoDetalle.Add(new DAL.AlumnoSaldoDetalle
                                                {
                                                    AlumnoId = a.AlumnoId,
                                                    Importe = Valor,
                                                    Rubro = a.Cuota1.PagoConceptoId,
                                                    ReferenciaId = a.ReferenciaId,
                                                    FechaPago = DateTime.Now,
                                                    FechaAplicacion = DateTime.Now,
                                                    HoraAplicacion = DateTime.Now.TimeOfDay
                                                });
                                            }

                                            else
                                            {
                                                db.AlumnoSaldo.Add(new DAL.AlumnoSaldo
                                                {
                                                    AlumnoId = a.AlumnoId,
                                                    Saldo = Valor,
                                                    AlumnoSaldoDetalle = new List<DAL.AlumnoSaldoDetalle>{new DAL.AlumnoSaldoDetalle{
                                                        AlumnoId = a.AlumnoId,
                                                        Importe = Valor,
                                                        Rubro = a.Cuota1.PagoConceptoId,
                                                        ReferenciaId = a.ReferenciaId,
                                                        FechaPago = DateTime.Now,
                                                        FechaAplicacion = DateTime.Now,
                                                        HoraAplicacion = DateTime.Now.TimeOfDay
                                                    }}
                                                });
                                            }

                                            #endregion Saldo a Favor
                                        }
                                    }
                                    #endregion Estatus 1, 13 - Pagos > Nueva Promesa

                                    db.PagoDescuento.Remove(Descuento);
                                }

                                db.SaveChanges();
                            }

                            #endregion Tiene AlumnoDescuento

                            #region No Tiene AlumnoDescuento
                            //Si no existe un AlumnoDescuento
                            else if (DescuentoColegiatura == null)
                            {
                                var PromesaAnterior = a.Promesa;

                                db.PagoDescuento.Add(
                                   new PagoDescuento
                                   {
                                       PagoId = a.PagoId,
                                       DescuentoId = (from z in db.Descuento
                                                      where z.PagoConceptoId == a.Cuota1.PagoConceptoId
                                                            && z.Descripcion == "Descuento en colegiatura"
                                                            && z.OfertaEducativaId == a.OfertaEducativaId
                                                      select z.DescuentoId).FirstOrDefault(),
                                       Monto = a.Promesa * (AlumnoBeca.porcentajeBeca / 100)
                                   }
                               );

                                var PromesaNueva = a.Promesa - (a.Promesa * (AlumnoBeca.porcentajeBeca / 100));
                                a.Promesa = (a.EstatusId == 4 || a.EstatusId == 14) ? a.Promesa : a.Promesa - (a.Promesa * (AlumnoBeca.porcentajeBeca / 100));

                                #region Estatus 4, 14 - Pagos > Nueva Promesa

                                if (a.EstatusId == 4 || a.EstatusId == 14)
                                {
                                    var Valor = (PromesaAnterior > PromesaNueva)
                                           ? (PromesaAnterior - PromesaNueva) //Mayor la primera, Sobra 
                                           : (PromesaAnterior == PromesaNueva)
                                               ? 0   //Iguales, no pasa nada
                                               : -1; //Menor; aumentar promesa de pago y cambiar EstatusId

                                    //Menor la nueva, nos debe
                                    if (Valor == -1)
                                    {
                                        a.EstatusId = 1;
                                        a.Promesa = PromesaNueva;
                                    }

                                    #region Saldo a Favor

                                    if (db.AlumnoInscritoBeca.Where(x => x.AlumnoId == AlumnoBeca.alumnoId
                                                                    && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                                    && x.Anio == AlumnoBeca.anio
                                                                    && x.PeriodoId == AlumnoBeca.periodoId
                                                                    ).ToList().Count == 0 && Valor > 0)
                                    {
                                        int alumnoSaldo = db.AlumnoSaldo.Count(al => al.AlumnoId == a.AlumnoId);

                                        if (alumnoSaldo > 0)
                                        {
                                            var Saldo = db.AlumnoSaldo.Where(al => al.AlumnoId == a.AlumnoId).FirstOrDefault();
                                            Saldo.Saldo = Saldo.Saldo + Valor;
                                            Saldo.AlumnoSaldoDetalle.Add(new DAL.AlumnoSaldoDetalle
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Importe = Valor,
                                                Rubro = a.Cuota1.PagoConceptoId,
                                                ReferenciaId = a.ReferenciaId,
                                                FechaPago = DateTime.Now,
                                                FechaAplicacion = DateTime.Now,
                                                HoraAplicacion = DateTime.Now.TimeOfDay
                                            });
                                        }

                                        else
                                        {
                                            db.AlumnoSaldo.Add(new DAL.AlumnoSaldo
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Saldo = Valor,
                                                AlumnoSaldoDetalle = new List<DAL.AlumnoSaldoDetalle>{new DAL.AlumnoSaldoDetalle{
                                                        AlumnoId = a.AlumnoId,
                                                        Importe = Valor,
                                                        Rubro = a.Cuota1.PagoConceptoId,
                                                        ReferenciaId = a.ReferenciaId,
                                                        FechaPago = DateTime.Now,
                                                        FechaAplicacion = DateTime.Now,
                                                        HoraAplicacion = DateTime.Now.TimeOfDay
                                                    }}
                                            });
                                        }
                                    }
                                    #endregion Saldo a Favor
                                }

                                #endregion Estatus 4, 14 - Pagos > Nueva Promesa

                                #region Estatus 1, 13 - Pagos > Nueva Promesa

                                if (a.EstatusId == 1 || a.EstatusId == 13)
                                {
                                    var refcabecero = (db.ReferenciadoCabecero.Count(x => x.PagoId == a.PagoId) > 0 ? db.ReferenciadoCabecero.Where(x => x.PagoId == a.PagoId && x.EstatusId == 8).FirstOrDefault().ImporteTotal : 0);
                                    var parciales = (db.PagoParcial.Count(x => x.PagoId == a.PagoId)) > 0 ? db.PagoParcial.Where(x => x.PagoId == a.PagoId).Sum(x => x.Pago) : 0;

                                    if ((refcabecero + parciales) > PromesaNueva)
                                    {
                                        var Valor = (refcabecero + parciales) - PromesaNueva;
                                        a.EstatusId = 4;

                                        #region Saldo a Favor

                                        int alumnoSaldo = db.AlumnoSaldo.Count(al => al.AlumnoId == a.AlumnoId);

                                        if (alumnoSaldo > 0)
                                        {
                                            var Saldo = db.AlumnoSaldo.Where(al => al.AlumnoId == a.AlumnoId).FirstOrDefault();
                                            Saldo.Saldo = Saldo.Saldo + Valor;
                                            Saldo.AlumnoSaldoDetalle.Add(new DAL.AlumnoSaldoDetalle
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Importe = Valor,
                                                Rubro = a.Cuota1.PagoConceptoId,
                                                ReferenciaId = a.ReferenciaId,
                                                FechaPago = DateTime.Now,
                                                FechaAplicacion = DateTime.Now,
                                                HoraAplicacion = DateTime.Now.TimeOfDay
                                            });
                                        }

                                        else
                                        {
                                            db.AlumnoSaldo.Add(new DAL.AlumnoSaldo
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Saldo = Valor,
                                                AlumnoSaldoDetalle = new List<DAL.AlumnoSaldoDetalle>{new DAL.AlumnoSaldoDetalle{
                                                        AlumnoId = a.AlumnoId,
                                                        Importe = Valor,
                                                        Rubro = a.Cuota1.PagoConceptoId,
                                                        ReferenciaId = a.ReferenciaId,
                                                        FechaPago = DateTime.Now,
                                                        FechaAplicacion = DateTime.Now,
                                                        HoraAplicacion = DateTime.Now.TimeOfDay
                                                    }}
                                            });
                                        }

                                        #endregion Saldo a Favor
                                    }
                                }

                                #endregion Estatus 1, 13 - Pagos > Nueva Promesa

                                db.SaveChanges();
                            }

                            #endregion No Tiene AlumnoDescuento
                        }

                        #endregion Colegiatura

                        #region Inscripcion

                        if (a.Cuota1.PagoConceptoId == 802)
                        {
                            #region Tiene AlumnoDescuento

                            if (DescuentoInscripcion != null)
                            {
                                PagoDescuento Descuento = db.PagoDescuento
                                        .Where(n => n.PagoId == a.PagoId && n.DescuentoId == DescuentoInscripcion.DescuentoId).FirstOrDefault();

                                if (Descuento != null)
                                {
                                    var PromesaAnterior = a.Promesa;

                                    a.Promesa = (a.Promesa + Descuento.Monto);
                                    db.PagoDescuento.Add(new PagoDescuento
                                    {
                                        PagoId = a.PagoId,
                                        DescuentoId = (from z in db.Descuento
                                                       where z.PagoConceptoId == a.Cuota1.PagoConceptoId
                                                            && z.Descripcion == "Descuento en inscripción"
                                                            && z.OfertaEducativaId == a.OfertaEducativaId
                                                       select z.DescuentoId).FirstOrDefault(),
                                        Monto = (a.Promesa) * (AlumnoBeca.porcentajeInscripcion / 100)
                                    });

                                    Descuento.Monto = a.Promesa * (AlumnoBeca.porcentajeInscripcion / 100);
                                    var PromesaNueva = a.Promesa - Descuento.Monto;
                                    a.Promesa = (a.EstatusId == 4 || a.EstatusId == 14) ? a.Promesa : a.Promesa - Descuento.Monto;

                                    #region Estatus 4, 14 - Pagos > Nueva Promesa
                                    if (a.EstatusId == 4 || a.EstatusId == 14)
                                    {
                                        var Valor = (PromesaAnterior > PromesaNueva)
                                            ? (PromesaAnterior - PromesaNueva) //Mayor la primera, Sobra 
                                            : (PromesaAnterior == PromesaNueva)
                                                ? 0   //Iguales, no pasa nada
                                                : -1; //Menor; aumentar promesa de pago y cambiar EstatusId

                                        //Menor la nueva, nos debe
                                        if (Valor == -1)
                                        {
                                            a.EstatusId = 1;
                                            a.Promesa = PromesaNueva;
                                        }

                                        #region Saldo a Favor

                                        if (db.AlumnoInscritoBeca.Where(x => x.AlumnoId == AlumnoBeca.alumnoId
                                                                        && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                                        && x.Anio == AlumnoBeca.anio
                                                                        && x.PeriodoId == AlumnoBeca.periodoId
                                                                        ).ToList().Count == 0 && Valor > 0)
                                        {
                                            int alumnoSaldo = db.AlumnoSaldo.Count(al => al.AlumnoId == a.AlumnoId);

                                            if (alumnoSaldo > 0)
                                            {
                                                var Saldo = db.AlumnoSaldo.Where(al => al.AlumnoId == a.AlumnoId).FirstOrDefault();
                                                Saldo.Saldo = Saldo.Saldo + Valor;
                                                Saldo.AlumnoSaldoDetalle.Add(new DAL.AlumnoSaldoDetalle
                                                {
                                                    AlumnoId = a.AlumnoId,
                                                    Importe = Valor,
                                                    Rubro = a.Cuota1.PagoConceptoId,
                                                    ReferenciaId = a.ReferenciaId,
                                                    FechaPago = DateTime.Now,
                                                    FechaAplicacion = DateTime.Now,
                                                    HoraAplicacion = DateTime.Now.TimeOfDay
                                                });
                                            }

                                            else
                                            {
                                                db.AlumnoSaldo.Add(new DAL.AlumnoSaldo
                                                {
                                                    AlumnoId = a.AlumnoId,
                                                    Saldo = Valor,
                                                    AlumnoSaldoDetalle = new List<DAL.AlumnoSaldoDetalle>{new DAL.AlumnoSaldoDetalle{
                                                        AlumnoId = a.AlumnoId,
                                                        Importe = Valor,
                                                        Rubro = a.Cuota1.PagoConceptoId,
                                                        ReferenciaId = a.ReferenciaId,
                                                        FechaPago = DateTime.Now,
                                                        FechaAplicacion = DateTime.Now,
                                                        HoraAplicacion = DateTime.Now.TimeOfDay
                                                    }}
                                                });
                                            }
                                        }
                                        #endregion Saldo a Favor
                                    }
                                    #endregion Estatus 4, 14 - Pagos > Nueva Promesa

                                    #region Estatus 1, 13 - Pagos > Nueva Promesa
                                    if (a.EstatusId == 1 || a.EstatusId == 13)
                                    {
                                        var refcabecero = (db.ReferenciadoCabecero.Count(x => x.PagoId == a.PagoId) > 0 ? db.ReferenciadoCabecero.Where(x => x.PagoId == a.PagoId && x.EstatusId == 8).FirstOrDefault().ImporteTotal : 0);
                                        var parciales = (db.PagoParcial.Count(x => x.PagoId == a.PagoId)) > 0 ? db.PagoParcial.Where(x => x.PagoId == a.PagoId).Sum(x => x.Pago) : 0;

                                        if ((refcabecero + parciales) > PromesaNueva)
                                        {
                                            var Valor = (refcabecero + parciales) - PromesaNueva;
                                            a.EstatusId = 4;

                                            #region Saldo a Favor

                                            int alumnoSaldo = db.AlumnoSaldo.Count(al => al.AlumnoId == a.AlumnoId);

                                            if (alumnoSaldo > 0)
                                            {
                                                var Saldo = db.AlumnoSaldo.Where(al => al.AlumnoId == a.AlumnoId).FirstOrDefault();
                                                Saldo.Saldo = Saldo.Saldo + Valor;
                                                Saldo.AlumnoSaldoDetalle.Add(new DAL.AlumnoSaldoDetalle
                                                {
                                                    AlumnoId = a.AlumnoId,
                                                    Importe = Valor,
                                                    Rubro = a.Cuota1.PagoConceptoId,
                                                    ReferenciaId = a.ReferenciaId,
                                                    FechaPago = DateTime.Now,
                                                    FechaAplicacion = DateTime.Now,
                                                    HoraAplicacion = DateTime.Now.TimeOfDay
                                                });
                                            }

                                            else
                                            {
                                                db.AlumnoSaldo.Add(new DAL.AlumnoSaldo
                                                {
                                                    AlumnoId = a.AlumnoId,
                                                    Saldo = Valor,
                                                    AlumnoSaldoDetalle = new List<DAL.AlumnoSaldoDetalle>{new DAL.AlumnoSaldoDetalle{
                                                        AlumnoId = a.AlumnoId,
                                                        Importe = Valor,
                                                        Rubro = a.Cuota1.PagoConceptoId,
                                                        ReferenciaId = a.ReferenciaId,
                                                        FechaPago = DateTime.Now,
                                                        FechaAplicacion = DateTime.Now,
                                                        HoraAplicacion = DateTime.Now.TimeOfDay
                                                    }}
                                                });
                                            }

                                            #endregion Saldo a Favor
                                        }
                                    }
                                    #endregion Estatus 1, 13 - Pagos > Nueva Promesa

                                    db.PagoDescuento.Remove(Descuento);
                                }

                                db.SaveChanges();
                            }

                            #endregion Tiene AlumnoDescuento

                            #region No Tiene AlumnoDescuento
                            else if (DescuentoInscripcion == null && AlumnoBeca.porcentajeInscripcion > 0)
                            {
                                var PromesaAnterior = a.Promesa;

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = a.PagoId,
                                    DescuentoId = (from z in db.Descuento
                                                   where z.PagoConceptoId == a.Cuota1.PagoConceptoId
                                                        && z.Descripcion == "Descuento en inscripción"
                                                        && z.OfertaEducativaId == a.OfertaEducativaId
                                                   select z.DescuentoId).FirstOrDefault(),
                                    Monto = (a.Promesa) * (AlumnoBeca.porcentajeInscripcion / 100)
                                });

                                var PromesaNueva = a.Promesa - (a.Promesa * (AlumnoBeca.porcentajeInscripcion / 100));
                                a.Promesa = (a.EstatusId == 4 || a.EstatusId == 14) ? a.Promesa : a.Promesa - ((a.Promesa) * (AlumnoBeca.porcentajeInscripcion / 100));

                                #region Estatus 4, 14 - Pagos > Nueva Promesa

                                if (a.EstatusId == 4 || a.EstatusId == 14)
                                {
                                    var Valor = (PromesaAnterior > PromesaNueva)
                                           ? (PromesaAnterior - PromesaNueva) //Mayor la primera, Sobra 
                                           : (PromesaAnterior == PromesaNueva)
                                               ? 0   //Iguales, no pasa nada
                                               : -1; //Menor; aumentar promesa de pago y cambiar EstatusId

                                    //Menor la nueva, nos debe
                                    if (Valor == -1)
                                    {
                                        a.EstatusId = 1;
                                        a.Promesa = PromesaNueva;
                                    }

                                    #region Saldo a Favor

                                    if (db.AlumnoInscritoBeca.Where(x => x.AlumnoId == AlumnoBeca.alumnoId
                                                                    && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                                    && x.Anio == AlumnoBeca.anio
                                                                    && x.PeriodoId == AlumnoBeca.periodoId
                                                                    ).ToList().Count == 0 && Valor > 0)
                                    {
                                        int alumnoSaldo = db.AlumnoSaldo.Count(al => al.AlumnoId == a.AlumnoId);

                                        if (alumnoSaldo > 0)
                                        {
                                            var Saldo = db.AlumnoSaldo.Where(al => al.AlumnoId == a.AlumnoId).FirstOrDefault();
                                            Saldo.Saldo = Saldo.Saldo + Valor;
                                            Saldo.AlumnoSaldoDetalle.Add(new DAL.AlumnoSaldoDetalle
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Importe = Valor,
                                                Rubro = a.Cuota1.PagoConceptoId,
                                                ReferenciaId = a.ReferenciaId,
                                                FechaPago = DateTime.Now,
                                                FechaAplicacion = DateTime.Now,
                                                HoraAplicacion = DateTime.Now.TimeOfDay
                                            });
                                        }

                                        else
                                        {
                                            db.AlumnoSaldo.Add(new DAL.AlumnoSaldo
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Saldo = Valor,
                                                AlumnoSaldoDetalle = new List<DAL.AlumnoSaldoDetalle>{new DAL.AlumnoSaldoDetalle{
                                                        AlumnoId = a.AlumnoId,
                                                        Importe = Valor,
                                                        Rubro = a.Cuota1.PagoConceptoId,
                                                        ReferenciaId = a.ReferenciaId,
                                                        FechaPago = DateTime.Now,
                                                        FechaAplicacion = DateTime.Now,
                                                        HoraAplicacion = DateTime.Now.TimeOfDay
                                                    }}
                                            });
                                        }
                                    }
                                    #endregion Saldo a Favor
                                }

                                #endregion Estatus 4, 14 - Pagos > Nueva Promesa

                                #region Estatus 1, 13 - Pagos > Nueva Promesa

                                if (a.EstatusId == 1 || a.EstatusId == 13)
                                {
                                    var refcabecero = (db.ReferenciadoCabecero.Count(x => x.PagoId == a.PagoId) > 0 ? db.ReferenciadoCabecero.Where(x => x.PagoId == a.PagoId && x.EstatusId == 8).FirstOrDefault().ImporteTotal : 0);
                                    var parciales = (db.PagoParcial.Count(x => x.PagoId == a.PagoId)) > 0 ? db.PagoParcial.Where(x => x.PagoId == a.PagoId).Sum(x => x.Pago) : 0;

                                    if ((refcabecero + parciales) > PromesaNueva)
                                    {
                                        var Valor = (refcabecero + parciales) - PromesaNueva;
                                        a.EstatusId = 4;

                                        #region Saldo a Favor

                                        int alumnoSaldo = db.AlumnoSaldo.Count(al => al.AlumnoId == a.AlumnoId);

                                        if (alumnoSaldo > 0)
                                        {
                                            var Saldo = db.AlumnoSaldo.Where(al => al.AlumnoId == a.AlumnoId).FirstOrDefault();
                                            Saldo.Saldo = Saldo.Saldo + Valor;
                                            Saldo.AlumnoSaldoDetalle.Add(new DAL.AlumnoSaldoDetalle
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Importe = Valor,
                                                Rubro = a.Cuota1.PagoConceptoId,
                                                ReferenciaId = a.ReferenciaId,
                                                FechaPago = DateTime.Now,
                                                FechaAplicacion = DateTime.Now,
                                                HoraAplicacion = DateTime.Now.TimeOfDay
                                            });
                                        }

                                        else
                                        {
                                            db.AlumnoSaldo.Add(new DAL.AlumnoSaldo
                                            {
                                                AlumnoId = a.AlumnoId,
                                                Saldo = Valor,
                                                AlumnoSaldoDetalle = new List<DAL.AlumnoSaldoDetalle>{new DAL.AlumnoSaldoDetalle{
                                                        AlumnoId = a.AlumnoId,
                                                        Importe = Valor,
                                                        Rubro = a.Cuota1.PagoConceptoId,
                                                        ReferenciaId = a.ReferenciaId,
                                                        FechaPago = DateTime.Now,
                                                        FechaAplicacion = DateTime.Now,
                                                        HoraAplicacion = DateTime.Now.TimeOfDay
                                                    }}
                                            });
                                        }

                                        #endregion Saldo a Favor
                                    }
                                }

                                #endregion Estatus 1, 13 - Pagos > Nueva Promesa

                                db.SaveChanges();
                            }

                            #endregion No Tiene AlumnoDescuento
                        }

                        #endregion Inscripcion
                    });

                    #region Actualizar AlumnoDescuento Colegiatura

                    #region Existe
                    if (DescuentoColegiatura != null)
                    {
                        //Actualizar AlumnoDescuento
                        DescuentoColegiatura.Monto = AlumnoBeca.porcentajeBeca;
                        DescuentoColegiatura.DescuentoId = (from z in db.Descuento
                                                            where z.PagoConceptoId == 800
                                                                 && z.Descripcion == "Descuento en colegiatura"
                                                                 && z.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                            select z.DescuentoId).FirstOrDefault();
                    }
                    #endregion Existe

                    #region No Existe
                    else if (DescuentoColegiatura == null)
                    {
                        db.AlumnoDescuento.Add(new AlumnoDescuento
                        {
                            AlumnoId = AlumnoBeca.alumnoId,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            PagoConceptoId = 800,
                            Monto = AlumnoBeca.porcentajeBeca,
                            UsuarioId = AlumnoBeca.usuarioId,
                            Comentario = "",
                            FechaGeneracion = DateTime.Now,
                            FechaAplicacion = DateTime.Now,
                            EstatusId = 2,
                            DescuentoId = (from z in db.Descuento
                                           where z.PagoConceptoId == 800
                                                && z.Descripcion == "Descuento en colegiatura"
                                                && z.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                           select z.DescuentoId).FirstOrDefault()
                        });
                    }
                    #endregion No Existe

                    db.SaveChanges();
                    #endregion Actualizar AlumnoDescuento Colegiatura

                    #region Actualizar AlumnoDescuento Inscripcion

                    #region Existe
                    if (DescuentoInscripcion != null)
                    {
                        DescuentoInscripcion.Monto = AlumnoBeca.porcentajeBeca;
                        DescuentoInscripcion.DescuentoId = (from z in db.Descuento
                                                            where z.PagoConceptoId == 802
                                                                  && z.Descripcion == "Descuento en inscripción"
                                                                  && z.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                            select z.DescuentoId).FirstOrDefault();
                    }
                    #endregion Existe

                    #region No Existe
                    else if (DescuentoInscripcion == null && AlumnoBeca.porcentajeInscripcion > 0)
                    {
                        db.AlumnoDescuento.Add(new AlumnoDescuento
                        {
                            AlumnoId = AlumnoBeca.alumnoId,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            DescuentoId = (from z in db.Descuento
                                           where z.PagoConceptoId == 802
                                                 && z.Descripcion == "Descuento en inscripción"
                                                 && z.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                           select z.DescuentoId).FirstOrDefault(),
                            PagoConceptoId = 802,
                            Monto = AlumnoBeca.porcentajeInscripcion,
                            UsuarioId = AlumnoBeca.usuarioId,
                            Comentario = "",
                            FechaGeneracion = DateTime.Now,
                            FechaAplicacion = DateTime.Now,
                            EstatusId = 2
                        });
                    }
                    #endregion No Existe

                    db.SaveChanges();

                    #endregion Actualizar AlumnoDescuento Inscripcion

                    #region Generación de Cargos

                    #region Verifica Cargos

                    int[] Estatus = { 1, 4, 13, 14 };

                    var InscripcionUno = (from x in db.Pago
                                          join y in db.Cuota on x.CuotaId equals y.CuotaId
                                          where x.AlumnoId == AlumnoBeca.alumnoId
                                                && Estatus.Contains(x.EstatusId)
                                                && y.PagoConceptoId == 802
                                                && x.SubperiodoId == 1
                                                && x.Anio == AlumnoBeca.anio
                                                && x.PeriodoId == AlumnoBeca.periodoId
                                                && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                          select x).ToList().FirstOrDefault();

                    var ColegiaturaUno = (from x in db.Pago
                                          join y in db.Cuota on x.CuotaId equals y.CuotaId
                                          where x.AlumnoId == AlumnoBeca.alumnoId
                                                && Estatus.Contains(x.EstatusId)
                                                && y.PagoConceptoId == 800
                                                && x.SubperiodoId == 1
                                                && x.Anio == AlumnoBeca.anio
                                                && x.PeriodoId == AlumnoBeca.periodoId
                                                && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                          select x).ToList().FirstOrDefault();

                    var ColegiaturaDos = (from x in db.Pago
                                          join y in db.Cuota on x.CuotaId equals y.CuotaId
                                          where x.AlumnoId == AlumnoBeca.alumnoId
                                                && Estatus.Contains(x.EstatusId)
                                                && y.PagoConceptoId == 800
                                                && x.SubperiodoId == 2
                                                && x.Anio == AlumnoBeca.anio
                                                && x.PeriodoId == AlumnoBeca.periodoId
                                                && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                          select x).ToList().FirstOrDefault();

                    var ColegiaturaTres = (from x in db.Pago
                                           join y in db.Cuota on x.CuotaId equals y.CuotaId
                                           where x.AlumnoId == AlumnoBeca.alumnoId
                                                 && Estatus.Contains(x.EstatusId)
                                                 && y.PagoConceptoId == 800
                                                 && x.SubperiodoId == 3
                                                 && x.Anio == AlumnoBeca.anio
                                                 && x.PeriodoId == AlumnoBeca.periodoId
                                                 && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                           select x).ToList().FirstOrDefault();

                    var ColegiaturaCuatro = (from x in db.Pago
                                             join y in db.Cuota on x.CuotaId equals y.CuotaId
                                             where x.AlumnoId == AlumnoBeca.alumnoId
                                                   && Estatus.Contains(x.EstatusId)
                                                   && y.PagoConceptoId == 800
                                                   && x.SubperiodoId == 4
                                                   && x.Anio == AlumnoBeca.anio
                                                   && x.PeriodoId == AlumnoBeca.periodoId
                                                   && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                             select x).ToList().FirstOrDefault();

                    int descuentoIdColegiatura = (from z in db.Descuento
                                                  where z.PagoConceptoId == 800
                                                       && z.Descripcion == "Descuento en colegiatura"
                                                       && z.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                  select z.DescuentoId).FirstOrDefault();

                    int descuentoIdInscripcion = (from z in db.Descuento
                                                  where z.PagoConceptoId == 802
                                                       && z.Descripcion == "Descuento en inscripción"
                                                       && z.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                  select z.DescuentoId).FirstOrDefault();

                    decimal importeBeca = 0;
                    decimal importeInscripcion = 0;

                    if (InscripcionUno == null || ColegiaturaUno == null || ColegiaturaDos == null || ColegiaturaTres == null || ColegiaturaCuatro == null)
                    {
                        CQ = (from x in db.Cuota
                              where x.Anio == AlumnoBeca.anio
                              && x.PeriodoId == AlumnoBeca.periodoId
                              && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                              && x.PagoConceptoId == 800
                              select x).ToList().FirstOrDefault();

                        importeBeca = CQ.Monto * (AlumnoBeca.porcentajeBeca / 100);

                        CQI = (from x in db.Cuota
                               where x.Anio == AlumnoBeca.anio
                               && x.PeriodoId == AlumnoBeca.periodoId
                               && x.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                               && x.PagoConceptoId == 802
                               select x).ToList().FirstOrDefault();

                        importeInscripcion = CQI.Monto * (AlumnoBeca.porcentajeInscripcion / 100);
                    }

                    #endregion Verifica Cargos

                    #region Inscripcion
                    if (InscripcionUno == null && AlumnoBeca.porcentajeInscripcion > 0)
                    {
                        List<PagoDescuento> Descuentos = new List<PagoDescuento>();

                        Descuentos.Add(new PagoDescuento
                        {
                            DescuentoId = descuentoIdInscripcion,
                            Monto = importeInscripcion
                        });

                        Cabecero.Add(new Pago
                        {
                            ReferenciaId = "",
                            AlumnoId = AlumnoBeca.alumnoId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            SubperiodoId = 1,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            FechaGeneracion = DateTime.Now,
                            CuotaId = CQI.CuotaId,
                            Cuota = CQI.Monto,
                            EstatusId = 1,
                            EsEmpresa = true,
                            EsAnticipado = false,
                            PeriodoAnticipadoId = 0,
                            Promesa = CQI.Monto - (CQI.Monto * (AlumnoBeca.porcentajeInscripcion / 100)),
                            PagoDescuento = Descuentos
                        });
                    }

                    else if (InscripcionUno == null && AlumnoBeca.porcentajeInscripcion == 0)
                    {
                        Cabecero.Add(new Pago
                        {
                            ReferenciaId = "",
                            AlumnoId = AlumnoBeca.alumnoId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            SubperiodoId = 1,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            FechaGeneracion = DateTime.Now,
                            CuotaId = CQI.CuotaId,
                            Cuota = CQI.Monto,
                            EstatusId = 1,
                            EsEmpresa = true,
                            EsAnticipado = false,
                            PeriodoAnticipadoId = 0,
                            Promesa = CQI.Monto
                        });
                    }
                    #endregion Inscripcion

                    #region Colegiatura 1
                    if (ColegiaturaUno == null)
                    {
                        List<PagoDescuento> Descuentos = new List<PagoDescuento>();

                        Descuentos.Add(new PagoDescuento
                        {
                            DescuentoId = descuentoIdColegiatura,
                            Monto = importeBeca
                        });

                        Cabecero.Add(new Pago
                        {
                            ReferenciaId = "",
                            AlumnoId = AlumnoBeca.alumnoId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            SubperiodoId = 1,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            FechaGeneracion = DateTime.Now,
                            CuotaId = CQ.CuotaId,
                            Cuota = CQ.Monto,
                            EstatusId = 1,
                            EsEmpresa = true,
                            EsAnticipado = false,
                            PeriodoAnticipadoId = 0,
                            Promesa = CQ.Monto - (CQ.Monto * (AlumnoBeca.porcentajeBeca / 100)),
                            PagoDescuento = Descuentos
                        });
                    }
                    #endregion Colegiatura 1

                    #region Colegiatura 2
                    if (ColegiaturaDos == null)
                    {
                        List<PagoDescuento> Descuentos = new List<PagoDescuento>();

                        Descuentos.Add(new PagoDescuento
                        {
                            DescuentoId = descuentoIdColegiatura,
                            Monto = importeBeca
                        });

                        Cabecero.Add(new Pago
                        {
                            ReferenciaId = "",
                            AlumnoId = AlumnoBeca.alumnoId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            SubperiodoId = 2,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            FechaGeneracion = DateTime.Now,
                            CuotaId = CQ.CuotaId,
                            Cuota = CQ.Monto,
                            EstatusId = 1,
                            EsEmpresa = true,
                            EsAnticipado = false,
                            PeriodoAnticipadoId = 0,
                            Promesa = CQ.Monto - (CQ.Monto * (AlumnoBeca.porcentajeBeca / 100)),
                            PagoDescuento = Descuentos
                        });
                    }
                    #endregion Colegiatura 2

                    #region Colegiatura 3
                    if (ColegiaturaTres == null)
                    {
                        List<PagoDescuento> Descuentos = new List<PagoDescuento>();

                        Descuentos.Add(new PagoDescuento
                        {
                            DescuentoId = descuentoIdColegiatura,
                            Monto = importeBeca
                        });

                        Cabecero.Add(new Pago
                        {
                            ReferenciaId = "",
                            AlumnoId = AlumnoBeca.alumnoId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            SubperiodoId = 3,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            FechaGeneracion = DateTime.Now,
                            CuotaId = CQ.CuotaId,
                            Cuota = CQ.Monto,
                            EstatusId = 1,
                            EsEmpresa = true,
                            EsAnticipado = false,
                            PeriodoAnticipadoId = 0,
                            Promesa = CQ.Monto - (CQ.Monto * (AlumnoBeca.porcentajeBeca / 100)),
                            PagoDescuento = Descuentos
                        });
                    }
                    #endregion Colegiatura 3

                    #region Colegiatura 4
                    if (ColegiaturaCuatro == null)
                    {
                        List<PagoDescuento> Descuentos = new List<PagoDescuento>();

                        Descuentos.Add(new PagoDescuento
                        {
                            DescuentoId = descuentoIdColegiatura,
                            Monto = importeBeca
                        });

                        Cabecero.Add(new Pago
                        {
                            ReferenciaId = "",
                            AlumnoId = AlumnoBeca.alumnoId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            SubperiodoId = 4,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            FechaGeneracion = DateTime.Now,
                            CuotaId = CQ.CuotaId,
                            Cuota = CQ.Monto,
                            EstatusId = 1,
                            EsEmpresa = true,
                            EsAnticipado = false,
                            PeriodoAnticipadoId = 0,
                            Promesa = CQ.Monto - (CQ.Monto * (AlumnoBeca.porcentajeBeca / 100)),
                            PagoDescuento = Descuentos
                        });
                    }
                    #endregion Colegiatura 4

                    #region ReferenciaId
                    Cabecero.ForEach(x =>
                    {
                        db.Pago.Add(x);
                    });

                    db.SaveChanges();

                    Cabecero.ForEach(x =>
                    {
                        x.ReferenciaId = db.spGeneraReferencia(x.PagoId).FirstOrDefault();
                    });

                    db.SaveChanges();
                    #endregion ReferenciaId

                    #endregion Generación de Cargos
                    /*
                    #region Bitacora

                    db.AlumnoInscritoBeca.Add(new AlumnoInscritoBeca
                    {
                        AlumnoId = AlumnoBeca.alumnoId,
                        OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                        Anio = AlumnoBeca.anio,
                        PeriodoId = AlumnoBeca.periodoId,
                        UsuarioId = AlumnoBeca.usuarioId,
                        FechaAplicacion = DateTime.Now,
                        HoraAplicacion = DateTime.Now.TimeOfDay,
                        EsSEP = AlumnoBeca.esSEP,
                        Porcentaje = AlumnoBeca.porcentajeBeca
                    });

                    db.SaveChanges();

                    #endregion Bitacora
                     * */

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static void AplicaBeca(DTO.Alumno.Beca.DTOAlumnoBeca AlumnoBeca, bool aplicacionExtemporanea) {
            List<DAL.Pago> PagosPendientes = new List<Pago>();
            List<DAL.Pago> PagosPendientes2 = new List<Pago>();
            List<Pago> Cabecero = new List<Pago>();
            DAL.AlumnoDescuento DescuentoColegiatura = new AlumnoDescuento();
            DAL.AlumnoDescuento DescuentoInscripcion = new AlumnoDescuento();
            List<DTO.Varios.DTOEstatus> EstadosActivos = BLL.BLLVarios.EstatusActivos();
            int[] Estatus = { 1, 4, 13, 14 };
            int[] DescuentosColegiatura, DescuentosInscripcion;
            List<DTO.ReciboDatos> recibos = new List<ReciboDatos>();
            List<DAL.Recibo> Recibos = new List<DAL.Recibo>();
            int descuentoIdColegiatura, descuentoIdInscripcion;
            DTO.Usuario.DTOUsuario Usuario;
            bool seReviso = false;

            using (UniversidadEntities db = new UniversidadEntities())
            {
                Usuario = (from a in db.Usuario
                           where a.UsuarioId == AlumnoBeca.usuarioId
                           select new DTO.Usuario.DTOUsuario
                           {
                               usuarioId = a.UsuarioId,
                               usuarioTipoId = a.UsuarioTipoId
                           }).AsNoTracking().FirstOrDefault();

                #region Verificar Descuentos

                #region Colegiatura
                DescuentoColegiatura = (from a in db.AlumnoDescuento
                                        join b in db.Descuento on a.DescuentoId equals b.DescuentoId
                                        where a.AlumnoId == AlumnoBeca.alumnoId
                                        && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                        && a.Anio == AlumnoBeca.anio
                                        && a.PeriodoId == AlumnoBeca.periodoId
                                        && a.PagoConceptoId == 800
                                        && (b.Descripcion == "Descuento en colegiatura" || b.Descripcion == "Beca Académica" || b.Descripcion == "Beca SEP")
                                        select a).ToList().FirstOrDefault();

                descuentoIdColegiatura = AlumnoBeca.esSEP
                        ? (from a in db.Descuento.AsNoTracking()
                           where a.PagoConceptoId == 800
                           && a.Descripcion == "Beca SEP"
                           && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                           select a.DescuentoId).FirstOrDefault()
                        : !AlumnoBeca.esEmpresa ? (from a in db.Descuento.AsNoTracking()
                                                   where a.PagoConceptoId == 800
                                                   && a.Descripcion == "Beca Académica"
                                                   && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                   select a.DescuentoId).FirstOrDefault()
                           : (from a in db.Descuento.AsNoTracking()
                              where a.PagoConceptoId == 800
                              && a.Descripcion == "Descuento en colegiatura"
                              && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                              select a.DescuentoId).FirstOrDefault();

                descuentoIdColegiatura = AlumnoBeca.esComite ? (DescuentoColegiatura != null ? DescuentoColegiatura.DescuentoId : descuentoIdColegiatura) : descuentoIdColegiatura;

                var DesCol = db.Descuento.AsNoTracking().Where(n => n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                   && n.PagoConceptoId == 800
                                   && (n.Descripcion == "Descuento en colegiatura" || n.Descripcion == "Beca Académica" || n.Descripcion == "Beca SEP")).ToList();

                DescuentosColegiatura = new int[DesCol.Count];
                int contador = 0;
                DesCol.ForEach(n => { DescuentosColegiatura[contador] = n.DescuentoId; contador++; });

                #endregion Colegiatura

                #region Inscripcion
                DescuentoInscripcion = (from a in db.AlumnoDescuento
                                        join b in db.Descuento on a.DescuentoId equals b.DescuentoId
                                        where a.AlumnoId == AlumnoBeca.alumnoId
                                              && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                              && a.Anio == AlumnoBeca.anio
                                              && a.PeriodoId == AlumnoBeca.periodoId
                                              && a.PagoConceptoId == 802
                                              && (b.Descripcion == "Descuento en inscripción" || b.Descripcion == "Beca SEP")
                                        select a).ToList().FirstOrDefault();

                descuentoIdInscripcion = AlumnoBeca.esSEP
                        ? (from a in db.Descuento.AsNoTracking()
                           where a.PagoConceptoId == 802
                           && a.Descripcion == "Beca SEP"
                           && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                           select a.DescuentoId).FirstOrDefault()
                        : AlumnoBeca.esEmpresa && AlumnoBeca.porcentajeInscripcion > 0
                        ? (from a in db.Descuento.AsNoTracking()
                           where a.PagoConceptoId == 802
                           && a.Descripcion == "Descuento en inscripción"
                           && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                           select a.DescuentoId).FirstOrDefault()
                        : 0;

                descuentoIdInscripcion = AlumnoBeca.esComite ? (DescuentoInscripcion != null ? DescuentoInscripcion.DescuentoId : descuentoIdInscripcion) : descuentoIdInscripcion;

                var DesIns = db.Descuento.Where(n => n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                    && n.PagoConceptoId == 802
                                    && (n.Descripcion == "Descuento en inscripción" || n.Descripcion == "Beca SEP")).ToList();

                DescuentosInscripcion = new int[DesIns.Count];
                contador = 0;
                DesIns.ForEach(n => { DescuentosInscripcion[contador] = n.DescuentoId; contador++; });

                #endregion Inscripcion

                #endregion Verificar Descuentos

                #region Revisión Coordinadores

                seReviso = db.AlumnoRevision.Count(n => n.AlumnoId == AlumnoBeca.alumnoId
                                                        && n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                        && n.Anio == AlumnoBeca.anio
                                                        && n.PeriodoId == AlumnoBeca.periodoId) > 0 ? true : false;

                var Revision = db.AlumnoRevision.Where(n => n.AlumnoId == AlumnoBeca.alumnoId
                                                        && n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                        && n.Anio == AlumnoBeca.anio
                                                        && n.PeriodoId == AlumnoBeca.periodoId).FirstOrDefault();

                bool esCompleta = Revision != null ? Revision.InscripcionCompleta : false;

                #endregion Revisión Coordinadores

                //Si fue revisado por los coordinadores y es inscripción completa
                //Si es de periodos anteriores a 2017-2
                if ((seReviso && esCompleta) || (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) < 20172) || AlumnoBeca.genera)
                {
                    #region Pagos Pendientes

                    PagosPendientes.AddRange((from a in db.Pago
                                              where (a.Cuota1.PagoConceptoId == 802 || a.Cuota1.PagoConceptoId == 800)
                                                 && Estatus.Contains(a.EstatusId)
                                                 && (a.Anio == AlumnoBeca.anio && a.PeriodoId == AlumnoBeca.periodoId)
                                                 && a.Promesa >= 0
                                                 && a.AlumnoId == AlumnoBeca.alumnoId
                                                 && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                              select a).ToList());

                    #endregion Pagos Pendientes

                    #region Generación de Cargos

                    var Colegiaturas = (from a in db.Pago
                                        join b in db.Cuota on a.CuotaId equals b.CuotaId
                                        where a.AlumnoId == AlumnoBeca.alumnoId
                                        && Estatus.Contains(a.EstatusId)
                                        && b.PagoConceptoId == 800
                                        && a.Anio == AlumnoBeca.anio
                                        && a.PeriodoId == AlumnoBeca.periodoId
                                        && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                        select a).AsNoTracking().ToList();

                    var Inscripcion = (from a in db.Pago
                                       join b in db.Cuota on a.CuotaId equals b.CuotaId
                                       where a.AlumnoId == AlumnoBeca.alumnoId
                                       && Estatus.Contains(a.EstatusId)
                                       && b.PagoConceptoId == 802
                                       && a.Anio == AlumnoBeca.anio
                                       && a.PeriodoId == AlumnoBeca.periodoId
                                       && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                       select a).AsNoTracking().ToList();

                    var CuotaColegiatura = (from a in db.Cuota
                                            where a.Anio == AlumnoBeca.anio
                                            && a.PeriodoId == AlumnoBeca.periodoId
                                            && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                            && a.PagoConceptoId == 800
                                            select a).AsNoTracking().FirstOrDefault();

                    var CuotaInscripcion = Inscripcion != null ? (from a in db.Cuota
                                                                  where a.Anio == AlumnoBeca.anio
                                                                  && a.PeriodoId == AlumnoBeca.periodoId
                                                                  && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                                  && a.PagoConceptoId == 802
                                                                  select a).AsNoTracking().FirstOrDefault() : null;

                    if (AlumnoBeca.ofertaEducativaId != 44)
                    {
                        int numeroPagos = 0;

                        #region Validación No. de Pagos

                        try
                        {
                            var Inscritos = db.AlumnoInscrito.Where(n => n.AlumnoId == AlumnoBeca.alumnoId && n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId).ToList();
                            int maxAnio = Inscritos.Max(n => n.Anio);
                            int maxPeriodoId = Inscritos.Where(n => n.Anio == maxAnio).ToList().Min(n => n.PeriodoId);
                            var Filtro = Inscritos.Where(n => n.Anio == maxAnio && n.PeriodoId == maxPeriodoId).FirstOrDefault();
                            numeroPagos = Filtro == null ? 0 : Filtro.PagoPlan.Pagos;
                        }

                        catch (Exception ex)
                        {
                            numeroPagos = 0;
                        }

                        #endregion Validación No. de Pagos

                        #region Cargos

                        if (Colegiaturas.Count != numeroPagos)
                        {
                            for (int i = 1; i <= numeroPagos; i++)

                                if (Colegiaturas.Count(n => n.SubperiodoId == i) == 0)
                                    Cabecero.Add(new Pago
                                    {
                                        ReferenciaId = "",
                                        AlumnoId = AlumnoBeca.alumnoId,
                                        Anio = AlumnoBeca.anio,
                                        PeriodoId = AlumnoBeca.periodoId,
                                        SubperiodoId = i,
                                        OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                                        FechaGeneracion = DateTime.Now,
                                        HoraGeneracion = DateTime.Now.TimeOfDay,
                                        CuotaId = CuotaColegiatura.CuotaId,
                                        Cuota = CuotaColegiatura.Monto,
                                        Promesa = CuotaColegiatura.Monto,
                                        Restante = CuotaColegiatura.Monto,
                                        EstatusId = 1,
                                        EsEmpresa = false,
                                        EsAnticipado = false,
                                        UsuarioId = Usuario.usuarioId,
                                        UsuarioTipoId = Usuario.usuarioTipoId,
                                        PeriodoAnticipadoId = 0,
                                        Cuota1 = CuotaColegiatura
                                    });
                        }

                        if (Inscripcion == null || Inscripcion.Count == 0)
                            Cabecero.Add(new Pago
                            {
                                ReferenciaId = "",
                                AlumnoId = AlumnoBeca.alumnoId,
                                Anio = AlumnoBeca.anio,
                                PeriodoId = AlumnoBeca.periodoId,
                                SubperiodoId = 1,
                                OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                                FechaGeneracion = DateTime.Now,
                                HoraGeneracion = DateTime.Now.TimeOfDay,
                                CuotaId = CuotaInscripcion.CuotaId,
                                Cuota = CuotaInscripcion.Monto,
                                Promesa = CuotaInscripcion.Monto,
                                Restante = CuotaInscripcion.Monto,
                                EstatusId = 1,
                                EsEmpresa = false,
                                EsAnticipado = false,
                                UsuarioId = Usuario.usuarioId,
                                UsuarioTipoId = Usuario.usuarioTipoId,
                                PeriodoAnticipadoId = 0,
                                Cuota1 = CuotaInscripcion
                            });

                        #endregion Cargos
                    }

                    if (Cabecero.Count > 0)
                    {
                        Cabecero.ForEach(n => { db.Pago.Add(n); });
                        db.SaveChanges();

                        Cabecero.ForEach(n => { n.ReferenciaId = db.spGeneraReferencia(n.PagoId).FirstOrDefault(); });
                        db.SaveChanges();
                    }

                    #endregion Generación de Cargos

                    #region Pagos Pendientes

                    if (Cabecero.Count > 0)
                    {
                        PagosPendientes.Clear();

                        PagosPendientes.AddRange((from a in db.Pago
                                                  where (a.Cuota1.PagoConceptoId == 800 || a.Cuota1.PagoConceptoId == 802)
                                                     && Estatus.Contains(a.EstatusId)
                                                     && (a.Anio == AlumnoBeca.anio && a.PeriodoId == AlumnoBeca.periodoId)
                                                     && a.Promesa >= 0
                                                     && a.AlumnoId == AlumnoBeca.alumnoId
                                                     && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                  select a).ToList());
                    }

                    #endregion Pagos Pendientes

                    PagosPendientes.ForEach(n =>
                    {
                        #region Colegiatura

                        if (n.Cuota1.PagoConceptoId == 800)
                        {
                            #region Existe Descuento

                            if (DescuentoColegiatura != null)
                            {
                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                List<PagoDescuento> Descuentos = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && DescuentosColegiatura.Contains(s.DescuentoId)).ToList();

                                sumaAnterior = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0);
                                n.Promesa = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0) - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdColegiatura,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                        };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                        };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                        };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });


                                                //No modificar el recibo que se emitio en PagoDetlla
                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                        };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda
                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor

                                if (Descuentos != null)
                                    db.PagoDescuento.RemoveRange(Descuentos);
                            }

                            #endregion Existe Descuento

                            #region No Existe Descuento

                            else if (DescuentoColegiatura == null)
                            {
                                List<PagoDescuento> Descuentos = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && DescuentosColegiatura.Contains(s.DescuentoId)).ToList();

                                Descuentos.ForEach(d =>
                                {
                                    n.Promesa = n.Promesa + d.Monto;
                                    n.Restante = n.Restante + d.Monto;
                                });

                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                sumaAnterior = n.Promesa;
                                n.Promesa = n.Promesa - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdColegiatura,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;

                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {

                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                        };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                        };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                        };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                        };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda
                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor

                                if (Descuentos != null)
                                    db.PagoDescuento.RemoveRange(Descuentos);
                            }

                            #endregion No Existe Descuento

                            db.SaveChanges();
                        }

                        #endregion Colegiatura

                        #region Inscripción

                        else if ((n.Cuota1.PagoConceptoId == 802 && AlumnoBeca.esSEP && !AlumnoBeca.esEmpresa))
                        {
                            #region Existe Descuento

                            if (DescuentoInscripcion != null)
                            {
                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                List<PagoDescuento> Descuentos = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && DescuentosInscripcion.Contains(s.DescuentoId)).ToList();
                                sumaAnterior = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0);
                                n.Promesa = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0) - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdInscripcion,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda

                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor

                                if (Descuentos != null)
                                    db.PagoDescuento.RemoveRange(Descuentos);
                            }

                            #endregion Existe Descuento

                            #region No Existe Descuento

                            else if (DescuentoInscripcion == null)
                            {
                                List<PagoDescuento> Descuentos = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && DescuentosInscripcion.Contains(s.DescuentoId)).ToList();

                                Descuentos.ForEach(d =>
                                {
                                    n.Promesa = n.Promesa + d.Monto;
                                    n.Restante = n.Restante + d.Monto;
                                });

                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                sumaAnterior = n.Promesa;
                                n.Promesa = n.Promesa - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdInscripcion,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda
                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor

                                if (Descuentos != null)
                                    db.PagoDescuento.RemoveRange(Descuentos);
                            }

                            #endregion No Existe Descuento

                            db.SaveChanges();
                        }

                        else if (AlumnoBeca.esEmpresa && AlumnoBeca.porcentajeInscripcion > 0)
                        {
                            #region Existe Descuento

                            if (DescuentoInscripcion != null)
                            {
                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                //PagoDescuento Descuento = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && s.DescuentoId == DescuentoInscripcion.DescuentoId).FirstOrDefault();
                                List<PagoDescuento> Descuentos = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && DescuentosInscripcion.Contains(s.DescuentoId)).ToList();
                                sumaAnterior = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0);
                                n.Promesa = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0) - ((n.Cuota) * (AlumnoBeca.porcentajeInscripcion / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdInscripcion,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda

                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor

                                if (Descuentos != null)
                                    db.PagoDescuento.RemoveRange(Descuentos);
                            }

                            #endregion Existe Descuento

                            #region No Existe Descuento

                            else if (DescuentoInscripcion == null)
                            {
                                List<PagoDescuento> Descuentos = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && DescuentosInscripcion.Contains(s.DescuentoId)).ToList();

                                Descuentos.ForEach(d =>
                                {
                                    n.Promesa = n.Promesa + d.Monto;
                                    n.Restante = n.Restante + d.Monto;
                                });

                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                sumaAnterior = n.Promesa;
                                n.Promesa = n.Promesa - ((n.Cuota) * (AlumnoBeca.porcentajeInscripcion / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdInscripcion,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                    };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda
                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor

                                if (Descuentos != null)
                                    db.PagoDescuento.RemoveRange(Descuentos);
                            }

                            #endregion No Existe Descuento

                            db.SaveChanges();
                        }

                        #endregion Inscripción
                    });
                }

                if (!aplicacionExtemporanea)
                {
                    #region Actualizar Descuento Colegiatura

                    #region Existe Descuento

                    if (DescuentoColegiatura != null)
                    {
                        DescuentoBitacora(DescuentoColegiatura);

                        if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))
                        {
                            if (db.AlumnoDescuentoPendiente.Count(n => n.AlumnoDescuentoId == DescuentoColegiatura.AlumnoDescuentoId) > 0)
                            {
                                db.AlumnoDescuentoPendiente.Remove(db.AlumnoDescuentoPendiente.Where(n => n.AlumnoDescuentoId == DescuentoColegiatura.AlumnoDescuentoId).FirstOrDefault());
                            }
                        }

                        DescuentoColegiatura.Monto = AlumnoBeca.porcentajeBeca;
                        DescuentoColegiatura.DescuentoId = descuentoIdColegiatura;
                        DescuentoColegiatura.UsuarioId = Usuario.usuarioId;
                        DescuentoColegiatura.FechaGeneracion = AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha);
                        DescuentoColegiatura.FechaAplicacion = DateTime.Now;
                        DescuentoColegiatura.HoraGeneracion = DateTime.Now.TimeOfDay;
                        DescuentoColegiatura.EsComite = AlumnoBeca.esComite;
                        DescuentoColegiatura.EsSEP = AlumnoBeca.esComite ? (DescuentoColegiatura.EsSEP ? true : false) : (AlumnoBeca.esSEP ? true : false);
                        DescuentoColegiatura.EsDeportiva = false;
                        //No se reviso y es una beca mayor a 2017-1
                        DescuentoColegiatura.AlumnoDescuentoPendiente = (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171)) ? new AlumnoDescuentoPendiente
                        {
                            FechaPendiente = DateTime.Now,
                            HoraPendiente = DateTime.Now.TimeOfDay,
                            UsuarioId = Usuario.usuarioId,
                            FechaAplicacion = DateTime.Now,
                            HoraAplicacion = DateTime.Now.TimeOfDay,
                            UsuarioIdAplicacion = 0,
                            EstatusId = 1
                        } : null;
                    }

                    #endregion Existe Descuento

                    #region No Existe Descuento

                    else if (DescuentoColegiatura == null)
                    {
                        db.AlumnoDescuento.Add(new AlumnoDescuento
                        {
                            AlumnoId = AlumnoBeca.alumnoId,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            PagoConceptoId = 800,
                            Monto = AlumnoBeca.porcentajeBeca,
                            UsuarioId = Usuario.usuarioId,
                            Comentario = "",
                            FechaGeneracion = AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha),
                            FechaAplicacion = DateTime.Now,
                            HoraGeneracion = DateTime.Now.TimeOfDay,
                            EstatusId = 2,
                            DescuentoId = descuentoIdColegiatura,
                            EsDeportiva = false,
                            EsComite = AlumnoBeca.esComite,
                            EsSEP = AlumnoBeca.esSEP,
                            AlumnoDescuentoPendiente =
                            !seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171) ?
                                new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                }
                                : null
                        });
                    }

                    #endregion No Existe Descuento

                    #endregion Actualizar Descuento Colegiatura

                    #region Actualizar Descuento Inscripcion

                    if ((AlumnoBeca.esSEP && !AlumnoBeca.esEmpresa))
                    {
                        #region Existe Descuento

                        if (DescuentoInscripcion != null)
                        {
                            DescuentoBitacora(DescuentoInscripcion);

                            if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))
                            {
                                if (db.AlumnoDescuentoPendiente.Count(n => n.AlumnoDescuentoId == DescuentoInscripcion.AlumnoDescuentoId) > 0)
                                {
                                    db.AlumnoDescuentoPendiente.Remove(db.AlumnoDescuentoPendiente.Where(n => n.AlumnoDescuentoId == DescuentoInscripcion.AlumnoDescuentoId).FirstOrDefault());
                                }
                            }

                            DescuentoInscripcion.Monto = AlumnoBeca.porcentajeBeca;
                            DescuentoInscripcion.DescuentoId = descuentoIdInscripcion;
                            DescuentoInscripcion.UsuarioId = Usuario.usuarioId;
                            DescuentoInscripcion.FechaGeneracion = AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha);
                            DescuentoInscripcion.FechaAplicacion = DateTime.Now;
                            DescuentoInscripcion.HoraGeneracion = DateTime.Now.TimeOfDay;
                            DescuentoInscripcion.EsComite = AlumnoBeca.esComite;
                            DescuentoInscripcion.EsSEP = AlumnoBeca.esComite ? (DescuentoInscripcion.EsSEP ? true : false) : (AlumnoBeca.esSEP ? true : false);
                            DescuentoInscripcion.EsDeportiva = false;

                            if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))

                                DescuentoInscripcion.AlumnoDescuentoPendiente = new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                };
                        }

                        #endregion Existe Descuento

                        #region No Existe Descuento

                        else if (DescuentoInscripcion == null)
                        {
                            db.AlumnoDescuento.Add(new AlumnoDescuento
                            {
                                AlumnoId = AlumnoBeca.alumnoId,
                                OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                                Anio = AlumnoBeca.anio,
                                PeriodoId = AlumnoBeca.periodoId,
                                PagoConceptoId = 802,
                                Monto = AlumnoBeca.porcentajeBeca,
                                UsuarioId = Usuario.usuarioId,
                                Comentario = "",
                                FechaGeneracion = AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha),
                                FechaAplicacion = DateTime.Now,
                                HoraGeneracion = DateTime.Now.TimeOfDay,
                                EstatusId = 2,
                                DescuentoId = descuentoIdInscripcion,
                                EsDeportiva = false,
                                EsComite = AlumnoBeca.esComite,
                                EsSEP = AlumnoBeca.esSEP,
                                AlumnoDescuentoPendiente = (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171)) ? new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                } : null
                            });
                        }

                        #endregion No Existe Descuento
                    }

                    else if ((AlumnoBeca.esEmpresa))
                    {
                        #region Existe Descuento

                        if (DescuentoInscripcion != null)
                        {
                            DescuentoBitacora(DescuentoInscripcion);

                            if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))
                            {
                                if (db.AlumnoDescuentoPendiente.Count(n => n.AlumnoDescuentoId == DescuentoInscripcion.AlumnoDescuentoId) > 0)
                                {
                                    db.AlumnoDescuentoPendiente.Remove(db.AlumnoDescuentoPendiente.Where(n => n.AlumnoDescuentoId == DescuentoInscripcion.AlumnoDescuentoId).FirstOrDefault());
                                }
                            }

                            DescuentoInscripcion.Monto = AlumnoBeca.porcentajeInscripcion;
                            DescuentoInscripcion.DescuentoId = descuentoIdInscripcion;
                            DescuentoInscripcion.UsuarioId = Usuario.usuarioId;
                            DescuentoInscripcion.FechaGeneracion = AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha);
                            DescuentoInscripcion.FechaAplicacion = DateTime.Now;
                            DescuentoInscripcion.HoraGeneracion = DateTime.Now.TimeOfDay;
                            DescuentoInscripcion.EsComite = AlumnoBeca.esComite;
                            DescuentoInscripcion.EsSEP = AlumnoBeca.esComite ? (DescuentoInscripcion.EsSEP ? true : false) : (AlumnoBeca.esSEP ? true : false);
                            DescuentoInscripcion.EsDeportiva = false;

                            if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))

                                DescuentoInscripcion.AlumnoDescuentoPendiente = new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                };
                        }

                        #endregion Existe Descuento

                        #region No Existe Descuento

                        else if (DescuentoInscripcion == null)
                        {
                            db.AlumnoDescuento.Add(new AlumnoDescuento
                            {
                                AlumnoId = AlumnoBeca.alumnoId,
                                OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                                Anio = AlumnoBeca.anio,
                                PeriodoId = AlumnoBeca.periodoId,
                                PagoConceptoId = 802,
                                Monto = AlumnoBeca.porcentajeInscripcion,
                                UsuarioId = Usuario.usuarioId,
                                Comentario = "",
                                FechaGeneracion = AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha),
                                FechaAplicacion = DateTime.Now,
                                HoraGeneracion = DateTime.Now.TimeOfDay,
                                EstatusId = 2,
                                DescuentoId = descuentoIdInscripcion,
                                EsDeportiva = false,
                                EsComite = AlumnoBeca.esComite,
                                EsSEP = AlumnoBeca.esSEP,
                                AlumnoDescuentoPendiente = (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171)) ? new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                } : null
                            });
                        }

                        #endregion No Existe Descuento
                    }

                    #endregion Actualizar Descuento Inscripcion

                    var F = db.AlumnoInscrito.AsNoTracking().Where(n => n.AlumnoId == AlumnoBeca.alumnoId && n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId && n.Anio == AlumnoBeca.anio && n.PeriodoId == AlumnoBeca.periodoId).Count();

                    if (F == 0)
                        Inscribir(AlumnoBeca, Usuario);
                }

                db.SaveChanges();
            }
        }

        public static void AplicaBeca_Excepcion(DTO.Alumno.Beca.DTOAlumnoBeca AlumnoBeca, bool aplicacionExtemporanea)
        {
            List<DAL.Pago> PagosPendientes = new List<Pago>();
            List<DAL.Pago> PagosPendientes2 = new List<Pago>();
            List<Pago> Cabecero = new List<Pago>();
            DAL.AlumnoDescuento DescuentoColegiatura = new AlumnoDescuento();
            DAL.AlumnoDescuento DescuentoInscripcion = new AlumnoDescuento();
            List<DTO.Varios.DTOEstatus> EstadosActivos = BLL.BLLVarios.EstatusActivos();
            int[] Estatus = { 1, 4, 13, 14 };
            int[] DescuentosColegiatura, DescuentosInscripcion;
            List<DTO.ReciboDatos> recibos = new List<ReciboDatos>();
            List<DAL.Recibo> Recibos = new List<DAL.Recibo>();
            int descuentoIdColegiatura, descuentoIdInscripcion;
            DTO.Usuario.DTOUsuario Usuario;
            bool seReviso = false;

            using (UniversidadEntities db = new UniversidadEntities())
            {
                Usuario = (from a in db.Usuario
                           where a.UsuarioId == AlumnoBeca.usuarioId
                           select new DTO.Usuario.DTOUsuario
                           {
                               usuarioId = a.UsuarioId,
                               usuarioTipoId = a.UsuarioTipoId
                           }).AsNoTracking().FirstOrDefault();

                #region Verificar Descuentos

                #region Colegiatura
                DescuentoColegiatura = (from a in db.AlumnoDescuento
                                        join b in db.Descuento on a.DescuentoId equals b.DescuentoId
                                        where a.AlumnoId == AlumnoBeca.alumnoId
                                        && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                        && a.Anio == AlumnoBeca.anio
                                        && a.PeriodoId == AlumnoBeca.periodoId
                                        && a.PagoConceptoId == 800
                                        && (b.Descripcion == "Descuento en colegiatura" || b.Descripcion == "Beca Académica" || b.Descripcion == "Beca SEP")
                                        select a).ToList().FirstOrDefault();

                descuentoIdColegiatura = AlumnoBeca.esSEP
                        ? (from a in db.Descuento.AsNoTracking()
                           where a.PagoConceptoId == 800
                           && a.Descripcion == "Beca SEP"
                           && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                           select a.DescuentoId).FirstOrDefault()
                        : !AlumnoBeca.esEmpresa ? (from a in db.Descuento.AsNoTracking()
                                                   where a.PagoConceptoId == 800
                                                   && a.Descripcion == "Beca Académica"
                                                   && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                   select a.DescuentoId).FirstOrDefault()
                           : (from a in db.Descuento.AsNoTracking()
                              where a.PagoConceptoId == 800
                              && a.Descripcion == "Descuento en colegiatura"
                              && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                              select a.DescuentoId).FirstOrDefault();

                descuentoIdColegiatura = AlumnoBeca.esComite ? (DescuentoColegiatura != null ? DescuentoColegiatura.DescuentoId : descuentoIdColegiatura) : descuentoIdColegiatura;

                var DesCol = db.Descuento.AsNoTracking().Where(n => n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                   && n.PagoConceptoId == 800
                                   && (n.Descripcion == "Descuento en colegiatura" || n.Descripcion == "Beca Académica" || n.Descripcion == "Beca SEP")).ToList();

                DescuentosColegiatura = new int[DesCol.Count];
                int contador = 0;
                DesCol.ForEach(n => { DescuentosColegiatura[contador] = n.DescuentoId; contador++; });

                #endregion Colegiatura

                #region Inscripcion
                DescuentoInscripcion = (from a in db.AlumnoDescuento
                                        join b in db.Descuento on a.DescuentoId equals b.DescuentoId
                                        where a.AlumnoId == AlumnoBeca.alumnoId
                                              && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                              && a.Anio == AlumnoBeca.anio
                                              && a.PeriodoId == AlumnoBeca.periodoId
                                              && a.PagoConceptoId == 802
                                              && (b.Descripcion == "Descuento en inscripción" || b.Descripcion == "Beca SEP")
                                        select a).ToList().FirstOrDefault();

                descuentoIdInscripcion = AlumnoBeca.esSEP
                        ? (from a in db.Descuento.AsNoTracking()
                           where a.PagoConceptoId == 802
                           && a.Descripcion == "Beca SEP"
                           && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                           select a.DescuentoId).FirstOrDefault()
                        : AlumnoBeca.esEmpresa && AlumnoBeca.porcentajeInscripcion > 0
                        ? (from a in db.Descuento.AsNoTracking()
                           where a.PagoConceptoId == 802
                           && a.Descripcion == "Descuento en inscripción"
                           && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                           select a.DescuentoId).FirstOrDefault()
                        : 0;

                descuentoIdInscripcion = AlumnoBeca.esComite ? (DescuentoInscripcion != null ? DescuentoInscripcion.DescuentoId : descuentoIdInscripcion) : descuentoIdInscripcion;

                var DesIns = db.Descuento.Where(n => n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                    && n.PagoConceptoId == 802
                                    && (n.Descripcion == "Descuento en inscripción" || n.Descripcion == "Beca SEP")).ToList();

                DescuentosInscripcion = new int[DesIns.Count];
                contador = 0;
                DesIns.ForEach(n => { DescuentosInscripcion[contador] = n.DescuentoId; contador++; });

                #endregion Inscripcion

                #endregion Verificar Descuentos

                #region Revisión Coordinadores

                seReviso = db.AlumnoRevision.Count(n => n.AlumnoId == AlumnoBeca.alumnoId
                                                        && n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                        && n.Anio == AlumnoBeca.anio
                                                        && n.PeriodoId == AlumnoBeca.periodoId) > 0 ? true : false;

                var Revision = db.AlumnoRevision.Where(n => n.AlumnoId == AlumnoBeca.alumnoId
                                                        && n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                        && n.Anio == AlumnoBeca.anio
                                                        && n.PeriodoId == AlumnoBeca.periodoId).FirstOrDefault();

                bool esCompleta = Revision != null ? Revision.InscripcionCompleta : false;

                #endregion Revisión Coordinadores

                //Si fue revisado por los coordinadores y es inscripción completa
                //Si es de periodos anteriores a 2017-2
                if ((seReviso && esCompleta) || (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) < 20172) || AlumnoBeca.genera)
                {
                    #region Pagos Pendientes

                    PagosPendientes.AddRange((from a in db.Pago
                                              where (a.Cuota1.PagoConceptoId == 802 || a.Cuota1.PagoConceptoId == 800)
                                                 && Estatus.Contains(a.EstatusId)
                                                 && (a.Anio == AlumnoBeca.anio && a.PeriodoId == AlumnoBeca.periodoId)
                                                 && a.Promesa > 0
                                                 && a.AlumnoId == AlumnoBeca.alumnoId
                                                 && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                              select a).ToList());

                    #endregion Pagos Pendientes

                    #region Generación de Cargos

                    var Colegiaturas = (from a in db.Pago
                                        join b in db.Cuota on a.CuotaId equals b.CuotaId
                                        where a.AlumnoId == AlumnoBeca.alumnoId
                                        && Estatus.Contains(a.EstatusId)
                                        && b.PagoConceptoId == 800
                                        && a.Anio == AlumnoBeca.anio
                                        && a.PeriodoId == AlumnoBeca.periodoId
                                        && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                        select a).AsNoTracking().ToList();

                    var Inscripcion = (from a in db.Pago
                                       join b in db.Cuota on a.CuotaId equals b.CuotaId
                                       where a.AlumnoId == AlumnoBeca.alumnoId
                                       && Estatus.Contains(a.EstatusId)
                                       && b.PagoConceptoId == 802
                                       && a.Anio == AlumnoBeca.anio
                                       && a.PeriodoId == AlumnoBeca.periodoId
                                       && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                       select a).AsNoTracking().ToList();

                    var CuotaColegiatura = (from a in db.Cuota
                                            where a.Anio == AlumnoBeca.anio
                                            && a.PeriodoId == AlumnoBeca.periodoId
                                            && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                            && a.PagoConceptoId == 800
                                            select a).AsNoTracking().FirstOrDefault();

                    var CuotaInscripcion = Inscripcion != null ? (from a in db.Cuota
                                                                  where a.Anio == AlumnoBeca.anio
                                                                  && a.PeriodoId == AlumnoBeca.periodoId
                                                                  && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                                  && a.PagoConceptoId == 802
                                                                  select a).AsNoTracking().FirstOrDefault() : null;

                    if (AlumnoBeca.ofertaEducativaId != 44)
                    {
                        int numeroPagos = 0;

                        #region Validación No. de Pagos

                        try
                        {
                            var Inscritos = db.AlumnoInscrito.Where(n => n.AlumnoId == AlumnoBeca.alumnoId && n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId).ToList();
                            int maxAnio = Inscritos.Max(n => n.Anio);
                            int maxPeriodoId = Inscritos.Where(n => n.Anio == maxAnio).ToList().Min(n => n.PeriodoId);
                            var Filtro = Inscritos.Where(n => n.Anio == maxAnio && n.PeriodoId == maxPeriodoId).FirstOrDefault();
                            numeroPagos = Filtro == null ? 0 : Filtro.PagoPlan.Pagos;
                        }

                        catch (Exception ex)
                        {
                            numeroPagos = 0;
                        }

                        #endregion Validación No. de Pagos

                        #region Cargos

                        if (Colegiaturas.Count != numeroPagos)
                        {
                            for (int i = 1; i <= numeroPagos; i++)

                                if (Colegiaturas.Count(n => n.SubperiodoId == i) == 0)
                                    Cabecero.Add(new Pago
                                    {
                                        ReferenciaId = "",
                                        AlumnoId = AlumnoBeca.alumnoId,
                                        Anio = AlumnoBeca.anio,
                                        PeriodoId = AlumnoBeca.periodoId,
                                        SubperiodoId = i,
                                        OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                                        FechaGeneracion = DateTime.Now,
                                        HoraGeneracion = DateTime.Now.TimeOfDay,
                                        CuotaId = CuotaColegiatura.CuotaId,
                                        Cuota = CuotaColegiatura.Monto,
                                        Promesa = CuotaColegiatura.Monto,
                                        Restante = CuotaColegiatura.Monto,
                                        EstatusId = 1,
                                        EsEmpresa = false,
                                        EsAnticipado = false,
                                        UsuarioId = Usuario.usuarioId,
                                        UsuarioTipoId = Usuario.usuarioTipoId,
                                        PeriodoAnticipadoId = 0,
                                        Cuota1 = CuotaColegiatura
                                    });
                        }

                        if (Inscripcion == null || Inscripcion.Count == 0)
                            Cabecero.Add(new Pago
                            {
                                ReferenciaId = "",
                                AlumnoId = AlumnoBeca.alumnoId,
                                Anio = AlumnoBeca.anio,
                                PeriodoId = AlumnoBeca.periodoId,
                                SubperiodoId = 1,
                                OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                                FechaGeneracion = DateTime.Now,
                                HoraGeneracion = DateTime.Now.TimeOfDay,
                                CuotaId = CuotaInscripcion.CuotaId,
                                Cuota = CuotaInscripcion.Monto,
                                Promesa = CuotaInscripcion.Monto,
                                Restante = CuotaInscripcion.Monto,
                                EstatusId = 1,
                                EsEmpresa = false,
                                EsAnticipado = false,
                                UsuarioId = Usuario.usuarioId,
                                UsuarioTipoId = Usuario.usuarioTipoId,
                                PeriodoAnticipadoId = 0,
                                Cuota1 = CuotaInscripcion
                            });

                        #endregion Cargos
                    }

                    if (Cabecero.Count > 0)
                    {
                        Cabecero.ForEach(n => { db.Pago.Add(n); });
                        db.SaveChanges();

                        Cabecero.ForEach(n => { n.ReferenciaId = db.spGeneraReferencia(n.PagoId).FirstOrDefault(); });
                        db.SaveChanges();
                    }

                    #endregion Generación de Cargos

                    #region Pagos Pendientes

                    if (Cabecero.Count > 0)
                    {
                        PagosPendientes.Clear();

                        PagosPendientes.AddRange((from a in db.Pago
                                                  where (a.Cuota1.PagoConceptoId == 800 || a.Cuota1.PagoConceptoId == 802)
                                                     && Estatus.Contains(a.EstatusId)
                                                     && (a.Anio == AlumnoBeca.anio && a.PeriodoId == AlumnoBeca.periodoId)
                                                     && a.Promesa > 0
                                                     && a.AlumnoId == AlumnoBeca.alumnoId
                                                     && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                  select a).ToList());
                    }

                    #endregion Pagos Pendientes

                    PagosPendientes.ForEach(n =>
                    {
                        #region Colegiatura

                        if (n.Cuota1.PagoConceptoId == 800)
                        {
                            #region Existe Descuento

                            if (DescuentoColegiatura != null)
                            {
                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                List<PagoDescuento> Descuentos = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && DescuentosColegiatura.Contains(s.DescuentoId)).ToList();

                                sumaAnterior = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0);
                                n.Promesa = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0) - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdColegiatura,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });


                                                //No modificar el recibo que se emitio en PagoDetlla
                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda
                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor

                                if (Descuentos != null)
                                    db.PagoDescuento.RemoveRange(Descuentos);
                            }

                            #endregion Existe Descuento

                            #region No Existe Descuento

                            else if (DescuentoColegiatura == null)
                            {
                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                sumaAnterior = n.Promesa;
                                n.Promesa = n.Promesa - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdColegiatura,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;

                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {

                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda
                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor
                            }

                            #endregion No Existe Descuento

                            db.SaveChanges();
                        }

                        #endregion Colegiatura

                        #region Inscripción

                        else if ((n.Cuota1.PagoConceptoId == 802 && AlumnoBeca.esSEP && !AlumnoBeca.esEmpresa))
                        {
                            #region Existe Descuento

                            if (DescuentoInscripcion != null)
                            {
                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                //PagoDescuento Descuento = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && s.DescuentoId == DescuentoInscripcion.DescuentoId).FirstOrDefault();
                                List<PagoDescuento> Descuentos = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && DescuentosColegiatura.Contains(s.DescuentoId)).ToList();
                                sumaAnterior = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0);
                                n.Promesa = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0) - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdInscripcion,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda

                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor

                                if (Descuentos != null)
                                    db.PagoDescuento.RemoveRange(Descuentos);
                            }

                            #endregion Existe Descuento

                            #region No Existe Descuento

                            else if (DescuentoInscripcion == null)
                            {
                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                sumaAnterior = n.Promesa;
                                n.Promesa = n.Promesa - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdInscripcion,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda
                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor
                            }

                            #endregion No Existe Descuento

                            db.SaveChanges();
                        }

                        else if (AlumnoBeca.esEmpresa && AlumnoBeca.porcentajeInscripcion > 0)
                        {
                            #region Existe Descuento

                            if (DescuentoInscripcion != null)
                            {
                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                //PagoDescuento Descuento = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && s.DescuentoId == DescuentoInscripcion.DescuentoId).FirstOrDefault();
                                List<PagoDescuento> Descuentos = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && DescuentosInscripcion.Contains(s.DescuentoId)).ToList();
                                sumaAnterior = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0);
                                n.Promesa = n.Promesa + (Descuentos != null ? Descuentos.Sum(s => s.Monto) : 0) - ((n.Cuota) * (AlumnoBeca.porcentajeInscripcion / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdInscripcion,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda

                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor

                                if (Descuentos != null)
                                    db.PagoDescuento.RemoveRange(Descuentos);
                            }

                            #endregion Existe Descuento

                            #region No Existe Descuento

                            else if (DescuentoInscripcion == null)
                            {
                                #region Calculos

                                decimal diferencia = n.Promesa;
                                decimal importe = 0;
                                decimal saldo = 0;
                                decimal sumaAnterior = 0;
                                decimal montoDescuento = 0;

                                sumaAnterior = n.Promesa;
                                n.Promesa = n.Promesa - ((n.Cuota) * (AlumnoBeca.porcentajeInscripcion / 100));
                                montoDescuento = sumaAnterior - Math.Round(n.Promesa, 0);

                                db.PagoDescuento.Add(new PagoDescuento
                                {
                                    PagoId = n.PagoId,
                                    DescuentoId = descuentoIdInscripcion,
                                    Monto = montoDescuento
                                });

                                n.Promesa = sumaAnterior - montoDescuento;
                                importe = diferencia - n.Promesa;

                                if (importe >= 0)
                                    n.Restante = n.Restante - importe;
                                else if (importe < 0)
                                    n.Restante = n.Restante + Math.Abs(importe);

                                if (n.Restante <= 0)
                                {
                                    saldo = Math.Abs(n.Restante);
                                    n.Restante = 0;
                                }

                                if (n.Restante > 0)
                                    n.EstatusId = 1;
                                else if (n.Restante == 0)
                                    n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                                #endregion Calculos

                                #region Saldo a Favor

                                if (saldo > 0)
                                {
                                    #region Referenciados

                                    var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                                    Referenciados.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                                s.PagoDetalle.FirstOrDefault().Importe = 0;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #endregion Referenciados

                                    #region Caja

                                    var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                                    Caja.ForEach(s =>
                                    {
                                        if (saldo > 0)
                                        {
                                            #region PagoParcial Bitacora

                                            db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                            {
                                                PagoParcialId = s.PagoParcialId,
                                                PagoId = s.PagoId,
                                                SucursalCajaId = s.SucursalCajaId,
                                                ReciboId = s.ReciboId,
                                                Pago = s.Pago,
                                                FechaPago = s.FechaPago,
                                                HoraPago = s.HoraPago,
                                                EstatusId = s.EstatusId,
                                                TieneMovimientos = s.TieneMovimientos,
                                                PagoTipoId = s.PagoTipoId,
                                                ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                                FechaBitacora = DateTime.Now.Date,
                                                HoraBitacora = DateTime.Now.TimeOfDay,
                                                UsuarioId = Usuario.usuarioId
                                            });

                                            #endregion PagoParcial Bitacora

                                            if (saldo <= s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = saldo
                                                });

                                                /*
                                                var Detalles = s.PagoDetalle.ToList();
                                                decimal saldoDetalles = saldo;

                                                Detalles.ForEach(a =>
                                                {
                                                    if (saldoDetalles > 0)
                                                    {
                                                        if (saldoDetalles <= a.Importe)
                                                        {
                                                            a.Importe = a.Importe - saldoDetalles;
                                                            saldoDetalles = 0;
                                                        }

                                                        else if (saldoDetalles > a.Importe)
                                                        {
                                                            saldoDetalles = saldoDetalles - a.Importe;
                                                            a.Importe = 0;
                                                        }
                                                    }
                                                });
                                                */

                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                                s.Pago = s.Pago - saldo;
                                                saldo = 0;
                                            }

                                            else if (saldo > s.Pago)
                                            {
                                                #region Reclasificacion

                                                s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                                };

                                                #endregion Reclasificacion

                                                /* Recibo */
                                                recibos.Add(new ReciboDatos
                                                {
                                                    reciboId = s.ReciboId,
                                                    sucursalCajaId = s.SucursalCajaId,
                                                    importe = s.Pago
                                                });

                                                //Duda
                                                //s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                                s.ReferenciaProcesada.SeGasto = false;
                                                s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                                saldo = saldo - s.Pago;
                                                s.Pago = 0;
                                                s.EstatusId = 2;
                                            }
                                        }
                                    });

                                    #region Edición de Recibo
                                    /*
                                    if (recibos.Count > 0)
                                    {
                                        var RecibosTotal = (from consulta in (from a in recibos
                                                                              select new { a })
                                                            group consulta by new
                                                            {
                                                                consulta.a.reciboId,
                                                                consulta.a.sucursalCajaId
                                                            } into g

                                                            select new DTO.ReciboDatos
                                                            {
                                                                reciboId = g.Key.reciboId,
                                                                sucursalCajaId = g.Key.sucursalCajaId,
                                                                importe = g.Sum(a => a.a.importe)
                                                            }).ToList();

                                        RecibosTotal.ForEach(a =>
                                        {
                                            Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                        });

                                        Recibos.ForEach(a =>
                                        {
                                            a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                        });
                                    }
                                    */
                                    #endregion Edición de Recibo

                                    #endregion Caja
                                }

                                #endregion Saldo a Favor
                            }

                            #endregion No Existe Descuento

                            db.SaveChanges();
                        }

                        #endregion Inscripción
                    });
                }

                if (!aplicacionExtemporanea)
                {
                    #region Actualizar Descuento Colegiatura

                    #region Existe Descuento

                    if (DescuentoColegiatura != null)
                    {
                        DescuentoBitacora(DescuentoColegiatura);

                        if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))
                        {
                            if (db.AlumnoDescuentoPendiente.Count(n => n.AlumnoDescuentoId == DescuentoColegiatura.AlumnoDescuentoId) > 0)
                            {
                                db.AlumnoDescuentoPendiente.Remove(db.AlumnoDescuentoPendiente.Where(n => n.AlumnoDescuentoId == DescuentoColegiatura.AlumnoDescuentoId).FirstOrDefault());
                            }
                        }

                        DescuentoColegiatura.Monto = AlumnoBeca.porcentajeBeca;
                        DescuentoColegiatura.DescuentoId = descuentoIdColegiatura;
                        DescuentoColegiatura.UsuarioId = Usuario.usuarioId;
                        DescuentoColegiatura.FechaGeneracion = AlumnoBeca.fecha != null ? AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha) : DateTime.Now;
                        DescuentoColegiatura.FechaAplicacion = DateTime.Now;
                        DescuentoColegiatura.HoraGeneracion = DateTime.Now.TimeOfDay;
                        DescuentoColegiatura.EsComite = AlumnoBeca.esComite;
                        DescuentoColegiatura.EsSEP = AlumnoBeca.esComite ? (DescuentoColegiatura.EsSEP ? true : false) : (AlumnoBeca.esSEP ? true : false);
                        DescuentoColegiatura.EsDeportiva = false;
                        //No se reviso y es una beca mayor a 2017-1
                        DescuentoColegiatura.AlumnoDescuentoPendiente = (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171)) ? new AlumnoDescuentoPendiente
                        {
                            FechaPendiente = DateTime.Now,
                            HoraPendiente = DateTime.Now.TimeOfDay,
                            UsuarioId = Usuario.usuarioId,
                            FechaAplicacion = DateTime.Now,
                            HoraAplicacion = DateTime.Now.TimeOfDay,
                            UsuarioIdAplicacion = 0,
                            EstatusId = 1
                        } : null;
                    }

                    #endregion Existe Descuento

                    #region No Existe Descuento

                    else if (DescuentoColegiatura == null)
                    {
                        db.AlumnoDescuento.Add(new AlumnoDescuento
                        {
                            AlumnoId = AlumnoBeca.alumnoId,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            PagoConceptoId = 800,
                            Monto = AlumnoBeca.porcentajeBeca,
                            UsuarioId = Usuario.usuarioId,
                            Comentario = "",
                            FechaGeneracion = AlumnoBeca.fecha != null ? AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha) : DateTime.Now,
                            FechaAplicacion = DateTime.Now,
                            HoraGeneracion = DateTime.Now.TimeOfDay,
                            EstatusId = 2,
                            DescuentoId = descuentoIdColegiatura,
                            EsDeportiva = false,
                            EsComite = AlumnoBeca.esComite,
                            EsSEP = AlumnoBeca.esSEP,
                            AlumnoDescuentoPendiente =
                            !seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171) ?
                                new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                }
                                : null
                        });
                    }

                    #endregion No Existe Descuento

                    #endregion Actualizar Descuento Colegiatura

                    #region Actualizar Descuento Inscripcion

                    if ((AlumnoBeca.esSEP && !AlumnoBeca.esEmpresa))
                    {
                        #region Existe Descuento

                        if (DescuentoInscripcion != null)
                        {
                            DescuentoBitacora(DescuentoInscripcion);

                            if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))
                            {
                                if (db.AlumnoDescuentoPendiente.Count(n => n.AlumnoDescuentoId == DescuentoInscripcion.AlumnoDescuentoId) > 0)
                                {
                                    db.AlumnoDescuentoPendiente.Remove(db.AlumnoDescuentoPendiente.Where(n => n.AlumnoDescuentoId == DescuentoInscripcion.AlumnoDescuentoId).FirstOrDefault());
                                }
                            }

                            DescuentoInscripcion.Monto = AlumnoBeca.porcentajeBeca;
                            DescuentoInscripcion.DescuentoId = descuentoIdInscripcion;
                            DescuentoInscripcion.UsuarioId = Usuario.usuarioId;
                            DescuentoInscripcion.FechaGeneracion = AlumnoBeca.fecha != null ? AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha) : DateTime.Now;
                            DescuentoInscripcion.FechaAplicacion = DateTime.Now;
                            DescuentoInscripcion.HoraGeneracion = DateTime.Now.TimeOfDay;
                            DescuentoInscripcion.EsComite = AlumnoBeca.esComite;
                            DescuentoInscripcion.EsSEP = AlumnoBeca.esComite ? (DescuentoInscripcion.EsSEP ? true : false) : (AlumnoBeca.esSEP ? true : false);
                            DescuentoInscripcion.EsDeportiva = false;

                            if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))

                                DescuentoInscripcion.AlumnoDescuentoPendiente = new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                };
                        }

                        #endregion Existe Descuento

                        #region No Existe Descuento

                        else if (DescuentoInscripcion == null)
                        {
                            db.AlumnoDescuento.Add(new AlumnoDescuento
                            {
                                AlumnoId = AlumnoBeca.alumnoId,
                                OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                                Anio = AlumnoBeca.anio,
                                PeriodoId = AlumnoBeca.periodoId,
                                PagoConceptoId = 802,
                                Monto = AlumnoBeca.porcentajeBeca,
                                UsuarioId = Usuario.usuarioId,
                                Comentario = "",
                                FechaGeneracion = AlumnoBeca.fecha != null ? AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha) : DateTime.Now,
                                FechaAplicacion = DateTime.Now,
                                HoraGeneracion = DateTime.Now.TimeOfDay,
                                EstatusId = 2,
                                DescuentoId = descuentoIdInscripcion,
                                EsDeportiva = false,
                                EsComite = AlumnoBeca.esComite,
                                EsSEP = AlumnoBeca.esSEP,
                                AlumnoDescuentoPendiente = (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171)) ? new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                } : null
                            });
                        }

                        #endregion No Existe Descuento
                    }

                    else if ((AlumnoBeca.esEmpresa))
                    {
                        #region Existe Descuento

                        if (DescuentoInscripcion != null)
                        {
                            DescuentoBitacora(DescuentoInscripcion);

                            if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))
                            {
                                if (db.AlumnoDescuentoPendiente.Count(n => n.AlumnoDescuentoId == DescuentoInscripcion.AlumnoDescuentoId) > 0)
                                {
                                    db.AlumnoDescuentoPendiente.Remove(db.AlumnoDescuentoPendiente.Where(n => n.AlumnoDescuentoId == DescuentoInscripcion.AlumnoDescuentoId).FirstOrDefault());
                                }
                            }

                            DescuentoInscripcion.Monto = AlumnoBeca.porcentajeInscripcion;
                            DescuentoInscripcion.DescuentoId = descuentoIdInscripcion;
                            DescuentoInscripcion.UsuarioId = Usuario.usuarioId;
                            DescuentoInscripcion.FechaGeneracion = AlumnoBeca.fecha != null ? AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha) : DateTime.Now;
                            DescuentoInscripcion.FechaAplicacion = DateTime.Now;
                            DescuentoInscripcion.HoraGeneracion = DateTime.Now.TimeOfDay;
                            DescuentoInscripcion.EsComite = AlumnoBeca.esComite;
                            DescuentoInscripcion.EsSEP = AlumnoBeca.esComite ? (DescuentoInscripcion.EsSEP ? true : false) : (AlumnoBeca.esSEP ? true : false);
                            DescuentoInscripcion.EsDeportiva = false;

                            if (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171))

                                DescuentoInscripcion.AlumnoDescuentoPendiente = new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                };
                        }

                        #endregion Existe Descuento

                        #region No Existe Descuento

                        else if (DescuentoInscripcion == null)
                        {
                            db.AlumnoDescuento.Add(new AlumnoDescuento
                            {
                                AlumnoId = AlumnoBeca.alumnoId,
                                OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                                Anio = AlumnoBeca.anio,
                                PeriodoId = AlumnoBeca.periodoId,
                                PagoConceptoId = 802,
                                Monto = AlumnoBeca.porcentajeInscripcion,
                                UsuarioId = Usuario.usuarioId,
                                Comentario = "",
                                FechaGeneracion = AlumnoBeca.fecha != null ? AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha) : DateTime.Now,
                                FechaAplicacion = DateTime.Now,
                                HoraGeneracion = DateTime.Now.TimeOfDay,
                                EstatusId = 2,
                                DescuentoId = descuentoIdInscripcion,
                                EsDeportiva = false,
                                EsComite = AlumnoBeca.esComite,
                                EsSEP = AlumnoBeca.esSEP,
                                AlumnoDescuentoPendiente = (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171)) ? new AlumnoDescuentoPendiente
                                {
                                    FechaPendiente = DateTime.Now,
                                    HoraPendiente = DateTime.Now.TimeOfDay,
                                    UsuarioId = Usuario.usuarioId,
                                    FechaAplicacion = DateTime.Now,
                                    HoraAplicacion = DateTime.Now.TimeOfDay,
                                    UsuarioIdAplicacion = 0,
                                    EstatusId = 1
                                } : null
                            });
                        }

                        #endregion No Existe Descuento
                    }

                    #endregion Actualizar Descuento Inscripcion

                    Inscribir(AlumnoBeca, Usuario);
                }

                db.SaveChanges();
            }
        }
        public static void AplicaBecaDeportiva(DTO.Alumno.Beca.DTOAlumnoBecaDeportiva AlumnoBeca, bool aplicacionExtemporanea)
        {
            List<DAL.Pago> PagosPendientes = new List<Pago>();
            List<Pago> Cabecero = new List<Pago>();
            DAL.AlumnoDescuento DescuentoColegiatura = new AlumnoDescuento();
            List<DTO.ReciboDatos> recibos = new List<ReciboDatos>();
            List<DAL.Recibo> Recibos = new List<DAL.Recibo>();
            List<DTO.Varios.DTOEstatus> EstadosActivos = BLL.BLLVarios.EstatusActivos();
            int[] Estatus = { 1, 4, 13, 14 };
            int descuentoIdColegiatura;
            DTO.Usuario.DTOUsuario Usuario;
            bool seReviso = false;

            using (UniversidadEntities db = new UniversidadEntities())
            {
                Usuario = (from a in db.Usuario
                           where a.UsuarioId == AlumnoBeca.usuarioId
                           select new DTO.Usuario.DTOUsuario
                           {
                               usuarioId = a.UsuarioId,
                               usuarioTipoId = a.UsuarioTipoId
                           }).AsNoTracking().FirstOrDefault();

                #region Pagos Pendientes

                PagosPendientes.AddRange((from a in db.Pago
                                          where (a.Cuota1.PagoConceptoId == 800)
                                                && Estatus.Contains(a.EstatusId)
                                                && (a.Anio == AlumnoBeca.anio && a.PeriodoId == AlumnoBeca.periodoId)
                                                && (a.Promesa > 0)
                                                && a.AlumnoId == AlumnoBeca.alumnoId
                                                && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                          select a).ToList());

                #endregion Pagos Pendientes

                #region Verificar Descuentos

                DescuentoColegiatura = (from a in db.AlumnoDescuento
                                        join b in db.Descuento on a.DescuentoId equals b.DescuentoId
                                        where a.AlumnoId == AlumnoBeca.alumnoId
                                        && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                        && a.Anio == AlumnoBeca.anio
                                        && a.PeriodoId == AlumnoBeca.periodoId
                                        && a.PagoConceptoId == 800
                                        && b.Descripcion == "Beca deportiva"
                                        select a).ToList().FirstOrDefault();

                descuentoIdColegiatura = (from a in db.Descuento.AsNoTracking()
                                          where a.PagoConceptoId == 800
                                          && a.Descripcion == "Beca deportiva"
                                          && a.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                          select a.DescuentoId).FirstOrDefault();

                #endregion Verificar Descuentos

                seReviso = db.AlumnoRevision.Count(n => n.AlumnoId == AlumnoBeca.alumnoId
                                                       && n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId
                                                       && n.Anio == AlumnoBeca.anio
                                                       && n.PeriodoId == AlumnoBeca.periodoId) > 0 ? true : false;

                PagosPendientes.ForEach(n =>
                {

                    #region Existe Descuento
                    if (DescuentoColegiatura != null)
                    {
                        #region Calculos

                        decimal diferencia = n.Promesa;
                        decimal importe = 0;
                        decimal saldo = 0;
                        decimal sumaAnterior = 0;
                        decimal montoDescuento = 0;

                        PagoDescuento Descuento = db.PagoDescuento.Where(s => s.PagoId == n.PagoId && s.DescuentoId == DescuentoColegiatura.DescuentoId).FirstOrDefault();
                        sumaAnterior = n.Promesa + (Descuento != null ? Descuento.Monto : 0);
                        n.Promesa = n.Promesa + (Descuento != null ? Descuento.Monto : 0) - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                        montoDescuento = sumaAnterior - Math.Round(n.Promesa, 2);

                        db.PagoDescuento.Add(new PagoDescuento
                        {
                            PagoId = n.PagoId,
                            DescuentoId = descuentoIdColegiatura,
                            Monto = montoDescuento
                        });

                        n.Promesa = sumaAnterior - montoDescuento;
                        importe = diferencia - n.Promesa;

                        if (importe >= 0)
                            n.Restante = n.Restante - importe;
                        else if (importe < 0)
                            n.Restante = n.Restante + Math.Abs(importe);

                        if (n.Restante <= 0)
                        {
                            saldo = Math.Abs(n.Restante);
                            n.Restante = 0;
                        }

                        if (n.Restante > 0)
                            n.EstatusId = 1;
                        else if (n.Restante == 0)
                            n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                        #endregion Calculos

                        #region Saldo a Favor

                        if (saldo > 0)
                        {
                            #region Referenciados

                            var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                            Referenciados.ForEach(s =>
                            {
                                if (saldo > 0)
                                {
                                    #region PagoParcial Bitacora

                                    db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                    {
                                        PagoParcialId = s.PagoParcialId,
                                        PagoId = s.PagoId,
                                        SucursalCajaId = s.SucursalCajaId,
                                        ReciboId = s.ReciboId,
                                        Pago = s.Pago,
                                        FechaPago = s.FechaPago,
                                        HoraPago = s.HoraPago,
                                        EstatusId = s.EstatusId,
                                        TieneMovimientos = s.TieneMovimientos,
                                        PagoTipoId = s.PagoTipoId,
                                        ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                        FechaBitacora = DateTime.Now.Date,
                                        HoraBitacora = DateTime.Now.TimeOfDay,
                                        UsuarioId = Usuario.usuarioId
                                    });

                                    #endregion PagoParcial Bitacora

                                    if (saldo <= s.Pago)
                                    {
                                        #region Reclasificacion

                                        s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                            };

                                        #endregion Reclasificacion

                                        s.ReferenciaProcesada.SeGasto = false;
                                        s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                        s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                        s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                        s.Pago = s.Pago - saldo;
                                        saldo = 0;
                                    }

                                    else if (saldo > s.Pago)
                                    {
                                        #region Reclasificacion

                                        s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                            };

                                        #endregion Reclasificacion

                                        s.ReferenciaProcesada.SeGasto = false;
                                        s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                        s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                        s.PagoDetalle.FirstOrDefault().Importe = 0;
                                        saldo = saldo - s.Pago;
                                        s.Pago = 0;
                                        s.EstatusId = 2;
                                    }
                                }
                            });

                            #endregion Referenciados

                            #region Caja

                            var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                            Caja.ForEach(s =>
                            {
                                if (saldo > 0)
                                {
                                    #region PagoParcial Bitacora

                                    db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                    {
                                        PagoParcialId = s.PagoParcialId,
                                        PagoId = s.PagoId,
                                        SucursalCajaId = s.SucursalCajaId,
                                        ReciboId = s.ReciboId,
                                        Pago = s.Pago,
                                        FechaPago = s.FechaPago,
                                        HoraPago = s.HoraPago,
                                        EstatusId = s.EstatusId,
                                        TieneMovimientos = s.TieneMovimientos,
                                        PagoTipoId = s.PagoTipoId,
                                        ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                        FechaBitacora = DateTime.Now.Date,
                                        HoraBitacora = DateTime.Now.TimeOfDay,
                                        UsuarioId = Usuario.usuarioId
                                    });

                                    #endregion PagoParcial Bitacora

                                    if (saldo <= s.Pago)
                                    {
                                        #region Reclasificacion

                                        s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                            };

                                        #endregion Reclasificacion

                                        /* Recibo */
                                        recibos.Add(new ReciboDatos
                                        {
                                            reciboId = s.ReciboId,
                                            sucursalCajaId = s.SucursalCajaId,
                                            importe = saldo
                                        });

                                        var Detalles = s.PagoDetalle.ToList();
                                        decimal saldoDetalles = saldo;

                                        Detalles.ForEach(a =>
                                        {
                                            if (saldoDetalles > 0)
                                            {
                                                if (saldoDetalles <= a.Importe)
                                                {
                                                    a.Importe = a.Importe - saldoDetalles;
                                                    saldoDetalles = 0;
                                                }

                                                else if (saldoDetalles > a.Importe)
                                                {
                                                    saldoDetalles = saldoDetalles - a.Importe;
                                                    a.Importe = 0;
                                                }
                                            }
                                        });

                                        s.ReferenciaProcesada.SeGasto = false;
                                        s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                        s.Pago = s.Pago - saldo;
                                        saldo = 0;
                                    }

                                    else if (saldo > s.Pago)
                                    {
                                        #region Reclasificacion

                                        s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                            };

                                        #endregion Reclasificacion

                                        /* Recibo */
                                        recibos.Add(new ReciboDatos
                                        {
                                            reciboId = s.ReciboId,
                                            sucursalCajaId = s.SucursalCajaId,
                                            importe = s.Pago
                                        });

                                        //Duda
                                        s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                        s.ReferenciaProcesada.SeGasto = false;
                                        s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                        saldo = saldo - s.Pago;
                                        s.Pago = 0;
                                        s.EstatusId = 2;
                                    }
                                }
                            });

                            if (recibos.Count > 0)
                            {
                                var RecibosTotal = (from consulta in
                                                        (from a in recibos
                                                         select new { a })
                                                    group consulta by new
                                                    {
                                                        consulta.a.reciboId,
                                                        consulta.a.sucursalCajaId
                                                    } into g

                                                    select new DTO.ReciboDatos
                                                    {
                                                        reciboId = g.Key.reciboId,
                                                        sucursalCajaId = g.Key.sucursalCajaId,
                                                        importe = g.Sum(a => a.a.importe)
                                                    }).ToList();

                                RecibosTotal.ForEach(a =>
                                {
                                    Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                });

                                Recibos.ForEach(a =>
                                {
                                    a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                });
                            }

                            #endregion Caja
                        }

                        #endregion Saldo a Favor

                        if (Descuento != null)
                            db.PagoDescuento.Remove(Descuento);
                    }
                    #endregion Existe Descuento

                    #region No Existe Descuento
                    if (DescuentoColegiatura == null)
                    {
                        #region Calculos

                        decimal diferencia = n.Promesa;
                        decimal importe = 0;
                        decimal saldo = 0;
                        decimal sumaAnterior = 0;
                        decimal montoDescuento = 0;


                        sumaAnterior = n.Promesa;
                        n.Promesa = n.Promesa - ((n.Cuota) * (AlumnoBeca.porcentajeBeca / 100));
                        montoDescuento = sumaAnterior - Math.Round(n.Promesa, 2);

                        db.PagoDescuento.Add(new PagoDescuento
                        {
                            PagoId = n.PagoId,
                            DescuentoId = descuentoIdColegiatura,
                            Monto = montoDescuento
                        });

                        n.Promesa = sumaAnterior - montoDescuento;
                        importe = diferencia - n.Promesa;

                        if (importe >= 0)
                            n.Restante = n.Restante - importe;
                        else if (importe < 0)
                            n.Restante = n.Restante + Math.Abs(importe);

                        if (n.Restante <= 0)
                        {
                            saldo = Math.Abs(n.Restante);
                            n.Restante = 0;
                        }

                        if (n.Restante > 0)
                            n.EstatusId = 1;
                        else if (n.Restante == 0)
                            n.EstatusId = db.PagoParcial.Count(s => s.PagoId == n.PagoId && s.EstatusId == 4) > 1 ? 14 : 4;

                        #endregion Calculos

                        #region Saldo a Favor

                        if (saldo > 0)
                        {
                            #region Referenciados

                            var Referenciados = db.PagoParcial.Where(s => s.PagoTipoId == 2 && s.PagoId == n.PagoId && s.EstatusId == 4 && s.ReferenciaProcesada.EsIngles == false).ToList();
                            Referenciados.ForEach(s =>
                            {

                                if (saldo > 0)
                                {
                                    #region PagoParcial Bitacora

                                    db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                    {
                                        PagoParcialId = s.PagoParcialId,
                                        PagoId = s.PagoId,
                                        SucursalCajaId = s.SucursalCajaId,
                                        ReciboId = s.ReciboId,
                                        Pago = s.Pago,
                                        FechaPago = s.FechaPago,
                                        HoraPago = s.HoraPago,
                                        EstatusId = s.EstatusId,
                                        TieneMovimientos = s.TieneMovimientos,
                                        PagoTipoId = s.PagoTipoId,
                                        ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                        FechaBitacora = DateTime.Now.Date,
                                        HoraBitacora = DateTime.Now.TimeOfDay,
                                        UsuarioId = Usuario.usuarioId
                                    });

                                    #endregion PagoParcial Bitacora

                                    if (saldo <= s.Pago)
                                    {
                                        #region Reclasificacion

                                        s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                            };

                                        #endregion Reclasificacion

                                        s.ReferenciaProcesada.SeGasto = false;
                                        s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                        s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                        s.PagoDetalle.FirstOrDefault().Importe = s.PagoDetalle.FirstOrDefault().Importe - saldo;
                                        s.Pago = s.Pago - saldo;
                                        saldo = 0;
                                    }

                                    else if (saldo > s.Pago)
                                    {
                                        #region Reclasificacion

                                        s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                            };

                                        #endregion Reclasificacion

                                        s.ReferenciaProcesada.SeGasto = false;
                                        s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                        s.ReferenciaProcesada.Importe = s.ReferenciaProcesada.ReferenciaTipoId == 4 ? s.ReferenciaProcesada.Restante : s.ReferenciaProcesada.Importe;
                                        s.PagoDetalle.FirstOrDefault().Importe = 0;
                                        saldo = saldo - s.Pago;
                                        s.Pago = 0;
                                        s.EstatusId = 2;
                                    }
                                }
                            });

                            #endregion Referenciados

                            #region Caja

                            var Caja = db.PagoParcial.Where(s => s.PagoTipoId == 1 && s.PagoId == n.PagoId && s.EstatusId == 4).ToList();
                            Caja.ForEach(s =>
                            {
                                if (saldo > 0)
                                {
                                    #region PagoParcial Bitacora

                                    db.PagoParcialBitacora.Add(new PagoParcialBitacora
                                    {
                                        PagoParcialId = s.PagoParcialId,
                                        PagoId = s.PagoId,
                                        SucursalCajaId = s.SucursalCajaId,
                                        ReciboId = s.ReciboId,
                                        Pago = s.Pago,
                                        FechaPago = s.FechaPago,
                                        HoraPago = s.HoraPago,
                                        EstatusId = s.EstatusId,
                                        TieneMovimientos = s.TieneMovimientos,
                                        PagoTipoId = s.PagoTipoId,
                                        ReferenciaProcesadaId = s.ReferenciaProcesadaId,
                                        FechaBitacora = DateTime.Now.Date,
                                        HoraBitacora = DateTime.Now.TimeOfDay,
                                        UsuarioId = Usuario.usuarioId
                                    });

                                    #endregion PagoParcial Bitacora

                                    if (saldo <= s.Pago)
                                    {
                                        #region Reclasificacion

                                        s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = saldo,
                                                    ReclasificacionTipoId = 1
                                                }
                                            };

                                        #endregion Reclasificacion

                                        /* Recibo */
                                        recibos.Add(new ReciboDatos
                                        {
                                            reciboId = s.ReciboId,
                                            sucursalCajaId = s.SucursalCajaId,
                                            importe = saldo
                                        });

                                        var Detalles = s.PagoDetalle.ToList();
                                        decimal saldoDetalles = saldo;

                                        Detalles.ForEach(a =>
                                        {
                                            if (saldoDetalles > 0)
                                            {
                                                if (saldoDetalles <= a.Importe)
                                                {
                                                    a.Importe = a.Importe - saldoDetalles;
                                                    saldoDetalles = 0;
                                                }

                                                else if (saldoDetalles > a.Importe)
                                                {
                                                    saldoDetalles = saldoDetalles - a.Importe;
                                                    a.Importe = 0;
                                                }
                                            }
                                        });

                                        s.ReferenciaProcesada.SeGasto = false;
                                        s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + saldo;
                                        s.Pago = s.Pago - saldo;
                                        saldo = 0;
                                    }

                                    else if (saldo > s.Pago)
                                    {
                                        #region Reclasificacion

                                        s.Reclasificacion = new List<Reclasificacion> {
                                                new Reclasificacion {
                                                    UsuarioId = Usuario.usuarioId,
                                                    FechaReclasificacion = DateTime.Now,
                                                    HoraReclasificacion = DateTime.Now.TimeOfDay,
                                                    Importe = s.Pago,
                                                    ReclasificacionTipoId = 1
                                                }
                                            };

                                        #endregion Reclasificacion

                                        /* Recibo */
                                        recibos.Add(new ReciboDatos
                                        {
                                            reciboId = s.ReciboId,
                                            sucursalCajaId = s.SucursalCajaId,
                                            importe = s.Pago
                                        });

                                        //Duda
                                        s.PagoDetalle.ToList().ForEach(a => { a.Importe = 0; });
                                        s.ReferenciaProcesada.SeGasto = false;
                                        s.ReferenciaProcesada.Restante = s.ReferenciaProcesada.Restante + s.Pago;
                                        saldo = saldo - s.Pago;
                                        s.Pago = 0;
                                        s.EstatusId = 2;
                                    }
                                }
                            });

                            if (recibos.Count > 0)
                            {
                                var RecibosTotal = (from consulta in
                                                        (from a in recibos
                                                         select new { a })
                                                    group consulta by new
                                                    {
                                                        consulta.a.reciboId,
                                                        consulta.a.sucursalCajaId
                                                    } into g

                                                    select new DTO.ReciboDatos
                                                    {
                                                        reciboId = g.Key.reciboId,
                                                        sucursalCajaId = g.Key.sucursalCajaId,
                                                        importe = g.Sum(a => a.a.importe)
                                                    }).ToList();

                                RecibosTotal.ForEach(a =>
                                {
                                    Recibos.Add(db.Recibo.Where(p => p.ReciboId == a.reciboId && p.SucursalCajaId == a.sucursalCajaId).FirstOrDefault());
                                });

                                Recibos.ForEach(a =>
                                {
                                    a.Importe = a.Importe - (RecibosTotal.Where(p => p.reciboId == a.ReciboId && p.sucursalCajaId == a.SucursalCajaId).FirstOrDefault().importe);
                                });
                            }

                            #endregion Caja
                        }

                        #endregion Saldo a Favor
                    }
                    #endregion No Existe Descuento

                    db.SaveChanges();
                });

                if (!aplicacionExtemporanea)
                {
                    #region Actualizar Descuento Colegiatura

                    #region Existe Descuento

                    if (DescuentoColegiatura != null)
                    {
                        DescuentoBitacora(DescuentoColegiatura);

                        DescuentoColegiatura.Monto = AlumnoBeca.porcentajeBeca;
                        DescuentoColegiatura.DescuentoId = descuentoIdColegiatura;
                        DescuentoColegiatura.UsuarioId = Usuario.usuarioId;
                        DescuentoColegiatura.FechaGeneracion = DateTime.Now;
                        DescuentoColegiatura.FechaAplicacion = DateTime.Now;
                        DescuentoColegiatura.HoraGeneracion = DateTime.Now.TimeOfDay;
                        DescuentoColegiatura.EsDeportiva = true;
                        DescuentoColegiatura.AlumnoDescuentoPendiente = (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171)) ? new AlumnoDescuentoPendiente
                        {
                            FechaPendiente = DateTime.Now,
                            HoraPendiente = DateTime.Now.TimeOfDay,
                            UsuarioId = Usuario.usuarioId,
                            FechaAplicacion = DateTime.Now,
                            HoraAplicacion = DateTime.Now.TimeOfDay,
                            UsuarioIdAplicacion = 0,
                            EstatusId = 1
                        } : null;
                    }

                    #endregion Existe Descuento

                    #region No Existe Descuento

                    else if (DescuentoColegiatura == null)
                    {
                        db.AlumnoDescuento.Add(new AlumnoDescuento
                        {
                            AlumnoId = AlumnoBeca.alumnoId,
                            OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                            Anio = AlumnoBeca.anio,
                            PeriodoId = AlumnoBeca.periodoId,
                            PagoConceptoId = 800,
                            Monto = AlumnoBeca.porcentajeBeca,
                            UsuarioId = Usuario.usuarioId,
                            Comentario = "",
                            FechaGeneracion = DateTime.Now,
                            FechaAplicacion = DateTime.Now,
                            HoraGeneracion = DateTime.Now.TimeOfDay,
                            EstatusId = 2,
                            DescuentoId = descuentoIdColegiatura,
                            EsDeportiva = true,
                            EsComite = false,
                            EsSEP = false,
                            AlumnoDescuentoPendiente = (!seReviso && (int.Parse(AlumnoBeca.anio + "" + AlumnoBeca.periodoId) > 20171)) ? new AlumnoDescuentoPendiente
                            {
                                FechaPendiente = DateTime.Now,
                                HoraPendiente = DateTime.Now.TimeOfDay,
                                UsuarioId = Usuario.usuarioId,
                                FechaAplicacion = DateTime.Now,
                                HoraAplicacion = DateTime.Now.TimeOfDay,
                                UsuarioIdAplicacion = 0,
                                EstatusId = 1
                            } : null
                        });
                    }

                    #endregion No Existe Descuento
                }

                db.SaveChanges();

                #endregion Actualizar Descuento Colegiatura
            }
        }
        public static void DescuentoBitacora(DAL.AlumnoDescuento Descuento)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                db.AlumnoDescuentoBitacora.Add(new AlumnoDescuentoBitacora
                {
                    AlumnoDescuentoId = Descuento.AlumnoDescuentoId,
                    AlumnoId = Descuento.AlumnoId,
                    OfertaEducativaId = Descuento.OfertaEducativaId,
                    Anio = Descuento.Anio,
                    PeriodoId = Descuento.PeriodoId,
                    DescuentoId = Descuento.DescuentoId,
                    PagoConceptoId = Descuento.PagoConceptoId,
                    Monto = Descuento.Monto,
                    UsuarioId = Descuento.UsuarioId,
                    Comentario = Descuento.Comentario,
                    FechaGeneracion = Descuento.FechaGeneracion,
                    HoraGeneracion = DateTime.Now.TimeOfDay,
                    EsSEP = Descuento.EsSEP,
                    EsComite = Descuento.EsComite,
                    EsDeportiva = Descuento.EsDeportiva,
                    FechaAplicacion = Descuento.FechaAplicacion,
                    EstatusId = Descuento.EstatusId
                });

                db.SaveChanges();
            }
        }
        public static void Inscribir(DTO.Alumno.Beca.DTOAlumnoBeca AlumnoBeca, DTO.Usuario.DTOUsuario Usuario)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                var Inscritos = db.AlumnoInscrito.Where(n => n.AlumnoId == AlumnoBeca.alumnoId && n.OfertaEducativaId == AlumnoBeca.ofertaEducativaId).ToList();
                int maxAnio = Inscritos.Max(n => n.Anio);
                int maxPeriodoId = Inscritos.Where(n => n.Anio == maxAnio).ToList().Min(n => n.PeriodoId);
                var Filtro = Inscritos.Where(n => n.Anio == maxAnio && n.PeriodoId == maxPeriodoId).FirstOrDefault();

                if ((Filtro.Anio == AlumnoBeca.anio && Filtro.PeriodoId <= AlumnoBeca.periodoId) || (Filtro.Anio < AlumnoBeca.anio))
                {
                    db.AlumnoInscritoBitacora.Add(new AlumnoInscritoBitacora
                    {
                        AlumnoId = Filtro.AlumnoId,
                        OfertaEducativaId = Filtro.OfertaEducativaId,
                        Anio = Filtro.Anio,
                        PeriodoId = Filtro.PeriodoId,
                        FechaInscripcion = Filtro.FechaInscripcion,
                        HoraInscripcion = Filtro.HoraInscripcion,
                        PagoPlanId = Filtro.PagoPlanId,
                        TurnoId = Filtro.TurnoId,
                        EsEmpresa = Filtro.EsEmpresa,
                        UsuarioId = Filtro.UsuarioId
                    });

                    db.AlumnoInscrito.Add(new AlumnoInscrito
                    {
                        AlumnoId = AlumnoBeca.alumnoId,
                        OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                        Anio = AlumnoBeca.anio,
                        PeriodoId = AlumnoBeca.periodoId,
                        FechaInscripcion = AlumnoBeca.fecha != null ? AlumnoBeca.fecha == "" ? DateTime.Now : DateTime.Parse(AlumnoBeca.fecha) : DateTime.Now,
                        HoraInscripcion = AlumnoBeca.fecha == "" ? DateTime.Now.TimeOfDay : Filtro.HoraInscripcion,
                        PagoPlanId = Filtro.PagoPlanId,
                        TurnoId = Filtro.TurnoId,
                        EsEmpresa = Filtro.EsEmpresa,
                        UsuarioId = Usuario.usuarioId,
                        EstatusId = 1
                    });

                    db.AlumnoInscrito.Remove(Filtro);
                }

                else if ((Filtro.Anio > AlumnoBeca.anio) || (Filtro.Anio == AlumnoBeca.anio && Filtro.PeriodoId > AlumnoBeca.periodoId))
                {
                    db.AlumnoInscritoBitacora.Add(new AlumnoInscritoBitacora
                    {
                        AlumnoId = AlumnoBeca.alumnoId,
                        OfertaEducativaId = AlumnoBeca.ofertaEducativaId,
                        Anio = AlumnoBeca.anio,
                        PeriodoId = AlumnoBeca.periodoId,
                        FechaInscripcion = DateTime.Now,
                        HoraInscripcion = DateTime.Now.TimeOfDay,
                        PagoPlanId = Filtro.PagoPlanId,
                        TurnoId = Filtro.TurnoId,
                        EsEmpresa = Filtro.EsEmpresa,
                        UsuarioId = Usuario.usuarioId
                    });
                }

                db.SaveChanges();
            }
        }
        public static bool UpdateAlumno(DTOAlumno objAlumno, DTOProspectoDetalle objAlumnoD, int UsuarioId)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    Alumno objAlumnoDB = db.Alumno.Where(a => a.AlumnoId == objAlumno.AlumnoId).FirstOrDefault();
                    //Alumno
                    db.AlumnoBitacora.Add(new AlumnoBitacora
                    {
                        AlumnoId = objAlumnoDB.AlumnoId,
                        Anio = objAlumnoDB.Anio,
                        EstatusId = objAlumnoDB.EstatusId,
                        Fecha = DateTime.Now,
                        FechaRegistro = objAlumnoDB.FechaRegistro,
                        Materno = objAlumnoDB.Materno,
                        MatriculaId = objAlumnoDB.MatriculaId,
                        Nombre = objAlumnoDB.Nombre,
                        Paterno = objAlumnoDB.Paterno,
                        PeriodoId = objAlumnoDB.PeriodoId,
                        UsuarioId = objAlumnoDB.UsuarioId,
                        UsuarioIdBitacora = UsuarioId
                    });
                    objAlumnoDB.Nombre = objAlumno.Nombre;
                    objAlumnoDB.Paterno = objAlumno.Paterno;
                    objAlumnoDB.Materno = objAlumno.Materno;


                    //AlumnoDetalle
                    db.AlumnoDetalleBitacora.Add(new AlumnoDetalleBitacora
                    {
                        AlumnoId = objAlumnoDB.AlumnoDetalle.AlumnoId,
                        Calle = objAlumnoDB.AlumnoDetalle.Calle,
                        UsuarioId = UsuarioId,
                        Celular = objAlumnoDB.AlumnoDetalle.Celular,
                        Colonia = objAlumnoDB.AlumnoDetalle.Colonia,
                        CP = objAlumnoDB.AlumnoDetalle.CP,
                        CURP = objAlumnoDB.AlumnoDetalle.CURP,
                        Email = objAlumnoDB.AlumnoDetalle.Email,
                        EntidadFederativaId = objAlumnoDB.AlumnoDetalle.EntidadFederativaId,
                        EntidadNacimientoId = objAlumnoDB.AlumnoDetalle.EntidadNacimientoId,
                        EstadoCivilId = objAlumnoDB.AlumnoDetalle.EstadoCivilId,
                        Fecha = DateTime.Now,
                        FechaNacimiento = objAlumnoDB.AlumnoDetalle.FechaNacimiento,
                        GeneroId = objAlumnoDB.AlumnoDetalle.GeneroId,
                        MunicipioId = objAlumnoDB.AlumnoDetalle.MunicipioId,
                        NoExterior = objAlumnoDB.AlumnoDetalle.NoExterior,
                        NoInterior = objAlumnoDB.AlumnoDetalle.NoInterior,
                        PaisId = objAlumnoDB.AlumnoDetalle.PaisId,
                        TelefonoCasa = objAlumnoDB.AlumnoDetalle.TelefonoCasa,
                        TelefonoOficina = objAlumnoDB.AlumnoDetalle.TelefonoOficina
                    });
                    objAlumnoDB.AlumnoDetalle.Calle = objAlumno.DTOAlumnoDetalle.Calle;
                    objAlumnoDB.AlumnoDetalle.Celular = objAlumno.DTOAlumnoDetalle.Celular;
                    objAlumnoDB.AlumnoDetalle.Colonia = objAlumno.DTOAlumnoDetalle.Colonia;
                    objAlumnoDB.AlumnoDetalle.CP = objAlumno.DTOAlumnoDetalle.Cp;
                    objAlumnoDB.AlumnoDetalle.CURP = objAlumno.DTOAlumnoDetalle.CURP;
                    objAlumnoDB.AlumnoDetalle.Email = objAlumno.DTOAlumnoDetalle.Email;
                    objAlumnoDB.AlumnoDetalle.EntidadFederativaId = objAlumno.DTOAlumnoDetalle.EntidadFederativaId;
                    objAlumnoDB.AlumnoDetalle.EntidadNacimientoId = objAlumno.DTOAlumnoDetalle.EntidadNacimientoId;
                    objAlumnoDB.AlumnoDetalle.EstadoCivilId = objAlumno.DTOAlumnoDetalle.EstadoCivilId;
                    objAlumnoDB.AlumnoDetalle.FechaNacimiento = objAlumno.DTOAlumnoDetalle.FechaNacimiento;
                    objAlumnoDB.AlumnoDetalle.GeneroId = objAlumno.DTOAlumnoDetalle.GeneroId;
                    objAlumnoDB.AlumnoDetalle.MunicipioId = objAlumno.DTOAlumnoDetalle.MunicipioId;
                    objAlumnoDB.AlumnoDetalle.NoExterior = objAlumno.DTOAlumnoDetalle.NoExterior;
                    objAlumnoDB.AlumnoDetalle.NoInterior = objAlumno.DTOAlumnoDetalle.NoInterior;
                    objAlumnoDB.AlumnoDetalle.PaisId = objAlumno.DTOAlumnoDetalle.PaisId;
                    //objAlumnoDB.AlumnoDetalle.ProspectoId = objAlumno.DTOAlumnoDetalle.AlumnoId;
                    objAlumnoDB.AlumnoDetalle.TelefonoCasa = objAlumno.DTOAlumnoDetalle.TelefonoCasa;
                    objAlumnoDB.AlumnoDetalle.TelefonoOficina = objAlumno.DTOAlumnoDetalle.TelefonoOficina;


                    #region Personas Autorizadas
                    if (objAlumno.DTOPersonaAutorizada.Count > 0)
                    {
                        if (objAlumnoDB.PersonaAutorizada.Count > 0)
                        {

                            db.PersonaAutorizadaBitacora.Add(new PersonaAutorizadaBitacora
                            {
                                AlumnoId = objAlumnoDB.PersonaAutorizada.First().AlumnoId,
                                Celular = objAlumnoDB.PersonaAutorizada.First().Celular,
                                Email = objAlumnoDB.PersonaAutorizada.First().Email,
                                EsAutorizada = objAlumnoDB.PersonaAutorizada.First().EsAutorizada,
                                Materno = objAlumnoDB.PersonaAutorizada.First().Materno,
                                Nombre = objAlumnoDB.PersonaAutorizada.First().Nombre,
                                ParentescoId = objAlumnoDB.PersonaAutorizada.First().ParentescoId,
                                Paterno = objAlumnoDB.PersonaAutorizada.First().Paterno,
                                PersonaAutorizadaId = objAlumnoDB.PersonaAutorizada.First().PersonaAutorizadaId,
                                Telefono = objAlumnoDB.PersonaAutorizada.First().Telefono,
                                Fecha = DateTime.Now,
                                UsuarioId = UsuarioId
                            });

                            objAlumnoDB.PersonaAutorizada.First().Celular = objAlumno.DTOPersonaAutorizada[0].Celular;
                            objAlumnoDB.PersonaAutorizada.First().Email = objAlumno.DTOPersonaAutorizada[0].Email;
                            objAlumnoDB.PersonaAutorizada.First().EsAutorizada = objAlumno.DTOPersonaAutorizada[0].Autoriza;
                            objAlumnoDB.PersonaAutorizada.First().Materno = objAlumno.DTOPersonaAutorizada[0].Materno;
                            objAlumnoDB.PersonaAutorizada.First().Nombre = objAlumno.DTOPersonaAutorizada[0].Nombre;
                            objAlumnoDB.PersonaAutorizada.First().ParentescoId = objAlumno.DTOPersonaAutorizada[0].ParentescoId;
                            objAlumnoDB.PersonaAutorizada.First().Paterno = objAlumno.DTOPersonaAutorizada[0].Paterno;
                            objAlumnoDB.PersonaAutorizada.First().Telefono = objAlumno.DTOPersonaAutorizada[0].Telefono;

                            if (objAlumnoDB.PersonaAutorizada.Count > 1)
                            {
                                if (objAlumno.DTOPersonaAutorizada.Count > 1)
                                {
                                    db.PersonaAutorizadaBitacora.Add(new PersonaAutorizadaBitacora
                                    {
                                        AlumnoId = objAlumnoDB.PersonaAutorizada.Last().AlumnoId,
                                        Celular = objAlumnoDB.PersonaAutorizada.Last().Celular,
                                        Email = objAlumnoDB.PersonaAutorizada.Last().Email,
                                        EsAutorizada = objAlumnoDB.PersonaAutorizada.Last().EsAutorizada,
                                        Materno = objAlumnoDB.PersonaAutorizada.Last().Materno,
                                        Nombre = objAlumnoDB.PersonaAutorizada.Last().Nombre,
                                        ParentescoId = objAlumnoDB.PersonaAutorizada.Last().ParentescoId,
                                        Paterno = objAlumnoDB.PersonaAutorizada.Last().Paterno,
                                        PersonaAutorizadaId = objAlumnoDB.PersonaAutorizada.Last().PersonaAutorizadaId,
                                        Telefono = objAlumnoDB.PersonaAutorizada.Last().Telefono,
                                        Fecha = DateTime.Now,
                                        UsuarioId = UsuarioId
                                    });

                                    objAlumnoDB.PersonaAutorizada.Last().Celular = objAlumno.DTOPersonaAutorizada[1].Celular;
                                    objAlumnoDB.PersonaAutorizada.Last().Email = objAlumno.DTOPersonaAutorizada[1].Email;
                                    objAlumnoDB.PersonaAutorizada.Last().EsAutorizada = objAlumno.DTOPersonaAutorizada[1].Autoriza;
                                    objAlumnoDB.PersonaAutorizada.Last().Materno = objAlumno.DTOPersonaAutorizada[1].Materno;
                                    objAlumnoDB.PersonaAutorizada.Last().Nombre = objAlumno.DTOPersonaAutorizada[1].Nombre;
                                    objAlumnoDB.PersonaAutorizada.Last().ParentescoId = objAlumno.DTOPersonaAutorizada[1].ParentescoId;
                                    objAlumnoDB.PersonaAutorizada.Last().Paterno = objAlumno.DTOPersonaAutorizada[1].Paterno;
                                    objAlumnoDB.PersonaAutorizada.Last().Telefono = objAlumno.DTOPersonaAutorizada[1].Telefono;
                                }
                            }
                            else
                            {
                                if (objAlumno.DTOPersonaAutorizada.Count > 1)
                                {
                                    if (objAlumno.DTOPersonaAutorizada[1].ParentescoId > 0)
                                    {
                                        objAlumnoDB.PersonaAutorizada.Add(new PersonaAutorizada
                                        {
                                            AlumnoId = objAlumno.AlumnoId,
                                            Celular = objAlumno.DTOPersonaAutorizada[1].Celular,
                                            Email = objAlumno.DTOPersonaAutorizada[1].Email,
                                            EsAutorizada = objAlumno.DTOPersonaAutorizada[1].Autoriza,
                                            Materno = objAlumno.DTOPersonaAutorizada[1].Materno,
                                            Nombre = objAlumno.DTOPersonaAutorizada[1].Nombre,
                                            ParentescoId = objAlumno.DTOPersonaAutorizada[1].ParentescoId,
                                            Paterno = objAlumno.DTOPersonaAutorizada[1].Paterno,
                                            Telefono = objAlumno.DTOPersonaAutorizada[1].Telefono
                                        });
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (objAlumno.DTOPersonaAutorizada[0].ParentescoId > 0)
                            {
                                objAlumnoDB.PersonaAutorizada.Add(new PersonaAutorizada
                                {
                                    AlumnoId = objAlumno.AlumnoId,
                                    Celular = objAlumno.DTOPersonaAutorizada[0].Celular,
                                    Email = objAlumno.DTOPersonaAutorizada[0].Email,
                                    EsAutorizada = objAlumno.DTOPersonaAutorizada[0].Autoriza,
                                    Materno = objAlumno.DTOPersonaAutorizada[0].Materno,
                                    Nombre = objAlumno.DTOPersonaAutorizada[0].Nombre,
                                    ParentescoId = objAlumno.DTOPersonaAutorizada[0].ParentescoId,
                                    Paterno = objAlumno.DTOPersonaAutorizada[0].Paterno,
                                    Telefono = objAlumno.DTOPersonaAutorizada[0].Telefono
                                });
                            }
                            if (objAlumno.DTOPersonaAutorizada.Count > 1)
                            {
                                objAlumnoDB.PersonaAutorizada.Add(new PersonaAutorizada
                                {
                                    AlumnoId = objAlumno.AlumnoId,
                                    Celular = objAlumno.DTOPersonaAutorizada[1].Celular,
                                    Email = objAlumno.DTOPersonaAutorizada[1].Email,
                                    EsAutorizada = objAlumno.DTOPersonaAutorizada[1].Autoriza,
                                    Materno = objAlumno.DTOPersonaAutorizada[1].Materno,
                                    Nombre = objAlumno.DTOPersonaAutorizada[1].Nombre,
                                    ParentescoId = objAlumno.DTOPersonaAutorizada[1].ParentescoId,
                                    Paterno = objAlumno.DTOPersonaAutorizada[1].Paterno,
                                    Telefono = objAlumno.DTOPersonaAutorizada[1].Telefono
                                });
                            }
                        }
                    }
                    #endregion




                    db.SaveChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static List<DTOAlumno> BuscarAlumnoTexto(string Cadena)
        {
            using (UniversidadEntities db = new UniversidadEntities())
            {
                try
                {
                    List<DTOAlumno> lstAlumnos = (from a in db.Alumno
                                                  where (a.Nombre + " " + a.Paterno + " " + a.Materno).Contains(Cadena)
                                                  select new DTOAlumno
                                                  {
                                                     


                                                      AlumnoId = a.AlumnoId,
                                                      Nombre = a.Nombre + " " + a.Paterno + " " + a.Materno,
                                                      FechaRegistro = (a.FechaRegistro.Day.ToString().Length < 2 ? "0" + a.FechaRegistro.Day.ToString() : a.FechaRegistro.Day.ToString()) + "/" +
                                                          (a.FechaRegistro.Month.ToString().Length < 2 ? "0" + a.FechaRegistro.Month.ToString() : a.FechaRegistro.Month.ToString()) + "/" +
                                                          a.FechaRegistro.Year.ToString(),
                                                      DTOAlumnoDetalle = new DTOAlumnoDetalle
                                                      {
                                                          AlumnoId = a.AlumnoDetalle.AlumnoId,
                                                          FechaNacimientoC = (a.AlumnoDetalle.FechaNacimiento.Day.ToString().Length < 2 ? "0" + a.AlumnoDetalle.FechaNacimiento.Day.ToString() : a.AlumnoDetalle.FechaNacimiento.Day.ToString()) + "/" +
                                                          (a.AlumnoDetalle.FechaNacimiento.Month.ToString().Length < 2 ? "0" + a.AlumnoDetalle.FechaNacimiento.Month.ToString() : a.AlumnoDetalle.FechaNacimiento.Month.ToString()) + "/" +
                                                          a.AlumnoDetalle.FechaNacimiento.Year.ToString(),
                                                      },
                                                      AlumnoInscrito = (from b in db.AlumnoInscrito
                                                                        where a.AlumnoId == b.AlumnoId
                                                                        select new DTOAlumnoInscrito
                                                                        {
                                                                            AlumnoId = b.AlumnoId,
                                                                            OfertaEducativaId = b.OfertaEducativaId,
                                                                            OfertaEducativa = new DTOOfertaEducativa
                                                                            {
                                                                                OfertaEducativaId = b.OfertaEducativaId,
                                                                                Descripcion = b.OfertaEducativa.Descripcion
                                                                            }
                                                                        }).FirstOrDefault(),
                                                      Usuario = (from f in db.Usuario
                                                                 where f.UsuarioId == a.UsuarioId
                                                                 select new DTOUsuario
                                                                 {
                                                                     UsuarioId = f.UsuarioId,
                                                                     Nombre = f.Nombre
                                                                 }).FirstOrDefault()
                                                  }).ToList();
                    lstAlumnos.ForEach(delegate (DTOAlumno objAlumno)
                    {
                        if (objAlumno.AlumnoInscrito == null)
                        {
                            objAlumno.AlumnoInscrito = new DTOAlumnoInscrito
                            {
                                OfertaEducativaId = 0,
                                OfertaEducativa = new DTOOfertaEducativa
                                {
                                    OfertaEducativaId = 0,
                                    Descripcion = ""
                                }
                            };
                        }
                    });
                    //List<DTOAlumno> lstAlumnos2 = lstAlumnos.FindAll(X => X.Paterno.Contains(Paterno));
                    //lstAlumnos = lstAlumnos2.FindAll(X => X.Materno.Contains(Materno));
                    return lstAlumnos;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}

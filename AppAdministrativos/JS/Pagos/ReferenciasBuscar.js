﻿$(document).ready(function () {
    var AlumnoId;
    var PeriodoId;
    var Anio;
    var tblReferencias;
    var PeriodoAlcorriente = null;
    var Periodo = null;
    var Tipo;

    $('#btnBuscar').click(function () {
        var lbl = $('#lblNombre');
        lbl[0].innerHTML = "";
        AlumnoId = $('#txtClave').val();
        if (AlumnoId.length == 0) { return false; }
        if (tblReferencias != undefined) {
            tblReferencias.fnClearTable();
        }
        $('#Load').modal('show');
        BuscarAlumno(AlumnoId);
        $('#sclPeriodo').empty();

        CargarPeriodosAlumno();
    });
    function BuscarAlumno(idAlumno) {
        $.ajax({
            type: "POST",
            url: "WS/Alumno.asmx/ConsultarAlumno",
            data: "{AlumnoId:'" + idAlumno +"'}",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                var lbl = $('#lblNombre');
                lbl[0].innerHTML = data.d.Nombre + " " + data.d.Paterno + " " + data.d.Materno;
                lbl[0].innerHTML += data.d.AlumnoInscrito.EsEmpresa == true ? (data.d.AlumnoInscrito.EsEspecial == true ? " - Alumno Especial  " : " - Grupo  Empresarial") + " - " + data.d.Grupo.Descripcion : "";
            }
        });
    }
    function CargarPagos() {
        var BECA;
        var Anio = $(Tipo).data("anio");
        var PeriodoId = $(Tipo).data("periodoid");

        $.ajax({
            type: "POST",
            url: "WS/Alumno.asmx/ConsultaPagosDetalle",
            data: "{AlumnoId:'" + AlumnoId + "',Anio:'" + Anio + "',PeriodoId:'" + PeriodoId + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (res) {
                var data = res.d.item1;
                var dk = res.d.item2;
                var Especial = res.d.item1[0].EsEspecial;
                if (data[0].esEmpresa) {
                    $('#tblReferencias2').hide();
                    $('#tblReferencias').hide();
                    $('#tblReferencias3').show();
                    tblReferencias = $('#tblReferencias3').dataTable({
                        "aaData": data,
                        "bSort": false,
                        "aoColumns": [
                            { "mDataProp": "Concepto" },
                            { "mDataProp": "ReferenciaId" },
                            { "mDataProp": "CargoFechaLimite" },
                            { "mDataProp": "TotalMDescuentoMBecas" },
                            { "mDataProp": "OtroDescuento" },
                            { "mDataProp": "SaldoPagado" }
                        ],
                        "columnDefs": [
                          {
                              "targets": [4],
                              "visible": dk,
                              "searchable": false
                          },
                        ],
                        "lengthMenu": [[20, 50, 100, -1], [20, 50, 100, 'Todos']],
                        "searching": false,
                        "ordering": false,
                        "async": true,
                        "bDestroy": true,
                        "bPaginate": false,
                        "bLengthChange": false,
                        "bFilter": false,
                        "bInfo": false,
                        "bAutoWidth": false,
                        "asStripClasses": null,
                        "language": {
                            "lengthMenu": "_MENU_  Registros",
                            "paginate": {
                                "previous": "<",
                                "next": ">"
                            },
                            "search": "Buscar Alumno "
                        },
                        "createdRow": function (row, data, dataIndex) {
                            row.childNodes[0].style.textAlign = 'left';
                            row.childNodes[1].style.textAlign = 'center';
                            row.childNodes[2].style.textAlign = 'center';
                            row.childNodes[3].style.textAlign = 'right';
                            row.childNodes[4].style.textAlign = 'right';
                            if (dk) {
                                row.childNodes[4].style.textAlign = 'right';
                            }
                            if (data.Pagoid == 0) {
                                row.childNodes[0].style.fontWeight = 'bold';
                                row.childNodes[0].style.fontSize = '12px';
                            } if (data.Adeudo == true) {
                                row.style.color = "#FFFFFF";
                                row.style.backgroundColor = '#e35b5a';
                            }
                        }
                    });
                  
                    var tr
                    if (dk) {
                        tr = '<tr>' +
                         '<th></th>' +
                         '<th></th>' +
                         '<th></th>' +
                         '<th></th>' +
                         '<th></th>' +
                         '<th style="text-align:right">' + data[0].TotalPagado + '</th></tr>';
                    } else {
                        tr = '<tr>' +
                            '<th></th>' +
                            '<th></th>' +
                            '<th></th>' +
                            '<th></th>' +
                            '<th style="text-align:right">' + data[0].TotalPagado + '</th></tr>';
                    }
                    //var tabla = document.getElementById("tblReferencias3");
                    document.getElementById("tblReferencias3").insertRow(-1).innerHTML = tr;
                    
                } else {
                    if (data[0].BecaSEP != null) {

                        $('#tblReferencias2').hide(); 
                        $('#tblReferencias3').hide();
                        $('#tblReferencias').show();
                      
                        tblReferencias = $('#tblReferencias').dataTable({
                            "aaData": data,
                            "bSort": false,
                            "aoColumns": [
                                { "mDataProp": "Concepto" },
                                { "mDataProp": "ReferenciaId" },
                                { "mDataProp": "CargoMonto" },
                                { "mDataProp": "CargoFechaLimite", },
                                { "mDataProp": "DescuentoXAnticipado" },
                                { "mDataProp": "Cargo_Descuento" },
                                { "mDataProp": "BecaAcademica_Pcj" },
                                { "mDataProp": "BecaAcademica_Monto" },
                                { "mDataProp": "BecaOpcional_Pcj" },
                                { "mDataProp": "BecaOpcional_Monto" },
                                { "mDataProp": "OtroDescuento" },
                                { "mDataProp": "TotalMDescuentoMBecas" },
                                { "mDataProp": "SaldoPagado" },
                            ],
                            "columnDefs": [
                        {
                            "targets": [10],
                            "visible": dk,
                            "searchable": false
                        }, ],
                            "lengthMenu": [[20, 50, 100, -1], [20, 50, 100, 'Todos']],
                            "searching": false,
                            "ordering": false,
                            "async": true,
                            "bDestroy": true,
                            "bPaginate": false,
                            "bLengthChange": false,
                            "bFilter": false,
                            "bInfo": false,
                            "bAutoWidth": false,
                            "asStripClasses": null,
                            "language": {
                                "lengthMenu": "_MENU_  Registros",
                                "paginate": {
                                    "previous": "<",
                                    "next": ">"
                                },
                                "search": "Buscar Alumno "
                            },
                            "createdRow": function (row, data, dataIndex) {
                                try {
                                    row.childNodes[0].style.textAlign = 'left';
                                    row.childNodes[1].style.textAlign = 'center';
                                    row.childNodes[2].style.textAlign = 'right';
                                    row.childNodes[3].style.textAlign = 'center';
                                    row.childNodes[4].style.textAlign = 'right';
                                    row.childNodes[5].style.textAlign = 'right';
                                    row.childNodes[6].style.textAlign = 'center';
                                    row.childNodes[7].style.textAlign = 'right';
                                    row.childNodes[8].style.textAlign = 'right';
                                    row.childNodes[9].style.textAlign = 'right';
                                    if (dk) {
                                        var child = row.childNodes[10].style.textAlign = 'right';
                                    }
                                    if (data.Pagoid == 0) {
                                        row.childNodes[0].style.fontWeight = 'bold';
                                        row.childNodes[0].style.fontSize = '12px';
                                    } if (data.Adeudo == true) {
                                        row.style.color = "#FFFFFF";
                                        row.style.backgroundColor = '#e35b5a';
                                    }
                                } catch (err) {
                                    console.log(err.message);
                                }
                            }
                        });

                        var tr;
                        if (dk) { tr = '<tr><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th style="text-align:right">' + data[0].TotalPagado + '</th></tr>'; }
                        else { tr = '<tr><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th style="text-align:right">' + data[0].TotalPagado + '</th></tr>'; }

                        document.getElementById("tblReferencias").insertRow(-1).innerHTML = tr;
                        var th = $('#thBeca');
                        th[0].innerHTML = data[0].BecaSEP;
                        var titulo = $('#tblReferencias');
                        titulo = titulo[0];
                        var DescripcipT = data[0].EsSep == 1 ? "BECA SEP" : data[0].EsSep == 2 ? "BECA Académica" : data[0].EsSep == 3 ? "BECA Comite" : 0;
                        titulo.childNodes[1].childNodes[1].childNodes[3].textContent = DescripcipT;
                        //$('#thBeca').innerHTML = data.d[0].BecaSEP;

                    } else if (data[0].BecaSEP == null) {
                        $('#tblReferencias').hide();
                        $('#tblReferencias3').hide();
                        $('#tblReferencias2').show();
                        tblReferencias = $('#tblReferencias2').dataTable({
                            "aaData": data,
                            "bSort": false,
                            "aoColumns": [
                                { "mDataProp": "Concepto" },
                                { "mDataProp": "ReferenciaId" },
                                { "mDataProp": "CargoMonto" },
                                { "mDataProp": "CargoFechaLimite", },
                                { "mDataProp": "DescuentoXAnticipado" },
                                { "mDataProp": "Cargo_Descuento" },
                                { "mDataProp": "BecaAcademica_Pcj" },
                                { "mDataProp": "BecaAcademica_Monto" },
                                { "mDataProp": "OtroDescuento" },
                                { "mDataProp": "TotalMDescuentoMBecas" },
                                { "mDataProp": "SaldoPagado" },
                            ],
                            "columnDefs": [
                         {
                             "targets": [8],
                             "visible": dk,
                             "searchable": false
                         }, ],
                            "lengthMenu": [[20, 50, 100, -1], [20, 50, 100, 'Todos']],
                            "searching": false,
                            "ordering": false,
                            "async": true,
                            "bDestroy": true,
                            "bPaginate": false,
                            "bLengthChange": false,
                            "bFilter": false,
                            "bInfo": false,
                            "bAutoWidth": false,
                            "asStripClasses": null,
                            "language": {
                                "lengthMenu": "_MENU_  Registros",
                                "paginate": {
                                    "previous": "<",
                                    "next": ">"
                                },
                                "search": "Buscar Alumno "
                            },
                            "createdRow": function (row, data, dataIndex) {
                                try {
                                    row.childNodes[0].style.textAlign = 'left';
                                    row.childNodes[1].style.textAlign = 'center';
                                    row.childNodes[2].style.textAlign = 'right';
                                    row.childNodes[3].style.textAlign = 'center';
                                    row.childNodes[4].style.textAlign = 'right';
                                    row.childNodes[5].style.textAlign = 'right';
                                    row.childNodes[6].style.textAlign = 'center';
                                    row.childNodes[7].style.textAlign = 'right';
                                    row.childNodes[8].style.textAlign = 'right';
                                    row.childNodes[9].style.textAlign = 'right';
                                    if (dk) {
                                        var child = row.childNodes[10].style.textAlign = 'right';
                                    }
                                    if (data.Pagoid == 0) {
                                        row.childNodes[0].style.fontWeight = 'bold';
                                        row.childNodes[0].style.fontSize = '12px';
                                    }
                                    if (data.Adeudo == true) {
                                        row.style.color = "#FFFFFF";
                                        row.style.backgroundColor = '#e35b5a';
                                    }
                                } catch (err) {
                                    console.log(err.message);
                                }
                            }
                        });
                   
                        var tr;
                        if (dk) {
                            tr = '<tr><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th style="text-align:right">' + data[0].TotalPagado + '</th></tr>';
                        }
                        else { tr = '</th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th></th><th style="text-align:right">' + data[0].TotalPagado + '</th></tr>'; }
                        document.getElementById("tblReferencias2").insertRow(-1).innerHTML = tr;

                        var titulo = $('#tblReferencias2');
                        titulo = titulo[0];
                        var DescripcipT1 = data[0].EsSep == 1 ? "BECA SEP" : data[0].EsSep == 2 ? "BECA Académica" : "BECA Comite";
                        titulo.childNodes[1].childNodes[1].childNodes[3].textContent = DescripcipT1;
                        $.ajax({
                            type: "POST",
                            url: "WS/General.asmx/Ofertas_costos_Alumno",
                            data: "{AlumnoId:'" + AlumnoId + "',Anio:'" + Anio + "',PeriodoId:'" + PeriodoId + "'}",
                            contentType: "application/json; charset=utf-8",
                            dataType: 'json',
                            success: function (data) {
                                if (data.d == null) { return null; }
                                var Genera = 0;
                                $(data.d).each(function () {
                                    var tabla1 = '<br />' +
                                                        '<table>' +
                                                            '<thead>' +
                                                                '<tr style="position:center">' +
                                                                    '<th>' +
                                                                        'Inscripción' +
                                                                    '</th>' +
                                                                    '<th>&emsp;&emsp;&emsp;</th>' +
                                                                    '<th>' +
                                                                        'Referencia' +
                                                                    '</th>' +
                                                                '</tr>' +
                                                                '<tr>' +
                                                                    '<th style="text-align:center" id="thInscripcion">' +
                                                                        this.MontoReins +
                                                                    '</th>' +
                                                                    '<th></th>' +
                                                                    '<th style="text-align:center" id="thRefIns">' +
                                                                        this.ReferenciaInsc +
                                                                    '</th>' +
                                                                '</tr>' +
                                                            '</thead>' +
                                                        '</table>';
                                    var tabla2 = '<br />' +
                                                        '<table>' +
                                                            '<thead>' +
                                                                '<tr style="position:center">' +
                                                                    '<th>' +
                                                                        'Colegiatura' +
                                                                    '</th>' +
                                                                    '<th>&emsp;&emsp;&emsp;</th>' +
                                                                    '<th>' +
                                                                        'Referencia' +
                                                                    '</th>' +
                                                                '</tr>' +
                                                                '<tr>' +
                                                                    '<th style="text-align:center" id="thInscripcion">' +
                                                                        this.MontoColeg +
                                                                    '</th>' +
                                                                    '<th></th>' +
                                                                    '<th style="text-align:center" id="thRefIns">' +
                                                                        this.ReferenciaColg +
                                                                    '</th>' +
                                                                '</tr>' +
                                                            '</thead>' +
                                                        '</table>';
                                    if (this.MontoReins != null) {
                                        $('#divtablas').append(tabla1);
                                        Genera = 1;
                                    }
                                    if (this.MontoColeg != null) {
                                        $('#divtablas').append(tabla2);
                                        Genera = 1;
                                    }
                                });
                                if (Genera == 1) {
                                    $('#divAnticipado').show();
                                } else { $('#divAnticipado').hide(); }
                                //AlumnoId = "";
                            }
                        });

                    }
                }
                $('#Load').modal('hide');
            }
        });
    }
    function CargarPeriodosAlumno() {
        //AlumnoId = $.cookie('user');
        var option1 = $(document.createElement('option'));
        option1.text("--Seleccionar--");
        option1.val(-1);
        $("#sclPeriodo").append(option1);
        $.ajax({
            type: "POST",
            url: "WS/Alumno.asmx/ConsultarPeriodosAlumno",
            data: "{AlumnoId:'" + AlumnoId + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                var datos = data.d;
                var sig = null, act = null;
                if (data.d.length > 0 && data.d.length == 1) {
                    $(datos).each(function () {
                        var option = $(document.createElement('option'));
                        option.text(this.Descripcion);
                        option.val(this.PeriodoId + " " + this.Anio);
                        option.attr("data-Anio", this.Anio);
                        option.attr("data-PeriodoId", this.PeriodoId);
                        $("#sclPeriodo").append(option);
                    });
                    $("#sclPeriodo").val(data.d[0].PeriodoId + " " + data.d[0].Anio);
                    $("#sclPeriodo").change();
                } else {
                    $(datos).each(function () {
                        var option = $(document.createElement('option'));
                        option.text(this.Descripcion);
                        option.val(this.PeriodoId + " " + this.Anio);
                        option.attr("data-Anio", this.Anio);
                        option.attr("data-PeriodoId", this.PeriodoId);
                        $("#sclPeriodo").append(option);
                    });
                    $('#Load').modal('hide');
                }
            }
        });
    }
    $('#txtClave').on('keydown', function (e) {
        if (e.which == 13) {
            $('#btnBuscar').click();
        }
    });
    $("#sclPeriodo").change(function () {
        $('#divAnticipado').hide();
        $('#divtablas').empty();
        $('#tblReferencias').hide();
        $('#tblReferencias2').hide();
        $('#tblReferencias3').hide();
        Tipo = $(this).find('option:selected');
        if ($("#sclPeriodo").val() == -1) {
            if (tblReferencias != undefined) {
                tblReferencias.fnClearTable();
            }
            return false;
        }
        var load = $('#Load').modal();

        $("#sclPeriodo option:selected").each(function () {
            Tipo = this;
        });
        CargarPagos();
    });

});
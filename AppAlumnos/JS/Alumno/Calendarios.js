﻿/* -*- Mode: Java; tab-width: 2; indent-tabs-mode: nil; c-basic-offset: 2 -*- */
/* vim: set shiftwidth=2 tabstop=2 autoindent cindent expandtab: */
//
// See README for overview
//
$(document).ready(function () {
    //$.cookie('user', 7633, { expires: 1 });
    CargarDocumento();
    function CargarDocumento() {
        var Alumno = $.cookie('user');
        $.ajax({
            url: 'Services/General.asmx/NombreCalendario',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: '{Alumno:"' + Alumno + '"}',
            dataType: 'json',
            success: function (data) {
                if (data.d == null) { return null; }
                CargarPDF(data.d);
            }
        });
    }
    function CargarPDF(Nombre) {
        'use strict';

        //
        // Fetch the PDF document from the URL using promises
        //
        PDFJS.getDocument(Nombre).then(function (pdf) {
            // Using promise to fetch the page
            pdf.getPage(1).then(function (page) {
                var scale = 1.5;
                var viewport = page.getViewport(scale);

                //
                // Prepare canvas using PDF page dimensions
                //
                var canvas = document.getElementById('the-canvas');
                var context = canvas.getContext('2d');
                canvas.height = viewport.height;
                canvas.width = viewport.width;

                //
                // Render PDF page into canvas context
                //
                var renderContext = {
                    canvasContext: context,
                    viewport: viewport
                };
                page.render(renderContext);
            });
        });

    }
});
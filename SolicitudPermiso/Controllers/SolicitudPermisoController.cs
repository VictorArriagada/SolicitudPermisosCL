using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SolicitudPermiso.Datos;
using SolicitudPermiso.Models;

namespace SolicitudPermiso.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudPermisoController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        private readonly PermisosCirculacionSolicitudServicio _permisosCirculacionSolicitudServicio;
        private static IConfigurationRoot config = new ConfigurationBuilder()
     .AddJsonFile("appsettings.json", optional: false, true)
     .Build();

        private readonly ILogger<SolicitudPermisoController> _log;

        public SolicitudPermisoController(IConfiguration configuration,
            PermisosCirculacionSolicitudServicio permisosCirculacionSolicitudServicio,
            ILogger<SolicitudPermisoController> log)
        {
            Configuration = configuration;
            _permisosCirculacionSolicitudServicio = permisosCirculacionSolicitudServicio;
            _log = log;
        }

        [HttpPost]
        public string Post(
            string Rut,
            string Patente,
            int pagocuota,
            string fechaVenci,
            string nroserieid,
            string dv,
            string marca,
            string modelo,
            string anio,
            string tipovehiculo,
            string nummotor,
            string color,
            string chasis,
            string numPuerta,
            string numAsiento,
            string tara,
            string codigosii,
            int tasacion,
            string cilindrada,
            string tipoCombustible,
            string transmision,
            string equipamiento,
            string nombrePropietario,
            string domicilioPripietario,
            string comunaPropietario,
            string telefonoPropietario,
            int pagototal,
            string selloverde,
            string comunaAnterior,
            string zonaFranca,
            string carga,
            int multa,
            int ipc,
            int interes,
            int totalNeto,
            int totalPagado,
            string cuota,
            string Empresa,
            string usuario
            )
        {
            var resp = new RespuestaGenericaModel();
            var res = new RespuestaSolicitudPermisoModel();

            long id_permisosCirculacion = -1;

            if (nroserieid != "")
            {
                try
                {
                    id_permisosCirculacion = long.Parse(nroserieid);
                }
                catch (Exception ex) { }
            }

            PermisoCirculacionSolicitudModel solicitud = new PermisoCirculacionSolicitudModel();

            solicitud.id_permisosCirculacion = id_permisosCirculacion;
            solicitud.tipo = 2;
            solicitud.rut = Rut;
            solicitud.patente = Patente;
            solicitud.monto_neto = pagocuota;
            solicitud.fecha_vencimiento = DateTime.Parse(fechaVenci);
            solicitud.digito_verificador = dv;
            solicitud.Marca = marca;
            solicitud.Modelo = modelo;
            solicitud.ano = anio;
            solicitud.tipo_vehiculo = tipovehiculo;
            solicitud.motor = nummotor;
            solicitud.color = color;
            solicitud.chasis = chasis;
            solicitud.n_puertas = numPuerta;
            solicitud.n_asiento = numAsiento;
            solicitud.tara = tara;
            solicitud.codigo_sii = codigosii;
            solicitud.tasacion = tasacion;
            solicitud.cilindrada = cilindrada;
            solicitud.combustible = tipoCombustible;
            solicitud.transmision = transmision;
            solicitud.equipamiento = equipamiento;
            solicitud.nombre_propietario = nombrePropietario;
            solicitud.domicilio_propietario = domicilioPripietario;
            solicitud.comuna_propietario = comunaPropietario;
            solicitud.telefono_propietario = telefonoPropietario;
            solicitud.pago_total = pagototal;
            solicitud.sello_verde = selloverde;
            solicitud.comuna_anterior = comunaAnterior;
            solicitud.zona_franca = zonaFranca;
            solicitud.carga = carga;
            solicitud.multa = multa;
            solicitud.ipc = ipc;
            solicitud.interes = interes;
            solicitud.total_neto = totalNeto;
            solicitud.total_pagado = totalPagado;
            solicitud.cuota = cuota;
            solicitud.empresa = Empresa;
            solicitud.usuario = usuario;

            var respuesta = _permisosCirculacionSolicitudServicio.Ins(solicitud);

            string[] resul = respuesta.Split('|');

            long nuevoID = long.Parse(resul[0]);
            string fechaPermiso = resul[1];

            string ipcmiles = ipc.ToString("N0");
            string interesmiles = interes.ToString("N0");
            string total_netomiles = totalNeto.ToString("N0");
            string total_pagadomiles = totalPagado.ToString("N0");
            string pago_cuotamiles = pagocuota.ToString("N0");
            string tasacionmiles = tasacion.ToString("N0");
            string multamiles = multa.ToString("N0");

            string pdfCarpetaPDF = GenerarPDF(ValidarRut(Rut), patente(Patente), pago_cuotamiles, fechaValida(fechaVenci), nuevoID.ToString(), dv, marca, modelo, anio, tipovehiculo, nummotor, color, chasis, numPuerta, numAsiento, tara, codigosii, tasacionmiles, cilindrada, tipoCombustible, transmision,
                equipamiento, nombrePropietario, domicilioPripietario, comunaPropietario, telefonoPropietario, total_pagadomiles, selloverde, comunaAnterior, zonaFranca, carga, multamiles, ipcmiles, interesmiles, total_netomiles, total_pagadomiles, cuota, fechaPermiso, usuario); ;


            return pdfCarpetaPDF;
        }
        [HttpPost("RAR")]
        public string ComprimirRAR(string ruta, string usuario)
        {
            _log.LogInformation($"Ejecutando ComprimirRAR {ruta} {usuario}");

            string pdfCarpetaPDF = "C:\\SolicitudPermisosCL\\PDF_" + usuario;
            string zipPath = "";
            try
            {

                zipPath = "Permisos_" + usuario + ".rar";
                ZipFile.CreateFromDirectory(ruta, zipPath);
                _log.LogInformation($"Ejecutando CreateFromDirectory {ruta} {zipPath}");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            if (Directory.Exists(pdfCarpetaPDF))
            {
                try
                {
                    Directory.Delete(pdfCarpetaPDF, true);
                    _log.LogInformation($"Ejecutando Delete {ruta} {zipPath}");
                }
                catch (Exception ex)
                {

                }
            }

            return zipPath;
        }

        [HttpGet("Descargar")]
        public async Task<ActionResult> Descargar(string ruta)
        {
            _log.LogInformation($"Ejecutando Descargar {ruta}");

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(ruta, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(ruta);
            return File(bytes, contentType, Path.GetFileName(ruta));

        }

        private string GenerarPDF(
            string rut, string patente, string pago_cuota, string fecha_venci, string nuevoID, string dv, string marca, string modelo, string ano, string tipo_vehiculo, string nro_motor, string color, string chasis, string nro_puertas,
            string nro_asientos, string tara, string codigo_sii, string tasacion, string cilindrada, string tipo_combustible, string transmision, string equipamiento, string nombre_propietario, string domicilio_propietario, string comuna_propietario,
            string telefono_propietario, string pago_total, string sello_verde, string comuna_anterior, string zona_franca, string carga, string multa, string ipc, string interes, string total_neto, string total_a_pagar, string cuota, string fechaPermiso, string usuario)
        {
            _log.LogInformation($"Ejecutando GenerarPDF {patente} ");

            string archivoPdfTimbre = config.GetSection("GenerarPDF").GetValue<string>("ArchivoPdfTimbre");
            string archivoPdfFondo = config.GetSection("GenerarPDF").GetValue<string>("ArchivoPdfFondo");
            string pdfCarpetaPDF = "C:\\SolicitudPermisosCL\\PDF_" + usuario;

            if (!Directory.Exists(pdfCarpetaPDF))
            {
                try
                {
                    Directory.CreateDirectory(pdfCarpetaPDF);
                }
                catch (Exception ex)
                {

                }

            }
            string archivoPDF = patente + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".PDF";
            string rutaInternaPdf = Path.Combine(pdfCarpetaPDF + "\\" + archivoPDF);

            Document doc = new Document(PageSize.LETTER, 30f, 30f, 5f, 40f);
            // Indicamos donde vamos a guardar el documento

            PdfWriter writer = null;

            DateTime Hoy = DateTime.Today;
            string fecha_actual = Hoy.ToString("yyyy");
            string fecha_actualFicha;

            fecha_actualFicha = DateTime.Parse(fecha_venci).ToString("yyyy");


            if (cuota == "TOTAL")
            {
                {
                    writer = PdfWriter.GetInstance(doc,
                                new System.IO.FileStream(rutaInternaPdf, System.IO.FileMode.Create));
                }
            }

            if (cuota == "CUOTA 1" || cuota == "CUOTA 2")
            {
                {
                    writer = PdfWriter.GetInstance(doc,
                                new System.IO.FileStream(rutaInternaPdf, System.IO.FileMode.Create));
                }
            }

            doc.AddTitle("PERMISO CIRCULACION");
            doc.AddCreator("GOSOLUTIONSCHILE");

            doc.Open();
            PdfContentByte cb = writer.DirectContent;

            // Escribimos el encabezamiento en el documento

            iTextSharp.text.Font _tituloFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 7, iTextSharp.text.Font.NORMAL, new BaseColor(82, 100, 205));
            iTextSharp.text.Font _standardFont2 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font _tituloFont2 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font _IDFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.BOLD, BaseColor.RED);
            iTextSharp.text.Font _standardFont3 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 6, iTextSharp.text.Font.NORMAL, new BaseColor(82, 100, 205));

            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("\n"));


            //-----------------------------imagen de fondo------------------------------------


            iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(archivoPdfFondo);
            imagen.BorderWidth = 0;
            imagen.Alignment = Element.ALIGN_RIGHT;
            float percentage = 150 / imagen.Width;
            imagen.ScalePercent(percentage * 400);
            //imagen.SetAbsolutePosition(25f, doc.PageSize.Height - 290f);
            imagen.SetAbsolutePosition(0f, 10f);
            doc.Add(imagen);

            // Timbre
            iTextSharp.text.Image imagentimbre = iTextSharp.text.Image.GetInstance(archivoPdfTimbre);
            imagentimbre.BorderWidth = 0;
            //imagen.Alignment = Element.ALIGN_RIGHT;
            float percentage1 = 150 / imagentimbre.Width;
            imagentimbre.ScalePercent(20);
            imagentimbre.SetAbsolutePosition(435f, doc.PageSize.Height - 290f);
            doc.Add(imagentimbre);

            //-----------------------------imagen fin-----------------------------------------


            //----------------------------------------- Codigo QR -------------------------------------

            var url = "PCL" + nuevoID;

            BarcodeQRCode Qr = new BarcodeQRCode("https://www.gosolutionschile.com/Validador.aspx?codid=" + url.ToString(), 300, 300, null);
            iTextSharp.text.Image img = Qr.GetImage();
            //img.ScaleToFit(120f, 120f);            
            img.BorderWidthTop = 1f;
            img.BorderWidthRight = 1f;
            img.BorderWidthLeft = 1f;
            img.BorderWidthBottom = 1f;
            img.SetAbsolutePosition(455f, 295f);
            img.ScaleAbsolute(128f, 128f);
            cb.AddImage(img);


            Qr = new BarcodeQRCode("https://www.gosolutionschile.com/Validador.aspx?codid=" + url.ToString(), 300, 300, null);
            img = Qr.GetImage();
            //img.ScaleToFit(120f, 120f);
            img.BorderWidthTop = 1f;
            img.BorderWidthRight = 1f;
            img.BorderWidthLeft = 1f;
            img.BorderWidthBottom = 1f;
            img.SetAbsolutePosition(455f, 95f);
            img.ScaleAbsolute(128f, 128f);
            //img.BorderColor = BaseColor.BLUE;
            cb.AddImage(img);


            //------------------------------------------posicion validar documento--------------------------------------------
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Consulte en  https://gosolutionschile.com/Validador.aspx - Código de validación: " + url, 315f, 285f, 0);
            cb.EndText();


            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Consulte en https://gosolutionschile.com/Validador.aspx - Código de validación: " + url, 315f, 85f, 0);
            cb.EndText();


            //------------------------------------------ se dibuja las lineas-------------------------------------------------

            //rectangulo permiso 1
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);

            cb.RoundRectangle(doc.LeftMargin - 5f, doc.PageSize.Height - 295f, doc.PageSize.Width - doc.LeftMargin * 2 + 10f, 155f, 3f);
            cb.Stroke();

            //rectangulo permiso 2
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);

            cb.RoundRectangle(doc.LeftMargin - 5f, doc.PageSize.Height - 500f, (doc.PageSize.Width - doc.LeftMargin * 2) / 2, 134f, 3f);
            cb.Stroke();

            //rectangulo permiso 3
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);

            cb.RoundRectangle((doc.PageSize.Width / 2) + 3f, doc.PageSize.Height - 500f, (doc.PageSize.Width - doc.LeftMargin * 2) / 2, 134f, 3f);
            cb.Stroke();

            //rectangulo permiso 4
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);

            cb.RoundRectangle(doc.LeftMargin - 5f, doc.PageSize.Height - 700f, (doc.PageSize.Width - doc.LeftMargin * 2) / 2, 134f, 3f);
            cb.Stroke();

            //rectangulo permiso 5
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);

            cb.RoundRectangle((doc.PageSize.Width / 2) + 3f, doc.PageSize.Height - 700f, (doc.PageSize.Width - doc.LeftMargin * 2) / 2, 134f, 3f);
            cb.Stroke();

            //--------------------------------- cuota total cuadrado--------------------------------------

            cb.SaveState();
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetRGBColorFill(230, 230, 230);
            cb.RoundRectangle(550f, 575f, 20f, 20f, 3f);
            cb.FillStroke();


            //cuadrado pagado 2
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetRGBColorFill(230, 230, 230);
            cb.RoundRectangle(550f, 540f, 20f, 20f, 3f);
            cb.FillStroke();


            //cuadrado pagado 3
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetRGBColorFill(230, 230, 230);
            cb.RoundRectangle(550f, 505f, 20f, 20f, 3f);
            cb.FillStroke();


            //cuadrado pagado 4
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetRGBColorFill(230, 230, 230);
            cb.RoundRectangle(316f, 295f, 20f, 20f, 3f);
            cb.FillStroke();


            //cuadrado pagado 5
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetRGBColorFill(230, 230, 230);
            cb.RoundRectangle(368f, 295f, 20f, 20f, 3f);
            cb.FillStroke();


            //cuadrado pagado 6
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetRGBColorFill(230, 230, 230);
            cb.RoundRectangle(420f, 295f, 20f, 20f, 3f);
            cb.FillStroke();



            //cuadrado pagado 7
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetRGBColorFill(230, 230, 230);
            cb.RoundRectangle(316f, 95f, 20f, 20f, 3f);
            cb.FillStroke();


            //cuadrado pagado 8
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetRGBColorFill(230, 230, 230);
            cb.RoundRectangle(368f, 95f, 20f, 20f, 3f);
            cb.FillStroke();


            //cuadrado pagado 9
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetRGBColorFill(230, 230, 230);
            cb.RoundRectangle(420f, 95f, 20f, 20f, 3f);
            cb.FillStroke();
            cb.RestoreState();


            //---------------------------------------------marca X segun pago------------------------------



            if (cuota == "TOTAL")
            {
                // 1 copia
                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 18f);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "X", 554, 578f, 0);
                cb.EndText();

                // 2 copia
                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 18f);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "X", 424f, 298f, 0);
                cb.EndText();

                // 3 copia
                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 18f);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "X", 424f, 98f, 0);
                cb.EndText();

            }

            if (cuota == "CUOTA 1")
            {
                // 1 copia
                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 18f);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "X", 554, 543f, 0);
                cb.EndText();

                //copia 2
                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 18f);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "X", 320f, 298f, 0);
                cb.EndText();


                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 18f);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "X", 320f, 98f, 0);
                cb.EndText();

            }


            if (cuota == "CUOTA 2")
            {
                // 1 copia
                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 18f);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "X", 554, 508f, 0);
                cb.EndText();

                //copia 2
                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 18f);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "X", 372f, 298f, 0);
                cb.EndText();


                cb.BeginText();
                cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 18f);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "X", 372f, 98f, 0);
                cb.EndText();

            }

            //------------------------------ se firman las lineas entre cortadas

            //1 linea 
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);

            cb.MoveTo(doc.LeftMargin, doc.PageSize.Height - 335f);
            cb.LineTo(doc.PageSize.Width - doc.LeftMargin, doc.PageSize.Height - 335f);
            cb.SetLineDash(2, 1);
            cb.Stroke();


            //2 linea 
            cb.SetLineWidth(0.5);
            cb.SetColorStroke(BaseColor.BLACK);

            cb.MoveTo(doc.LeftMargin, doc.PageSize.Height - 535f);
            cb.LineTo(doc.PageSize.Width - doc.LeftMargin, doc.PageSize.Height - 535f);
            cb.SetLineDash(2, 1);
            cb.Stroke();


            //--------------------------------------------posicion firma electronica texto-----------------------------------------------------
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 7f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Este documento contiene una firma electrónica avanzada", 70f, 285f, 0);
            cb.EndText();


            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 7f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Este documento contiene una firma electrónica avanzada", 70f, 85f, 0);
            cb.EndText();

            //--------------------------------------------dominio del vehiculo texto-----------------------------------------------------
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 7f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "No acredita dominio del vehículo", 110f, 295f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 7f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "No acredita dominio del vehículo", 110f, 95f, 0);
            cb.EndText();



            //------------------------------------------posicion absoluta pago total--------------------------------------------------
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TOTAL", 550f, 600f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CUOTA 1", 550f, 565f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CUOTA 2", 550f, 530f, 0);
            cb.EndText();

            //----------------------------------------- posicion absoluta contribuyente--------------------------------------------
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "1. CONTRIBUYENYE", 23f, 550f, 90);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "2. CONTRIBUYENYE", 23f, 330f, 90);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "3. CONTRIBUYENYE", 23f, 130f, 90);
            cb.EndText();

            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("\n"));

            //---------------------------------------------1 tabla--------------------------------------------------------------

            //TITULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "COMPROBANTE DE PAGO DE PERMISO DE CIRCULACION", 32f, 655f, 0);
            cb.EndText();

            //N° SERIE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "N° Serie", 420f, 655f, 0);
            cb.EndText();

            cb.SaveState();
            cb.SetRGBColorFill(222, 222, 222);
            cb.RoundRectangle(482f, 654f, 100f, 10f, 2f);
            cb.Fill();
            cb.RestoreState();

            cb.SaveState();
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.SetColorFill(new BaseColor(255, 0, 0));
            cb.ShowTextAligned(Element.ALIGN_LEFT, nuevoID, 530f, 655f, 0);
            cb.EndText();
            cb.RestoreState();

            //------------------------------------------------------------------------------------------------------------

            //Nombre municipalidad

            cb.SaveState();
            cb.SetRGBColorFill(222, 222, 222);
            cb.RoundRectangle(30f, 639f, 552f, 12f, 2f);
            cb.Fill();
            cb.RestoreState();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "I. Municipalidad de Calle Larga", 32f, 641f, 0);
            cb.EndText();


            //VALIDA HASTA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 7f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "FECHA", 315f, 641f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, fechaValida(fecha_venci), 345f, 641f, 0);
            cb.EndText();

            //PLACA UNICA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PLACA UNICA", 435f, 641f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 12f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, patente + "-" + dv, 485f, 641f, 0);
            cb.EndText();

            //--------------------------------------------------------------- Definimos la 2º linea

            //NOMBRE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "NOMBRE (O RAZON SOCIAL)", 32f, 629f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nombre_propietario, 125f, 629f, 0);
            cb.EndText();

            //RUT
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "RUT", 315f, 629f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, rut, 345f, 629f, 0);
            cb.EndText();


            //FONO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "FONO", 435f, 629f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, telefono_propietario, 465f, 629f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 3º linea

            //DOMICILIO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "DOMICILIO", 32f, 617f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, domicilio_propietario, 75f, 617f, 0);
            cb.EndText();


            //COMUNA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "COMUNA", 315f, 617f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, comuna_propietario, 345f, 617f, 0);
            cb.EndText();

            //FECHA EMISION
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "FECHA EMISION", 435f, 617f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, fechaValida(fechaPermiso), 490f, 617f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 4º linea
            //VEHICULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "VEHICULO", 32f, 605f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, tipo_vehiculo, 75f, 605f, 0);
            cb.EndText();

            //MARCA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MARCA", 165f, 605f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, marca, 210f, 605f, 0);
            cb.EndText();

            //MODELO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MODELO", 315f, 605f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, modelo, 345f, 605f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 5º linea
            //PST
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PTS.", 32f, 593f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nro_puertas, 52f, 593f, 0);
            cb.EndText();

            //AST
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "AST.", 80f, 593f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nro_asientos, 105f, 593f, 0);
            cb.EndText();

            //KG
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "KG.", 120f, 593f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 135f, 593f, 0);
            cb.EndText();

            //CHASIS
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CHASIS", 165f, 593f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, chasis, 210f, 593f, 0);
            cb.EndText();

            //MOTOR
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MOTOR", 315f, 593f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nro_motor, 345f, 593f, 0);
            cb.EndText();

            //AÑO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "AÑO", 435f, 593f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, ano, 470f, 593f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 6º linea

            //CODIGO S.I.I.
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CODIGO S.I.I.", 32f, 581f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, codigo_sii, 90f, 581f, 0);
            cb.EndText();

            //TASACION
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TASACION", 165f, 581f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, tasacion, 210f, 581f, 0);
            cb.EndText();


            //COLOR
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "COLOR", 315f, 581f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, color, 345f, 581f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 7º linea

            //PAGO EN CUOTAS
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PAGO EN CUOTAS", 32f, 569f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, pago_cuota, 95f, 569f, 0);
            cb.EndText();

            //PAGO TOTAL
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PAGO TOTAL", 165f, 569f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, pago_total, 210f, 569f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 8º linea

            //PERM. ANT.
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PERM. ANT.", 32f, 557f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, comuna_anterior, 70f, 557f, 0);
            cb.EndText();

            //CORRECCION MONETARIA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MULTA", 315f, 557f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, multa, 390f, 557f, 0);
            cb.EndText();
            //--------------------------------------------------------------------- Definimos la 9º linea

            //C.C.
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "C.C.", 32f, 545f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cilindrada, 58f, 545f, 0);
            cb.EndText();

            //COMB
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "COMB.", 85f, 545f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, tipo_combustible, 115f, 545f, 0);
            cb.EndText();

            //TRM
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TRM.", 140f, 545f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, transmision, 160f, 545f, 0);
            cb.EndText();

            //EQU
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "EQU.", 200f, 545f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, equipamiento, 225f, 545f, 0);
            cb.EndText();

            //I.P.C
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "I.P.C", 315f, 545f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, ipc, 390f, 545f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 10º linea

            //SELLO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "SELLO", 32f, 533f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, sello_verde, 58f, 533f, 0);
            cb.EndText();

            //INTERESES
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "INTERESES", 315f, 533f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, interes, 390f, 533f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 11º linea

            //TOTAL A PAGAR
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TOTAL NETO", 315f, 521f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, total_neto, 390f, 521f, 0);
            cb.EndText();

            //---------------------------------------------------------------------

            //TOTAL A PAGAR
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TOTAL A PAGAR", 315f, 509f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, total_a_pagar, 390f, 509f, 0);
            cb.EndText();

            //---------------------------------------------2 tabla--------------------------------------------------------------

            //TITULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PERMISO DE CIRCULACION", 32f, 430f, 0);
            cb.EndText();

            //N° SERIE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "N° Serie", 420f, 430f, 0);
            cb.EndText();

            cb.SaveState();
            cb.SetRGBColorFill(222, 222, 222);
            cb.RoundRectangle(482f, 429f, 100f, 10f, 2f);
            cb.Fill();
            cb.RestoreState();

            cb.SaveState();
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.SetColorFill(new BaseColor(255, 0, 0));
            cb.ShowTextAligned(Element.ALIGN_LEFT, nuevoID, 530f, 430f, 0);
            cb.EndText();
            cb.RestoreState();

            cb.SaveState();
            cb.SetRGBColorFill(222, 222, 222);
            cb.RoundRectangle(30f, 412f, 268f, 12f, 2f);
            cb.Fill();
            cb.RestoreState();

            //Nombre municipalidad
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "I. Municipalidad de Calle Larga", 32f, 415f, 0);
            cb.EndText();

            //AÑO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "AÑO", 260f, 415f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, fecha_actual, 280f, 415f, 0);
            cb.EndText();

            cb.SaveState();
            cb.SetRGBColorFill(222, 222, 222);
            cb.RoundRectangle(313f, 399f, 140f, 25f, 2f);
            cb.Fill();
            cb.RestoreState();

            //VALIDA HASTA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 7f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "VALIDO HASTA", 315f, 415f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, fechaValida(fecha_venci), 318f, 405f, 0);
            cb.EndText();

            //PLACA UNICA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 7f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PLACA UNICA", 395f, 415f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 12f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, patente + "-" + dv, 383f, 402f, 0);
            cb.EndText();

            //--------------------------------------------------------------- Definimos la 2º linea

            //VECHICULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "VEHICULO", 32f, 399f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, tipo_vehiculo, 68f, 399f, 0);
            cb.EndText();

            //MARCA VEHICULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MARCA", 150f, 399, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, marca, 182f, 399, 0);
            cb.EndText();


            //AÑO VEHICULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "AÑO", 260f, 399, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, ano, 280f, 399, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 3º linea

            //MODELO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MODELO", 32f, 381f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, modelo, 65f, 381f, 0);
            cb.EndText();


            //AÑO VEHICULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TOTAL PAGADO", 315f, 384f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, total_a_pagar, 390f, 384f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 4º linea
            //COLOR
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "COLOR", 32f, 363f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, color, 58f, 363f, 0);
            cb.EndText();

            //MOTOR
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MOTOR", 182f, 363f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nro_motor, 215f, 363f, 0);
            cb.EndText();

            //CODIGO SII
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CODIGO S.I.I", 315f, 365f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, codigo_sii, 390f, 365f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 5º linea
            //CARGA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CARGA", 32f, 349f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, carga, 58f, 349f, 0);
            cb.EndText();

            //AST
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "AST.", 95f, 349f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nro_asientos, 120f, 349f, 0);
            cb.EndText();

            //PST
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PTS.", 150f, 349f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nro_puertas, 170f, 349f, 0);
            cb.EndText();

            //C.C.
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "C.C.", 315f, 349f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cilindrada, 335f, 349f, 0);
            cb.EndText();

            //COMBUSTIBLE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "COMB.", 380f, 349f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, tipo_combustible, 410f, 349f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 6º linea

            //CONTRIBUYENTE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CONTRIBUYENTE", 32f, 333f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nombre_propietario, 90f, 333f, 0);
            cb.EndText();

            //TRM
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TRM.", 315f, 333f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, transmision, 335f, 333f, 0);
            cb.EndText();


            //COMBUSTIBLE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "EQU.", 380f, 333f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, equipamiento, 410f, 333f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 7º linea

            //RUT
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "RUT", 32f, 317f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, rut, 65f, 317f, 0);
            cb.EndText();

            //RUT
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "FECHA EMISION", 150f, 317f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, fechaValida(fechaPermiso), 210f, 317f, 0);
            cb.EndText();

            //CUOTA 1
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 5f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CUOTA 1", 315f, 317f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 335f, 317f, 0);
            cb.EndText();


            //CUOTA 2
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 5f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CUOTA 2", 368f, 317f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 410f, 317f, 0);
            cb.EndText();


            //TOTAL
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 5f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TOTAL", 420f, 317f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 410f, 317f, 0);
            cb.EndText();

            //---------------------------------------------3 tabla--------------------------------------------------------------


            //TITULO                
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PERMISO DE CIRCULACION", 32f, 230f, 0);
            cb.EndText();

            //N° SERIE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "N° Serie", 420f, 230f, 0);
            cb.EndText();

            cb.SaveState();
            cb.SetRGBColorFill(222, 222, 222);
            cb.RoundRectangle(482f, 229f, 100f, 10f, 2f);
            cb.Fill();
            cb.RestoreState();

            cb.SaveState();
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.SetColorFill(new BaseColor(255, 0, 0));
            cb.ShowTextAligned(Element.ALIGN_LEFT, nuevoID, 530f, 230f, 0);
            cb.EndText();
            cb.RestoreState();

            cb.SaveState();
            cb.SetRGBColorFill(222, 222, 222);
            cb.RoundRectangle(30f, 212f, 268f, 12f, 2f);
            cb.Fill();
            cb.RestoreState();

            //Nombre municipalidad
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 9f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "I. Municipalidad de Calle Larga", 32f, 215f, 0);
            cb.EndText();


            //AÑO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "AÑO", 260f, 215f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, fecha_actual, 280f, 215f, 0);
            cb.EndText();

            cb.SaveState();
            cb.SetRGBColorFill(222, 222, 222);
            cb.RoundRectangle(313f, 199f, 140f, 25f, 2f);
            cb.Fill();
            cb.RestoreState();

            //VALIDA HASTA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 7f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "VALIDO HASTA", 315f, 215f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, fechaValida(fecha_venci), 318f, 205f, 0);
            cb.EndText();

            //PLACA UNICA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 7f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PLACA UNICA", 395f, 215f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 12f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, patente + "-" + dv, 383f, 202f, 0);
            cb.EndText();

            //--------------------------------------------------------------- Definimos la 2º linea

            //VECHICULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "VEHICULO", 32f, 199f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, tipo_vehiculo, 65f, 199f, 0);
            cb.EndText();

            //MARCA VEHICULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MARCA", 150f, 199f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, marca, 182f, 199f, 0);
            cb.EndText();


            //AÑO VEHICULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "AÑO", 260f, 199f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, ano, 280f, 199f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 3º linea

            //COLOR
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MODELO", 32f, 183f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, modelo, 65f, 183f, 0);
            cb.EndText();


            //AÑO VEHICULO
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TOTAL PAGADO", 315f, 183f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, total_a_pagar, 390f, 183f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 4º linea

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "COLOR", 32f, 167f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, color, 58f, 167f, 0);
            cb.EndText();

            //MOTOR
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "MOTOR", 182f, 167f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nro_motor, 215f, 167f, 0);
            cb.EndText();

            //CODIGO SII
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CODIGO S.I.I", 315f, 167f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, codigo_sii, 390f, 167f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 5º linea

            //CARGA
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CARGA", 32f, 151f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, carga, 58f, 151f, 0);
            cb.EndText();

            //AST
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "AST.", 95f, 151f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nro_asientos, 120f, 151f, 0);
            cb.EndText();

            //PST
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "PTS.", 150f, 151f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nro_puertas, 170f, 151f, 0);
            cb.EndText();

            //C.C.
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "C.C.", 315f, 151f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cilindrada, 335f, 151f, 0);
            cb.EndText();

            //COMBUSTIBLE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "COMB.", 380f, 151f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, tipo_combustible, 410f, 151f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 6º linea

            //CONTRIBUYENTE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CONTRIBUYENTE", 32f, 135f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, nombre_propietario, 90f, 135f, 0);
            cb.EndText();

            //TRM
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TRM.", 315f, 135f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, transmision, 335f, 135f, 0);
            cb.EndText();


            //COMBUSTIBLE
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "EQU.", 380f, 135f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, equipamiento, 410f, 135f, 0);
            cb.EndText();

            //--------------------------------------------------------------------- Definimos la 7º linea

            //RUT
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "RUT", 32f, 119f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, rut, 65f, 119f, 0);
            cb.EndText();

            //RUT
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 6f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "FECHA EMISION", 150f, 119f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, fechaPermiso, 210f, 119f, 0);
            cb.EndText();

            //CUOTA 1
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 5f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CUOTA 1", 315f, 119f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 335f, 119f, 0);
            cb.EndText();


            //CUOTA 2
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 5f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "CUOTA 2", 368f, 119f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 410f, 119f, 0);
            cb.EndText();

            //TOTAL
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, false), 5f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TOTAL", 420f, 119f, 0);
            cb.EndText();

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false), 8f);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 410f, 119f, 0);
            cb.EndText();

            doc.Close();
            writer.Close();

            _log.LogInformation($"Finaliza GenerarPDF {patente} ");

            return pdfCarpetaPDF;

        }


        private string patente(string patente)
        {
            MatchCollection matches;
            Regex regex;

            string Phrase = patente;

            regex = new Regex(@"([A-Z])");
            matches = regex.Matches(Phrase);
            string LetraPatente = matches.Count.ToString();

            if (LetraPatente == "4")
            {
                patente = patente.Insert(4, ".");
            }
            else if (LetraPatente == "3")
            {
                patente = patente.Insert(3, ".");
            }
            else
            {
                patente = patente.Insert(2, ".");
            }

            return patente;
        }

        private string fechaValida(string fecha)
        {
            string fecha2 = "";
            try
            {
                fecha2 = DateTime.Parse(fecha).ToString("dd-MM-yyyy");
            }
            catch (Exception ex)
            {
            }
            return fecha2;
        }

        private string ValidarRut(string rutExcel)
        {
            string rutSinFormato = rutExcel.Replace("-", "");
            string rutSinPunto = rutSinFormato.Replace(".", "");
            string rutFormateado = String.Empty;

            //obtengo la parte numerica del RUT
            string rutTemporal = rutSinPunto.Substring(0, rutSinPunto.Length - 1);

            //obtengo el Digito Verificador del RUT
            string dv = rutSinFormato.Substring(rutSinFormato.Length - 1, 1);

            Int64 rut;

            //aqui convierto a un numero el RUT si ocurre un error lo deja en CERO
            if (!long.TryParse(rutTemporal, out rut))
            {
                rut = 0;
            }

            //este comando es el que formatea con los separadores de miles
            rutFormateado = rut.ToString("N0");

            if (rutFormateado.Equals("0"))
            {
                rutFormateado = string.Empty;
            }
            else
            {
                //si no hubo problemas con el formateo agrego el DV a la salida
                rutFormateado += "-" + dv;

                //y hago este replace por si el servidor tuviese configuracion anglosajona y reemplazo las comas por puntos
                rutFormateado = rutFormateado.Replace(".", "");
            }
            return rutFormateado;
        }

    }
}

using Certificado2.Servicios;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using Microsoft.AspNetCore.Mvc;
using Certificado2.Modelos;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using Microsoft.AspNetCore.Hosting;
using iText.Barcodes;
using iText.Kernel.Pdf.Xobject;

namespace Certificado2.Controllers
{
    public class MonedasController : Controller
    {
        private readonly IRepositorioMonedas _repositorioMonedas;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MonedasController(IRepositorioMonedas repositorioMonedas, IWebHostEnvironment webHostEnvironment)
        {
            _repositorioMonedas = repositorioMonedas;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult BuscarCertificado()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BuscarCertificadoAsync(string Serie, string Folio)
        {
            int folio1 = int.Parse(Folio);
            VMoneda datosMoneda = await _repositorioMonedas.ObtenerDatosMoneda(Serie.ToUpper(), folio1);

            if (datosMoneda == null)
            {
                // Manejar el caso donde no se encuentren los datos
                return View("NoEncontrado");
            }


            var pdfResult = await CreatePdf(Serie, folio1);

            // Retornar el PDF generado
            return pdfResult;
        }

    public async Task<FileStreamResult> CreatePdf( string Serie, int Folio)
      {
          
        VMoneda datosMoneda = (VMoneda)await _repositorioMonedas.ObtenerDatosMoneda(Serie, Folio);

        string imageName = "certificado2.png";
        string imagePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", imageName);

        byte[] bytesStream;

        using (MemoryStream ms = new MemoryStream())
        {
            PdfWriter pw = new PdfWriter(ms);
            using (PdfDocument pdfDocument = new PdfDocument(pw))
            {
                // Obtener el tamaño de la página
                PageSize pageSize = pdfDocument.GetDefaultPageSize();

                Style negrita12 = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(12);

                Style Arial12 = new Style()
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                    .SetFontSize(12);

                // Configurar los márgenes del documento a cero
                Document doc = new Document(pdfDocument, pageSize);
                doc.SetMargins(0, 0, 0, 0);

                Image formatImage = new Image(ImageDataFactory.Create(imagePath));
                formatImage.SetAutoScale(true).SetWidth(pageSize.GetWidth()).SetHeight(pageSize.GetHeight()).SetFixedPosition(0, 0);

                doc.Add(formatImage);

                float[] columnWidths = new float[] { 100, 380, 80 };
                Table table = new Table(UnitValue.CreatePointArray(columnWidths))
                    .SetBorder(Border.NO_BORDER)
                    .SetMarginTop(40);

                Cell emptyCell = new Cell().SetBorder(Border.NO_BORDER);

                Cell tituloLATERAL = new Cell().SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER);
                tituloLATERAL.Add(new Paragraph(""));

                Cell tituloCell = new Cell().SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER);
                tituloCell.Add(new Paragraph("CERTIFICADO DE AUTENTICIDAD").SetFontSize(20));

                Cell serieFolioCell = new Cell().SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER);
                serieFolioCell.Add(new Paragraph($"{Serie.ToUpper()} {Folio}").SetFontSize(16));

                table.AddCell(tituloLATERAL);
                table.AddCell(tituloCell);
                table.AddCell(serieFolioCell);

                doc.Add(table);

                float[] columnWidthscerti = new float[] { 120, 460 };
                Table certificado = new Table(UnitValue.CreatePointArray(columnWidthscerti))
                    .SetBorder(Border.NO_BORDER)
                    .SetMarginTop(40);

                certificado.AddCell(emptyCell);

                Cell reda1Cell = new Cell().SetTextAlignment(TextAlignment.JUSTIFIED).SetBorder(Border.NO_BORDER);
                reda1Cell.Add(new Paragraph($"Este certificado de autenticidad es emitido por {datosMoneda.RazonSocial} para certificar la autenticidad de la siguiente moneda:").SetFontSize(12));
                certificado.AddCell(reda1Cell);

                certificado.AddCell(emptyCell);

                float[] columnWidthsatributos = new float[] { 100, 360 };
                Table atributos = new Table(UnitValue.CreatePointArray(columnWidthsatributos))
                    .SetBorder(Border.NO_BORDER)
                    .SetMarginTop(20)
                    .SetMarginBottom(20);

                    Paragraph fecha = new Paragraph("Fecha").AddStyle(negrita12);
                    string fecha1 = datosMoneda.fecha.Day + "/" + datosMoneda.fecha.Month + "/" + datosMoneda.fecha.Year;
                    Paragraph valorfecha = new Paragraph(fecha1).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(fecha).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valorfecha).SetBorder(new SolidBorder(1)));


                    Paragraph Moneda = new Paragraph("Moneda").AddStyle(negrita12);
                Paragraph valormoneda = new Paragraph(datosMoneda.Moneda).AddStyle(negrita12);
                atributos.AddCell(new Cell().Add(Moneda).SetBorder(new SolidBorder(1)));
                atributos.AddCell(new Cell().Add(valormoneda).SetBorder(new SolidBorder(1)));

                Paragraph ano = new Paragraph("Año de Acuñación:").AddStyle(negrita12);
                Paragraph valorano = new Paragraph(datosMoneda.Ano).AddStyle(negrita12);
                atributos.AddCell(new Cell().Add(ano).SetBorder(new SolidBorder(1)));
                atributos.AddCell(new Cell().Add(valorano).SetBorder(new SolidBorder(1)));

                Paragraph ceca = new Paragraph("Ceca:").AddStyle(negrita12);
                Paragraph valorceca = new Paragraph(datosMoneda.Ceca).AddStyle(negrita12);
                atributos.AddCell(new Cell().Add(ceca).SetBorder(new SolidBorder(1)));
                atributos.AddCell(new Cell().Add(valorceca).SetBorder(new SolidBorder(1)));

                Paragraph Material = new Paragraph("Material:").AddStyle(negrita12);
                Paragraph valorMaterial = new Paragraph(datosMoneda.Material).AddStyle(negrita12);
                atributos.AddCell(new Cell().Add(Material).SetBorder(new SolidBorder(1)));
                atributos.AddCell(new Cell().Add(valorMaterial).SetBorder(new SolidBorder(1)));

                Paragraph Estado = new Paragraph("Estado de la moneda:").AddStyle(negrita12);
                Paragraph valorEstado = new Paragraph(datosMoneda.Estado).AddStyle(negrita12);
                atributos.AddCell(new Cell().Add(Estado).SetBorder(new SolidBorder(1)));
                atributos.AddCell(new Cell().Add(valorEstado).SetBorder(new SolidBorder(1)));

                certificado.AddCell(new Cell().Add(atributos).SetBorder(Border.NO_BORDER));

                certificado.AddCell(emptyCell.SetMarginTop(20));

                Cell reda1CellCUERPO = new Cell().SetTextAlignment(TextAlignment.JUSTIFIED).SetBorder(Border.NO_BORDER);
                reda1CellCUERPO.Add(new Paragraph("La moneda mencionada anteriormente ha sido autenticada por expertos numismáticos y se garantiza que es genuina. Este certificado es una prueba de la autenticidad de la moneda y puede ser utilizado como referencia en transacciones comerciales o para fines de coleccionismo.").SetFontSize(12));
                certificado.AddCell(reda1CellCUERPO);

                certificado.AddCell(emptyCell.SetMarginTop(20));

                Cell reda1CellCUERPO2 = new Cell().SetTextAlignment(TextAlignment.JUSTIFIED).SetBorder(Border.NO_BORDER);
                reda1CellCUERPO2.Add(new Paragraph("Se recomienda conservar este certificado de autenticidad junto con la moneda para garantizar su valor y autenticidad a lo largo del tiempo.").SetFontSize(12));
                certificado.AddCell(reda1CellCUERPO2);

                doc.Add(certificado);

                    // Crear la tabla con dos columnas
                    float[] columnWidthsqr = new float[] { 250, 250 };
                    Table tableWithImageAndQR = new Table(UnitValue.CreatePointArray(columnWidthsqr))
                        .SetBorder(Border.NO_BORDER)
                        .SetMarginTop(20).SetMarginLeft(110);
                    

                    // Generar el código QR
                    string verificationUrl = Url.Action("BuscarCertificadoAsync", "Moneda", new { Serie = Serie, Folio = Folio }, protocol: Request.Scheme);
                    BarcodeQRCode qrCode = new BarcodeQRCode(verificationUrl);
                    PdfFormXObject qrCodeObject = qrCode.CreateFormXObject(ColorConstants.BLACK, pdfDocument);
                    Image qrCodeImage = new Image(qrCodeObject);
                    float qrSize = 120;
                    qrCodeImage.SetWidth(qrSize).SetHeight(qrSize);

                    // Añadir la celda del código QR
                    Cell qrCell = new Cell().Add(qrCodeImage).SetBorder(Border.NO_BORDER);
                    tableWithImageAndQR.AddCell(qrCell);

                    // Crear la imagen de la moneda y ajustarla al mismo tamaño que el código QR
                    Image fotoMoneda = new Image(ImageDataFactory.Create(datosMoneda.Foto));
                    fotoMoneda.SetWidth(qrSize).SetHeight(qrSize);
                    Cell imageCell = new Cell().Add(fotoMoneda).SetBorder(Border.NO_BORDER);
                    tableWithImageAndQR.AddCell(imageCell);

                    // Añadir la tabla al documento
                    doc.Add(tableWithImageAndQR);
                    // Crear la instancia de la clase CodificadorMoneda
                    CodificadorMoneda codificador = new CodificadorMoneda();

                    // Crear la tabla de una columna
                    Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 90 }))
                        .SetWidth(UnitValue.CreatePercentValue(90))
                        .SetMarginTop(20);

                    tabla.SetMarginLeft(120);

                    Cell codificadoCell = new Cell().Add(new Paragraph("Firma:").SetBold());
                    Cell valorCodificadoCell = new Cell().Add(new Paragraph(codificador.CodificarMoneda(datosMoneda)));

                    tabla.AddCell(codificadoCell);
                    tabla.AddCell(valorCodificadoCell);

                    doc.Add(tabla);

                    doc.Close();
            }

            bytesStream = ms.ToArray();
        }

        MemoryStream finalMs = new MemoryStream(bytesStream);
        finalMs.Position = 0;

        return new FileStreamResult(finalMs, "application/pdf");
    }


}
}

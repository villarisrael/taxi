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
using System.Threading.Tasks;
using System.IO;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace Certificado2.Controllers
{
    public class ArtesaniaController : Controller
    {
        private readonly IRepositorioArtesania _repositorioArtesania;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ArtesaniaController(IRepositorioArtesania repositorioArtesania, IWebHostEnvironment webHostEnvironment)
        {
            _repositorioArtesania = repositorioArtesania;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult BuscarCertificado()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BuscarCertificadoAsync(string Serie, int Folio)
        {
            var artesania = await _repositorioArtesania.ObtenerArtesaniaPorFolio(Serie.ToUpper(), Folio);

            if (artesania == null)
            {
                return View("NoEncontrado");
            }

            var pdfResult = await CreatePdf(artesania);

            return pdfResult;
        }

        private async Task<FileStreamResult> CreatePdf(VArtesania artesania)
        {
            string imageName = "certificado2.png"; // Asegúrate de que esta imagen esté en la carpeta correcta
            string imagePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", imageName);

            byte[] bytesStream;

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter pw = new PdfWriter(ms);
                using (PdfDocument pdfDocument = new PdfDocument(pw))
                {
                    PageSize pageSize = pdfDocument.GetDefaultPageSize();

                    Style negrita12 = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                        .SetFontSize(12);

                    Style Arial12 = new Style()
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(12);

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
                    serieFolioCell.Add(new Paragraph($"{artesania.Serie.ToUpper()} {artesania.Folio}").SetFontSize(16));

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
                    reda1Cell.Add(new Paragraph($"Este certificado de autenticidad es emitido por {artesania.RazonSocial} para certificar la autenticidad de la siguiente artesanía:").SetFontSize(12));
                    certificado.AddCell(reda1Cell);

                    certificado.AddCell(emptyCell);

                    float[] columnWidthsatributos = new float[] { 100, 360 };
                    Table atributos = new Table(UnitValue.CreatePointArray(columnWidthsatributos))
                        .SetBorder(Border.NO_BORDER)
                        .SetMarginTop(20)
                        .SetMarginBottom(20);

                    Paragraph fecha = new Paragraph("Fecha de Creación").AddStyle(negrita12);
                    string fechaCreacion = artesania.FechaCreación.ToString("dd/MM/yyyy");
                    Paragraph valorfecha = new Paragraph(fechaCreacion).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(fecha).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valorfecha).SetBorder(new SolidBorder(1)));

                    Paragraph modelo = new Paragraph("Fabricante").AddStyle(negrita12);
                    Paragraph valormodelo = new Paragraph(artesania.Fabricante).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(modelo).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valormodelo).SetBorder(new SolidBorder(1)));

                    Paragraph descripcion = new Paragraph("Descripción").AddStyle(negrita12);
                    Paragraph valordescripcion = new Paragraph(artesania.Descripción).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(descripcion).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valordescripcion).SetBorder(new SolidBorder(1)));

                    Paragraph materiales = new Paragraph("Materiales").AddStyle(negrita12);
                    Paragraph valormateriales = new Paragraph(artesania.Materiales).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(materiales).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valormateriales).SetBorder(new SolidBorder(1)));

                    Paragraph dimensiones = new Paragraph("Dimensiones").AddStyle(negrita12);
                    Paragraph valordimensiones = new Paragraph(artesania.Dimensiones).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(dimensiones).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valordimensiones).SetBorder(new SolidBorder(1)));

                    Paragraph peso = new Paragraph("Peso").AddStyle(negrita12);
                    Paragraph valorpeso = new Paragraph(artesania.Peso.ToString()).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(peso).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valorpeso).SetBorder(new SolidBorder(1)));

                    certificado.AddCell(new Cell().Add(atributos).SetBorder(Border.NO_BORDER));

                    certificado.AddCell(emptyCell.SetMarginTop(20));

                    Cell reda1CellCUERPO = new Cell().SetTextAlignment(TextAlignment.JUSTIFIED).SetBorder(Border.NO_BORDER);
                    reda1CellCUERPO.Add(new Paragraph("La artesanía mencionada anteriormente ha sido autenticada y se garantiza que es genuina. Este certificado es una prueba de la autenticidad y puede ser utilizado para transacciones comerciales o fines de coleccionismo.").SetFontSize(12));
                    certificado.AddCell(reda1CellCUERPO);

                    certificado.AddCell(emptyCell.SetMarginTop(20));

                    Cell reda1CellCUERPO2 = new Cell().SetTextAlignment(TextAlignment.JUSTIFIED).SetBorder(Border.NO_BORDER);
                    reda1CellCUERPO2.Add(new Paragraph("Se recomienda conservar este certificado junto con la artesanía para garantizar su valor y autenticidad a lo largo del tiempo.").SetFontSize(12));
                    certificado.AddCell(reda1CellCUERPO2);

                    doc.Add(certificado);

                    float[] columnWidthsqr = new float[] { 250, 250 };
                    Table tableWithImageAndQR = new Table(UnitValue.CreatePointArray(columnWidthsqr))
                        .SetBorder(Border.NO_BORDER)
                        .SetMarginTop(20).SetMarginLeft(110);

                    string verificationUrl = Url.Action("BuscarCertificadoAsync", "Artesania", new { Serie = artesania.Serie, Folio = artesania.Folio }, protocol: Request.Scheme);
                    BarcodeQRCode qrCode = new BarcodeQRCode(verificationUrl);
                    PdfFormXObject qrCodeObject = qrCode.CreateFormXObject(ColorConstants.BLACK, pdfDocument);
                    Image qrCodeImage = new Image(qrCodeObject);
                    float qrSize = 120;
                    qrCodeImage.SetWidth(qrSize).SetHeight(qrSize);

                    Cell qrCell = new Cell().Add(qrCodeImage).SetBorder(Border.NO_BORDER);
                    tableWithImageAndQR.AddCell(qrCell);

                    Image fotoArtesania = new Image(ImageDataFactory.Create(artesania.Imagen));
                    fotoArtesania.SetWidth(qrSize).SetHeight(qrSize);

                    Cell fotoCell = new Cell().Add(fotoArtesania).SetBorder(Border.NO_BORDER);
                    tableWithImageAndQR.AddCell(fotoCell);

                    doc.Add(tableWithImageAndQR);

                    // Crear la instancia de la clase CodificadorMoneda
                    CodificadorArtesania codificador = new CodificadorArtesania();

                    // Crear la tabla de una columna
                    Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 90 }))
                        .SetWidth(UnitValue.CreatePercentValue(90))
                        .SetMarginTop(20);

                    tabla.SetMarginLeft(120);

                    Cell codificadoCell = new Cell().Add(new Paragraph("Firma:").SetBold());
                    Cell valorCodificadoCell = new Cell().Add(new Paragraph(codificador.CodificarArtesania(artesania)));

                    tabla.AddCell(codificadoCell);
                    tabla.AddCell(valorCodificadoCell);

                    doc.Add(tabla);


                    doc.Close();
                }

                bytesStream = ms.ToArray();
            }

            //var stream = new MemoryStream(bytesStream);
            //return new FileStreamResult(stream, "application/pdf")
            //{
            //    FileDownloadName = $"Artesania_{artesania.Serie.ToUpper()}_{artesania.Folio.ToString()}.pdf"
            //};

            var stream = new MemoryStream(bytesStream);
            Response.Headers.Add("Content-Disposition", "inline; filename=" + $"Artesania_{artesania.Serie.ToUpper()}_{artesania.Folio.ToString()}.pdf"); // Forzar visualización inline en el navegador
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}

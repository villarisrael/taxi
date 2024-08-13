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


namespace Certificado2.Controllers
{
    public class JoyeriaController : Controller
    {
        private  IRepositorioVJoyeria _repositorioJoyeria;
        private  IWebHostEnvironment _webHostEnvironment;

        public JoyeriaController(IRepositorioVJoyeria repositorioJoyeria, IWebHostEnvironment webHostEnvironment)
        {
            _repositorioJoyeria = repositorioJoyeria;
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
            VJoyeria datosJoyeria = await _repositorioJoyeria.ObtenerJoyeriaPorFolio(Serie.ToUpper(), folio1);

            if (datosJoyeria == null)
            {
                return View("NoEncontrado");
            }

            var pdfResult = await CreatePdf(Serie, folio1);

            return pdfResult;
        }

        public async Task<FileStreamResult> CreatePdf(string Serie, int Folio)
        {
            VJoyeria datosJoyeria = (VJoyeria)await _repositorioJoyeria.ObtenerJoyeriaPorFolio(Serie, Folio);

            string imageName = "certificado2.png";
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
                    reda1Cell.Add(new Paragraph($"Este certificado de autenticidad es emitido por {datosJoyeria.RazonSocial} para certificar la autenticidad de la siguiente joya:").SetFontSize(12));
                    certificado.AddCell(reda1Cell);

                    certificado.AddCell(emptyCell);

                    float[] columnWidthsatributos = new float[] { 100, 360 };
                    Table atributos = new Table(UnitValue.CreatePointArray(columnWidthsatributos))
                        .SetBorder(Border.NO_BORDER)
                        .SetMarginTop(20)
                        .SetMarginBottom(20);

                    Paragraph fecha = new Paragraph("Fecha").AddStyle(negrita12);
                    string fecha1 = datosJoyeria.fecha.Day + "/" + datosJoyeria.fecha.Month + "/" + datosJoyeria.fecha.Year;
                    Paragraph valorfecha = new Paragraph(fecha1).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(fecha).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valorfecha).SetBorder(new SolidBorder(1)));

                    Paragraph objeto = new Paragraph("Objeto").AddStyle(negrita12);
                    Paragraph valorobjeto = new Paragraph(datosJoyeria.Objeto).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(objeto).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valorobjeto).SetBorder(new SolidBorder(1)));

                    Paragraph material = new Paragraph("Material").AddStyle(negrita12);
                    Paragraph valormaterial = new Paragraph(datosJoyeria.Material).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(material).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valormaterial).SetBorder(new SolidBorder(1)));

                    Paragraph estado = new Paragraph("Estado de la joya").AddStyle(negrita12);
                    Paragraph valorestado = new Paragraph(datosJoyeria.Estado).AddStyle(negrita12);
                    atributos.AddCell(new Cell().Add(estado).SetBorder(new SolidBorder(1)));
                    atributos.AddCell(new Cell().Add(valorestado).SetBorder(new SolidBorder(1)));

                    certificado.AddCell(new Cell().Add(atributos).SetBorder(Border.NO_BORDER));

                    certificado.AddCell(emptyCell.SetMarginTop(20));

                    Cell reda1CellCUERPO = new Cell().SetTextAlignment(TextAlignment.JUSTIFIED).SetBorder(Border.NO_BORDER);
                    reda1CellCUERPO.Add(new Paragraph("La joya mencionada anteriormente ha sido autenticada y se garantiza que es genuina. Este certificado es una prueba de la autenticidad de la joya y puede ser utilizado como referencia en transacciones comerciales o para fines de coleccionismo.").SetFontSize(12));
                    certificado.AddCell(reda1CellCUERPO);

                    certificado.AddCell(emptyCell.SetMarginTop(20));

                    Cell reda1CellCUERPO2 = new Cell().SetTextAlignment(TextAlignment.JUSTIFIED).SetBorder(Border.NO_BORDER);
                    reda1CellCUERPO2.Add(new Paragraph("Se recomienda conservar este certificado de autenticidad junto con la joya para garantizar su valor y autenticidad a lo largo del tiempo.").SetFontSize(12));
                    certificado.AddCell(reda1CellCUERPO2);

                    doc.Add(certificado);

                    float[] columnWidthsqr = new float[] { 250, 250 };
                    Table tableWithImageAndQR = new Table(UnitValue.CreatePointArray(columnWidthsqr))
                        .SetBorder(Border.NO_BORDER)
                        .SetMarginTop(20).SetMarginLeft(110);

                    string verificationUrl = Url.Action("BuscarCertificadoAsync", "Joyeria", new { Serie = Serie, Folio = Folio }, protocol: Request.Scheme);
                    BarcodeQRCode qrCode = new BarcodeQRCode(verificationUrl);
                    PdfFormXObject qrCodeObject = qrCode.CreateFormXObject(ColorConstants.BLACK, pdfDocument);
                    Image qrCodeImage = new Image(qrCodeObject);
                    float qrSize = 120;
                    qrCodeImage.SetWidth(qrSize).SetHeight(qrSize);

                    Cell qrCell = new Cell().Add(qrCodeImage).SetBorder(Border.NO_BORDER);
                    tableWithImageAndQR.AddCell(qrCell);

                    Image fotoJoyeria = new Image(ImageDataFactory.Create(datosJoyeria.Foto));
                    fotoJoyeria.SetWidth(qrSize).SetHeight(qrSize);
                    Cell imageCell = new Cell().Add(fotoJoyeria).SetBorder(Border.NO_BORDER);
                    tableWithImageAndQR.AddCell(imageCell);

                    doc.Add(tableWithImageAndQR);

                    CodificadorJoyeria codificador = new CodificadorJoyeria();

                    Table tabla = new Table(UnitValue.CreatePercentArray(new float[] { 90 }))
                        .SetWidth(UnitValue.CreatePercentValue(90))
                        .SetMarginTop(20);

                    tabla.SetMarginLeft(120);

                    Cell codificadoCell = new Cell().Add(new Paragraph("Firma:").SetBold());
                    Cell valorCodificadoCell = new Cell().Add(new Paragraph(codificador.CodificarJoyeria(datosJoyeria)));

                    tabla.AddCell(codificadoCell);
                    tabla.AddCell(valorCodificadoCell);

                    doc.Add(tabla);

                    doc.Close();
                }

                bytesStream = ms.ToArray();
            }

            var stream = new MemoryStream(bytesStream);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"Joyeria_{datosJoyeria.Serie.ToUpper()}_{datosJoyeria.Folio.ToString()}.pdf"
            };
        }
    }
}

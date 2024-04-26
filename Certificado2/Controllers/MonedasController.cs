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

namespace Certificado2.Controllers
{
    public class MonedasController : Controller
    {
        private readonly IRepositorioMonedas _repositorioMonedas;

        public MonedasController(IRepositorioMonedas repositorioMonedas)
        {
            _repositorioMonedas = repositorioMonedas;
        }

        public IActionResult BuscarCertificado()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BuscarCertificado(string Serie, int Folio)
        {
            return View();
        }

        public async Task<IActionResult> GenerarPdf()
        {
            //Variable string igualado con DateTime para obtener la fecha
            string date1 = DateTime.UtcNow.ToString("dd-MM-yyyy");

            //// Se obtiene la lista completa de rutas
            //var listadoRutas = await _repositorioMonedas.ObtenerListado();

            //// Obtener los bytes del logo

            // Obtener el logotipo de la empresa
            //byte[] logotipoBytes = await _repositorioMonedas.ObtenerLogotipoEmpresaAsync();



            // Se crea un MemoryStream para almacenar el PDF generado
            MemoryStream ms = new MemoryStream();



            // Se crea un escritor PDF y un documento PDF utilizando iTextSharp

            PdfWriter pw = new PdfWriter(ms);
            PdfDocument pdfDocument = new PdfDocument(pw);
            Document doc = new Document(pdfDocument, PageSize.LETTER);
            doc.SetMargins(75, 35, 70, 35);


            doc.Add(new Paragraph("COMISION DE AGUA Y ALCANTARILLADO DEL MUNICIPIO DE ACTOPAN, HIDALGO").SetTextAlignment(TextAlignment.CENTER).SetFontSize(11));
            doc.Add(new Paragraph(date1).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(9));

            //// Obtener el nombre de la empresa desde la base de datos
            //string nombreEmpresa = await _repositorioMonedas.ObtenerNombreEmpresaAsync();



            // Agregar el nombre de la empresa al documento PDF
            doc.Add(new Paragraph("EMPRESA").SetTextAlignment(TextAlignment.CENTER).SetFontSize(10).SetBold());


            //if (logotipoBytes == null || logotipoBytes.Length == 0)
            //{
            //    // Manejar el caso en que el logotipo no se cargue correctamente
            //    return BadRequest("No se pudo cargar el logotipo de la empresa.");
            //}

            //// Convertir el logotipo a ImageData para usarlo en el PDF
            //ImageData logotipoData = ImageDataFactory.Create(logotipoBytes);

            // Crear el documento PDF y agregar el logotipo
            //Image logo = new Image(logotipoData).ScaleToFit(50, 50); // Ajusta el tamaño según sea necesario
            //doc.Add(logo);


            doc.Add(new Paragraph("\n"));
            Table table1 = new Table(1).UseAllAvailableWidth();
            Cell cell1 = new Cell().Add(new Paragraph("Reporte de Rutas").SetFontSize(9).SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER));
            table1.AddCell(cell1);
            doc.Add(table1);

            Style styleCell = new Style().SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetFontSize(9);
            //aqui se genera la tabla 
            Table _table = new Table(10).UseAllAvailableWidth();
            Cell _cell = new Cell().Add(new Paragraph("#"));
            // Se añaden los encabezados de la tabla

            _table.AddHeaderCell(_cell.AddStyle(styleCell));
            _cell = new Cell().Add(new Paragraph("IDRuta"));

            _table.AddHeaderCell(_cell.AddStyle(styleCell));
            _cell = new Cell().Add(new Paragraph("Ruta"));

            _table.AddHeaderCell(_cell.AddStyle(styleCell));
            _cell = new Cell().Add(new Paragraph("IDRegion"));

            _table.AddHeaderCell(_cell.AddStyle(styleCell));
            _cell = new Cell().Add(new Paragraph("TomaLec"));

            _table.AddHeaderCell(_cell.AddStyle(styleCell));
            _cell = new Cell().Add(new Paragraph("Capturas"));

            _table.AddHeaderCell(_cell.AddStyle(styleCell));
            _cell = new Cell().Add(new Paragraph("Calculo"));
            _table.AddHeaderCell(_cell.AddStyle(styleCell));
            _cell = new Cell().Add(new Paragraph("Aplicacion"));
            _table.AddHeaderCell(_cell.AddStyle(styleCell));
            _cell = new Cell().Add(new Paragraph("Cierre"));
            _table.AddHeaderCell(_cell.AddStyle(styleCell));
            _cell = new Cell().Add(new Paragraph("Num Usuarios"));
            _table.AddHeaderCell(_cell.AddStyle(styleCell));

            //List<Rutas> model = listadoRutas.Where(ruta => !string.IsNullOrEmpty(ruta.Id)).ToList();


            //int x = 0;
            //foreach (var item in model)
            //{
            //    x++;
            //    _cell = new Cell().Add(new Paragraph(x.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);
            //    _cell = new Cell().Add(new Paragraph(item.Id.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);
            //    _cell = new Cell().Add(new Paragraph(item.Nombre.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);
            //    _cell = new Cell().Add(new Paragraph(item.IdRegion.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);
            //    _cell = new Cell().Add(new Paragraph(item.TomaLec.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);
            //    _cell = new Cell().Add(new Paragraph(item.Captura.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);
            //    _cell = new Cell().Add(new Paragraph(item.Calculo.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);
            //    _cell = new Cell().Add(new Paragraph(item.Aplicacion.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);
            //    _cell = new Cell().Add(new Paragraph(item.Cierre.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);
            //    _cell = new Cell().Add(new Paragraph(item.NUsuarios.ToString()).SetFontSize(8));
            //    _table.AddCell(_cell);


            //}

            doc.Add(_table);

            doc.Close();


            byte[] bytesStream = ms.ToArray();
            ms = new MemoryStream();
            ms.Write(bytesStream, 0, bytesStream.Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "application/pdf");
        }
    }
}

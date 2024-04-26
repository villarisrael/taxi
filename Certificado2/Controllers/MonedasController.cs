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
        public IActionResult BuscarCertificado(string Serie, int Folio)
        {
            return View();
        }

        public async Task<IActionResult> GenerarPdf(string Serie, string Folio)
        {
            //Variable string igualado con DateTime para obtener la fecha
            string date1 = DateTime.UtcNow.ToString("dd-MM-yyyy");


            string imageName = "certificado2.png";

            // Ruta relativa de la imagen dentro de wwwroot
            string imagePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "imagenes", imageName);
            
            MemoryStream ms = new MemoryStream();



            

            PdfWriter pw = new PdfWriter(ms);
            PdfDocument pdfDocument = new PdfDocument(pw);
            Document doc = new Document(pdfDocument, PageSize.LETTER);

            
            Image formatImage = new Image(ImageDataFactory.Create(imagePath));

            
            formatImage.SetAutoScale(true);

            
            doc.Add(formatImage);


            doc.SetMargins(75, 35, 70, 35);

            Paragraph paragraph = new Paragraph($"CERTIFICADO_DE_AUTENTICIDAD.COM")
            .SetFixedPosition(130, 700, 400) // Establece la posición del texto sobre la imagen
            .SetTextAlignment(TextAlignment.LEFT) // Alinea el texto
            .SetFontSize(12) // Establece el tamaño de fuente
            .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK); // Establece el color de fuente

            doc.Add(paragraph);


            Paragraph paragraphSerie = new Paragraph($"{Serie} {Folio}")
            .SetFixedPosition(460, 700, 400) // Establece la posición del texto sobre la imagen
            .SetTextAlignment(TextAlignment.LEFT) // Alinea el texto
            .SetFontSize(12) // Establece el tamaño de fuente
            .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK); // Establece el color de fuente

            doc.Add(paragraphSerie);

            doc.Close();


            byte[] bytesStream = ms.ToArray();
            ms = new MemoryStream();
            ms.Write(bytesStream, 0, bytesStream.Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "application/pdf");
        }
    }
}

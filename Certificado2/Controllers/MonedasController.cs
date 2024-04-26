using Microsoft.AspNetCore.Mvc;

namespace Certificado2.Controllers
{
    public class MonedasController : Controller
    {
        public IActionResult BuscarCertificado()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BuscarCertificado(string Serie, int Folio )
        {
            return View();
        }


    }
}

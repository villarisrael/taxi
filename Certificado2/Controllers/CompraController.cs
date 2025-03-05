using Certificado2.Modelos;
using Certificado2.Repositorios;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Certificado2.Controllers
{
    public class CompraController : Controller
    {
        private readonly IRepositorioOrganizaciones _repositorioOrganizaciones;
        private readonly ICertificadoresFoliosRepository _repositorioFolios;
        private readonly SignInManager<UsuarioCertificados> _signInManager;
        private readonly UserManager<UsuarioCertificados> _userManager;
        private readonly IUsuarioRepository _userRepository;
       

        public CompraController(IRepositorioOrganizaciones repositorioCertificadores, ICertificadoresFoliosRepository repositoriofolios, UserManager<UsuarioCertificados> userManager, SignInManager<UsuarioCertificados> signInManager, IUsuarioRepository userRepository, IFoliosRepository foliosRepository)
        {
            _repositorioOrganizaciones = repositorioCertificadores;
            _repositorioFolios = repositoriofolios;
            _userManager = userManager;
            _signInManager = signInManager;
            _userRepository = userRepository;
            

        }

        [HttpGet]

        public async Task<IActionResult> CompraIndividual()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);




            var model = new CheckoutViewModel
            {
                Concepto = "50 Certificados - Uso trimestral",
                Costo = 50,
                cuantos = 50,
                plan = "Trimestral"
            };
            return View(model);
        }
        // GET: CompraController
        [HttpGet]

        public async Task<IActionResult> CompraMensual()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);


           

            var model = new CheckoutViewModel
            {
                Concepto = "500 Certificados - Uso dentro del mes",
                Costo = 100,
                cuantos = 500,
                plan="Mensual"
            };
            return View(model);
        }


        public async Task<IActionResult> CompraMensual300()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);




            var model = new CheckoutViewModel
            {
                Concepto = "2000 Certificados - Uso dentro del mes",
                Costo = 299,
                cuantos = 2000,
                plan = "Mensual"
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Pagar(CheckoutViewModel pago)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);

            // Lógica para procesar el pago
            // Por ejemplo, integración con Conekta u otro servicio de pago
            bool compra = await _repositorioFolios.AnadirFoliosAsync(usuario.idOrganizacion, pago.cuantos, pago.plan);
            TempData["Mensaje"] = "Pago realizado con éxito.";
            return RedirectToAction("Confirmacion", new {cuantos=pago.cuantos});
        }

        public IActionResult Confirmacion(int cuantos)
        {
            ViewData["cuantos"]= cuantos;
            return View();
        }
    }

    public class CheckoutViewModel
    {
        public string Concepto { get; set; }
        public decimal Costo { get; set; }

        public int cuantos { get; set; }

        public string plan { get;set; }
    }
}

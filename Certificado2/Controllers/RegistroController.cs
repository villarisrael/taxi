using Certificado2.Modelos;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Certificado2.Controllers
{
    public class RegistroController : Controller
    {
        private readonly IRepositorioMonedas repositorioMonedas;
        private readonly UserManager<UsuarioCertificados> _userManager;
        private readonly IFoliosRepository _repositoriofolios;

        public RegistroController(IRepositorioMonedas _repositorioMonedas, UserManager<UsuarioCertificados> userManager, IFoliosRepository repositoriofolios)
        {
            repositorioMonedas = _repositorioMonedas;
            _userManager = userManager;
            _repositoriofolios = repositoriofolios; 

        }

        [HttpGet]
      
        public async Task<IActionResult> CrearCertificadoMonedas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);


            Moneda moneda = new Moneda
            {
                fecha = DateTime.Now,
                IdCertificador = usuario.idcertificador
            };
            ViewBag.Certificador = usuario.NombreCompleto;
            return View(moneda);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCertificadoMonedas(Moneda moneda, IFormFile Foto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);

            moneda.fecha = DateTime.Now;
            moneda.idusucer = usuario.Id;
            moneda.IdCertificador = usuario.idcertificador;

            FolioSiguiente FOLIOS = await _repositoriofolios.GetFolioMonedaAsync();

            moneda.Serie = FOLIOS.Serie;
            moneda.Folio = FOLIOS.Folio;

            if (Foto != null && Foto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Foto.CopyToAsync(memoryStream);
                    moneda.Foto = memoryStream.ToArray();
                }
            }

           
                int nuevoId = await repositorioMonedas.CrearCertificado(moneda);
                if (nuevoId > 0)
                {
                    // Certificado creado exitosamente
                    return RedirectToAction("Numismatica");
                }
                else
                {
                    ModelState.AddModelError("", "Error al crear el certificado.");
                }
            

            return View(moneda);
        }

        [HttpGet]
        public IActionResult NumismaticaAdmin()
        {
            Moneda moneda = new Moneda
            {
                fecha = DateTime.Now
            };
            return View(moneda);
        }

        [HttpGet]
        public IActionResult Artesania()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ArtesaniaAdmin()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Joyeria()
        {
            return View();
        }

        [HttpGet]
        public IActionResult JoyeriaAdmin()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> IndexNumismatica(string Certificador, string Moneda, int page = 1, int pageSize = 10)
        {
            IEnumerable<VMoneda> listadoMonedas = new List<VMoneda>();

            if (Certificador == null && Moneda == null)
            {
                listadoMonedas = await repositorioMonedas.ObtenerListadoMoneda();
            }
            if (Certificador != null && Moneda == null)
            {

                listadoMonedas = await repositorioMonedas.ObtenerListadoMoneda(Certificador);
            }

            if (Certificador != null && Moneda != null)
            {

                listadoMonedas = await repositorioMonedas.ObtenerListadoMoneda(Certificador, Moneda);
            }


            var elementosPag = listadoMonedas.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadoMonedas.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.pageSize = pageSize;
            return View(elementosPag);
        }

        [HttpGet]
        public IActionResult IndexJoyeria(int id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult IndexArtesania(int id)
        {
            return View();
        }
    }
}

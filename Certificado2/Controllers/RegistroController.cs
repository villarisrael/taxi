using Certificado2.Modelos;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Certificado2.Controllers
{
    public class RegistroController : Controller
    {
        private readonly IRepositorioMonedas repositorioMonedas;
        private readonly IRepositorioJoyeria repositorioJoyeria;
        private readonly IRepositorioVJoyeria repositorioVJoyeria;
        private readonly UserManager<UsuarioCertificados> _userManager;
        private readonly IFoliosRepository _repositoriofolios;
        private readonly ICertificadoresFoliosRepository _repositorioFoliosCertificadores;
        private readonly IRepositorioCertificadores _repositorioCertificadores;
       public RegistroController(IRepositorioMonedas _repositorioMonedas, UserManager<UsuarioCertificados> userManager, IFoliosRepository repositoriofolios, ICertificadoresFoliosRepository repositorioFoliosCertificadores, IRepositorioCertificadores repositoriocer, IRepositorioVJoyeria _repositorioVJoyeria, IRepositorioJoyeria _repositorioJoyeria)
        {
            repositorioMonedas = _repositorioMonedas;
            _userManager = userManager;
            _repositoriofolios = repositoriofolios; 
            _repositorioFoliosCertificadores = repositorioFoliosCertificadores;
            _repositorioCertificadores = repositoriocer;
            repositorioJoyeria = _repositorioJoyeria;
            repositorioVJoyeria = _repositorioVJoyeria;
        }

        [HttpGet]
      
        public async Task<IActionResult> CrearCertificadoMonedas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);


            RespuestaDisponible respuestaDisponible = await _repositorioFoliosCertificadores.GetFoliosDisponiblesAsync(usuario.idcertificador);

            if (respuestaDisponible.Disponibles==0)
            {
                return RedirectToAction("SinFolios");
            }

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
                    await _repositorioFoliosCertificadores.ConsumirFoliosAsync(usuario.idcertificador); // registra el consumo del folio
                    await _repositoriofolios.ActualizaFolioMonedaAsync();
                    Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(usuario.idcertificador);
                   

                return RedirectToAction("IndexNumismaticaCertificador", new { certificador= usuario.idcertificador });
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
       public async Task<IActionResult> CrearCertificadoJoyeria()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);


            RespuestaDisponible respuestaDisponible = await _repositorioFoliosCertificadores.GetFoliosDisponiblesAsync(usuario.idcertificador);

            if (respuestaDisponible.Disponibles == 0)
            {
                return RedirectToAction("SinFolios");
            }

            Joyeria joya = new Joyeria
            {
                fecha = DateTime.Now,
                IdCertificador = usuario.idcertificador
            };
            ViewBag.Certificador = usuario.NombreCompleto;
            return View(joya);
           
        }

        [HttpPost]
        public async Task<IActionResult> CrearCertificadoJoyeria(Joyeria joyeria, IFormFile Foto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);

            joyeria.fecha = DateTime.Now;
            joyeria.idusucer = usuario.Id;
            joyeria.IdCertificador = usuario.idcertificador;

            FolioSiguiente FOLIOS = await _repositoriofolios.GetFolioJoyeriaAsync();

            joyeria.Serie = FOLIOS.Serie;
            joyeria.Folio = FOLIOS.Folio;

            if (Foto != null && Foto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Foto.CopyToAsync(memoryStream);
                    joyeria.Foto = memoryStream.ToArray();
                }
            }


            int nuevoId = await repositorioJoyeria.CrearCertificado(joyeria);
            if (nuevoId > 0)
            {
                // Certificado creado exitosamente
                await _repositorioFoliosCertificadores.ConsumirFoliosAsync(usuario.idcertificador); // registra el consumo del folio
                await _repositoriofolios.ActualizaFolioMonedaAsync();
                Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(usuario.idcertificador);


                return RedirectToAction("IndexJoyeriaCertificador", new { certificador = usuario.idcertificador });
            }
            else
            {
                ModelState.AddModelError("", "Error al crear el certificado.");
            }


            return RedirectToAction("IndexJoyeriaCertificador", new { Certificador = usuario.idcertificador });
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
        public async Task<IActionResult> IndexNumismaticaCertificador(int Certificador, int page = 1, int pageSize = 10)
        {
            IEnumerable<VMoneda> listadoMonedas = new List<VMoneda>();

            Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(Certificador);

            listadoMonedas = await repositorioMonedas.ObtenerListadoMoneda(certificadores.RazonSocial);
            


            var elementosPag = listadoMonedas.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadoMonedas.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.pageSize = pageSize;
            return View(elementosPag);
        }


        [HttpGet]
        public async Task<IActionResult> IndexJoyeriaCertificador(int Certificador, int page = 1, int pageSize = 10)
        {
            IEnumerable<VJoyeria> listadoJoyas = new List<VJoyeria>();

            Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(Certificador);

            listadoJoyas = await repositorioVJoyeria.ObtenerListadoJoyeria(Certificador);



            var elementosPag = listadoJoyas.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadoJoyas.Count();
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

        public IActionResult SinFolios()
        {
            return View();
        }
    }
}

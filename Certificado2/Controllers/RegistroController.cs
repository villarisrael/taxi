using Certificado2.Modelos;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Certificado2.Controllers
{
    public class RegistroController : Controller
    {
        private readonly IRepositorioMonedas repositorioMonedas;

        public RegistroController(IRepositorioMonedas _repositorioMonedas)
        {
            repositorioMonedas = _repositorioMonedas;
        }

        [HttpGet]
        public IActionResult Numismatica()
        {
            Moneda moneda = new Moneda
            {
                fecha = DateTime.Now,
                IdCertificador = 1
            };
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

using Certificado2.Modelos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Certificado2.Controllers
{
    public class RegistroController : Controller
    {
        [HttpGet]
        public IActionResult Numismatica()
        {
            Moneda moneda = new Moneda();
            moneda.fecha = DateTime.Now;



            moneda.IdCertificador = 1;
            return View(moneda);
            return View();
        }



        public ActionResult NumismaticaAdmin()
        {
            Moneda moneda = new Moneda();
            moneda.fecha = DateTime.Now;
            return View(moneda);
        }


        [HttpGet]
        public IActionResult Artesania()
        {
            return View();
        }

        public IActionResult ArtesaniaAdmin()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Joyeria()
        {
            return View();
        }

        public IActionResult JoyeriaAdmin()
        {
            return View();
        }

        public ActionResult Index(int id)
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View();



        }
    }
}

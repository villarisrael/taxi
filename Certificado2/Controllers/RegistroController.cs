using Certificado2.Modelos;
using Certificado2.Servicios;
using iText.Kernel.Geom;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        private readonly IRepositorioArtesania repositorioArtesania;
        private readonly IRepositorioJoyeria repositorioJoyeria;
        private readonly IRepositorioVJoyeria repositorioVJoyeria;
        private readonly IRepositorioVArtesania _repositorioVArtesania;
        private readonly UserManager<UsuarioCertificados> _userManager;
        private readonly IFoliosRepository _repositoriofolios;
        private readonly ICertificadoresFoliosRepository _repositorioFoliosCertificadores;
        private readonly IRepositorioCertificadores _repositorioCertificadores;
       public RegistroController(IRepositorioMonedas _repositorioMonedas, UserManager<UsuarioCertificados> userManager, IFoliosRepository repositoriofolios, ICertificadoresFoliosRepository repositorioFoliosCertificadores, 
           IRepositorioCertificadores repositoriocer, IRepositorioVJoyeria _repositorioVJoyeria, IRepositorioJoyeria _repositorioJoyeria,IRepositorioArtesania repositorio, IRepositorioVArtesania repositorioVArtesania)
        {
            repositorioMonedas = _repositorioMonedas;
            _userManager = userManager;
            _repositoriofolios = repositoriofolios; 
            _repositorioFoliosCertificadores = repositorioFoliosCertificadores;
            _repositorioCertificadores = repositoriocer;
            repositorioJoyeria = _repositorioJoyeria;
            repositorioVJoyeria = _repositorioVJoyeria;
            repositorioArtesania = repositorio;
            _repositorioVArtesania = repositorioVArtesania;
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
        public async Task<IActionResult> CrearCertificadoArtesania()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);


            RespuestaDisponible respuestaDisponible = await _repositorioFoliosCertificadores.GetFoliosDisponiblesAsync(usuario.idcertificador);

            if (respuestaDisponible.Disponibles == 0)
            {
                return RedirectToAction("SinFolios");
            }

            Artesania artesania = new Artesania();

            artesania.Fecha = DateTime.Now;
            artesania.IDCertificador = usuario.idcertificador;
           
            ViewBag.Certificador = usuario.NombreCompleto;
            return View(artesania);

        }

        [HttpPost]
        public async Task<IActionResult> CrearCertificadoArtesania(Artesania artesania, IFormFile Foto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);

            artesania.Fecha = DateTime.Now;
            artesania.idusucer = usuario.Id;
            artesania.IDCertificador = usuario.idcertificador;

            FolioSiguiente FOLIOS = await _repositoriofolios.GetFolioArtesaniaAsync();

            artesania.Serie = FOLIOS.Serie;
            artesania.Folio = FOLIOS.Folio;

            if (Foto != null && Foto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Foto.CopyToAsync(memoryStream);
                    artesania.Imagen = memoryStream.ToArray();
                }
            }


            int nuevoId = await repositorioArtesania.CrearCertificado(artesania);
            if (nuevoId > 0)
            {
                // Certificado creado exitosamente
                await _repositorioFoliosCertificadores.ConsumirFoliosAsync(usuario.idcertificador); // registra el consumo del folio
                await _repositoriofolios.ActualizaFolioArtesaniaAsync();
                Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(usuario.idcertificador);


                return RedirectToAction("IndexArtesaniaCertificador", new { certificador = usuario.idcertificador });
            }
            else
            {
                ModelState.AddModelError("", "Error al crear el certificado.");
            }


            return RedirectToAction("IndexArtesaniaCertificador", new { Certificador = usuario.idcertificador });
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
                await _repositoriofolios.ActualizaFolioJoyeriaAsync();
                Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(usuario.idcertificador);


                return RedirectToAction("IndexJoyeria", new { certificador = usuario.idcertificador });
            }
            else
            {
                ModelState.AddModelError("", "Error al crear el certificado.");
            }


            return RedirectToAction("IndexJoyeria", new { Certificador = usuario.idcertificador });
        }



        [HttpGet]

        public async Task<IActionResult> CrearCertificadoMonedasAdmin()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
         



            Moneda moneda = new Moneda
            {
                fecha = DateTime.Now,
               
            };
            ViewBag.IDCertificador = await _repositorioCertificadores.ObtenerListadoCertifica("");
            return View(moneda);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCertificadoMonedasAdmin(Moneda moneda, IFormFile Foto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);

            moneda.fecha = DateTime.Now;
            moneda.idusucer = usuario.Id;

            ViewBag.IDCertificador = await _repositorioCertificadores.ObtenerListadoCertifica("");

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


                return RedirectToAction("IndexNumismatica");
            }
            else
            {
                ModelState.AddModelError("", "Error al crear el certificado.");
            }


            return View(moneda);
        }

        [HttpGet]
        public async Task<IActionResult> CrearCertificadoArtesaniaAdmin()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);



            Artesania artesania = new Artesania();

            artesania.Fecha = DateTime.Now;


            ViewBag.IDCertificador = await _repositorioCertificadores.ObtenerListadoCertifica("");
            return View(artesania);

        }

        [HttpPost]
        public async Task<IActionResult> CrearCertificadoArtesaniaAdmin(Artesania artesania, IFormFile Foto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);

            artesania.Fecha = DateTime.Now;
            artesania.idusucer = usuario.Id;
            
            FolioSiguiente FOLIOS = await _repositoriofolios.GetFolioArtesaniaAsync();

            artesania.Serie = FOLIOS.Serie;
            artesania.Folio = FOLIOS.Folio;

            if (Foto != null && Foto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Foto.CopyToAsync(memoryStream);
                    artesania.Imagen = memoryStream.ToArray();
                }
            }


            int nuevoId = await repositorioArtesania.CrearCertificado(artesania);
            if (nuevoId > 0)
            {
                // Certificado creado exitosamente
                await _repositorioFoliosCertificadores.ConsumirFoliosAsync(usuario.idcertificador); // registra el consumo del folio
                await _repositoriofolios.ActualizaFolioArtesaniaAsync();
                Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(usuario.idcertificador);


                return RedirectToAction("IndexArtesania");
            }
            else
            {
                ModelState.AddModelError("", "Error al crear el certificado.");
            }


            return RedirectToAction("IndexArtesania");
        }



        [HttpGet]
        public async Task<IActionResult> CrearCertificadoJoyeriaAdmin()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);



            Joyeria joya = new Joyeria
            {
                fecha = DateTime.Now,
               
            };
            ViewBag.IDCertificador  = await _repositorioCertificadores.ObtenerListadoCertifica("");
            return View(joya);

        }

        [HttpPost]
        public async Task<IActionResult> CrearCertificadoJoyeriaAdmin(Joyeria joyeria, IFormFile Foto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener el objeto completo del usuario autenticado
            var usuario = await _userManager.FindByIdAsync(userId);

            joyeria.fecha = DateTime.Now;
            joyeria.idusucer = usuario.Id;
            

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
                await _repositoriofolios.ActualizaFolioJoyeriaAsync();
                Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(usuario.idcertificador);


                return RedirectToAction("IndexJoyeria");
            }
            else
            {
                ModelState.AddModelError("", "Error al crear el certificado.");
            }


            return RedirectToAction("IndexJoyeria");
        }



        [HttpGet]
        public async Task<IActionResult> IndexNumismatica(string Certificador, string Moneda, string ano, int page = 1, int pageSize = 10)
        {
            IEnumerable<VMoneda> listadoMonedas = new List<VMoneda>();

            ViewBag.ano = ano;
            ViewBag.Moneda = Moneda;
            ViewBag.Certificador = Certificador;

            if (Certificador == null && Moneda == null)
            {
                listadoMonedas = await repositorioMonedas.ObtenerListadoMoneda();
            }
            if (Certificador != null && Moneda == null)
            {

                listadoMonedas = await repositorioMonedas.ObtenerListadoMoneda(Certificador);
            }


            if (Certificador != null && Moneda != null && ano == null)
            {

                listadoMonedas = await repositorioMonedas.ObtenerListadoMoneda(Certificador, Moneda, "");
            }


            if (Certificador != null && Moneda != null && ano != null)
            {

                listadoMonedas = await repositorioMonedas.ObtenerListadoMoneda(Certificador, Moneda,ano);
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
        public async Task<IActionResult> IndexJoyeria(string Certificador, string Objeto,  int page = 1, int pageSize = 10)
        {
            IEnumerable<VJoyeria> listadojoyas = new List<VJoyeria>();

          
            ViewBag.Objeto = Objeto;
            ViewBag.Certificador = Certificador;

            if (Certificador == null && Objeto == null)
            {
                listadojoyas = await repositorioVJoyeria.ObtenerListadoJoyeria();
            }
            if (Certificador != null && Objeto == null)
            {

                listadojoyas = await repositorioVJoyeria.ObtenerListadoJoyeriacertificador(Certificador,"");
            }


            if (Certificador != null && Objeto != null )
            {

                listadojoyas = await repositorioVJoyeria.ObtenerListadoJoyeriacertificador(Certificador, Objeto);
            }



            var elementosPag = listadojoyas.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadojoyas.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.pageSize = pageSize;
            return View(elementosPag);
        }

        [HttpGet]
        public async Task<IActionResult> IndexArtesania(string Certificador, int page = 1, int pageSize = 10, string searchstring = "")
        {
            IEnumerable<VArtesania> listadoartesanias = new List<VArtesania>();

         

            listadoartesanias = await _repositorioVArtesania.ObtenerListadoArtesaniaxcertificador(Certificador, searchstring);

            ViewBag.Certificador = Certificador;
            ViewBag.searchstring = searchstring;

            var elementosPag = listadoartesanias.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadoartesanias.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.pageSize = pageSize;
            return View(elementosPag);
        }


        [HttpGet]
        public async Task<IActionResult> IndexNumismaticaCertificador(int Certificador, int page = 1, int pageSize = 10, string searchstring="", string ano="")
        {
            IEnumerable<VMoneda> listadoMonedas = new List<VMoneda>();

            Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(Certificador);

            listadoMonedas = await repositorioMonedas.ObtenerListadoMoneda(certificadores.RazonSocial, searchstring, ano);

            ViewBag.IDCertificador = Certificador;
            ViewBag.searchstring = searchstring;
            ViewBag.ano = ano;

            var elementosPag = listadoMonedas.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadoMonedas.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.pageSize = pageSize;
            return View(elementosPag);
        }


        [HttpGet]
        public async Task<IActionResult> IndexJoyeriaCertificador(int Certificador, int page = 1, int pageSize = 10, string searchstring="")
        {
            IEnumerable<VJoyeria> listadoJoyas = new List<VJoyeria>();


            listadoJoyas = await repositorioVJoyeria.ObtenerListadoJoyeria(Certificador, searchstring);
            ViewBag.IDCertificador = Certificador;
            ViewBag.searchstring = searchstring;


            var elementosPag = listadoJoyas.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadoJoyas.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.pageSize = pageSize;
            return View(elementosPag);
        }

        [HttpGet]
        public async Task<IActionResult> IndexArtesaniaCertificador(int Certificador, int page = 1, int pageSize = 10, string searchstring="")
        {
            IEnumerable<VArtesania> listadoartesanias = new List<VArtesania>();

            Certificadores certificadores = await _repositorioCertificadores.ObtenerDetalleAsync(Certificador);

            listadoartesanias = await _repositorioVArtesania.ObtenerListadoArtesaniaxcertificador(Certificador, searchstring);

            ViewBag.IDCertificador = Certificador;
            ViewBag.searchstring = searchstring;

            var elementosPag = listadoartesanias.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadoartesanias.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.pageSize = pageSize;
            return View(elementosPag);
        }




        public IActionResult SinFolios()
        {
            return View();
        }
    }
}

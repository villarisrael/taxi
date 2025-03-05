using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Certificado2.Modelos;
using Certificado2.Repositorios;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Certificado2.Controllers
{
    public class OrganizacionesController : Controller
    {
        private readonly IRepositorioOrganizaciones _repositorioOrganizaciones;
        private readonly IRepositorioConductor _repositorioConductores;
        private readonly ILogger<OrganizacionesController> _logger;
      
        private readonly UserManager<UsuarioCertificados> _userManager;
        public OrganizacionesController(
            IRepositorioOrganizaciones repositorioOrganizaciones,
            IRepositorioConductor repositorioConductores,
            UserManager<UsuarioCertificados> userManager,
            ILogger<OrganizacionesController> logger)
        {
            _repositorioOrganizaciones = repositorioOrganizaciones;
            _repositorioConductores = repositorioConductores;
            _logger = logger;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index(string organizacion, int page = 1, int pageSize = 20)
        {
            IEnumerable<Organizacion> organizaciones;
            if (string.IsNullOrEmpty(organizacion))
            {
                organizaciones = await _repositorioOrganizaciones.ObtenerListado();
            }
            else
            {
                organizaciones = await _repositorioOrganizaciones.ObtenerListadoCertifica(organizacion);
            }

            int count = organizaciones.Count();
            var elementosPaginados = organizaciones.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Organizacion = organizacion;
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;

            return View(elementosPaginados);
        }

        public async Task<IActionResult> ObtenerDetalle(int id)
        {
            try
            {
                var organizacion = await _repositorioOrganizaciones.ObtenerDetalleAsync(id);
                if (organizacion == null) return NotFound("Organización no encontrada.");

               
                return View(organizacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el detalle de la organización");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        public IActionResult Crear()
        {
            Organizacion orga = new Organizacion();
            
            return View(orga);
        }
        

        [HttpPost]
        public async Task<IActionResult> Crear(Organizacion organizacion, string UserName, string Password)
        {


            if (organizacion.Logo1 != null && organizacion.Logo1.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await organizacion.Logo1.CopyToAsync(memoryStream);
                organizacion.Logo = memoryStream.ToArray();


            }

            UsuarioCertificados usuarios= new UsuarioCertificados();

            
                usuarios = new UsuarioCertificados()
                {
                    UserName = UserName,
                    Email = organizacion.Email,
                    PhoneNumber = organizacion.Telefono,
                  
                };
           

            try
            {



                var resultusuario= await _userManager.CreateAsync(usuarios, Password);

                await _userManager.IsInRoleAsync(usuarios, "Organizacion");

                
                string  idusuariocreado = await _repositorioOrganizaciones.GetUserIdByUsername(usuarios.UserName);






                bool resultado = await _repositorioOrganizaciones.CrearAsync(organizacion);

                Organizacion orga = await _repositorioOrganizaciones.ObtenerDetallexrazonSocialAsync(organizacion.RazonSocial);

                await _repositorioOrganizaciones.AsignarOrganizacion(usuarios.Id.ToString(), orga.id);


                  if (!resultado)
                {
                    ModelState.AddModelError("", "No se pudo registrar la organización. Intente nuevamente.");
                    return View(organizacion);
                }




                TempData["SuccessMessage"] = "Organización registrada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la organización");
                ModelState.AddModelError("", "Ocurrió un error inesperado. Intente nuevamente.");
                return View(organizacion);
            }
        }

        public async Task<IActionResult> Modificar(int id)
        {
            var organizacion = await _repositorioOrganizaciones.ObtenerDetalleAsync(id);
            if (organizacion == null) return NotFound("Organización no encontrada.");

            ViewBag.Conductores = await _repositorioConductores.ObtenerConductoresParaDropdownAsync();
            return View(organizacion);
        }

        [HttpPost]
        public async Task<IActionResult> Modificar(Organizacion organizacion)
        {
            if (!ModelState.IsValid) return View(organizacion);

            ViewBag.Conductores = await _repositorioConductores.ObtenerConductoresParaDropdownAsync();

            try
            {
                if (organizacion.Logo1 != null && organizacion.Logo1.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await organizacion.Logo1.CopyToAsync(memoryStream);
                    organizacion.Logo = memoryStream.ToArray();
                }

                await _repositorioOrganizaciones.ModificarAsync(organizacion);
                return RedirectToAction(nameof(ObtenerDetalle), new { id = organizacion.id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar la organización");
                return StatusCode(500, "Error al modificar la organización");
            }
        }


        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            var organizacion = await _repositorioOrganizaciones.ObtenerDetalleAsync(id);
            if (organizacion == null)
            {
                return NotFound("Organización no encontrada.");
            }

            return View("ConfirmarEliminar", organizacion);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var organizacion = await _repositorioOrganizaciones.ObtenerDetalleAsync(id);
                if (organizacion == null)
                {
                    return NotFound("Organización no encontrada.");
                }

                bool resultado = await _repositorioOrganizaciones.EliminarAsync(id);

                if (resultado)
                {
                    TempData["SuccessMessage"] = "Organización eliminada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar la organización.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la organización");
                TempData["ErrorMessage"] = "Error al eliminar la organización.";
                return RedirectToAction(nameof(Index));
            }
        }



    }
}

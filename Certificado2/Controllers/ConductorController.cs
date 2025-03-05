using Certificado2.Modelos;
using Certificado2.Repositorios;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Certificado2.Controllers
{
    public class ConductorController : Controller
    {
        private readonly IRepositorioConductor _repositorioConductor;
        private readonly IRepositorioOrganizaciones _repositorioOrganizacion;
        private readonly UserManager<UsuarioCertificados> _userManager;

        public ConductorController(IRepositorioConductor repositorioConductor, UserManager<UsuarioCertificados> userManager, IRepositorioOrganizaciones repoorga )
        {
            _repositorioConductor = repositorioConductor;
            _userManager = userManager;
            _repositorioOrganizacion = repoorga;
        }


        // GET: Conductor/Index
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(User);
           

           
            var conductores = await _repositorioConductor.ObtenerConductoresvAsync( page, pageSize);
            int totalConductores =conductores.Count();

         
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalConductores / pageSize);

            return View(conductores);
        }


        public async Task<IActionResult> IndexXOrganizacion(int IDOrganzacion, int page = 1, int pageSize = 10)
        {
            

         
            var conductores = await _repositorioConductor.ObtenerConductoresPorOrganizacionAsync(IDOrganzacion, page, pageSize);
            int totalConductores = conductores.Count();

            ViewBag.IDOrganizacion = IDOrganzacion;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalConductores / pageSize);

            return View(conductores);
        }



        public async Task<IActionResult> IndexXOrganizacionuser( int page = 1, int pageSize = 10)
        {

            var user = await _userManager.GetUserAsync(User);
            int Idorganizacion = user.idOrganizacion;

            var conductores = await _repositorioConductor.ObtenerConductoresPorOrganizacionAsync(Idorganizacion, page, pageSize);
            int totalConductores = conductores.Count();

            Organizacion orga = await _repositorioOrganizacion.ObtenerDetalleAsync(Idorganizacion);

            ViewBag.IDOrganizacion = Idorganizacion;
            ViewBag.Nombredeorganizacion = orga.RazonSocial;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalConductores / pageSize);

            return View(conductores);
        }




        public async Task<IActionResult> CreateAsync()
        {

            Conductor conductor = new Conductor();
            return View(conductor);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Conductor conductor, string username, string password)
        {
            var user = await _userManager.GetUserAsync(User);

            int idOrganizacion = user.idOrganizacion;

            conductor.IDOrganizacion = idOrganizacion;


           
                bool creado = await _repositorioConductor.CrearConductorAsync(conductor, username, password);
                if (creado)
                {
                    return RedirectToAction("Index", new { idOrganizacion = conductor.IDOrganizacion });
                }
                ModelState.AddModelError("", "Error al crear el conductor.");
           
            ViewBag.IDOrganizacion = conductor.IDOrganizacion;
            return View(conductor);
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var conductor = await _repositorioConductor.ObtenerConductorPorIdAsync(id);
            if (conductor == null)
            {
                return NotFound(); // Si no se encuentra, mostrar página no encontrada
            }

            return View(conductor);  // Mostrar detalles del conductor
        }

        // GET: Conductor/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var conductor = await _repositorioConductor.ObtenerConductorPorIdAsync(id);
            if (conductor == null)
            {
                return NotFound(); // Si no se encuentra el conductor, mostrar error
            }

            return View(conductor);  // Mostrar formulario de edición con los datos del conductor
        }

        // POST: Conductor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Conductor conductor)
        {
            if (id != conductor.IDConductor)
            {
                return NotFound(); // Si no coinciden los IDs, mostrar error
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var conductorExistente = await _repositorioConductor.ObtenerConductorPorIdAsync(id);
                    if (conductorExistente == null)
                    {
                        return NotFound(); // Si no existe el conductor, mostrar error
                    }

                    await _repositorioConductor.ActualizarConductorAsync(conductor);  // Actualizar el conductor
                    return RedirectToAction(nameof(Index));  // Redirigir a la lista de conductores
                }
                catch
                {
                    // Si ocurre un error durante la actualización
                    return View(conductor); // Regresar a la vista con los datos actuales
                }
            }

            return View(conductor);  // Si el modelo no es válido, regresar al formulario
        }

        // GET: Conductor/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var conductor = await _repositorioConductor.ObtenerConductorPorIdAsync(id);
            if (conductor == null)
            {
                return NotFound(); // Si no se encuentra, mostrar error
            }

            return View(conductor); 
        }

        // POST: Conductor/Delete/5
        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repositorioConductor.EliminarConductorAsync(id);  
            return RedirectToAction(nameof(Index));  
        }

    }
}

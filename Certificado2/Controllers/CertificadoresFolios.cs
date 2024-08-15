using Certificado2.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Certificado2.Controllers
{
    public class CertificadoresFoliosController : Controller
    {
        private readonly ICertificadoresFoliosRepository _certificadoresFoliosRepository;

        public CertificadoresFoliosController(ICertificadoresFoliosRepository certificadoresFoliosRepository)
        {
            _certificadoresFoliosRepository = certificadoresFoliosRepository;
        }

        // Acción para mostrar los folios disponibles
        public async Task<IActionResult> FoliosDisponibles(int IDCertificador)
        {
            var foliosDisponibles = await _certificadoresFoliosRepository.GetFoliosDisponiblesAsync(IDCertificador);

            ViewBag.FoliosDisponibles = foliosDisponibles;
            ViewBag.IDCertificador = IDCertificador;
            return View();
        }

        // Acción para consumir folios
        [HttpPost]
        public async Task<IActionResult> FoliosDisponibles(int certificadorId, int cantidad, string plan)
        {
            if (cantidad <= 0 || string.IsNullOrEmpty(plan))
            {
                // Manejo de errores de entrada
                ModelState.AddModelError("", "Cantidad y plan deben ser válidos.");
                return View(); // Redirige o muestra la vista correspondiente
            }

            bool resultado = await _certificadoresFoliosRepository.AnadirFoliosAsync(certificadorId, cantidad, plan);

            var foliosDisponibles = await _certificadoresFoliosRepository.GetFoliosDisponiblesAsync(certificadorId);

            ViewBag.FoliosDisponibles = foliosDisponibles;

            if (resultado)
            {
                TempData["MensajeExito"] = "Folios añadidos exitosamente.";
            }
            else
            {
                TempData["MensajeError"] = "No se pudieron añadir los folios. Verifique los detalles e intente de nuevo.";
            }

            return View(); // Redirige a la vista o acción deseada
        }

        // Acción para cambiar el plan del certificador
        [HttpPost]
        public async Task<IActionResult> CambiarPlan(int certificadorId, string nuevoPlan)
        {
            var resultado = await _certificadoresFoliosRepository.CambiarPlanAsync(certificadorId, nuevoPlan);

            if (resultado)
            {
                TempData["SuccessMessage"] = "Plan cambiado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al cambiar el plan.";
            }

            return RedirectToAction("Index", new { certificadorId });
        }
    }
}

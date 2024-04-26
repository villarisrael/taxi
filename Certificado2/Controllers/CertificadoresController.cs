using Certificado2.Modelos;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Document = iText.Layout.Document;

namespace Certificado2.Controllers
{
    
    public class CertificadoresController : Controller
    {
        private readonly IRepositorioCertificadores _repositorioCertificadores;

        public CertificadoresController(IRepositorioCertificadores repositorioCertificadores)
        {
            _repositorioCertificadores = repositorioCertificadores;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int page = 1, int pageSize = 20)
        {
            var listadoFuentesAbastecimiento = await _repositorioCertificadores.ObtenerListado();
            var elementosPag = listadoFuentesAbastecimiento.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadoFuentesAbastecimiento.Count(); // Número total de elementos
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;

            return View(elementosPag);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Certificadores>> ObtenerDetalle(int id)
        {
            try
            {
                var certificador = await _repositorioCertificadores.ObtenerDetalleAsync(id);
                if (certificador == null)
                    return NotFound($"No se encontró el certificador con ID {id}");

                return Ok(certificador);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el detalle del certificador: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] Certificadores certificador)
        {
            try
            {
                await _repositorioCertificadores.CrearAsync(certificador);
                return Ok("Certificador creado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el certificador: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Modificar(int id, [FromBody] Certificadores certificador)
        {
            try
            {
                if (id != certificador.Id)
                    return BadRequest("El ID del certificador no coincide con el ID proporcionado en la solicitud.");

                await _repositorioCertificadores.ModificarAsync(certificador);
                return Ok($"Certificador con ID {id} modificado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al modificar el certificador: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            try
            {
                await _repositorioCertificadores.EliminarAsync(id);
                return Ok($"Certificador con ID {id} eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el certificador: {ex.Message}");
            }
        }
    }
}

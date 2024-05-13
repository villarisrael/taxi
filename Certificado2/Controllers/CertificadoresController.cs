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
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Certificado2.Controllers
{
    
    public class CertificadoresController : Controller
    {
        private readonly IRepositorioCertificadores _repositorioCertificadores;

        public CertificadoresController(IRepositorioCertificadores repositorioCertificadores)
        {
            _repositorioCertificadores = repositorioCertificadores;
        }

        //[HttpGet]
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

        //[HttpGet("{id}")]

        public async Task<ActionResult> ObtenerDetalle(int id)
        {
            try
            {
                var certificador = await _repositorioCertificadores.ObtenerDetalleAsync(id);
                return View(certificador);

                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el detalle del certificador: {ex.Message}");
            }
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Certificadores certificador)
        {
            string imagenBase64 = "";
            certificador.Suspendido = false;
            try
            {
                try
                {
                    if (certificador.Logo1 != null && certificador.Logo1.Length > 0)
                    {
                        // Aquí puedes manipular los bytes de la imagen
                        // Por ejemplo, guardarlos en una base de datos, procesarlos, etc.

                        // Guardar los bytes de la imagen en una variable temporal
                        using (var memoryStream = new MemoryStream())
                        {
                            certificador.Logo1.CopyTo(memoryStream);
                            byte[] bytesImagen = memoryStream.ToArray();
                            // Ahora tienes los bytes de la imagen en 'bytesImagen'
                            certificador.Logo = bytesImagen;
                            // Puedes guardarlos en una base de datos, procesarlos, etc.
                        }

                    }
                }
                catch (Exception e)
                {

                }
                await _repositorioCertificadores.CrearAsync(certificador);
                // Redirige a la acción Index
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el certificador: {ex.Message}");
            }
        }
        public async Task<ActionResult> Modificar(int id)
        {
            var certificador =  await _repositorioCertificadores.ObtenerDetalleAsync(id);
            return View(certificador);
        }


        [HttpPost]
        public async Task<ActionResult> Modificar(Certificadores certificador)
        {
            try
            {
                if (certificador.Logo1 != null)
                {
                    try
                    {
                        if (certificador.Logo1 != null && certificador.Logo1.Length > 0)
                        {
                            // Aquí puedes manipular los bytes de la imagen
                            // Por ejemplo, guardarlos en una base de datos, procesarlos, etc.

                            // Guardar los bytes de la imagen en una variable temporal
                            using (var memoryStream = new MemoryStream())
                            {
                                certificador.Logo1.CopyTo(memoryStream);
                                byte[] bytesImagen = memoryStream.ToArray();
                                // Ahora tienes los bytes de la imagen en 'bytesImagen'
                                certificador.Logo = bytesImagen;
                                // Puedes guardarlos en una base de datos, procesarlos, etc.
                            }

                        }
                    }
                    catch (Exception e)
                    {

                    }
                    await _repositorioCertificadores.ModificarAsync(certificador);
                }
                else
                {
                    await _repositorioCertificadores.ModificarCLogoAsync(certificador);
                }

                
              
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al modificar el certificador: {ex.Message}");
            }
            return RedirectToAction("ObtenerDetalle", new {id= certificador.Id});
        }

        [HttpPost]
        public async Task<ActionResult> Suspender(int id)
        {
            try
            {
               
                    await _repositorioCertificadores.SuspenderCertificado(id);
                



            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al modificar el certificador: {ex.Message}");
            }
            return RedirectToAction("ObtenerDetalle", new { id = id });
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

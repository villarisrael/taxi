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


using Document = iText.Layout.Document;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

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
        public async Task<IActionResult> Index( string Certificador, int page = 1, int pageSize = 20)
        {
            IEnumerable<Certificadores> listadoFuentesAbastecimiento = new List<Certificadores>(); ;

            if (Certificador == string.Empty)
            {
                listadoFuentesAbastecimiento = await _repositorioCertificadores.ObtenerListado();
            }
            else
            {
                listadoFuentesAbastecimiento = await _repositorioCertificadores.ObtenerListadoCertifica(Certificador);
            }
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
               
                    await _repositorioCertificadores.SuspenderCertificador(id);
                



            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al modificar el certificador: {ex.Message}");
            }
            return RedirectToAction("ObtenerDetalle", new { id = id });
        }

        [HttpPost]
        public async Task<ActionResult> Activar(int id)
        {
            try
            {

                await _repositorioCertificadores.ActivarCertificador(id);




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


        //[HttpGet]
        public async Task<IActionResult> ListaUsuarios(int Certificador=0,string Usuario="", int page = 1, int pageSize = 20)
        {
            IEnumerable<UsuCertificadores> listadoUsuarios = new List<UsuCertificadores>(); 
            ViewBag.IDCertificador = Certificador;

            listadoUsuarios = await _repositorioCertificadores.ObtenerListadoUsuarios(Certificador, Usuario);
            if (Usuario!="")
            {
                listadoUsuarios = await _repositorioCertificadores.ObtenerListadoUsuarios(Certificador, Usuario);

            }
            var elementosPag = listadoUsuarios.Skip((page - 1) * pageSize).Take(pageSize);

            // Paginación
            int count = listadoUsuarios.Count(); // Número total de elementos
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / pageSize);
            ViewBag.CurrentPage = page;

            return View(elementosPag);
        }

        public IActionResult AgregarUsuario(UsuCertificadores usuarios, int IDCertificador, string mensaje = "")
        {
            ViewBag.Mensaje = mensaje;
        
            usuarios.IdCentificador = IDCertificador;
            return View(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarUsuario(UsuCertificadores usuarioscer, string mensaje = "", string Password1="")
        {
            ViewBag.Mensaje = mensaje;

            IEnumerable<UsuCertificadores>  listadoUsuarios = await _repositorioCertificadores.ObtenerListadoUsuarios((int)usuarioscer.IdCentificador, "");

            
            try
            {
               
                await _repositorioCertificadores.AgregarUsuario(usuarioscer);
                // Redirige a la acción Index
                return RedirectToAction("ListaUsuarios" , new { Certificador = usuarioscer.IdCentificador});

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al agregar usuario: {ex.Message}");
            }
        }

        public async Task<IActionResult> GetJsonUsuarios(string nombre, string username, int IDCertificador)
        {
            IEnumerable<UsuCertificadores> listadoUsuarios = await _repositorioCertificadores.ObtenerListadoUsuarios(IDCertificador, "");
            string mensaje = "";
            try
            {
                bool usuariosU = listadoUsuarios.Any(u => u.Usuario == username);
                bool usuariosN = listadoUsuarios.Any(u => u.Nombre == nombre);

                if (usuariosN)
                {
                    mensaje = "Ya existe un registro con este Nombre, intente nuevamente.";
                }
                if (usuariosU)
                {
                    if (!string.IsNullOrEmpty(mensaje))
                    {
                        mensaje += "\n";
                    }
                    mensaje += "Ya existe un registro con este Username, intente nuevamente.";
                }
            }
            catch (Exception e)
            {
                // Manejar la excepción de manera adecuada, quizás loguearla
                mensaje = "Ocurrió un error al procesar la solicitud.";
            }

            return Json(new { mensaje });
        }
        public async Task<IActionResult> GetJsonUsuariosE(int id,string nombre, string username, int IDCertificador)
        {
            IEnumerable<UsuCertificadores> listadoUsuarios = await _repositorioCertificadores.ObtenerListadoUsuarios(IDCertificador, "");
            string mensaje = "";
            try
            {
                bool usuariosU = listadoUsuarios.Any(u => u.Usuario == username && u.Id != id);
                bool usuariosN = listadoUsuarios.Any(u => u.Nombre == nombre && u.Id != id);
              
                if (usuariosN)
                {
                    mensaje = "Ya existe un registro con este Nombre, intente nuevamente.";
                }
                if (usuariosU)
                {
                    if (!string.IsNullOrEmpty(mensaje))
                    {
                        mensaje += "\n";
                    }
                    mensaje += "Ya existe un registro con este Username, intente nuevamente.";
                }
            }
            catch (Exception e)
            {
                // Manejar la excepción de manera adecuada, quizás loguearla
                mensaje = "Ocurrió un error al procesar la solicitud.";
            }

            return Json(new { mensaje });
        }

        public async Task<ActionResult> ModificarUsuario(int id, string mensaje="")
        {
            ViewBag.Mensaje = mensaje;

            var certificador = await _repositorioCertificadores.ObtenerDetalleUsuario(id);
            return View(certificador);
        }


        [HttpPost]
        public async Task<ActionResult> ModificarUsuario(UsuCertificadores certificador, string mensaje = "", string Password1 = "")
        {
           
            try
            {
                
                    await _repositorioCertificadores.ModificarUsuario(certificador);
               

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al modificar el certificador: {ex.Message}");
            }
            return RedirectToAction("DetalleUsuario", new { id = certificador.Id });
        }

        public async Task<ActionResult> DetalleUsuario(int id)
        {
            try
            {
                var certificador = await _repositorioCertificadores.ObtenerDetalleUsuario(id);
                return View(certificador);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el detalle del usuario: {ex.Message}");
            }
        }
    }
}

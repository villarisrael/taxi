using Certificado2.Modelos;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Mvc;


using Document = iText.Layout.Document;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Certificado2.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Certificado2.Controllers
{
    
    public class CertificadoresController : Controller
    {
        private readonly IRepositorioCertificadores _repositorioCertificadores;
        private readonly IRepositorioVendedor _repositorioVendedores;
      

        public CertificadoresController(IRepositorioCertificadores repositorioCertificadores, IRepositorioVendedor repositorioVendedor)
        {
            _repositorioCertificadores = repositorioCertificadores;
            _repositorioVendedores = repositorioVendedor;


        }


        //[HttpGet]
        public async Task<IActionResult> Index(string Certificador , int page = 1, int pageSize = 20)
        {
            IEnumerable<Certificadores> certificadores = new List<Certificadores>(); 

            if (Certificador == string.Empty)
            {
                certificadores = await _repositorioCertificadores.ObtenerListado();
            }
            else
            {
                certificadores = await _repositorioCertificadores.ObtenerListadoCertifica(Certificador);
            }
           
            // Paginación
            int count = certificadores.Count();
            var elementosPag = certificadores.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.Certificador = Certificador;
          
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
                Vendedor vendedor = await _repositorioVendedores.GetVendedorByIdAsync(certificador.IDVendedor);
                ViewBag.Vendedor = vendedor;
                return View(certificador);

                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el detalle del certificador: {ex.Message}");
            }
        }

        public async Task<IActionResult> Crear()
        {
            var vendedores = await _repositorioVendedores.GetVendedoresForDropdownAsync();

            // Asignar la lista al ViewBag como SelectList
            ViewBag.IDVendedor = new SelectList(vendedores, "IDVendedor", "Nombre");
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
            var vendedores = await _repositorioVendedores.GetVendedoresForDropdownAsync();

            // Asignar la lista al ViewBag como SelectList
            ViewBag.IDVendedor = new SelectList(vendedores, "IDVendedor", "Nombre", certificador.IDVendedor);

            return View(certificador);
        }


        [HttpPost]
        public async Task<ActionResult> Modificar(Certificadores certificador)
        {
            var vendedores = await _repositorioVendedores.GetVendedoresForDropdownAsync();

            // Asignar la lista al ViewBag como SelectList
            ViewBag.IDVendedor = new SelectList(vendedores, "IDVendedor", "Nombre", certificador.IDVendedor);
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
            IEnumerable<UsuarioCertificados> listadoUsuarios = new List<UsuarioCertificados>(); 
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


        public async Task<ActionResult> DetalleUsuario(string id)
        {
            try
            {
                var usuariodecertificador = await _repositorioCertificadores.ObtenerDetalleUsuario(id);

                var Certificador = await  _repositorioCertificadores.ObtenerDetalleAsync(usuariodecertificador.idcertificador);
                ViewBag.Certificador= Certificador.RazonSocial;
                return View(usuariodecertificador);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el detalle del usuario: {ex.Message}");
            }
        }

        public async Task<ActionResult> ModificarUsuario(string id)
        {
            try
            {
                var usuariodecertificador = await _repositorioCertificadores.ObtenerDetalleUsuario(id);

                var Certificador = await _repositorioCertificadores.ObtenerDetalleAsync(usuariodecertificador.idcertificador);
                ViewBag.Certificador = Certificador.RazonSocial;
                return View(usuariodecertificador);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el detalle del usuario: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ModificarUsuario(UsuarioCertificados model)
        {
            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, volver a cargar el certificador y regresar a la vista con los errores de validación
                try
                {
                    var Certificador = await _repositorioCertificadores.ObtenerDetalleAsync(model.idcertificador);
                    ViewBag.Certificador = Certificador.RazonSocial;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error al obtener el detalle del certificador: {ex.Message}");
                }

                return View(model);
            }

            try
            {
                // Obtener el usuario actual desde la base de datos
                var usuario = await _repositorioCertificadores.ObtenerDetalleUsuario(model.Id);

                if (usuario == null)
                {
                    return NotFound("Usuario no encontrado");
                }

                // Actualizar las propiedades del usuario
                usuario.Email = model.Email;
                usuario.PhoneNumber = model.PhoneNumber;
                usuario.NombreCompleto = model.NombreCompleto;
                usuario.idcertificador = model.idcertificador;
                usuario.EmailConfirmed = model.EmailConfirmed;
                usuario.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
                usuario.LockoutEnabled = model.LockoutEnabled;

                // Guardar los cambios en el repositorio o base de datos
                var result = await _repositorioCertificadores.ActualizarUsuario(usuario);

                if (result)
                {
                    return RedirectToAction("DetalleUsuario", new { id = model.Id });
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar el usuario.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al modificar el usuario: {ex.Message}");
            }

            // Si llega aquí, algo salió mal, volver a cargar la vista con el modelo actual
            try
            {
                var Certificador = await _repositorioCertificadores.ObtenerDetalleAsync(model.idcertificador);
                ViewBag.Certificador = Certificador.RazonSocial;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el detalle del certificador: {ex.Message}");
            }

            return View(model);
        }

    }
}

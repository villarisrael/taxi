using Microsoft.AspNetCore.Identity;
using Certificado2.Modelos;
using Microsoft.AspNetCore.Identity;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Certificado2.Repositorios
{
    public interface IRepositorioConductor
    {
        Task<bool> CrearConductorAsync(Conductor conductor, string user, string pass);
        Task<List<Conductor>> ObtenerListadoConductoresAsync();
        Task<Conductor> ObtenerConductorPorIdAsync(int id);
        Task ActualizarConductorAsync(Conductor conductor);
        Task<List<Conductor>> ObtenerConductoresPorOrganizacionAsync(int idOrganizacion, int page, int pageSize);

        Task<List<VConductor>> ObtenerConductoresvAsync(int page, int pageSize);
        Task EliminarConductorAsync(int id);
        Task<IEnumerable<Conductor>> ObtenerConductoresParaDropdownAsync();
        Task<int> ContarConductoresPorOrganizacionAsync(int idOrganizacion);
    }

    public class RepositorioConductor : IRepositorioConductor
    {
        private readonly string connectionString;
        private UserManager<UsuarioCertificados> _usermanager;
        public RepositorioConductor(IConfiguration configuration, UserManager<UsuarioCertificados> userManager)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
            _usermanager = userManager;
        }

        // Create
        public async Task<bool> CrearConductorAsync(Conductor conductor, string username , string pass)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();



                    var user = await _usermanager.FindByEmailAsync(conductor.mail);

                    if (user == null)
                    {
                        user = new UsuarioCertificados()
                        {
                            UserName = username,
                            Email = conductor.mail,
                            NombreCompleto = conductor.Nombre,
                            idOrganizacion = conductor.IDOrganizacion

                        };
                        await _usermanager.CreateAsync(user, pass);

                    }
                    else
                    {
                        return false;
                    }
                    await _usermanager.AddToRoleAsync(user, "Conductor");


                    var userregistrado = await _usermanager.FindByEmailAsync(conductor.mail);

                    if (userregistrado == null)
                    {
                        Console.WriteLine("Advertencia: userregistrado es NULL. Se usará un valor por defecto o NULL.");
                    }


                    var userId = userregistrado?.Id ?? (object)DBNull.Value;

                    string insertQuery = @"
                        INSERT INTO conductor (Nombre, Telefono, Direccion, Colonia, Ciudad, IDEstado, IDPais, 
                        mail, Licencia, Placa, INE, Activo, IDOrganizacion, Independiente, IdUser)
                        VALUES (@Nombre, @Telefono, @Direccion, @Colonia, @Ciudad, @IDEstado, @IDPais, 
                        @mail, @Licencia, @Placa, @INE, @Activo, @IDOrganizacion, @Independiente, @Iduser)";

                    using (var insertCommand = new MySqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Nombre", conductor.Nombre);
                        insertCommand.Parameters.AddWithValue("@Telefono", conductor.Telefono);
                        insertCommand.Parameters.AddWithValue("@Direccion", conductor.Direccion);
                        insertCommand.Parameters.AddWithValue("@Colonia", conductor.Colonia);
                        insertCommand.Parameters.AddWithValue("@Ciudad", conductor.Ciudad);
                        insertCommand.Parameters.AddWithValue("@IDEstado", conductor.IDEstado);
                        insertCommand.Parameters.AddWithValue("@IDPais", conductor.IDPais);
                        insertCommand.Parameters.AddWithValue("@mail", conductor.mail);
                        insertCommand.Parameters.AddWithValue("@Licencia", conductor.Licencia);
                        insertCommand.Parameters.AddWithValue("@Placa", conductor.Placa);
                        insertCommand.Parameters.AddWithValue("@INE", conductor.INE);
                        insertCommand.Parameters.AddWithValue("@Activo", conductor.Activo);
                        insertCommand.Parameters.AddWithValue("@IDOrganizacion", conductor.IDOrganizacion);
                        insertCommand.Parameters.AddWithValue("@Independiente", conductor.Independiente);
                        insertCommand.Parameters.AddWithValue("@Iduser", userregistrado.Id);


                        await insertCommand.ExecuteNonQueryAsync();
                        return true;
                    }




                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar el conductor: {ex.Message}");
                return false;
            }
        }

        // Read (Get All)
        public async Task<List<Conductor>> ObtenerListadoConductoresAsync()
        {
            List<Conductor> listado = new List<Conductor>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "SELECT * FROM conductor ORDER BY Nombre";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Conductor conductor = new Conductor
                                {
                                    IDConductor = (int)reader["IDConductor"],
                                    Nombre = reader["Nombre"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    Direccion = reader["Direccion"] as string,
                                    Colonia = reader["Colonia"] as string,
                                    Ciudad = reader["Ciudad"] as string,
                                    IDEstado = reader["IDEstado"] as string ,
                                    IDPais = reader["IDPais"] as string,
                                    mail = reader["mail"] as string,
                                    Licencia = reader["Licencia"] as string,
                                    Placa = reader["Placa"] as string,
                                    INE = reader["INE"] as string,
                                    Activo = (int)reader["Activo"],
                                    IDOrganizacion = (int)reader["IDOrganizacion"],
                                    Independiente = reader["Independiente"] as string
                                };

                                listado.Add(conductor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listado de conductores: {ex.Message}");
            }

            return listado;
        }


     

        // Read (Get by ID)
        public async Task<Conductor> ObtenerConductorPorIdAsync(int id)
        {
            Conductor conductor = null;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "SELECT * FROM conductor WHERE IDConductor = @IDConductor";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@IDConductor", id);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                conductor = new Conductor
                                {
                                    IDConductor = (int)reader["IDConductor"],
                                    Nombre = reader["Nombre"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    Direccion = reader["Direccion"] as string,
                                    Colonia = reader["Colonia"] as string,
                                    Ciudad = reader["Ciudad"] as string,
                                    IDEstado = reader["IDEstado"] as string,
                                    IDPais = reader["IDPais"] as string,
                                    mail = reader["mail"] as string,
                                    Licencia = reader["Licencia"] as string,
                                    Placa = reader["Placa"] as string,
                                    INE = reader["INE"] as string,
                                    Activo = (int)reader["Activo"],
                                    IDOrganizacion = (int)reader["IDOrganizacion"],
                                    Independiente = reader["Independiente"] as string
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el conductor por ID: {ex.Message}");
            }

            return conductor;
        }

        // Update
        public async Task ActualizarConductorAsync(Conductor conductor)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string updateQuery = @"
                        UPDATE conductor 
                        SET Nombre = @Nombre, Telefono = @Telefono, Direccion = @Direccion, 
                            Colonia = @Colonia, Ciudad = @Ciudad, IDEstado = @IDEstado, 
                            IDPais = @IDPais, mail = @mail, Licencia = @Licencia, 
                            Placa = @Placa, INE = @INE, Activo = @Activo, 
                            IDOrganizacion = @IDOrganizacion, Independiente = @Independiente 
                        WHERE IDConductor = @IDConductor";

                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@IDConductor", conductor.IDConductor);
                        updateCommand.Parameters.AddWithValue("@Nombre", conductor.Nombre);
                        updateCommand.Parameters.AddWithValue("@Telefono", conductor.Telefono);
                        updateCommand.Parameters.AddWithValue("@Direccion", conductor.Direccion);
                        updateCommand.Parameters.AddWithValue("@Colonia", conductor.Colonia);
                        updateCommand.Parameters.AddWithValue("@Ciudad", conductor.Ciudad);
                        updateCommand.Parameters.AddWithValue("@IDEstado", conductor.IDEstado);
                        updateCommand.Parameters.AddWithValue("@IDPais", conductor.IDPais);
                        updateCommand.Parameters.AddWithValue("@mail", conductor.mail);
                        updateCommand.Parameters.AddWithValue("@Licencia", conductor.Licencia);
                        updateCommand.Parameters.AddWithValue("@Placa", conductor.Placa);
                        updateCommand.Parameters.AddWithValue("@INE", conductor.INE);
                        updateCommand.Parameters.AddWithValue("@Activo", conductor.Activo);
                        updateCommand.Parameters.AddWithValue("@IDOrganizacion", conductor.IDOrganizacion);
                        updateCommand.Parameters.AddWithValue("@Independiente", conductor.Independiente);

                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el conductor: {ex.Message}");
            }
        }

        public async Task<List<Conductor>> ObtenerConductoresPorOrganizacionAsync(int idOrganizacion, int page, int pageSize)
        {
            List<Conductor> conductores = new List<Conductor>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = @"
                SELECT * FROM conductor 
                WHERE IDOrganizacion = @IDOrganizacion 
                ORDER BY Nombre 
                LIMIT @PageSize OFFSET @Offset";

                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IDOrganizacion", idOrganizacion);
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Conductor conductor = new Conductor
                                {
                                    IDConductor = (int)reader["IDConductor"],
                                    Nombre = reader["Nombre"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    Direccion = reader["Direccion"] as string,
                                    Colonia = reader["Colonia"] as string,
                                    Ciudad = reader["Ciudad"] as string,
                                    IDEstado = reader["IDEstado"] as string,
                                    IDPais = reader["IDPais"] as string,
                                    mail = reader["mail"] as string,
                                    Licencia = reader["Licencia"] as string,
                                    Placa = reader["Placa"] as string,
                                    INE = reader["INE"] as string,
                                    Activo = (int)reader["Activo"],
                                    IDOrganizacion = (int)reader["IDOrganizacion"],
                                    Independiente = reader["Independiente"] as string
                                };

                                conductores.Add(conductor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener conductores por organización con paginación: {ex.Message}");
            }

            return conductores;
        }

        public async Task<List<VConductor>> ObtenerConductoresvAsync(int page, int pageSize)
        {
            List<VConductor> conductores = new List<VConductor>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = @"
                SELECT * FROM vcoductor 
                ORDER BY Nombre 
                LIMIT @PageSize OFFSET @Offset";

                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                VConductor conductor = new VConductor
                                {
                                    IDConductor = (int)reader["IDConductor"],
                                    Nombre = reader["Nombre"] as string,
                                    Telefono = reader["Telefono"] as string,
                                    Direccion = reader["Direccion"] as string,
                                    Colonia = reader["Colonia"] as string,
                                    Ciudad = reader["Ciudad"] as string,
                                    IDEstado = reader["IDEstado"] as string,
                                    IDPais = reader["IDPais"] as string,
                                    mail = reader["mail"] as string,
                                    Licencia = reader["Licencia"] as string,
                                    Placa = reader["Placa"] as string,
                                    INE = reader["INE"] as string,
                                    Activo = (int)reader["Activo"],
                                    IDOrganizacion = (int)reader["IDOrganizacion"],
                                    Independiente = reader["Independiente"] as string,
                                    RazonSocial = reader["RazonSocial"] as string
                                };

                                conductores.Add(conductor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener conductores desde vconductor: {ex.Message}");
            }

            Console.WriteLine($"Total conductores obtenidos desde vconductor: {conductores.Count}"); 
            return conductores;
        }




        // Delete
        public async Task EliminarConductorAsync(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string deleteQuery = "DELETE FROM conductor WHERE IDConductor = @IDConductor";

                    using (var deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@IDConductor", id);

                        await deleteCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el conductor: {ex.Message}");
            }
        }

        public async Task<int> ContarConductoresPorOrganizacionAsync(int idOrganizacion)
        {
            int total = 0;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string countQuery = "SELECT COUNT(*) FROM conductor WHERE IDOrganizacion = @IDOrganizacion";

                    using (var command = new MySqlCommand(countQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IDOrganizacion", idOrganizacion);
                        total = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al contar conductores: {ex.Message}");
            }

            return total;
        }


        public async Task<IEnumerable<Conductor>> ObtenerConductoresParaDropdownAsync()
        {
            List<Conductor> conductores = new List<Conductor>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "SELECT IDConductor, Nombre FROM conductor";

                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Conductor conductor = new Conductor
                                {
                                    IDConductor = (int)reader["IDConductor"],
                                    Nombre = reader["Nombre"] as string
                                };

                                conductores.Add(conductor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener conductores para el dropdown: {ex.Message}");
            }

            return conductores;
        }
    }
}

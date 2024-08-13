using Certificado2.Modelos;

using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Certificado2.Repositorios
{

    public interface IRepositorioVendedor
    {
        Task AddVendedorAsync(Vendedor vendedor);
        Task<List<Vendedor>> GetAllVendedoresAsync();
        Task<Vendedor> GetVendedorByIdAsync(int id);
        Task UpdateVendedorAsync(Vendedor vendedor);
        Task DeleteVendedorAsync(int id);
        Task<IEnumerable<Vendedor>> GetVendedoresForDropdownAsync();
    }
    public class RepositorioVendedor : IRepositorioVendedor
    {
        private readonly string connectionString;

        public RepositorioVendedor(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        // Create
        public async Task AddVendedorAsync(Vendedor vendedor)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string insertQuery = "INSERT INTO vendedor (Nombre, Telefono) VALUES (@Nombre, @Telefono)";

                    using (var insertCommand = new MySqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Nombre", vendedor.Nombre);
                        insertCommand.Parameters.AddWithValue("@Telefono", vendedor.Telefono);

                        await insertCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar el vendedor: {ex.Message}");
            }
        }

        // Read (Get All)
        public async Task<List<Vendedor>> GetAllVendedoresAsync()
        {
            List<Vendedor> listado = new List<Vendedor>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "SELECT * FROM vendedor order by nombre";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Vendedor vendedor = new Vendedor
                                {
                                    IDVendedor = (int)reader["IDVendedor"],
                                    Nombre = reader["Nombre"] as string,
                                    Telefono = reader["Telefono"] as string
                                };

                                listado.Add(vendedor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener listado de vendedores: {ex.Message}");
            }

            return listado;
        }

        // Read (Get by ID)
        public async Task<Vendedor> GetVendedorByIdAsync(int id)
        {
            Vendedor vendedor = null;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "SELECT * FROM vendedor WHERE IDVendedor = @IDVendedor";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@IDVendedor", id);

                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                vendedor = new Vendedor
                                {
                                    IDVendedor = (int)reader["IDVendedor"],
                                    Nombre = reader["Nombre"] as string,
                                    Telefono = reader["Telefono"] as string
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el vendedor por ID: {ex.Message}");
            }

            return vendedor;
        }

        // Update
        public async Task UpdateVendedorAsync(Vendedor vendedor)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string updateQuery = "UPDATE vendedor SET Nombre = @Nombre, Telefono = @Telefono WHERE IDVendedor = @IDVendedor";

                    using (var updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@IDVendedor", vendedor.IDVendedor);
                        updateCommand.Parameters.AddWithValue("@Nombre", vendedor.Nombre);
                        updateCommand.Parameters.AddWithValue("@Telefono", vendedor.Telefono);

                        await updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el vendedor: {ex.Message}");
            }
        }

        // Delete
        public async Task DeleteVendedorAsync(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string deleteQuery = "DELETE FROM vendedor WHERE IDVendedor = @IDVendedor";

                    using (var deleteCommand = new MySqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@IDVendedor", id);

                        await deleteCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el vendedor: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Vendedor>> GetVendedoresForDropdownAsync()
        {
            List<Vendedor> vendedores = new List<Vendedor>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string selectQuery = "SELECT IDVendedor, Nombre FROM vendedor";

                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Vendedor vendedor = new Vendedor
                                {
                                    IDVendedor = (int)reader["IDVendedor"],
                                    Nombre = reader["Nombre"] as string
                                };

                                vendedores.Add(vendedor);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener vendedores para el dropdown: {ex.Message}");
            }

            return vendedores;
        }

    }
}

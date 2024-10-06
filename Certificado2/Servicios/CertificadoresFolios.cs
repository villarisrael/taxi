using System;
using System.Data;
using System.Numerics;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Certificado2.Servicios
{

    public interface ICertificadoresFoliosRepository
    {
        Task<RespuestaDisponible> GetFoliosDisponiblesAsync(int certificadorId);
        Task<bool> ConsumirFoliosAsync(int certificadorId);
        Task<bool> CambiarPlanAsync(int certificadorId, string nuevoPlan);


        Task<bool> AnadirFoliosAsync(int certificadorId, int cantidad, string plan);
    }
    public class CertificadoresFoliosRepository : ICertificadoresFoliosRepository
    {
        private readonly string _connectionString;

        public CertificadoresFoliosRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        public async Task<RespuestaDisponible> GetFoliosDisponiblesAsync(int certificadorId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Consulta para obtener el plan actual y folios consumidos
                string query = @"
                    SELECT Plan, FoliosDisponibles , FoliosConsumidos
                    FROM certificadores_folios 
                    WHERE IDCertificador = @certificadorId 
                    AND Mes = @mes AND Año = @año";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@certificadorId", certificadorId);
                    command.Parameters.AddWithValue("@mes", DateTime.Now.Month);
                    command.Parameters.AddWithValue("@año", DateTime.Now.Year);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string plan = reader["Plan"].ToString();
                            int foliosDisponibles = Convert.ToInt32(reader["FoliosDisponibles"]);
                            int foliosConsumidos = Convert.ToInt32(reader["FoliosConsumidos"]);
                            int ff = foliosDisponibles - foliosConsumidos;
                            if (ff<0)
                            { ff=0; }

                            // Determina los folios disponibles según el plan

                            RespuestaDisponible respuesta= new RespuestaDisponible();
                            respuesta.Plan = plan;
                            respuesta.Disponibles=ff;
                            return respuesta;


                        }

                        return new RespuestaDisponible(); // Si no hay registros, devuelve 0 folios disponibles
                    }
                }
            }
        }


        public async Task<bool> ConsumirFoliosAsync(int certificadorId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                

                // Verifica los folios disponibles
                RespuestaDisponible foliosDisponibles = await GetFoliosDisponiblesAsync(certificadorId);


                if (foliosDisponibles.Disponibles ==0 )
                {
                    return false; // No hay suficientes folios disponibles
                }

                // Actualiza el consumo de folios
                string updateQuery = @"
                    UPDATE certificadores_folios 
                    SET FoliosConsumidos = FoliosConsumidos + @cantidad 
                    WHERE IDCertificador = @certificadorId 
                    AND Mes = @mes AND Año = @año";

                using (var command = new MySqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@cantidad", 1);
                    command.Parameters.AddWithValue("@certificadorId", certificadorId);
                    command.Parameters.AddWithValue("@mes", DateTime.Now.Month);
                    command.Parameters.AddWithValue("@año", DateTime.Now.Year);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> AnadirFoliosAsync(int certificadorId, int cantidad, string plan)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Verifica si existe un registro para el certificador en el mes y año actuales
                string selectQuery = @"
                                    SELECT COUNT(*) 
                                    FROM certificadores_folios 
                                    WHERE IDCertificador = @certificadorId 
                                    AND Mes = @mes 
                                    AND Año = @año";

                using (var selectCommand = new MySqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@certificadorId", certificadorId);
                    selectCommand.Parameters.AddWithValue("@mes", DateTime.Now.Month);
                    selectCommand.Parameters.AddWithValue("@año", DateTime.Now.Year);

                    int count = Convert.ToInt32(await selectCommand.ExecuteScalarAsync());

                    if (count == 0)
                    {
                        // No existe un registro, se debe insertar uno nuevo
                        string insertQuery = @"
                    INSERT INTO certificadores_folios (IDCertificador, Plan, FoliosConsumidos,FoliosDisponibles, Mes, Año) 
                    VALUES (@certificadorId, @plan,0, @cantidad, @mes, @año)";

                        using (var insertCommand = new MySqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@certificadorId", certificadorId);
                            insertCommand.Parameters.AddWithValue("@plan", plan);
                            insertCommand.Parameters.AddWithValue("@cantidad", cantidad);
                            insertCommand.Parameters.AddWithValue("@mes", DateTime.Now.Month);
                            insertCommand.Parameters.AddWithValue("@año", DateTime.Now.Year);

                            int rowsAffected = await insertCommand.ExecuteNonQueryAsync();
                            return rowsAffected > 0;
                        }
                    }
                    else
                    {
                        // Existe un registro, se debe actualizar
                        string updateQuery = @"
                    UPDATE certificadores_folios 
                    SET FoliosDisponibles = Foliosdisponibles + @cantidad , plan = @plan
                    WHERE IDCertificador = @certificadorId 
                    AND Mes = @mes 
                    AND Año = @año";

                        using (var updateCommand = new MySqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@cantidad", cantidad);
                            updateCommand.Parameters.AddWithValue("@plan", plan);
                            updateCommand.Parameters.AddWithValue("@certificadorId", certificadorId);
                            updateCommand.Parameters.AddWithValue("@mes", DateTime.Now.Month);
                            updateCommand.Parameters.AddWithValue("@año", DateTime.Now.Year);

                            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                            return rowsAffected > 0;
                        }
                    }
                }
            }
        }

        public async Task<bool> CambiarPlanAsync(int certificadorId, string nuevoPlan)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Cambia el plan del certificador
                string updateQuery = @"
                    UPDATE certificadores_folios 
                    SET Plan = @nuevoPlan 
                    WHERE IDCertificador = @certificadorId 
                    AND Mes = @mes AND Año = @año";

                using (var command = new MySqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@nuevoPlan", nuevoPlan);
                    command.Parameters.AddWithValue("@certificadorId", certificadorId);
                    command.Parameters.AddWithValue("@mes", DateTime.Now.Month);
                    command.Parameters.AddWithValue("@año", DateTime.Now.Year);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }

    public class RespuestaDisponible
    {
       public string Plan { get; set; }
        public int Disponibles { get; set; }
    }
}

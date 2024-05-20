using Certificado2.Modelos;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Runtime.ConstrainedExecution;

namespace Certificado2.Servicios
{

    public interface IRepositorioMonedas
    {
        Task<VMoneda> ObtenerDatosMoneda(string serie, int folio);
        Task<byte[]> ObtenerLogotipoEmpresaAsync();
    }
    public class RepositorioMonedas : IRepositorioMonedas
    {

        private readonly string connectionString;

        public RepositorioMonedas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("ConexionMySql");
        }

        public async Task<byte[]> ObtenerLogotipoEmpresaAsync()
        {
            byte[] logotipo = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string selectQuery = "SELECT Logo_Empresa FROM empresa";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                logotipo = (byte[])reader["Logo_Empresa"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el logotipo de la empresa: {ex.Message}");
            }

            return logotipo;
        }

        public async Task<VMoneda> ObtenerDatosMoneda(string serie, int folio)
        {
            Moneda listado = new Moneda();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Error en esta consulta, no tengo acceso a la BD
                    string selectQuery = $"SELECT * FROM Vmonedas WHERE SERIE = '{serie}' AND FOLIO = {folio}";

                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        using (var reader = await selectCommand.ExecuteReaderAsync())
                        {
                            reader.ReadAsync();
                            
                                VMoneda certificador = new VMoneda
                                {
                                    RazonSocial = (string)reader["RazonSocial"],
                                    NombreResponsable= (string)reader["NombreResponsable"],
                                    Serie = reader["Serie"] as string,
                                    Folio = (int)reader["Folio"],
                                 Telefono = reader["Telefono"] as string,
                                    Moneda = reader["Moneda"] as string,
                                    Ano = (string)reader["ano"],
                                    Ceca = reader["ceca"] as string,
                                    Material = reader["Material"] as string,
                                    Estado = reader["Estado"] as string,
                                    fecha = (DateTime) reader["fecha"] ,
                                    Foto = reader["Foto"] as byte[]
                                };

                            return certificador;
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos de la moneda: {ex.Message}");
                return new VMoneda();
            }

           
        }

    }
}

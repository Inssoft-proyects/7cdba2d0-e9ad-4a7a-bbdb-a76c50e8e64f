using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace GuanajuatoAdminUsuarios.Services
{
    public class CatColoresService : IColores
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatColoresService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public int obtenerIdPorColor(string colorLimpio)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("SELECT idColor FROM catColores WHERE color = @color", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@color", SqlDbType.NVarChar)).Value = colorLimpio;
                    sqlCommand.CommandType = CommandType.Text;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read()) // Intenta leer un registro del resultado
                        {
                            // Obtiene el valor de la columna "idMunicipio"
                            result = Convert.ToInt32(reader["idColor"]);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Manejo de errores y log
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }
    }
}

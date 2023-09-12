using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatTiposVehiculosService : ICatTiposVehiculosService

    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatTiposVehiculosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<TiposVehiculosModel> GetTiposVehiculos()
        {
            //
            List<TiposVehiculosModel> ListaTiposVehiculo = new List<TiposVehiculosModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT tv.*, e.estatusdesc FROM catTiposVehiculo AS tv LEFT JOIN estatus AS e ON tv.estatus = e.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            TiposVehiculosModel tipoVehiculo = new TiposVehiculosModel();

                            tipoVehiculo.IdTipoVehiculo = Convert.ToInt32(reader["IdTipoVehiculo"] is DBNull ? 0 : reader["IdTipoVehiculo"]);
                            tipoVehiculo.TipoVehiculo = reader["TipoVehiculo"] is DBNull ? string.Empty : reader["TipoVehiculo"].ToString();
                            tipoVehiculo.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            tipoVehiculo.EstatusDesc = reader["estatusDesc"] is DBNull ? string.Empty : reader["estatusDesc"].ToString();
                            tipoVehiculo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            tipoVehiculo.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            tipoVehiculo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);

                            ListaTiposVehiculo.Add(tipoVehiculo);
                        }

                    }

                }
                catch (SqlException ex)
                {
                    //Guardar la excepcion en algun log de errores
                    //ex
                }
                finally
                {
                    connection.Close();
                }
            return ListaTiposVehiculo;


        }
        public int obtenerIdPorTipo(string categoria)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("SELECT idTipoVehiculo FROM catTiposVehiculo WHERE tipoVehiculo = @tipoVehiculo", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@tipoVehiculo", SqlDbType.NVarChar)).Value = categoria;
                    sqlCommand.CommandType = CommandType.Text;

                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.Read()) // Intenta leer un registro del resultado
                        {
                            // Obtiene el valor de la columna "idMunicipio"
                            result = Convert.ToInt32(reader["idTipoVehiculo"]);
                        }
                    }
                }
                catch (SqlException ex)
                {
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

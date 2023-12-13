using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatSubtipoServicioService : ICatSubtipoServicio
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatSubtipoServicioService (ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<CatSubtipoServicioModel> GetSubtipoPorTipo(int tipoServicioDDlValue)
        {
            //
            List<CatSubtipoServicioModel> ListaSubtipos = new List<CatSubtipoServicioModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT csubt.*, cts.*, e.estatus FROM catSubtipoServicio AS csubt " +
                        "LEFT JOIN catTipoServicio AS cts ON csubt.idTipoServicio = cts.idCatTipoServicio " +
                        "LEFT JOIN estatus AS e ON csubt.estatus = e.estatus WHERE csubt.idTipoServicio = @idTipoServicio;\r\n", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idTipoServicio", SqlDbType.Int)).Value = (object)tipoServicioDDlValue ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatSubtipoServicioModel subtipo = new CatSubtipoServicioModel();
                            subtipo.idSubTipoServicio = Convert.ToInt32(reader["idSubTipoServicio"].ToString());
                            subtipo.idTipoServicio = Convert.ToInt32(reader["idTipoServicio"].ToString());
                            subtipo.subTipoServicio = reader["servicio"].ToString();
                            subtipo.estatusDesc = reader["estatus"].ToString();
                           // subtipo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            subtipo.estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            // subtipo.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaSubtipos.Add(subtipo);

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
            return ListaSubtipos;


        }
    }
}

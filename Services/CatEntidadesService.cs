using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatEntidadesService : ICatEntidadesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatEntidadesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatEntidadesModel> ObtenerEntidades()
        {
            //
            List<CatEntidadesModel> ListaEntidades = new List<CatEntidadesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ent.*, e.estatusDesc FROM catEntidades AS ent INNER JOIN estatus AS e ON ent.estatus = e.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatEntidadesModel entidad = new CatEntidadesModel();
                            entidad.idEntidad = Convert.ToInt32(reader["idEntidad"].ToString());
                            entidad.nombreEntidad = reader["nombreEntidad"].ToString();
                            entidad.estatusDesc = reader["estatusDesc"].ToString();
                            entidad.fechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            entidad.estatus = Convert.ToInt32(reader["estatus"].ToString());
                            entidad.actualizadoPor = Convert.ToInt32(reader["actualizadoPor"].ToString());
                            ListaEntidades.Add(entidad);

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
            return ListaEntidades;


        }

        public CatEntidadesModel ObtenerEntidadesByID(int idEntidad)
        {
            CatEntidadesModel entidad = new CatEntidadesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catEntidades where idEntidad=@idEntidad", connection);
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = idEntidad;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            entidad.idEntidad = Convert.ToInt32(reader["idEntidad"].ToString());
                            entidad.nombreEntidad = reader["nombreEntidad"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    connection.Close();
                }

            return entidad;
        }
        public int CrearEntidad(CatEntidadesModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catEntidades(nombreEntidad,estatus,fechaActualizacion,actualizadoPor) values(@nombreEntidad,@estatus,@fechaActualizacion,@actualizadoPor)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreEntidad", SqlDbType.VarChar)).Value = model.nombreEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;

                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
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
        public int EditarEntidad(CatEntidadesModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catEntidades set nombreEntidad=@nombreEntidad, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor where idEntidad=@idEntidad",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = model.idEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreEntidad", SqlDbType.NVarChar)).Value = model.nombreEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.VarChar)).Value = model.estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    //---Log
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


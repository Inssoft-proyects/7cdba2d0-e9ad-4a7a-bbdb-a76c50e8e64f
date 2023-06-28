using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatClasificacionAccidentesService : ICatClasificacionAccidentes
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatClasificacionAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatClasificacionAccidentesModel> GetClasificacionAccidentes()
        {
            //
            List<CatClasificacionAccidentesModel> ListaClasificaciones = new List<CatClasificacionAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT c.*, e.estatus FROM catClasificacionAccidentes AS c INNER JOIN estatus AS e ON c.estatus = e.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatClasificacionAccidentesModel clasificacion = new CatClasificacionAccidentesModel();
                            clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["IdClasificacionAccidente"].ToString());
                            clasificacion.NombreClasificacion = reader["NombreClasificacion"].ToString();
                            clasificacion.estatusDesc = reader["estatus"].ToString();
                            clasificacion.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            clasificacion.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            ListaClasificaciones.Add(clasificacion);

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
            return ListaClasificaciones;


        }



        public List<CatClasificacionAccidentesModel> ObtenerClasificacionesActivas()
        {
            //
            List<CatClasificacionAccidentesModel> ListaClasificaciones = new List<CatClasificacionAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT c.*, e.estatus FROM catClasificacionAccidentes AS c INNER JOIN estatus AS e ON c.estatus = e.estatus WHERE c.estatus = 1;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatClasificacionAccidentesModel clasificacion = new CatClasificacionAccidentesModel();
                            clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["IdClasificacionAccidente"].ToString());
                            clasificacion.NombreClasificacion = reader["NombreClasificacion"].ToString();
                            clasificacion.estatusDesc = reader["estatus"].ToString();
                            clasificacion.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            clasificacion.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            ListaClasificaciones.Add(clasificacion);

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
            return ListaClasificaciones;


        }


        public CatClasificacionAccidentesModel GetClasificacionAccidenteByID(int IdClasificacionAccidente)
        {
            CatClasificacionAccidentesModel clasificacion = new CatClasificacionAccidentesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catClasificacionAccidentes where idClasificacionAccidente=@idClasificacionAccidente", connection);
                    command.Parameters.Add(new SqlParameter("@idClasificacionAccidente", SqlDbType.Int)).Value = IdClasificacionAccidente;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["idClasificacionAccidente"].ToString());
                            clasificacion.NombreClasificacion = reader["nombreClasificacion"].ToString();

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

            return clasificacion;
        }
        public int CrearClasificacionAccidente(CatClasificacionAccidentesModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catClasificacionAccidentes(nombreClasificacion,estatus,fechaActualizacion,actualizadoPor) values(@nombreClasificacion,@estatus,@fechaActualizacion,@actualizadoPor)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreClasificacion", SqlDbType.VarChar)).Value = model.NombreClasificacion;
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
        public int EditarClasificacionAccidente(CatClasificacionAccidentesModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catClasificacionAccidentes set nombreClasificacion=@nombreClasificacion, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor where idClasificacionAccidente=@idClasificacionAccidente",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idClasificacionAccidente", SqlDbType.Int)).Value = model.IdClasificacionAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@nombreClasificacion", SqlDbType.NVarChar)).Value = model.NombreClasificacion;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.VarChar)).Value = model.Estatus;
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




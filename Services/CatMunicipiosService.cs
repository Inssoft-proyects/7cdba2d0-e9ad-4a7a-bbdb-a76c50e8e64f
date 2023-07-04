using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatMunicipiosService : ICatMunicipiosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatMunicipiosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatMunicipiosModel> GetMunicipios()
        {
            //
            List<CatMunicipiosModel> ListaMunicipios = new List<CatMunicipiosModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT m.*, e.estatusdesc, del.nombreOficina, ent.nombreEntidad FROM catMunicipios AS m INNER JOIN estatus AS e ON m.estatus = e.estatus INNER JOIN catDelegacionesOficinasTransporte AS del ON m.idOficinaTransporte = del.idOficinaTransporte INNER JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatMunicipiosModel municipio = new CatMunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
                            municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            municipio.Municipio = reader["Municipio"].ToString();
                            municipio.nombreOficina = reader["nombreOficina"].ToString();
                            municipio.nombreEntidad = reader["nombreEntidad"].ToString();
                            municipio.estatusDesc = reader["estatusDesc"].ToString();
                            municipio.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            municipio.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            municipio.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaMunicipios.Add(municipio);

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
            return ListaMunicipios;


        }
        public CatMunicipiosModel GetMunicipioByID(int IdMunicipio)
        {
            CatMunicipiosModel municipio = new CatMunicipiosModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT m.*, e.estatusdesc, del.nombreOficina, ent.nombreEntidad FROM catMunicipios AS m INNER JOIN estatus AS e ON m.estatus = e.estatus INNER JOIN catDelegacionesOficinasTransporte AS del ON m.idOficinaTransporte = del.idOficinaTransporte INNER JOIN catEntidades AS ent ON m.idEntidad = ent.idEntidad WHERE m.idMunicipio=@IdMunicipio", connection);
                    command.Parameters.Add(new SqlParameter("@IdMunicipio", SqlDbType.Int)).Value = IdMunicipio;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            municipio.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            municipio.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            municipio.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
                            municipio.nombreOficina = reader["nombreOficina"].ToString();
                            municipio.nombreEntidad = reader["nombreEntidad"].ToString();
                            municipio.Municipio = reader["Municipio"].ToString();
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

            return municipio;
        }
        public int AgregarMunicipio(CatMunicipiosModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catMunicipios(municipio,estatus,idOficinaTransporte,idEntidad,fechaActualizacion,actualizadoPor) values(@Municipio,@Estatus,@idOficinaTransporte,@idEntidad,@FechaActualizacion,@ActualizadoPor)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@Municipio", SqlDbType.VarChar)).Value = model.Municipio;
                    sqlCommand.Parameters.Add(new SqlParameter("@Estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value =model.IdOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = model.IdEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@FechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActualizadoPor", SqlDbType.Int)).Value = 1;

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
        public int EditarMunicipio(CatMunicipiosModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catMunicipios set municipio=@Municipio, idOficinaTransporte =@idOficinaTransporte,idEntidad = @idEntidad,estatus = @Estatus,fechaActualizacion = @FechaActualizacion, actualizadoPor =@ActualizadoPor where idMunicipio=@IdMunicipio",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@IdMunicipio", SqlDbType.Int)).Value = model.IdMunicipio;
                    sqlCommand.Parameters.Add(new SqlParameter("@Municipio", SqlDbType.NVarChar)).Value = model.Municipio;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = model.IdOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = model.IdEntidad;
                    sqlCommand.Parameters.Add(new SqlParameter("@Estatus", SqlDbType.VarChar)).Value = model.Estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@FechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@ActualizadoPor", SqlDbType.Int)).Value = 1;
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



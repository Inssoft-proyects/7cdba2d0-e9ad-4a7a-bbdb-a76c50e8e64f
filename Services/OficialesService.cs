using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class OficialesService : IOficiales
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public OficialesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        /// <summary>
        /// Consulta de Categorias con SQLClient
        /// </summary>
        /// <returns></returns>
        public List<CatOficialesModel> GetOficiales()
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select *, cd.nombreOficina from catOficiales co
                                                            LEFT JOIN catDelegacionesOficinasTransporte cd ON cd.idOficinaTransporte = co.idOficina", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.nombreOficina = reader["nombreOficina"].ToString();

                            object idOficinaValue = reader["idOficina"];
                            oficial.IdOficina = Convert.IsDBNull(idOficinaValue) ? 0 : Convert.ToInt32(idOficinaValue);




                            oficiales.Add(oficial);

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
            return oficiales;


        }
        public List<CatOficialesModel> GetOficialesActivos()
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ofi.*, e.estatusDesc FROM catOficiales AS ofi INNER JOIN estatus AS e ON ofi.estatus = e.estatus WHERE ofi.estatus = 1 ORDER BY Nombre ASC;", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
                            //oficial.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            oficial.Estatus = Convert.ToInt32(reader["Estatus"].ToString());


                            oficiales.Add(oficial);

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
            return oficiales;


        }

        public CatOficialesModel GetOficialById(int IdOficial)
        {
            CatOficialesModel oficial = new CatOficialesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catOficiales where idOficial=@IdOficial", connection);
                    command.Parameters.Add(new SqlParameter("@IdOficial", SqlDbType.Int)).Value = IdOficial;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();

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

            return oficial;
        }

        public int SaveOficial(CatOficialesModel oficial)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catOficiales(Nombre, estatus,ApellidoPaterno,ApellidoMaterno) values(@Nombre,@estatus,@ApellidoPaterno,@ApellidoMaterno)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar)).Value = oficial.Nombre;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoPaterno", SqlDbType.VarChar)).Value = oficial.ApellidoPaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoMaterno", SqlDbType.VarChar)).Value = oficial.ApellidoMaterno;

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
        public int UpdateOficial(CatOficialesModel oficial)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catOficiales set Nombre = @Nombre, ApellidoPaterno=@ApellidoPaterno, ApellidoMaterno=@ApellidoMaterno, estatus = @estatus where idOficial=@idOficial",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = oficial.IdOficial;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = oficial.Estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar)).Value = oficial.Nombre;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoPaterno", SqlDbType.VarChar)).Value = oficial.ApellidoPaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoMaterno", SqlDbType.VarChar)).Value = oficial.ApellidoMaterno;

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


        public int DeleteOficial(int IdOficial)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Delete from oficiales where idOficial=@idOficial", connection);
                    command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = IdOficial;
                    command.CommandType = CommandType.Text;
                    result = command.ExecuteNonQuery();

                }
                catch (Exception ex)
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
        public List<CatOficialesModel> GetOficialesFiltrados(int idOficina,int idDependencia)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ofi.*, e.estatusDesc 
                                                            FROM catOficiales AS ofi 
                                                            INNER JOIN estatus AS e ON ofi.estatus = e.estatus
                                                            WHERE ofi.estatus = 1 AND ofi.idOficina = @idOficina AND ofi.transito = @idDependencia
                                                            ORDER BY Nombre ASC;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficialesModel oficial = new CatOficialesModel();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
                            //oficial.FechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            oficial.Estatus = Convert.ToInt32(reader["Estatus"].ToString());


                            oficiales.Add(oficial);

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
            return oficiales;


        }

    }
}


using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
                    SqlCommand command = new SqlCommand(@"Select *, cd.nombreOficina,e.estatusDesc from catOficiales co
                                                            LEFT JOIN catDelegacionesOficinasTransporte cd ON cd.idOficinaTransporte = co.idOficina
                                                            LEFT JOIN estatus e ON e.estatus = co.estatus", connection);
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
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
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

        public List<CatOficialesModel> GetOficialesByCorporacion(int corporacion)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
Select *, cd.nombreOficina,e.estatusDesc from catOficiales co
                                                            LEFT JOIN catDelegacionesOficinasTransporte cd ON cd.idOficinaTransporte = co.idOficina
                                                            LEFT JOIN estatus e ON e.estatus = co.estatus
															where co.transito=@corp", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    command.Parameters.Add(new SqlParameter("@corp", SqlDbType.Int)).Value = (object)corporacion ?? DBNull.Value;

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
                            oficial.estatusDesc = reader["estatusDesc"].ToString();
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


        public List<CatOficialesModel> GetCatalogoOficialesDependencia(int idDependencia)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select *, cd.nombreOficina,e.estatusDesc from catOficiales co
                                                            LEFT JOIN catDelegacionesOficinasTransporte cd ON cd.idOficinaTransporte = co.idOficina
                                                            LEFT JOIN estatus e ON e.estatus = co.estatus
                                                            WHERE co.transito = @idDependencia", connection);
                    command.CommandType = CommandType.Text;
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
                            oficial.nombreOficina = reader["nombreOficina"].ToString();
                            oficial.estatusDesc = reader["estatusDesc"].ToString();

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
                            oficial.transito = (Convert.ToBoolean(reader["transito"])) ? 1 : 0;

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
        public List<CatOficialesModel> GetOficialesActivosTodos(bool todos, int tipo)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("usp_ObtieneOfialesActivos", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Todos", SqlDbType.Bit)).Value = todos;
                    command.Parameters.Add(new SqlParameter("@Tipo", SqlDbType.Int)).Value = tipo;
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
                            oficial.transito = (Convert.ToBoolean(reader["transito"])) ? 1 : 0;

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
                            oficial.IdOficina = Convert.ToInt32(reader["idOficina"]);
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

        public int SaveOficial(CatOficialesModel oficial, int idDependencia)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand(@"
                                Insert into catOficiales (Nombre, estatus,ApellidoPaterno,ApellidoMaterno,idOficina,transito) values
                                                        (@Nombre,@estatus,@ApellidoPaterno,@ApellidoMaterno,@idDependencia,@tran)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar)).Value = oficial.Nombre;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoPaterno", SqlDbType.VarChar)).Value = oficial.ApellidoPaterno==null ? "": oficial.ApellidoPaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoMaterno", SqlDbType.VarChar)).Value = oficial.ApellidoMaterno==null ? "": oficial.ApellidoMaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = oficial.IdOficina == null ? 0 : oficial.IdOficina;
                    sqlCommand.Parameters.Add(new SqlParameter("@tran", SqlDbType.Int)).Value = idDependencia == null ? 0 : idDependencia;

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
                        SqlCommand("Update catOficiales set Nombre = @Nombre, ApellidoPaterno=@ApellidoPaterno, ApellidoMaterno=@ApellidoMaterno, estatus = @estatus, idOficina=@idDependencia where idOficial=@idOficial",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = oficial.IdOficial;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = oficial.Estatus;
                    sqlCommand.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar)).Value = oficial.Nombre;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoPaterno", SqlDbType.VarChar)).Value = oficial.ApellidoPaterno == null ? "" : oficial.ApellidoPaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@ApellidoMaterno", SqlDbType.VarChar)).Value = oficial.ApellidoMaterno == null ? "" : oficial.ApellidoMaterno;
                    sqlCommand.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = oficial.IdOficina == null ? 0 : oficial.IdOficina;

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
        public List<CatOficialesModel> GetOficialesPorDependencia(int idDependencia)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ofi.*, e.estatusDesc, ISNULL(X.idOficinaTransporte,0) idOficinaTransporte, ISNULL(x.nombreOficina,'') nombreOficina
                                                            FROM catOficiales AS ofi 
                                                            INNER JOIN estatus AS e ON ofi.estatus = e.estatus
                                                            LEFT JOIN catDelegacionesOficinasTransporte X ON ofi.idOficina = x.idOficinaTransporte
                                                            WHERE ofi.estatus = 1 AND ofi.transito = @idDependencia
                                                            ORDER BY Nombre ASC;", connection);
                    command.CommandType = CommandType.Text;
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
                            oficial.nombreOficina = reader["nombreOficina"].ToString(); 
                            oficial.IdOficina = Convert.ToInt32(reader["idOficinaTransporte"].ToString()); 

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



        public List<CatOficialesModel> GetOficialesPorDependencia2(int idDependencia)
        {
            //
            List<CatOficialesModel> oficiales = new List<CatOficialesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT ofi.*, e.estatusDesc, ISNULL(X.idOficinaTransporte,0) idOficinaTransporte, ISNULL(x.nombreOficina,'') nombreOficina
                                                            FROM catOficiales AS ofi 
                                                            INNER JOIN estatus AS e ON ofi.estatus = e.estatus
                                                            LEFT JOIN catDelegacionesOficinasTransporte X ON ofi.idOficina = x.idOficinaTransporte
                                                            WHERE  ofi.transito = @idDependencia
                                                            ORDER BY idOficial desc;", connection);
                    command.CommandType = CommandType.Text;
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
                            oficial.nombreOficina = reader["nombreOficina"].ToString();
                            oficial.IdOficina = Convert.ToInt32(reader["idOficinaTransporte"].ToString());

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


        public List<CatOficialesModel> GetOficialesPorDependencia()
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
                                                            WHERE ofi.estatus = 1 
                                                            ORDER BY Nombre ASC;", connection);
                    command.CommandType = CommandType.Text;
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


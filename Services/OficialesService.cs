using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
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
        public List<Oficiales> GetOficiales()
        {
            //
            List<Oficiales> oficiales = new List<Oficiales>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from oficiales", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Oficiales oficial = new Oficiales();
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
                            oficial.Nombre = reader["Nombre"].ToString();
                            oficial.ApellidoPaterno = reader["ApellidoPaterno"].ToString();
                            oficial.ApellidoMaterno = reader["ApellidoMaterno"].ToString();


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

        public Oficiales GetOficialById(int IdOficial)
        {
            Oficiales oficial = new Oficiales();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from oficiales where idOficial=@IdOficial", connection);
                    command.Parameters.Add(new SqlParameter("@IdOficial", SqlDbType.Int)).Value = IdOficial;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            oficial.IdOficial = Convert.ToInt32(reader["IdOficial"].ToString());
                            oficial.Rango = reader["Rango"].ToString();
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

        public int SaveOficial(Oficiales oficial)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into oficiales(Rango,Nombre,ApellidoPaterno,ApellidoMaterno) values(@Rango,@Nombre,@ApellidoPaterno,@ApellidoMaterno)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@Rango", SqlDbType.VarChar)).Value = oficial.Rango;
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
        public int UpdateOficial(Oficiales oficial)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update oficiales set Rango=@Rango, Nombre = @Nombre, ApellidoPaterno=@ApellidoPaterno, ApellidoMaterno=@ApellidoMaterno where idOficial=@idOficial",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = oficial.IdOficial;
                    sqlCommand.Parameters.Add(new SqlParameter("@Rango", SqlDbType.VarChar)).Value = oficial.Rango;
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


    }
}


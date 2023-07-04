using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class DependenciasService : IDependencias
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public DependenciasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        /// <summary>
        /// Consulta de Categorias con SQLClient
        /// </summary>
        /// <returns></returns>
        public List<Dependencias> GetDependencias()
        {
            //
            List<Dependencias> dependencias = new List<Dependencias>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catDependencias where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Dependencias dependencia = new Dependencias();
                            dependencia.IdDependencia = Convert.ToInt32(reader["IdDependencia"].ToString());
                            dependencia.NombreDependencia = reader["NombreDependencia"].ToString();
                            dependencias.Add(dependencia);

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
            return dependencias;


        }

        public Dependencias GetDependenciaById(int idDependencia)
        {
            Dependencias dependencia = new Dependencias();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from dependencias where idDependencia=@idDependencia", connection);
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            dependencia.IdDependencia = Convert.ToInt32(reader["idDependencia"].ToString());
                            dependencia.NombreDependencia = reader["NombreDependencia"].ToString();
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

            return dependencia;
        }

        public int SaveDependencia(Dependencias dependencia)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into dependencias(nombreDependencia) values(@NombreDependencia)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@NombreDependencia", SqlDbType.VarChar)).Value = dependencia.NombreDependencia;
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
        public int UpdateDependencia(Dependencias dependencia)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update dependencias set NombreDependencia=@NombreDependencia where idDependencia=@idDependencia",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = dependencia.IdDependencia;
                    sqlCommand.Parameters.Add(new SqlParameter("@NombreDependencia", SqlDbType.VarChar)).Value = dependencia.NombreDependencia;
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


        public int DeleteDependencia(int idDependencia)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Delete from dependencias where idDependencia=@idDependencia", connection);
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;
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



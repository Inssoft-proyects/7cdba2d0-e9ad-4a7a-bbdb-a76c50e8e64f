using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatalogosService : ICatalogosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatalogosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<Dictionary<string, string>> GetGenericCatalogos(string tabla, string[] campos)
        {
            List<Dictionary<string, string>> modelList = new List<Dictionary<string, string>>();
            string strParams = string.Join(",", campos);
            string strQuery = @"SELECT
                                {0}
                                FROM {1}
                                WHERE estatus = 1";
            strQuery = string.Format(strQuery, strParams, tabla);
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            foreach (string campo in campos)
                            {
                                dictionary.Add(campo, Convert.ToString(reader[campo]));
                            }
                            modelList.Add(dictionary);
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
            return modelList;
        }

        public List<Dictionary<string, string>> GetGenericCatalogosByFilter(string tabla, string[] campos, string campoFiltro, int idFiltro)
        {
            List<Dictionary<string, string>> modelList = new List<Dictionary<string, string>>();
            string strCampos = string.Join(",", campos);
            string strQuery = @"SELECT
                                {0}
                                FROM {1}
                                WHERE estatus = 1
                                AND {2} = @idFiltro";
            strQuery = string.Format(strQuery, strCampos, tabla, campoFiltro);
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idFiltro", SqlDbType.Int)).Value = idFiltro;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            foreach (string campo in campos)
                            {
                                dictionary.Add(campo, Convert.ToString(reader[campo]));
                            }
                            modelList.Add(dictionary);
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
            return modelList;
        }

        public List<Dictionary<string, string>> GetGenericCatalogosByFilters(string tabla, string[] campos, string campoFiltro, int idFiltro, string campoFiltro2, int idFiltro2)
        {
            List<Dictionary<string, string>> modelList = new List<Dictionary<string, string>>();
            string strCampos = string.Join(",", campos);
            string strQuery = @"SELECT
                                {0}
                                FROM {1}
                                WHERE estatus = 1
                                AND {2} = @idFiltro 
                                AND {3} = @idFiltro2 ";
            strQuery = string.Format(strQuery, strCampos, tabla, campoFiltro, campoFiltro2);
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idFiltro", SqlDbType.Int)).Value = idFiltro;
                    command.Parameters.Add(new SqlParameter("@idFiltro2", SqlDbType.Int)).Value = idFiltro2;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            foreach (string campo in campos)
                            {
                                dictionary.Add(campo, Convert.ToString(reader[campo]));
                            }
                            modelList.Add(dictionary);
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
            return modelList;
        }
    }
}

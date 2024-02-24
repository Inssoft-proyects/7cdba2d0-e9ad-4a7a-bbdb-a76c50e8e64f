using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatDelegacionesOficinasTransporteService : ICatDelegacionesOficinasTransporteService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatDelegacionesOficinasTransporteService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinas()
        {
            //
            List<CatDelegacionesOficinasTransporteModel> ListaDelegacionsOficinas = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT del.*, e.estatusDesc,m.municipio FROM catDelegacionesOficinasTransporte AS del INNER JOIN estatus AS e ON del.estatus = e.estatus INNER JOIN catMunicipios AS m ON del.idMunicipio = m.idMunicipio;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDelegacionesOficinasTransporteModel delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdOficinaTransporte = Convert.ToInt32(reader["IdOficinaTransporte"].ToString());
                            delegacionOficina.NombreOficina = reader["NombreOficina"].ToString();
                            delegacionOficina.JefeOficina = reader["JefeOficina"].ToString();
                            delegacionOficina.Municipio = reader["Municipio"].ToString();
                            delegacionOficina.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            delegacionOficina.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            delegacionOficina.estatusDesc = reader["estatusDesc"].ToString();
                            delegacionOficina.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            //delegacionOficina.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaDelegacionsOficinas.Add(delegacionOficina);

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
            return ListaDelegacionsOficinas;


        }
        public List<CatDelegacionesOficinasTransporteModel> GetDelegacionesOficinasActivos()
        {
            //
            List<CatDelegacionesOficinasTransporteModel> ListaDelegacionsOficinas = new List<CatDelegacionesOficinasTransporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT del.*, e.estatusDesc FROM catDelegacionesOficinasTransporte AS del INNER JOIN estatus AS e ON del.estatus = e.estatus" +
                        " WHERE del.estatus = 1 ORDER BY nombreOficina ASC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatDelegacionesOficinasTransporteModel delegacionOficina = new CatDelegacionesOficinasTransporteModel();
                            delegacionOficina.IdDelegacion = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            delegacionOficina.Delegacion = reader["nombreOficina"].ToString();
                            delegacionOficina.FechaActualizacion = (reader["FechaActualizacion"] as DateTime?) ?? DateTime.MinValue;
                            delegacionOficina.estatusDesc = reader["estatusDesc"].ToString();
                            delegacionOficina.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            //delegacionOficina.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaDelegacionsOficinas.Add(delegacionOficina);

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
            return ListaDelegacionsOficinas;


        }

        public string GetDelegacionOficinaById(int idOficina)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT TOP 1 nombreOficina FROM catDelegacionesOficinasTransporte WHERE idOficinaTransporte = @IdOficina  ORDER BY nombreOficina ASC;", connection);
                    command.Parameters.AddWithValue("@IdOficina", idOficina);
                    command.CommandType = CommandType.Text;

                    var nombreOficina = command.ExecuteScalar()?.ToString();

                    return nombreOficina;
                }
                catch (SqlException ex)
                {
                    // Manejar la excepción, por ejemplo, escribir en un registro de errores
                    // ex
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
       
        public int EditarDelegacion(CatDelegacionesOficinasTransporteModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catDelegacionesOficinasTransporte set jefeOficina=@jefeOficina WHERE idOficinaTransporte = @idOficinaTransporte",connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = model.IdOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@jefeOficina", SqlDbType.NVarChar)).Value = model.JefeOficina;        
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

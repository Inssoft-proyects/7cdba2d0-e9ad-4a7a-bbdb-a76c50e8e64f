﻿using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatCarreterasService : ICatCarreterasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatCarreterasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatCarreterasModel> ObtenerCarreteras()
        {
            //
            List<CatCarreterasModel> ListaCarreteras = new List<CatCarreterasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT c.*, e.estatusDesc,del.nombreOficina FROM catCarreteras AS c INNER JOIN estatus AS e ON c.estatus = e.estatus" +
                                                       " INNER JOIN catDelegacionesOficinasTransporte AS del ON c.idOficinaTransporte = del.idOficinaTransporte" +
                                                       " ORDER BY Carretera ASC;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {

                        //command.Parameters.Add(new SqlParameter("@FechaIngreso", SqlDbType.DateTime)).Value = model.FechaIngreso == DateTime.MinValue ? new DateTime(1800, 01, 01) : (object)model.FechaIngreso;
                        while (reader.Read())
                        {
                            CatCarreterasModel carretera = new CatCarreterasModel();
                            carretera.IdCarretera = reader["IdCarretera"] != DBNull.Value ? Convert.ToInt32(reader["IdCarretera"]) : 0;
                            carretera.idOficinaTransporte = reader["idOficinaTransporte"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaTransporte"]) : 0;
                            carretera.Carretera = reader["Carretera"] != DBNull.Value ? reader["Carretera"].ToString() : string.Empty;
                            carretera.nombreOficina = reader["nombreOficina"] != DBNull.Value ? reader["nombreOficina"].ToString() : string.Empty;
                            carretera.estatusDesc = reader["estatusDesc"] != DBNull.Value ? reader["estatusDesc"].ToString() : string.Empty;
                            carretera.FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"]) : DateTime.MinValue;
                            carretera.Estatus = reader["estatus"] != DBNull.Value ? Convert.ToInt32(reader["estatus"]) : 0;
                            carretera.ActualizadoPor = reader["ActualizadoPor"] != DBNull.Value ? Convert.ToInt32(reader["ActualizadoPor"]) : 0;

                            ListaCarreteras.Add(carretera);

                        }

                    }

                }
                catch (SqlException ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            return ListaCarreteras;


        }
        public CatCarreterasModel ObtenerCarreteraByID(int IdCarretera)
        {
            CatCarreterasModel carretera = new CatCarreterasModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT c.*, e.estatusdesc, del.nombreOficina FROM catCarreteras AS c INNER JOIN estatus AS e ON c.estatus = e.estatus INNER JOIN catDelegacionesOficinasTransporte AS del ON c.idOficinaTransporte = del.idOficinaTransporte WHERE c.idCarretera=@IdCarretera", connection);
                    command.Parameters.Add(new SqlParameter("@IdCarretera", SqlDbType.Int)).Value = IdCarretera;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            carretera.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
                            carretera.idOficinaTransporte = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            carretera.Carretera = reader["Carretera"].ToString();
                            carretera.nombreOficina = reader["nombreOficina"].ToString();

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

            return carretera;
        }
        public int CrearCarretera(CatCarreterasModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catCarreteras(carretera,estatus,fechaActualizacion,actualizadoPor,idOficinaTransporte) values(@Carretera,@estatus,@fechaActualizacion,@actualizadoPor,@idOficinaTransporte)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@carretera", SqlDbType.VarChar)).Value = model.Carretera;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = model.idOficinaTransporte;
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
        public int EditarCarretera(CatCarreterasModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update catCarreteras set carretera=@carretera, estatus = @estatus,fechaActualizacion = @fechaActualizacion, actualizadoPor =@actualizadoPor, idOficinaTransporte =@idOficinaTransporte where idCarretera=@idCarretera",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = model.IdCarretera;
                    sqlCommand.Parameters.Add(new SqlParameter("@idOficinaTransporte", SqlDbType.Int)).Value = model.idOficinaTransporte;
                    sqlCommand.Parameters.Add(new SqlParameter("@carretera", SqlDbType.NVarChar)).Value = model.Carretera;
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

        public List<CatCarreterasModel> GetCarreterasPorDelegacion(int idOficina)
        {
            //
            List<CatCarreterasModel> ListaCarreteras = new List<CatCarreterasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT c.idCarretera,c.idOficinaTransporte,UPPER(c.carretera) AS carretera,
                                                        c.estatus,c.FechaActualizacion,c.ActualizadoPor,e.estatus 
                                                        FROM catCarreteras AS c LEFT JOIN estatus AS e ON c.estatus = e.estatus 
                                                        WHERE c.idOficinaTransporte = @idOficina OR c.idOficinaTransporte = 1 AND c.estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCarreterasModel carretera = new CatCarreterasModel();
                            carretera.IdCarretera = Convert.ToInt32(reader["idCarretera"].ToString());
                            carretera.idOficinaTransporte = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            carretera.Carretera = reader["carretera"].ToString();
                            carretera.estatusDesc = reader["estatus"].ToString();
                            carretera.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"] is DBNull ? DateTime.MinValue : reader["FechaActualizacion"]);
                            carretera.Estatus = Convert.ToInt32(reader["estatus"] is DBNull ? 0 : reader["estatus"]);
                            carretera.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"] is DBNull ? 0 : reader["ActualizadoPor"]);
                            ListaCarreteras.Add(carretera);

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
            return ListaCarreteras;


        }


    }
}




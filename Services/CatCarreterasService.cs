using GuanajuatoAdminUsuarios.Interfaces;
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
                    SqlCommand command = new SqlCommand("SELECT c.*, e.estatusDesc,del.nombreOficina FROM catCarreteras AS c INNER JOIN estatus AS e ON c.estatus = e.estatus INNER JOIN catDelegacionesOficinasTransporte AS del ON c.idOficinaTransporte = del.idOficinaTransporte;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCarreterasModel carretera = new CatCarreterasModel();
                            carretera.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
                            carretera.idOficinaTransporte = Convert.ToInt32(reader["idOficinaTransporte"].ToString());
                            carretera.Carretera = reader["Carretera"].ToString();
                            carretera.nombreOficina = reader["nombreOficina"].ToString();
                            carretera.estatusDesc = reader["estatusDesc"].ToString();
                            carretera.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            carretera.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            carretera.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
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

    }
}




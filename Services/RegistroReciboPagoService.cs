using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class RegistroReciboPagoService : IRegistroReciboPagoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public RegistroReciboPagoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<RegistroReciboPagoModel> ObtInfracciones(string FolioInfraccion)
        {
            //
            List<RegistroReciboPagoModel> ListaInfracciones = new List<RegistroReciboPagoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT i.*, p1.nombre AS nombre1, p1.apellidoPaterno AS apellidoPaterno1, p1.apellidoMaterno AS apellidoMaterno1, " +
                        "p2.nombre AS nombre2, p2.apellidoPaterno AS apellidoPaterno2, p2.apellidoMaterno AS apellidoMaterno2 " +
                        "FROM infracciones AS i INNER JOIN personas AS p1 ON i.idPersonaInfraccion = p1.idPersona " +
                        "LEFT JOIN personas AS p2 ON i.idPersona = p2.idPersona  where folioInfraccion = @FolioInfraccion", connection);

                    command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = FolioInfraccion;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            RegistroReciboPagoModel infraccion = new RegistroReciboPagoModel();
                            infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.FolioInfraccion = reader["folioInfraccion"].ToString();
                            infraccion.Placas = reader["placasVehiculo"].ToString();
                            infraccion.FechaInfraccion = Convert.ToDateTime(reader["FechaInfraccion"].ToString());
                            infraccion.Conductor = $"{reader["nombre1"]} {reader["apellidoPaterno1"]} {reader["apellidoMaterno1"]}";
                            //infraccion.Serie = reader["Serie"].ToString();
                            infraccion.Propietario = $"{reader["nombre2"]} {reader["apellidoPaterno2"]} {reader["apellidoMaterno2"]}";
                            infraccion.EstatusProceso = Convert.IsDBNull(reader["EstatusProceso"]) ? 0 : Convert.ToInt32(reader["EstatusProceso"]);

                            ListaInfracciones.Add(infraccion);

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
            return ListaInfracciones;


        }

        public RegistroReciboPagoModel ObtenerDetallePorId(int Id)
        {

            RegistroReciboPagoModel infraccion = new RegistroReciboPagoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT i.*, p1.nombre AS nombre1, p1.apellidoPaterno AS apellidoPaterno1, p1.apellidoMaterno AS apellidoMaterno1, " +
                        "p2.nombre AS nombre2, p2.apellidoPaterno AS apellidoPaterno2, p2.apellidoMaterno AS apellidoMaterno2 " +
                        "FROM infracciones AS i INNER JOIN personas AS p1 ON i.idPersonaInfraccion = p1.idPersona " +
                        "INNER JOIN personas AS p2 ON i.idPersona = p2.idPersona WHERE i.IdInfraccion = @Id;", connection);
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)).Value = Id;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.FolioInfraccion = reader["FolioInfraccion"].ToString();
                            infraccion.FechaInfraccion = Convert.ToDateTime(reader["FechaInfraccion"].ToString());
                            infraccion.Conductor = $"{reader["nombre1"]} {reader["apellidoPaterno1"]} {reader["apellidoMaterno1"]}";
                            infraccion.Placas = reader["placasVehiculo"].ToString();
                            //infraccion.Serie = reader["Serie"].ToString();
                            infraccion.Propietario = $"{reader["nombre2"]} {reader["apellidoPaterno2"]} {reader["apellidoMaterno2"]}";
                            infraccion.EstatusProceso = Convert.IsDBNull(reader["EstatusProceso"]) ? 0 : Convert.ToInt32(reader["EstatusProceso"]);

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
            return infraccion;


        }
        public int GuardarRecibo(string ReciboPago, float Monto, DateTime FechaPago, string LugarPago, int IdInfraccion)
        {
            int infraccionModificada = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE infracciones SET reciboPago = @reciboPago, monto = @monto, fechaPago = @fechaPago, lugarPago = @lugarPago WHERE idInfraccion = @idInfraccion";


                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@reciboPago", ReciboPago);
                    command.Parameters.AddWithValue("@monto", Monto);
                    command.Parameters.AddWithValue("@fechaPago", FechaPago);
                    command.Parameters.AddWithValue("@lugarPago", LugarPago != null ? LugarPago : DBNull.Value);
                    command.Parameters.AddWithValue("@idInfraccion", IdInfraccion);
                    command.ExecuteNonQuery();
                }



                catch (SqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    connection.Close();
                }
            }

            return infraccionModificada;
        }

    }
}

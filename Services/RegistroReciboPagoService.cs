using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;
using System.Globalization;

namespace GuanajuatoAdminUsuarios.Services
{
    public class RegistroReciboPagoService : IRegistroReciboPagoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IInfraccionesService _infraccionesService;
        public RegistroReciboPagoService(ISqlClientConnectionBD sqlClientConnectionBD, IInfraccionesService infraccionesService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _infraccionesService = infraccionesService;
        }

        public List<RegistroReciboPagoModel> ObtInfracciones(string FolioInfraccion)
        {
            //
            List<RegistroReciboPagoModel> ListaInfracciones = new List<RegistroReciboPagoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT i.*, v.serie, pveh.nombre AS nombre1, pveh.apellidoPaterno AS apellidoPaterno1,
                                                            pveh.apellidoMaterno AS apellidoMaterno1, pinf.nombre AS nombre2, 
                                                            pinf.apellidoPaterno AS apellidoPaterno2, pinf.apellidoMaterno AS apellidoMaterno2,
                                                            e.estatusInfraccion, cde.delegacion
                                                        FROM infracciones AS i 
                                                        JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion
                                                        JOIN vehiculos AS v ON v.idVehiculo = i.idVehiculo
                                                        LEFT JOIN personas AS pveh ON pveh.idPersona = v.idPersona
                                                        LEFT JOIN personas AS pinf ON i.idPersona = pinf.idPersona
                                                        LEFT JOIN catDelegaciones AS cde ON cde.idDelegacion = i.idDelegacion 
                                                        WHERE folioInfraccion = @FolioInfraccion", connection);

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
                            infraccion.Propietario = $"{reader["nombre1"]} {reader["apellidoPaterno1"]} {reader["apellidoMaterno1"]}";
                            infraccion.Serie = reader["Serie"] is DBNull ? string.Empty : reader["Serie"].ToString();
                            infraccion.Conductor = $"{reader["nombre2"]} {reader["apellidoPaterno2"]} {reader["apellidoMaterno2"]}";
                            infraccion.Delegacion = reader["delegacion"].ToString();
                            infraccion.EstatusInfraccion = reader["estatusInfraccion"].ToString();

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
                    SqlCommand command = new SqlCommand(@"SELECT i.*, pveh.nombre AS nombre1, pveh.apellidoPaterno AS apellidoPaterno1,
                                                            pveh.apellidoMaterno AS apellidoMaterno1, pinf.nombre AS nombre2,i.lugarPago, 
                                                            pinf.apellidoPaterno AS apellidoPaterno2, pinf.apellidoMaterno AS apellidoMaterno2,
                                                            e.estatusInfraccion, sum(mi.calificacion ) calificacion 
                                                        FROM infracciones AS i 
                                                        JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion
                                                        JOIN vehiculos AS v ON v.idVehiculo = i.idVehiculo
                                                        LEFT JOIN motivosInfraccion mi ON mi.idInfraccion = i.idInfraccion
                                                        LEFT JOIN personas AS pveh ON pveh.idPersona = v.idPersona
                                                        LEFT JOIN personas AS pinf ON i.idPersona = pinf.idPersona  
                                                        WHERE i.IdInfraccion = @Id 
                                                        group by i.idInfraccion, i.idOficial,i.idDependencia, i.idDelegacion, i.idVehiculo,
                                                        i.idAplicacion, i.idGarantia, i.idEstatusInfraccion, i.idMunicipio, i.idTramo,
                                                        i.idCarretera, i.idPersona, i.idPersonaInfraccion, i.placasVehiculo, i.folioInfraccion,
                                                        i.fechaInfraccion, i.kmCarretera, i.observaciones, i.lugarCalle, i.lugarNumero,
                                                        i.lugarColonia, i.lugarEntreCalle, infraccionCortesia,i.NumTarjetaCirculacion,
                                                        i.oficioRevocacion,i.estatusProceso,i.fechaActualizacion,i.actualizadoPor,
                                                        i.estatus,i.reciboPago,i.monto,i.fechaPago,i.lugarPago,i.idEstatusEnvio,
                                                        i.oficioEnvio,i.fechaEnvio,i.idOficinaRenta,i.inventario,i.partner,
                                                        i.cuenta,i.objeto,i.documento,pveh.nombre , pveh.apellidoPaterno,
                                                        pveh.apellidoMaterno , pinf.nombre , 
                                                        pinf.apellidoPaterno, pinf.apellidoMaterno,
                                                        e.estatusInfraccion
                                                        ;", connection);
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
                            infraccion.LugarPago = reader["lugarPago"].ToString();
                            
                            infraccion.Propietario = $"{reader["nombre2"]} {reader["apellidoPaterno2"]} {reader["apellidoMaterno2"]}";
                            infraccion.EstatusProceso = Convert.IsDBNull(reader["EstatusProceso"]) ? 0 : Convert.ToInt32(reader["EstatusProceso"]);
                            infraccion.Calificacion = Convert.ToInt32(reader["calificacion"].ToString());
                            infraccion.Monto = (float?) infraccion.Calificacion *  (float?)_infraccionesService.GetUmas(); ;
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

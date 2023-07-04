using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;
using Microsoft.Extensions.DependencyModel;

namespace GuanajuatoAdminUsuarios.Services
{

    public class CancelarInfraccionService : ICancelarInfraccionService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CancelarInfraccionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CancelarInfraccionModel> ObtenerInfraccionPorFolio(string FolioInfraccion)
        {
            //
            List<CancelarInfraccionModel> ListaInfracciones = new List<CancelarInfraccionModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT i.*, v.serie, e.estatusInfraccion, CONCAT( p.nombre,' ', p.apellidoPaterno,' ', p.apellidoMaterno)AS nombrePropietario,CONCAT(pi.nombre,' ',pi.apellidoPaterno,' ', pi.apellidoMaterno)AS nombreConductor FROM infracciones AS i INNER JOIN vehiculos AS v ON i.idVehiculo = v.idVehiculo INNER JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion INNER JOIN personas AS p ON i.IdPersona = p.IdPersona INNER JOIN personasInfracciones  AS pi ON i.idPersonaInfraccion = pi.idPersonaInfraccion WHERE i.FolioInfraccion LIKE '%' + @FolioInfraccion + '%' AND i.idEstatusInfraccion = 1;", connection);
                    command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = FolioInfraccion;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CancelarInfraccionModel infraccion = new CancelarInfraccionModel();
                            infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.FolioInfraccion = reader["FolioInfraccion"].ToString();
                            infraccion.FechaInfraccion = Convert.ToDateTime(reader["FechaInfraccion"].ToString());
                            infraccion.Conductor = reader["nombreConductor"].ToString();
                            infraccion.Placas = reader["placasVehiculo"].ToString();
                            infraccion.Serie = reader["Serie"].ToString();
                            infraccion.Propietario = reader["nombrePropietario"].ToString();
                            infraccion.EstatusProceso = Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
                            infraccion.descEstatusProceso = reader["estatusInfraccion"].ToString();


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



        public CancelarInfraccionModel ObtenerDetalleInfraccion(int Id)
        {

            CancelarInfraccionModel infraccion = new CancelarInfraccionModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT i.IdInfraccion, i.FolioInfraccion, i.FechaInfraccion," +
                   " i.placasVehiculo, i.idPersona, i.idEstatusInfraccion, v.Serie, e.estatusInfraccion, CONCAT(p.nombre, ' ', p.apellidoPaterno, ' ', p.apellidoMaterno) AS nombrePropietario, CONCAT(pi.nombre, ' ', pi.apellidoPaterno, ' ', pi.apellidoMaterno) AS nombreConductor" +
                   " FROM infracciones i" +
                   " JOIN vehiculos v ON i.idVehiculo = v.idVehiculo" +
                   " JOIN catEstatusInfraccion AS e ON i.idEstatusInfraccion = e.idEstatusInfraccion" +
                   " JOIN personas AS p ON i.idPersona = p.idPersona" +
                   " JOIN personasInfracciones AS pi ON i.idPersonaInfraccion = pi.idPersonaInfraccion" +
                   " WHERE i.IdInfraccion = @Id", connection);
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)).Value = Id;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.FolioInfraccion = reader["FolioInfraccion"].ToString();
                            infraccion.FechaInfraccion = Convert.ToDateTime(reader["FechaInfraccion"].ToString());
                            infraccion.Conductor = reader["nombreConductor"].ToString();
                            infraccion.Placas = reader["placasVehiculo"].ToString();
                            infraccion.Serie = reader["Serie"].ToString();
                            infraccion.Propietario = reader["nombrePropietario"].ToString();
                            infraccion.descEstatusProceso = reader["estatusInfraccion"].ToString();

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
        public CancelarInfraccionModel CancelarInfraccionBD(int IdInfraccion,string OficioRevocacion)
        {
            int result = 0;

            CancelarInfraccionModel infraccion = new CancelarInfraccionModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Update infracciones set idEstatusInfraccion = @idEstatusInfraccion, oficioRevocacion = @OficioRevocacion,fechaActualizacion = @fechaActualizacion,actualizadoPor = @actualizadoPor, estatus = @estatus where idInfraccion = @IdInfraccion", connection);
                    command.Parameters.Add(new SqlParameter("@IdInfraccion", SqlDbType.Int)).Value = IdInfraccion;
                    command.Parameters.Add(new SqlParameter("@OficioRevocacion", SqlDbType.VarChar)).Value = OficioRevocacion;
                    command.Parameters.Add(new SqlParameter("@idEstatusInfraccion", SqlDbType.Int)).Value = 4;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;

                    command.CommandType = CommandType.Text;
                    result = command.ExecuteNonQuery();
                    infraccion.EstatusProceso = 2;
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

    }
}




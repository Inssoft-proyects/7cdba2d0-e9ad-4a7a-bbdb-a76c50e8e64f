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
                    SqlCommand command = new SqlCommand("Select * from infracciones where FolioInfraccion = @FolioInfraccion", connection);
                    command.Parameters.Add(new SqlParameter("@FolioInfraccion", SqlDbType.NVarChar)).Value = FolioInfraccion;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            RegistroReciboPagoModel infraccion = new RegistroReciboPagoModel();
                            infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.FolioInfraccion = reader["FolioInfraccion"].ToString();
                            infraccion.Placas = reader["Placas"].ToString();
                            infraccion.EstatusProceso = Convert.ToInt32(reader["EstatusProceso"].ToString());

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
                    SqlCommand command = new SqlCommand("Select * from infracciones where IdInfraccion = @Id", connection);
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int)).Value = Id;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            infraccion.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            infraccion.FolioInfraccion = reader["FolioInfraccion"].ToString();
                            infraccion.FechaInfraccion = Convert.ToDateTime(reader["FechaInfraccion"].ToString());
                            infraccion.Conductor = reader["Conductor"].ToString();
                            infraccion.Placas = reader["Placas"].ToString();
                            //infraccion.Serie = reader["Serie"].ToString();
                            infraccion.Propietario = reader["Propietario"].ToString();
                            infraccion.EstatusProceso = Convert.ToInt32(reader["EstatusProceso"].ToString());

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

    }
}

using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net.NetworkInformation;

namespace GuanajuatoAdminUsuarios.Services
{
    public class LiberacionVehiculoService : ILiberacionVehiculoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public LiberacionVehiculoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<LiberacionVehiculoModel> GetAllTopDepositos()
        {
            List<LiberacionVehiculoModel> depositosList = new List<LiberacionVehiculoModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"select   top(100) d.IdDeposito,d.IdSolicitud,d.idDelegacion,d.IdMarca,d.IdSubmarca,d.IdPension,d.IdTramo,
                            d.IdColor,d.Serie,d.Placa,d.FechaIngreso,d.Folio,d.Km,d.Liberado,d.Autoriza,d.FechaActualizacion,
                            d.ActualizadoPor, d.estatus, sol.solicitanteNombre,
                            sol.solicitanteAp,sol.solicitanteAm,pen.pension	,del.delegacion,col.color,
                            cTra.tramo, m.marcaVehiculo	,subm.nombreSubmarca
                            from depositos d 
                            left join solicitudes sol on d.idSolicitud = sol.idSolicitud
                            left join pensiones pen on d.idPension	= pen.idPension
                            left join catDelegaciones del on d.idDelegacion= del.idDelegacion
                            left join catColores col on d.idColor = col.idColor
                            left join catTramos cTra  on d.Idtramo=cTra.idTramo
                            left join catMarcasVehiculos m on d.idMarca=m.idMarcaVehiculo
                            left join catSubmarcasVehiculos  subm on d.idSubmarca=subm.idSubmarca
                            where d.liberado=0 and d.estatus=1";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            LiberacionVehiculoModel deposito = new LiberacionVehiculoModel();
                            deposito.IdDeposito = reader["IdDeposito"] is int idDeposito ? idDeposito : 0;
                            deposito.IdSolicitud = reader["IdSolicitud"] is int idSolicitud ? idSolicitud : 0;
                            deposito.IdDelegacion = reader["IdDelegacion"] is int idDelegacion ? idDelegacion : 0;
                            deposito.IdMarca = reader["IdMarca"] is int idMarca ? idMarca : 0;
                            deposito.IdSubmarca = reader["IdSubmarca"] is int idSubmarca ? idSubmarca : 0;
                            deposito.IdPension = reader["IdPension"] is int idPension ? idPension : 0;
                            deposito.IdTramo = reader["IdTramo"] is int idTramo ? idTramo : 0;
                            deposito.IdColor = reader["IdColor"] is int idColor ? idColor : 0;

                            deposito.Serie = reader["Serie"]?.ToString();
                            deposito.Placa = reader["Placa"]?.ToString();
                            deposito.Km = reader["Km"]?.ToString();
                            deposito.Liberado = reader["Liberado"] is int liberado ? liberado : 0;

                            deposito.FechaIngreso = reader["FechaIngreso"] is DateTime fechaIngreso ? fechaIngreso : DateTime.MinValue;
                            deposito.Folio = reader["Folio"]?.ToString();
                            deposito.Autoriza = reader["Autoriza"]?.ToString();

                         

                            deposito.marcaVehiculo = reader["marcaVehiculo"]?.ToString();
                            deposito.nombreSubmarca = reader["nombreSubmarca"]?.ToString();
                            deposito.delegacion = reader["delegacion"]?.ToString();
                            deposito.solicitanteNombre = reader["solicitanteNombre"]?.ToString();
                            deposito.solicitanteAp = reader["solicitanteAp"]?.ToString();
                            deposito.solicitanteAm = reader["solicitanteAm"]?.ToString();
                            deposito.Color = reader["Color"]?.ToString();
                            deposito.pension = reader["pension"]?.ToString();
                            deposito.tramo = reader["tramo"]?.ToString();

                            depositosList.Add(deposito);
                           
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
            return depositosList;
        }

        public List<LiberacionVehiculoModel> GetDepositos(LiberacionVehiculoBusquedaModel model)
        {
            List<LiberacionVehiculoModel> depositosList = new List<LiberacionVehiculoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    string condicionFecha = model.FechaIngreso == DateTime.MinValue ? @"d.FechaIngreso >= @FechaIngreso " : @"d.FechaIngreso = @FechaIngreso ";

                    connection.Open();
                    string SqlTransact = string.Format(@"select top(100) d.IdDeposito,d.IdSolicitud,d.idDelegacion,d.IdMarca,d.IdSubmarca,d.IdPension,d.IdTramo,
                                d.IdColor,d.Serie,d.Placa,d.FechaIngreso,d.Folio,d.Km,d.Liberado,d.Autoriza,d.FechaActualizacion,
                                d.ActualizadoPor, d.estatus, sol.solicitanteNombre,
                                sol.solicitanteAp,sol.solicitanteAm,pen.pension	,del.delegacion,col.color,
                                cTra.tramo, m.marcaVehiculo	,subm.nombreSubmarca
                                from depositos d 
                                inner join solicitudes sol on d.idSolicitud = sol.idSolicitud
                                inner join pensiones pen on d.idPension	= pen.idPension
                                inner join catDelegaciones del on d.idDelegacion= del.idDelegacion
                                inner join catColores col on d.idColor = col.idColor
                                inner join catTramos cTra  on d.Idtramo=cTra.idTramo
                                inner join catMarcasVehiculos m on d.idMarca=m.idMarcaVehiculo
                                inner join catSubmarcasVehiculos  subm on d.idSubmarca=subm.idSubmarca
                                where d.liberado=0 and d.estatus=1	and
		                        (d.IdDeposito=@IdDeposito  OR d.IdMarca=@IdMarca 
		                        OR d.Serie LIKE '%' + @Serie + '%' OR  {0} 
		                        OR d.Folio LIKE '%' + @Folio + '%')", condicionFecha);


                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdDeposito", SqlDbType.Int)).Value = (object)model.IdDeposito ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdMarca", SqlDbType.Int)).Value = (object)model.IdMarcaVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Serie", SqlDbType.NVarChar)).Value = (object)model.Serie ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FechaIngreso", SqlDbType.DateTime)).Value = model.FechaIngreso == DateTime.MinValue ? new DateTime(1800,01,01) : (object)model.FechaIngreso;
                    command.Parameters.Add(new SqlParameter("@Folio", SqlDbType.NVarChar)).Value = (object)model.Folio ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            LiberacionVehiculoModel deposito = new LiberacionVehiculoModel();
                            deposito.IdDeposito = reader["IdDeposito"] != DBNull.Value ? Convert.ToInt32(reader["IdDeposito"]) : 0;
                            deposito.IdSolicitud = reader["IdSolicitud"] != DBNull.Value ? Convert.ToInt32(reader["IdSolicitud"]) : 0;
                            deposito.IdDelegacion = reader["IdDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["IdDelegacion"]) : 0;
                            deposito.IdMarca = reader["IdMarca"] != DBNull.Value ? Convert.ToInt32(reader["IdMarca"]) : 0;
                            deposito.IdSubmarca = reader["IdSubmarca"] != DBNull.Value ? Convert.ToInt32(reader["IdSubmarca"]) : 0;
                            deposito.IdPension = reader["IdPension"] != DBNull.Value ? Convert.ToInt32(reader["IdPension"]) : 0;
                            deposito.IdTramo = reader["IdTramo"] != DBNull.Value ? Convert.ToInt32(reader["IdTramo"]) : 0;
                            deposito.IdColor = reader["IdColor"] != DBNull.Value ? Convert.ToInt32(reader["IdColor"]) : 0;

                            deposito.Serie = reader["Serie"] != DBNull.Value ? reader["Serie"].ToString() : string.Empty;
                            deposito.Placa = reader["Placa"] != DBNull.Value ? reader["Placa"].ToString() : string.Empty;
                            deposito.FechaIngreso = reader["FechaIngreso"] != DBNull.Value ? Convert.ToDateTime(reader["FechaIngreso"]) : DateTime.MinValue;
                            deposito.Folio = reader["Folio"] != DBNull.Value ? reader["Folio"].ToString() : string.Empty;
                            deposito.Km = reader["Km"] != DBNull.Value ? reader["Km"].ToString() : string.Empty;
                            deposito.Liberado = reader["Liberado"] != DBNull.Value ? Convert.ToInt32(reader["Liberado"]) : 0;

                           // deposito.AcreditacionPropiedad = reader["AcreditacionPropiedad"] != DBNull.Value ? reader["AcreditacionPropiedad"].ToString() : null;
                           // deposito.AcreditacionPersonalidad = reader["AcreditacionPersonalidad"] != DBNull.Value ? reader["AcreditacionPersonalidad"].ToString() : null;
                           // deposito.ReciboPago = reader["ReciboPago"] != DBNull.Value ? reader["ReciboPago"].ToString() : null;
                           // deposito.Observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : null;

                            deposito.Autoriza = reader["Autoriza"] != DBNull.Value ? reader["Autoriza"].ToString() : string.Empty;
                            deposito.FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"]) : DateTime.MinValue;
                            deposito.ActualizadoPor = reader["ActualizadoPor"] != DBNull.Value ? Convert.ToInt32(reader["ActualizadoPor"]) : 0;
                            deposito.Estatus = reader["Estatus"] != DBNull.Value ? Convert.ToInt32(reader["Estatus"]) : 0;

                            deposito.marcaVehiculo = reader["marcaVehiculo"] != DBNull.Value ? reader["marcaVehiculo"].ToString() : null;
                            deposito.nombreSubmarca = reader["nombreSubmarca"] != DBNull.Value ? reader["nombreSubmarca"].ToString() : null;
                            deposito.delegacion = reader["delegacion"] != DBNull.Value ? reader["delegacion"].ToString() : null;
                            deposito.solicitanteNombre = reader["solicitanteNombre"] != DBNull.Value ? reader["solicitanteNombre"].ToString() : null;
                            deposito.solicitanteAp = reader["solicitanteAp"] != DBNull.Value ? reader["solicitanteAp"].ToString() : null;
                            deposito.solicitanteAm = reader["solicitanteAm"] != DBNull.Value ? reader["solicitanteAm"].ToString() : null;
                            deposito.Color = reader["Color"] != DBNull.Value ? reader["Color"].ToString() : null;
                            deposito.pension = reader["pension"] != DBNull.Value ? reader["pension"].ToString() : null;
                            deposito.tramo = reader["tramo"] != DBNull.Value ? reader["tramo"].ToString() : null;

                            depositosList.Add(deposito);
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
            return depositosList;
        }

        public LiberacionVehiculoModel GetDepositoByID(int Id)
        {
            LiberacionVehiculoModel deposito = new LiberacionVehiculoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"select d.IdDeposito,d.IdSolicitud,d.idDelegacion,d.IdMarca,d.IdSubmarca,d.IdPension,d.IdTramo,
		                d.IdColor,d.Serie,d.Placa,d.FechaIngreso,d.Folio,d.Km,d.Liberado,d.Autoriza,d.FechaActualizacion,
		                del.delegacion, d.ActualizadoPor, d.estatus, m.marcaVehiculo,subm.nombreSubmarca, sol.solicitanteNombre,
						sol.solicitanteAp,sol.solicitanteAm,col.color,pen.pension, cTra.tramo
		                from depositos d left join catDelegaciones del on d.idDelegacion= del.idDelegacion
		                left join catMarcasVehiculos m on d.idMarca=m.idMarcaVehiculo
		                left join catSubmarcasVehiculos  subm on m.idMarcaVehiculo=subm.idMarcaVehiculo
						left join solicitudes sol on d.idSolicitud = sol.idSolicitud
						left join catColores col on d.idColor = col.idColor
	                    left join pensiones pen on d.idPension	= pen.idPension
                        left join catTramos cTra  on d.Idtramo=cTra.idTramo
		                where d.liberado=0 and d.estatus=1 and d.IdDeposito=@IdDeposito";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdDeposito", SqlDbType.Int)).Value = (object)Id ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            deposito.IdDeposito = reader["IdDeposito"] as int? ?? 0;
                            deposito.IdSolicitud = reader["IdSolicitud"] as int? ?? 0;
                            deposito.IdDelegacion = reader["IdDelegacion"] as int? ?? 0;
                            deposito.IdMarca = reader["IdMarca"] as int? ?? 0;
                            deposito.IdSubmarca = reader["IdSubmarca"] as int? ?? 0;
                            deposito.IdPension = reader["IdPension"] as int? ?? 0;
                            deposito.IdTramo = reader["IdTramo"] as int? ?? 0;
                            deposito.IdColor = reader["IdColor"] as int? ?? 0;
                            deposito.Serie = reader["Serie"]?.ToString();
                            deposito.Placa = reader["Placa"]?.ToString();
                            deposito.FechaIngreso = reader["FechaIngreso"] as DateTime? ?? DateTime.MinValue;
                            deposito.Folio = reader["Folio"]?.ToString();
                            deposito.Km = reader["Km"]?.ToString();
                            deposito.Liberado = reader["Liberado"] as int? ?? 0;
                            deposito.Autoriza = reader["Autoriza"]?.ToString();
                            deposito.FechaActualizacion = reader["FechaActualizacion"] as DateTime? ?? DateTime.MinValue;
                            deposito.ActualizadoPor = reader["ActualizadoPor"] as int? ?? 0;
                            deposito.Estatus = reader["Estatus"] as int? ?? 0;
                            deposito.marcaVehiculo = reader["marcaVehiculo"]?.ToString();
                            deposito.nombreSubmarca = reader["nombreSubmarca"]?.ToString();
                            deposito.delegacion = reader["delegacion"]?.ToString();
                            deposito.solicitanteNombre = reader["solicitanteNombre"]?.ToString();
                            deposito.solicitanteAp = reader["solicitanteAp"]?.ToString();
                            deposito.solicitanteAm = reader["solicitanteAm"]?.ToString();
                            deposito.Color = reader["Color"]?.ToString();
                            deposito.pension = reader["pension"]?.ToString();
                            deposito.tramo = reader["tramo"]?.ToString();

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
            return deposito;
        }

        public int UpdateDeposito(LiberacionVehiculoModel model)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"Update depositos set AcreditacionPropiedad=@AcreditacionPropiedad,AcreditacionPersonalidad=@AcreditacionPersonalidad,
                          ReciboPago=@ReciboPago, Observaciones=@Observaciones, Autoriza=@Autoriza,Liberado=@liberado,FechaActualizacion=@FechaActualizacion,
                          FechaLiberacion=@FechaLiberacion
                          where IdDeposito=@IdDeposito";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdDeposito", SqlDbType.Int)).Value = (object)model.IdDeposito ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@AcreditacionPropiedad", SqlDbType.Image)).Value = (object)model.AcreditacionPropiedad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@AcreditacionPersonalidad", SqlDbType.Image)).Value = (object)model.AcreditacionPersonalidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@ReciboPago", SqlDbType.Image)).Value = (object)model.ReciboPago ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Observaciones", SqlDbType.Text)).Value = (object)model.Observaciones ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Autoriza", SqlDbType.NVarChar)).Value = (object)model.Autoriza ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Liberado", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@FechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@FechaLiberacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    command.CommandType = CommandType.Text;
                    result = command.ExecuteNonQuery();
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

    }
}

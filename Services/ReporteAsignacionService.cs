using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class ReporteAsignacionService : IReporteAsignacionService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public ReporteAsignacionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<ReporteAsignacionModel> GetAllReporteAsignaciones()
        {
            List<ReporteAsignacionModel> ReporteAsignacionesList = new List<ReporteAsignacionModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"select dep.iddeposito,dep.idsolicitud,dep.idDelegacion,dep.idmarca,dep.idsubmarca,dep.idpension,dep.idtramo,
                            dep.idcolor,dep.serie,dep.placa,dep.fechaingreso,dep.folio,dep.km,dep.liberado,dep.autoriza,dep.fechaactualizacion,
                            del.delegacion, dep.actualizadopor, dep.estatus, dep.FechaLiberacion,
                            dep.IdDependenciaGenera,dep.IdDependenciaTransito,dep.IdDependenciaNoTransito,
                            sol.solicitantenombre,sol.solicitanteap,sol.solicitanteam,pen.pension, sol.vehiculoCarretera,
                            sol.vehiculoTramo,sol.vehiculoKm, sol.fechasolicitud, sol.folio as FolioSolicitud,sol.evento,
                            sol.solicitanteColonia,sol.solicitanteCalle,sol.solicitanteNumero, sol.tipoVehiculo,
                            sol.oficial,sol.folio,sol.propietarioGrua,
                            g.IdGrua,g.noEconomico,
                            con.IdConcesionario, con.concesionario

                            from depositos dep 
                            inner join catDelegaciones del on dep.idDelegacion= del.idDelegacion
                            inner join pensiones pen on dep.idpension	= pen.idpension
                            inner join solicitudes sol on dep.idsolicitud = sol.idsolicitud
                            inner join concesionarios con on  con.IdConcesionario = dep.IdConcesionario
                            inner join gruas g  on g.idConcesionario = con.idConcesionario";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReporteAsignacionModel ReporteAsignacion = new ReporteAsignacionModel();
                            ReporteAsignacion.idSolicitud = Convert.ToInt32(reader["idSolicitud"].ToString());
                            ReporteAsignacion.vehiculoCarretera = reader["vehiculoCarretera"].ToString();
                            ReporteAsignacion.vehiculoTramo = reader["vehiculoTramo"].ToString();
                            ReporteAsignacion.vehiculoKm = reader["vehiculoKm"].ToString();
                            ReporteAsignacion.fechaSolicitud = reader["fechaLiberacion"] != DBNull.Value ? Convert.ToDateTime(reader["fechaSolicitud"].ToString()): DateTime.MinValue;
                            ReporteAsignacion.evento = reader["evento"].ToString();
                            ReporteAsignacion.solicitanteNombre = reader["solicitanteNombre"].ToString();
                            ReporteAsignacion.solicitanteAp = reader["solicitanteAp"].ToString();
                            ReporteAsignacion.solicitanteAm = reader["solicitanteAm"].ToString();
                            ReporteAsignacion.solicitanteColonia = reader["solicitanteColonia"].ToString();
                            ReporteAsignacion.solicitanteCalle = reader["solicitanteCalle"].ToString();
                            ReporteAsignacion.solicitanteNumero = reader["solicitanteNumero"].ToString();
                            ReporteAsignacion.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            ReporteAsignacion.propietarioGrua = reader["propietarioGrua"].ToString();
                            ReporteAsignacion.oficial = reader["oficial"].ToString();
                            ReporteAsignacion.folio = reader["folio"].ToString();
                            ReporteAsignacion.vehiculoPension = reader["pension"].ToString();
                            ReporteAsignacion.fechaLiberacion = reader["fechaLiberacion"] != DBNull.Value ? Convert.ToDateTime(reader["fechaLiberacion"].ToString()) : DateTime.MinValue ;
                            ReporteAsignacion.IdGrua = Convert.ToInt32(reader["IdGrua"].ToString());
                            ReporteAsignacion.noEconomico = reader["folio"].ToString();
                            ReporteAsignacion.Delegacion = reader["Delegacion"].ToString();
                            ReporteAsignacion.Alias = reader["concesionario"].ToString();
                            ReporteAsignacionesList.Add(ReporteAsignacion);

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
            return ReporteAsignacionesList;
        }

        public List<ReporteAsignacionModel> GetAllReporteAsignaciones(ReporteAsignacionBusquedaModel model)
        {
            List<ReporteAsignacionModel> ReporteAsignacionesList = new List<ReporteAsignacionModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"select dep.iddeposito,dep.idsolicitud,dep.idDelegacion,dep.idmarca,dep.idsubmarca,dep.idpension,dep.idtramo,
                            dep.idcolor,dep.serie,dep.placa,dep.fechaingreso,dep.folio,dep.km,dep.liberado,dep.autoriza,dep.fechaactualizacion,
                            del.delegacion, dep.actualizadopor, dep.estatus, dep.FechaLiberacion,
                            dep.IdDependenciaGenera,dep.IdDependenciaTransito,dep.IdDependenciaNoTransito,
                            sol.solicitantenombre,sol.solicitanteap,sol.solicitanteam,pen.pension, sol.vehiculoCarretera,
                            sol.vehiculoTramo,sol.vehiculoKm, sol.fechasolicitud, sol.folio as FolioSolicitud,sol.evento,
                            sol.solicitanteColonia,sol.solicitanteCalle,sol.solicitanteNumero, sol.tipoVehiculo,
                            sol.oficial,sol.folio,sol.propietarioGrua,
                            g.IdGrua,g.noEconomico,
                            con.IdConcesionario, con.concesionario

                            from depositos dep 
                            inner join catDelegaciones del on dep.idDelegacion= del.idDelegacion
                            inner join pensiones pen on dep.idpension	= pen.idpension
                            inner join solicitudes sol on dep.idsolicitud = sol.idsolicitud
                            inner join concesionarios con on  con.IdConcesionario = dep.IdConcesionario
                            inner join gruas g  on g.idConcesionario = con.idConcesionario 
                            where g.IdGrua=@IdGrua OR pen.idPension=@IdPension
                            OR  dep.fechaIngreso between @FechaIngreso and  @FechaIngresoFin
                            OR 	UPPER(sol.evento)=@Evento";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdGrua", SqlDbType.Int)).Value = (object)model.IdGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdPension", SqlDbType.Int)).Value = (object)model.IdPension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Evento", SqlDbType.NVarChar)).Value = (object)model.Evento != null ? model.Evento.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FechaIngreso", SqlDbType.DateTime)).Value = (object)model.FechaInicio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@FechaIngresoFin", SqlDbType.DateTime)).Value = (object)model.FechaFin ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ReporteAsignacionModel ReporteAsignacion = new ReporteAsignacionModel();
                            ReporteAsignacion.idSolicitud = Convert.ToInt32(reader["idSolicitud"].ToString());
                            ReporteAsignacion.vehiculoCarretera = reader["vehiculoCarretera"].ToString();
                            ReporteAsignacion.vehiculoTramo = reader["vehiculoTramo"].ToString();
                            ReporteAsignacion.vehiculoKm = reader["vehiculoKm"].ToString();
                            ReporteAsignacion.fechaSolicitud = Convert.ToDateTime(reader["fechaSolicitud"].ToString());
                            ReporteAsignacion.evento = reader["evento"].ToString();
                            ReporteAsignacion.solicitanteNombre = reader["solicitanteNombre"].ToString();
                            ReporteAsignacion.solicitanteAp = reader["solicitanteAp"].ToString();
                            ReporteAsignacion.solicitanteAm = reader["solicitanteAm"].ToString();
                            ReporteAsignacion.solicitanteColonia = reader["solicitanteColonia"].ToString();
                            ReporteAsignacion.solicitanteCalle = reader["solicitanteCalle"].ToString();
                            ReporteAsignacion.solicitanteNumero = reader["solicitanteNumero"].ToString();
                            ReporteAsignacion.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            ReporteAsignacion.propietarioGrua = reader["propietarioGrua"].ToString();
                            ReporteAsignacion.oficial = reader["oficial"].ToString();
                            ReporteAsignacion.folio = reader["folio"].ToString();
                            ReporteAsignacion.vehiculoPension = reader["pension"].ToString();
                            ReporteAsignacion.fechaLiberacion = Convert.ToDateTime(reader["fechaLiberacion"].ToString());
                            ReporteAsignacion.IdGrua = Convert.ToInt32(reader["IdGrua"].ToString());
                            ReporteAsignacion.noEconomico = reader["folio"].ToString();
                            ReporteAsignacion.Delegacion = reader["Delegacion"].ToString();
                            ReporteAsignacion.Alias = reader["concesionario"].ToString();
                            ReporteAsignacionesList.Add(ReporteAsignacion);
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
            return ReporteAsignacionesList;

        }


    }
}

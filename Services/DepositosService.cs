using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;
using Microsoft.Identity.Client;

namespace GuanajuatoAdminUsuarios.Services
{
    public class DepositosService : IDepositosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public DepositosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public int GuardarSolicitud(SolicitudDepositoModel model)

        {
            int result = 0;
            int idSolicitudInsert = 0;



            string strQuery = @"INSERT INTO solicitudes( 
                                        [fechaSolicitud]
                                        ,[idTipoVehiculo]
                                        ,[idPropietarioGrua]
                                        ,[idOficial]
                                        ,[idEvento]
                                        ,[idTipoUsuario]
                                        ,[solicitanteNombre]
                                        ,[solicitanteAp]
                                        ,[solicitanteAm]
                                        ,[solicitanteColonia] 
                                        ,[solicitanteCalle] 
                                        ,[solicitanteNumero]
                                        ,[solicitanteTel]
                                        ,[idEntidad]
                                        ,[idMunicipio]
                                        ,[idMotivoAsignacion]
                                        ,[fechaActualizacion]
                                        ,[actualizadoPor]
                                        ,[estatus])
                                VALUES (
                                        @fechaSolicitud
                                        ,@idTipoVehiculo
                                        ,@idPropietaroGrua
                                        ,@idOficial
                                        ,@idDescripcionEvento
                                        ,@idTipoUsuario
                                        ,@nombreUsuario
                                        ,@apellidoPaternoUsuario
                                        ,@apellidoMaternoUsuario
                                        ,@coloniaUsuario
                                        ,@calleUsuario
                                        ,@numeroUsuario
                                        ,@telefonoUsuario
                                        ,@idEntidad
                                        ,@idMunicipio
                                        ,@idMotivoAsignacion
                                        ,@fechaActualizacion
                                        ,@actualizadoPor
                                        ,@estatus);
                                    SELECT SCOPE_IDENTITY();"; // Obtener el último ID insertado
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@fechaSolicitud", SqlDbType.DateTime)).Value = (object)model.fechaSolicitud ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)model.idTipoVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPropietaroGrua", SqlDbType.Int)).Value = (object)model.idConcecionario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDescripcionEvento", SqlDbType.Int)).Value = (object)model.idDescripcionEvento ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoUsuario", SqlDbType.Int)).Value = (object)model.idTipoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombreUsuario", SqlDbType.NVarChar)).Value = (object)model.nombreUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaternoUsuario", SqlDbType.NVarChar)).Value = (object)model.apellidoPaternoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaternoUsuario", SqlDbType.NVarChar)).Value = (object)model.apellidoMaternoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@coloniaUsuario", SqlDbType.NVarChar)).Value = (object)model.coloniaUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@calleUsuario", SqlDbType.NVarChar)).Value = (object)model.calleUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroUsuario", SqlDbType.NVarChar)).Value = (object)model.numeroUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.idEntidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@telefonoUsuario", SqlDbType.NVarChar)).Value = (object)model.telefonoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMotivoAsignacion", SqlDbType.Int)).Value = (object)model.idMotivoAsignacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    result = Convert.ToInt32(command.ExecuteScalar()); // Valor de Id de este mismo registro
                    idSolicitudInsert = result; // Almacena el valor en la variable idSolicitudInsert
               
                }
                catch (SqlException ex)
                {
                    return idSolicitudInsert;
                }
                finally
                {
                    connection.Close();
                }
            }
            return idSolicitudInsert;
        }


        public int ActualizarSolicitud(int? Isol, SolicitudDepositoModel model)

        {
            int idActual = Isol.GetValueOrDefault(); 
            string strQuery = @"
                                UPDATE solicitudes
                                SET fechaSolicitud = @fechaSolicitud,
                                    idTipoVehiculo = @idTipoVehiculo,
                                    idPropietarioGrua = @idPropietarioGrua,
                                    idOficial = @idOficial,
                                    idEvento = @idDescripcionEvento,
                                    idTipoUsuario = @idTipoUsuario,
                                    solicitanteNombre = @nombreUsuario,
                                    solicitanteAp = @apellidoPaternoUsuario,
                                    solicitanteAm = @apellidoMaternoUsuario,
                                    solicitanteColonia = @coloniaUsuario,
                                    solicitanteCalle = @calleUsuario,
                                    solicitanteNumero = @numeroUsuario,
                                    solicitanteTel = @telefonoUsuario,
                                    idEntidad = @idEntidad,
                                    idMunicipio = @idMunicipio,
                                    idMotivoAsignacion = @idMotivoAsignacion,
                                    fechaActualizacion = @fechaActualizacion,
                                    actualizadoPor = @actualizadoPor,
                                    estatus = @estatus
                                WHERE idSolicitud = @idSolicitud;
                            ";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@idSolicitud", Isol);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@fechaSolicitud", SqlDbType.DateTime)).Value = (object)model.fechaSolicitud ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)model.idTipoVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPropietarioGrua", SqlDbType.Int)).Value = (object)model.idConcecionario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDescripcionEvento", SqlDbType.Int)).Value = (object)model.idDescripcionEvento ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoUsuario", SqlDbType.Int)).Value = (object)model.idTipoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombreUsuario", SqlDbType.NVarChar)).Value = (object)model.nombreUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaternoUsuario", SqlDbType.NVarChar)).Value = (object)model.apellidoPaternoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaternoUsuario", SqlDbType.NVarChar)).Value = (object)model.apellidoMaternoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@coloniaUsuario", SqlDbType.NVarChar)).Value = (object)model.coloniaUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@calleUsuario", SqlDbType.NVarChar)).Value = (object)model.calleUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroUsuario", SqlDbType.NVarChar)).Value = (object)model.numeroUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.idEntidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@telefonoUsuario", SqlDbType.NVarChar)).Value = (object)model.telefonoUsuario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMotivoAsignacion", SqlDbType.Int)).Value = (object)model.idMotivoAsignacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return idActual;
                }
                finally
                {
                    connection.Close();
                }
            }
            return idActual;
        }


        public SolicitudDepositoModel ObtenerSolicitudPorID(int Isol)
        {
            SolicitudDepositoModel solicitud = new SolicitudDepositoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT sol.idSolicitud,sol.folio, sol.fechaSolicitud, sol.idTipoVehiculo, sol.idPropietarioGrua, sol.idOficial, " +
                                    "sol.idEntidad, sol.idTipoUsuario, sol.solicitanteNombre, sol.solicitanteAp, sol.solicitanteAm, sol.solicitanteNumero, " +
                                    "sol.solicitanteColonia, sol.solicitanteCalle, sol.idEntidad, sol.idMunicipio, sol.solicitanteTel,sol.idEvento,sol.idMotivoAsignacion, " +
                                    "ctv.tipoVehiculo, " +
                                    "propg.responsable, " +
                                    "ofi.nombre, ofi.apellidoPaterno, ofi.apellidoMaterno, " +
                                    "dev.descripcionEvento, " +
                                    "tu.tipoUsuario, " +
                                    "ent.nombreEntidad, " +
                                    "mun.municipio " +
                            "FROM solicitudes AS sol " +
                            "LEFT JOIN catTiposVehiculo AS ctv ON sol.idTipoVehiculo = ctv.idTipoVehiculo " +
                            "LEFT JOIN catResponsablePensiones AS propg ON sol.idPropietarioGrua = propg.idResponsable " +
                            "LEFT JOIN catOficiales AS ofi ON sol.idOficial = ofi.idOficial " +
                            "LEFT JOIN catDescripcionesEvento as dev ON sol.idEvento = dev.idDescripcion " +
                            "LEFT JOIN catTiposUsuario AS tu ON sol.idTipoUsuario = tu.idTipoUsuario " +
                            "LEFT JOIN catEntidades AS ent ON sol.idEntidad = ent.idEntidad " +
                            "LEFT JOIN catMunicipios AS mun ON sol.idMunicipio = mun.idMunicipio " +
                            "WHERE sol.idSolicitud = @Isol", connection) ;
                    command.Parameters.Add(new SqlParameter("@Isol", SqlDbType.Int)).Value = Isol;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            solicitud.idSolicitud = reader["idSolicitud"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idSolicitud"].ToString());
                            solicitud.folio = reader["folio"].ToString();
                            solicitud.fechaSolicitud = Convert.ToDateTime(reader["fechaSolicitud"].ToString());
                            solicitud.idTipoVehiculo = reader["idTipoVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTipoVehiculo"].ToString());
                            solicitud.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            solicitud.idConcecionario = reader["idPropietarioGrua"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPropietarioGrua"].ToString());
                            solicitud.propietarioGrua = reader["responsable"].ToString();
                            solicitud.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
                            solicitud.oficial = String.Concat(reader["nombre"].ToString(), " ", reader["apellidoPaterno"].ToString(), " ", reader["apellidoMaterno"].ToString());
                            solicitud.idDescripcionEvento = reader["idEvento"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEvento"].ToString());
                            solicitud.descripcionEvento = reader["descripcionEvento"].ToString();
                            solicitud.idTipoUsuario = reader["idTipoUsuario"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTipoUsuario"].ToString());
                            solicitud.tipoUsuario = reader["tipoUsuario"].ToString();
                            solicitud.nombreUsuario = reader["solicitanteNombre"].ToString();
                            solicitud.apellidoPaternoUsuario = reader["solicitanteAp"].ToString();
                            solicitud.apellidoMaternoUsuario = reader["solicitanteAm"].ToString();
                            solicitud.usuarioCompleto = String.Concat(reader["solicitanteNombre"].ToString(), " ", reader["solicitanteAp"].ToString(), " ", reader["solicitanteAm"].ToString());
                            solicitud.numeroUsuario = reader["solicitanteNumero"].ToString();
                            solicitud.coloniaUsuario = reader["solicitanteColonia"].ToString();
                            solicitud.calleUsuario = reader["solicitanteCalle"].ToString();
                            solicitud.idEntidad = reader["idEntidad"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEntidad"].ToString());
                            solicitud.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            solicitud.entidad = reader["nombreEntidad"].ToString();
                            solicitud.municipio = reader["municipio"].ToString();
                            solicitud.telefonoUsuario = reader["solicitanteTel"].ToString();
                            solicitud.idMotivoAsignacion = reader["idMotivoAsignacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMotivoAsignacion"].ToString());

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

            return solicitud;
        }

        public int CompletarSolicitud(SolicitudDepositoModel model)

        {
            int resultado = 0;
            string strQuery = @"
                                UPDATE solicitudes
                                SET vehiculoNumero = @numeroUbicacion,
                                    vehiculoCalle = @calleUbicacion,
                                    vehiculoColonia = @coloniaUbicacion,
                                    vehiculoKm = @kilometroUbicacion,
                                    idCarreteraUbicacion = @idCarretera,
                                    idTramoUbicacion = @idTramo,
                                    idEntidadUbicacion = @idEntidadUbicacion,
                                    idMunicipioUbicacion = @idMunicipioUbicacion,
                                    idPension = @idPensionUbicacion,
                                    vehiculoInterseccion = @interseccion,
                                    fechaActualizacion = @fechaActualizacion,
                                    actualizadoPor = @actualizadoPor,
                                    estatus = @estatus
                                WHERE idSolicitud = @idSolicitud;
                            ";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@idSolicitud", model.idSolicitud);
                    command.CommandType = CommandType.Text;
                   
                    command.Parameters.Add(new SqlParameter("@numeroUbicacion", SqlDbType.NVarChar)).Value = (object)model.numeroUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@calleUbicacion", SqlDbType.NVarChar)).Value = (object)model.calleUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@coloniaUbicacion", SqlDbType.NVarChar)).Value = (object)model.coloniaUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@kilometroUbicacion", SqlDbType.NVarChar)).Value = (object)model.kilometroUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@interseccion", SqlDbType.NVarChar)).Value = (object)model.interseccion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)model.IdTramo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidadUbicacion", SqlDbType.Int)).Value = (object)model.idEntidadUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipioUbicacion", SqlDbType.Int)).Value = (object)model.idMunicipioUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPensionUbicacion", SqlDbType.Int)).Value = (object)model.idPensionUbicacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return resultado;
                }
                finally
                {
                    connection.Close();
                }
            }
            return resultado;
        }

    }
}

using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;
using Microsoft.Identity.Client;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class DepositosService : IDepositosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public DepositosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public string GuardarSolicitud(SolicitudDepositoModel model, int idOficina, string nombreOficina)

        {
            int result = 0;
            int idSolicitudInsert = 0;
            string folioSolicitud = "";



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
                                        ,[vehiculoNumero] 
                                        ,[vehiculoCalle] 
                                        ,[vehiculoColonia] 
                                        ,[vehiculoKm]  
                                        ,[idCarreteraUbicacion]  
                                        ,[idTramoUbicacion] 
                                        ,[idEntidadUbicacion]
                                        ,[idMunicipioUbicacion]
                                        ,[idPension]
                                        ,[vehiculoInterseccion]  
                                        ,[fechaActualizacion]
                                        ,[actualizadoPor]
                                        ,[estatus]
                                        ,[idServicioRequiere])
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
                                        ,@numeroUbicacion
                                        ,@calleUbicacion
                                        ,@coloniaUbicacion
                                        ,@kilometroUbicacion
                                        ,@idCarretera
                                        ,@idTramo
                                        ,@idEntidadUbicacion
                                        ,@idMunicipioUbicacion
                                        ,@idPensionUbicacion
                                        ,@interseccion
                                        ,@fechaActualizacion
                                        ,@actualizadoPor
                                        ,@estatus
                                        ,@idServicioRequiere);
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
                    //command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@idServicioRequiere", SqlDbType.Int)).Value = (object)model.idServicioRequiere ?? DBNull.Value;
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
                    result = Convert.ToInt32(command.ExecuteScalar()); // Valor de Id de este mismo registro
                    var ofi = nombreOficina.Trim().Substring(0, 3).ToUpper();

                    var newFolio = $"{ofi}{result}{DateTime.Now.Year}";

                    SqlCommand command2 = new SqlCommand(@"
                            update solicitudes set folio=@folio where idSolicitud=@id
                                        ", connection);
                    command2.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = (object)result ?? DBNull.Value;
                    command2.Parameters.Add(new SqlParameter("@folio", SqlDbType.VarChar)).Value = (object)newFolio ?? DBNull.Value;
                    command2.CommandType = CommandType.Text;
                    int rowsAffected = command2.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return newFolio;
                    }
                    else
                    {
                        throw new Exception("No se pudo crear el folio.");
                    }
                }
                catch (SqlException ex)
                {

                    Console.WriteLine("Error de SQL: " + ex.Message);
                    return null;
                }
                finally
                {
                    // Cerrar la conexión en el bloque finally
                    connection.Close();
                }
            }
        }
            private string ObtenerFolioSolicitud(SqlConnection connection, int solicitudId)
            {
                string folioSolicitud = "";
                string query = "SELECT folio FROM solicitudes WHERE idSolicitud = @solicitudId";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@solicitudId", SqlDbType.Int)).Value = solicitudId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            folioSolicitud = reader["folio"].ToString();
                        }
                    }
                }

                return folioSolicitud;
            }
        

        public int ActualizarSolicitud(int? Isol, SolicitudDepositoModel model)

        {
            int idActual = Isol.GetValueOrDefault(); 
            string strQuery = @"
                                UPDATE solicitudes
                                SET fechaSolicitud = @fechaSolicitud,
                                    idTipoVehiculo = @idTipoVehiculo,
                                    idPropietarioGrua = @idPropietarioGrua,
                                    idServicioRequiere = @idServicioRequiere,
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
                    command.Parameters.Add(new SqlParameter("@idServicioRequiere", SqlDbType.Int)).Value = (object)model.idServicioRequiere ?? DBNull.Value;
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

       public SolicitudDepositoModel ImportarInfraccion(string folioBusquedaInfraccion)
         {
            SolicitudDepositoModel model = new SolicitudDepositoModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                                            @"SELECT inf.idInfraccion
                                                    ,inf.idOficial
                                                    ,inf.idDependencia
                                                    ,inf.idDelegacion
                                                    ,inf.idVehiculo
                                                    ,inf.idAplicacion
                                                    ,inf.idGarantia
                                                    ,inf.idEstatusInfraccion
                                                    ,inf.idMunicipio
                                                    ,inf.idTramo
                                                    ,inf.idCarretera
                                                    ,inf.idPersona
                                                    ,inf.idPersonaInfraccion
                                                    ,inf.placasVehiculo
                                                    ,inf.folioInfraccion
                                                    ,inf.fechaInfraccion
                                                    ,inf.kmCarretera
                                                    ,inf.observaciones
                                                    ,inf.lugarCalle
                                                    ,inf.lugarNumero
                                                    ,inf.lugarColonia
                                                    ,inf.lugarEntreCalle
                                                    ,inf.infraccionCortesia
                                                    ,inf.NumTarjetaCirculacion
                                                    ,inf.fechaActualizacion
                                                    ,inf.actualizadoPor
                                                    ,inf.estatus
                                                    ,del.idOficinaTransporte, del.nombreOficina,dep.idDependencia,dep.nombreDependencia,catGar.idGarantia,catGar.garantia
                                                    ,estIn.idEstatusInfraccion, estIn.estatusInfraccion
                                                    ,gar.idGarantia,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                                    ,catMun.idMunicipio,catMun.municipio
                                                    ,catTra.idTramo,catTra.tramo
                                                    ,per.RFC,per.fechaNacimiento
                                                    ,catCarre.idCarretera,catCarre.carretera
                                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico
                                                    ,motInf.idMotivoInfraccion,motInf.idMotivoInfraccion
                                                    ,ci.nombre
                                                    ,ci.idCatMotivoInfraccion,ci.nombre
                                                    ,catSubInf.idSubConcepto,catSubInf.subConcepto
                                                    ,catConInf.idConcepto,catConInf.concepto
                                                    FROM infracciones as inf
                                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                                    left join catDelegacionesOficinasTransporte	del on inf.idDelegacion = del.idOficinaTransporte
                                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                                    left join motivosInfraccion motInf on inf.IdInfraccion = motInf.idInfraccion
												   INNER JOIN catMotivosInfraccion ci on motInf.idCatMotivosInfraccion = ci.idCatMotivoInfraccion 
                                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo
                                                    left join personas per on inf.idPersona = per.idPersona
                                                    left join catSubConceptoInfraccion catSubInf on ci.IdSubConcepto = catSubInf.idSubConcepto
                                                    left join catConceptoInfraccion catConInf on  catSubInf.idConcepto = catConInf.idConcepto
                                                    WHERE inf.folioInfraccion=@folioInfraccion";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@folioInfraccion", SqlDbType.NVarChar)).Value = folioBusquedaInfraccion;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
                            model.idMunicipioUbicacion = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.IdTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
                            model.IdCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
                            model.kilometroUbicacion = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
                            model.calleUbicacion = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
                            model.numeroUbicacion = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
                            model.coloniaUbicacion = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
                            model.interseccion = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
                            model.folio = reader["folioInfraccion"] == System.DBNull.Value ? string.Empty : reader["folioInfraccion"].ToString();
                            model.municipio = reader["municipio"].ToString();
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
            return model;
        }

        public List<SolicitudDepositoModel> ObtenerServicios()

        {
            //
            List<SolicitudDepositoModel> ListaServicios = new List<SolicitudDepositoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT sr.idServicio,sr.nombreServicio, e.estatusdesc 
                                                          FROM catServicioRequiere AS sr 
                                                          LEFT JOIN estatus AS e ON sr.estatus = e.estatus
                                                            WHERE sr.estatus = 1;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            SolicitudDepositoModel servicio = new SolicitudDepositoModel();
                            servicio.idServicioRequiere = reader["idServicio"] != DBNull.Value ? Convert.ToInt32(reader["idServicio"]) : 0;
                            servicio.servicioRequiere = reader["nombreServicio"] != DBNull.Value ? reader["nombreServicio"].ToString() : string.Empty;


                            ListaServicios.Add(servicio);
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
            return ListaServicios;


        }


    }
}

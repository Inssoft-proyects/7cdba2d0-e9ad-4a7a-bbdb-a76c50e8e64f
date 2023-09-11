using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class EstadisticasService : IEstadisticasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IPersonasService _personasService;
        private readonly IVehiculosService _vehiculosService;
        private readonly IInfraccionesService _infraccionesService;
        public EstadisticasService(ISqlClientConnectionBD sqlClientConnectionBD, 
            IVehiculosService vehiculosService, 
            IPersonasService personasService,
            IInfraccionesService infraccionesService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _vehiculosService = vehiculosService;
            _personasService = personasService;
            _infraccionesService = infraccionesService;
        }

        public IEnumerable<IncidenciasInfraccionesModel> IncidenciasInfracciones(IncidenciasBusquedaModel model)
        {
            List<IncidenciasInfraccionesModel> modelList = new List<IncidenciasInfraccionesModel>();
            string strQuery = @"SELECT inf.idInfraccion
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
                                    ,del.idDelegacion, del.delegacion,dep.idDependencia,dep.nombreDependencia,catGar.idGarantia,catGar.garantia
                                    ,estIn.idEstatusInfraccion, estIn.estatusInfraccion
                                    ,gar.idGarantia,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                    ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                    ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                    ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                    ,catMun.idMunicipio,catMun.municipio
                                    ,catTra.idTramo,catTra.tramo
                                    ,catCarre.idCarretera,catCarre.carretera
                                    ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                    ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico
                                    ,motInf.idMotivoInfraccion,motInf.nombre,motInf.fundamento,motInf.calificacionMinima,motInf.calificacionMaxima
                                    ,catMotInf.idMotivoInfraccion,catMotInf.catMotivo
                                    ,catSubInf.idSubConcepto,catSubInf.subConcepto
                                    ,catConInf.idConcepto,catConInf.concepto
                                    FROM infracciones as inf
                                    left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                    left join catDelegaciones	del on inf.idDelegacion = del.idDelegacion
                                    left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                    left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                    left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                    left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                    left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                    left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                    left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                    left join catTramos catTra on inf.idTramo = catTra.idTramo
                                    left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                    left join vehiculos veh on inf.idVehiculo = veh.idVehiculo
                                    left join motivosInfraccion motInf on inf.IdInfraccion = motInf.idInfraccion
                                    left join catMotivosInfraccion catMotInf on motInf.idCatMotivosInfraccion = catMotInf.idMotivoInfraccion
                                    left join catSubConceptoInfraccion catSubInf on catMotInf.IdSubConcepto = catSubInf.idSubConcepto
                                    left join catConceptoInfraccion catConInf on  catSubInf.idConcepto = catConInf.idConcepto
                                    WHERE inf.estatus = 1";

            return modelList;
        }

        public List<InfraccionesModel> GetAllInfraccionesEstadisticas(IncidenciasBusquedaModel modelBusqueda)
        {
            string condiciones = "";
             
            condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion==0 ? "" : " AND inf.idDelegacion = @idDelegacion ";
            condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND inf.idOficial =@idOficial ";
            condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND inf.idCarretera = @idCarretera ";
            condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND inf.idTramo = @idTramo ";
            condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
            condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
            condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
            condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND inf.idMunicipio =@idMunicipio ";

            string condicionFecha = "";

            if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
                condicionFecha = " AND inf.fechaInfraccion >= @fechaInicio AND inf.fechaInfraccion  <= @fechaFin ";
            else if (modelBusqueda.fechaInicio != DateTime.MinValue && modelBusqueda.fechaFin == DateTime.MinValue)
                condicionFecha = " AND inf.fechaInfraccion >= @fechaInicio ";
            else if (modelBusqueda.fechaInicio == DateTime.MinValue && modelBusqueda.fechaFin != DateTime.MinValue)
                condicionFecha = " AND inf.fechaInfraccion <= @fechaFin ";
            else
                condicionFecha = "";
             
              
            List<InfraccionesModel> modelList = new List<InfraccionesModel>();
            string strQuery = @"SELECT inf.idInfraccion
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
                                ,del.idDelegacion, del.delegacion,dep.idDependencia,dep.nombreDependencia,catGar.idGarantia,catGar.garantia
                                ,estIn.idEstatusInfraccion, estIn.estatusInfraccion
                                ,gar.idGarantia,gar.numPlaca,gar.numLicencia,gar.vehiculoDocumento
                                ,tipoP.idTipoPlaca, tipoP.tipoPlaca
                                ,tipoL.idTipoLicencia, tipoL.tipoLicencia
                                ,catOfi.idOficial,catOfi.nombre,catOfi.apellidoPaterno,catOfi.apellidoMaterno,catOfi.rango
                                ,catMun.idMunicipio,catMun.municipio
                                ,catTra.idTramo,catTra.tramo
                                ,catCarre.idCarretera,catCarre.carretera
                                ,veh.idMarcaVehiculo,veh.idMarcaVehiculo, veh.serie,veh.tarjeta, veh.vigenciaTarjeta,veh.idTipoVehiculo,veh.modelo
                                ,veh.idColor,veh.idEntidad,veh.idCatTipoServicio, veh.propietario, veh.numeroEconomico
                                ,motInf.idMotivoInfraccion ,catMotInf.nombre  ,catMotInf.fundamento,motInf.calificacionMinima,motInf.calificacionMaxima
                                ,catMotInf.idCatMotivoInfraccion 
                                ,catSubInf.idSubConcepto,catSubInf.subConcepto
                                ,catConInf.idConcepto,catConInf.concepto
                                FROM infracciones as inf
                                left join catDependencias dep on inf.idDependencia= dep.idDependencia
                                left join catDelegaciones	del on inf.idDelegacion = del.idDelegacion
                                left join catEstatusInfraccion  estIn on inf.IdEstatusInfraccion = estIn.idEstatusInfraccion
                                left join catGarantias catGar on inf.idGarantia = catGar.idGarantia
                                left join garantiasInfraccion gar on catGar.idGarantia= gar.idCatGarantia
                                left join catTipoPlaca  tipoP on gar.idTipoPlaca=tipoP.idTipoPlaca
                                left join catTipoLicencia tipoL on tipoL.idTipoLicencia= gar.idTipoLicencia
                                left join catOficiales catOfi on inf.idOficial = catOfi.idOficial
                                left join catMunicipios catMun on inf.idMunicipio =catMun.idMunicipio
                                left join catTramos catTra on inf.idTramo = catTra.idTramo
                                left join catCarreteras catCarre on catTra.IdCarretera = catCarre.idCarretera
                                left join vehiculos veh on inf.idVehiculo = veh.idVehiculo
                                left join motivosInfraccion motInf on inf.IdInfraccion = motInf.idInfraccion
                                left join catMotivosInfraccion catMotInf on motInf.idCatMotivosInfraccion = catMotInf.idCatMotivoInfraccion 
                                left join catSubConceptoInfraccion catSubInf on catMotInf.IdSubConcepto = catSubInf.idSubConcepto
                                left join catConceptoInfraccion catConInf on  catSubInf.idConcepto = catConInf.idConcepto
                                WHERE inf.estatus = 1 " + condiciones  ;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;

                    if (!modelBusqueda.idDelegacion.Equals(null) && modelBusqueda.idDelegacion != 0)
                        command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idDelegacion ?? DBNull.Value;

                    if (!modelBusqueda.idOficial.Equals(null) && modelBusqueda.idOficial != 0)
                        command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idOficial ?? DBNull.Value;

                    if (!modelBusqueda.idCarretera.Equals(null) && modelBusqueda.idCarretera != 0)
                        command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idCarretera ?? DBNull.Value;

                    if (!modelBusqueda.idTramo.Equals(null) && modelBusqueda.idTramo != 0)
                        command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idTramo ?? DBNull.Value;

                    if (!modelBusqueda.idTipoVehiculo.Equals(null) && modelBusqueda.idTipoVehiculo != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idTipoVehiculo ?? DBNull.Value;
                    
                    if (!modelBusqueda.idTipoServicio.Equals(null) && modelBusqueda.idTipoServicio != 0)
                        command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idTipoServicio ?? DBNull.Value;

                    if (!modelBusqueda.idTipoLicencia.Equals(null) && modelBusqueda.idTipoLicencia != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idTipoVehiculo ?? DBNull.Value;

                    if (!modelBusqueda.idMunicipio.Equals(null) && modelBusqueda.idMunicipio != 0)
                        command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.NVarChar)).Value = (object)modelBusqueda.idMunicipio ?? DBNull.Value;

                    if (modelBusqueda.fechaInicio != DateTime.MinValue)
                        command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.NVarChar)).Value = (object)modelBusqueda.fechaInicio ?? DBNull.Value;

                    if (modelBusqueda.fechaFin != DateTime.MinValue)
                        command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.NVarChar)).Value = (object)modelBusqueda.fechaFin ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            InfraccionesModel model = new InfraccionesModel();
                            model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
                            model.idOficial = reader["idOficial"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idOficial"].ToString());
                            model.idDependencia = reader["idDependencia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDependencia"].ToString());
                            model.idDelegacion = reader["idDelegacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idDelegacion"].ToString());
                            model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
                            model.idAplicacion = reader["idAplicacion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idAplicacion"].ToString());
                            model.idGarantia = reader["idGarantia"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idGarantia"].ToString());
                            model.idEstatusInfraccion = reader["idEstatusInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idEstatusInfraccion"].ToString());
                            model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.municipio = reader["municipio"].ToString();
                            model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
                            model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
                            model.placasVehiculo = reader["placasVehiculo"].ToString();
                            model.folioInfraccion = reader["folioInfraccion"].ToString();
                            model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
                            model.kmCarretera = reader["kmCarretera"] == System.DBNull.Value ? string.Empty : reader["kmCarretera"].ToString();
                            model.observaciones = reader["observaciones"] == System.DBNull.Value ? string.Empty : reader["observaciones"].ToString();
                            model.lugarCalle = reader["lugarCalle"] == System.DBNull.Value ? string.Empty : reader["lugarCalle"].ToString();
                            model.lugarNumero = reader["lugarNumero"] == System.DBNull.Value ? string.Empty : reader["lugarNumero"].ToString();
                            model.lugarColonia = reader["lugarColonia"] == System.DBNull.Value ? string.Empty : reader["lugarColonia"].ToString();
                            model.lugarEntreCalle = reader["lugarEntreCalle"] == System.DBNull.Value ? string.Empty : reader["lugarEntreCalle"].ToString();
                            model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
                            model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
                            model.Persona = _personasService.GetPersonaById((int)model.idPersona);
                            model.PersonaInfraccion = model.idPersonaInfraccion == null ? new PersonaInfraccionModel() : _infraccionesService.GetPersonaInfraccionById((int)model.idPersonaInfraccion);
                            model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
                            model.MotivosInfraccion = _infraccionesService.GetMotivosInfraccionByIdInfraccion(model.idInfraccion);
                            model.Garantia = model.idGarantia == null ? new GarantiaInfraccionModel() : _infraccionesService.GetGarantiaById((int)model.idGarantia);
                            model.umas = _infraccionesService.GetUmas();
                            if (model.MotivosInfraccion.Any(w => w.calificacion != null))
                            {
                                model.totalInfraccion = (model.MotivosInfraccion.Sum(s => (int)s.calificacion) * model.umas);
                            }
                            modelList.Add(model);
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
            } 
            return modelList;
        }
    }
}

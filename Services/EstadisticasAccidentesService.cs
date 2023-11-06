using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class EstadisticasAccidentesService : IEstadisticasAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        public EstadisticasAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD
                                  , IVehiculosService vehiculosService
                                  , IPersonasService personasService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _vehiculosService = vehiculosService;
            _personasService = personasService;
        }


        public IEnumerable<IncidenciasInfraccionesModel> IncidenciasInfracciones(AccidentesBusquedaModel model)
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


        public List<InfraccionesModel> GetAllInfracciones2()
        {
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
                                      ,mun.municipio
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
                               FROM infracciones inf
                               LEFT JOIN catMunicipios mun
                               ON inf.idMunicipio = mun.idMunicipio
                               WHERE inf.estatus = 1"
            ;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
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
                            model.idTramo = reader["idTramo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idTramo"].ToString());
                            model.idCarretera = reader["idCarretera"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCarretera"].ToString());
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.idPersonaInfraccion = reader["idPersonaInfraccion"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idPersonaInfraccion"].ToString());
                            model.placasVehiculo = reader["placasVehiculo"].ToString();
                            model.folioInfraccion = reader["folioInfraccion"].ToString();
                            model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
                            model.kmCarretera = reader["kmCarretera"].ToString();
                            model.observaciones = reader["observaciones"].ToString();
                            model.lugarCalle = reader["lugarCalle"].ToString();
                            model.lugarNumero = reader["lugarNumero"].ToString();
                            model.lugarColonia = reader["lugarColonia"].ToString();
                            model.lugarEntreCalle = reader["lugarEntreCalle"].ToString();
                            model.infraccionCortesia = reader["infraccionCortesia"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["infraccionCortesia"].ToString());
                            model.NumTarjetaCirculacion = reader["NumTarjetaCirculacion"].ToString();
                            model.Persona = _personasService.GetPersonaById((int)model.idPersona);
                            model.PersonaInfraccion = null;
                            model.Vehiculo = _vehiculosService.GetVehiculoById((int)model.idVehiculo);
                            model.MotivosInfraccion = null;
                            model.Garantia = null;
                            model.umas = 0;
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

        public List<BusquedaAccidentesModel> ObtenerAccidentes()
        {
            //
            List<BusquedaAccidentesModel> ListaAccidentes = new List<BusquedaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try 
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@" SELECT 
                                                                acc.idAccidente, 
                                                                MAX(acc.idMunicipio) AS idMunicipio,
                                                                MAX(mun.municipio) AS municipio,
                                                                MAX(acc.idOficinaDelegacion) AS idOficinaDelegacion,
                                                                MAX(acc.idElabora) AS idElabora,
                                                                MAX(acc.idCarretera) AS idCarretera, 
                                                                MAX(acc.idTramo) AS idTramo, 
                                                                MAX(acc.idClasificacionAccidente) AS idClasificacionAccidente,
                                                                MAX(cond.idTipoLicencia) AS idTipoLicencia,
                                                                MAX(acc.idFactorAccidente) AS idFactorAccidente,
                                                                MAX(acc.idFactorOpcionAccidente) AS idFactorOpcionAccidente,
                                                                MAX(acc.estatus) AS estatus,
                                                                MAX(acc.numeroReporte) AS numeroReporte,
                                                                MAX(acc.fecha) AS fecha,
                                                                MAX(acc.hora) AS hora,
                                                                MAX(veh.idVehiculo) AS idVehiculo,
                                                                MAX(veh.idCatTipoServicio) AS idCatTipoServicio,
                                                                MAX(veh.idTipoVehiculo) AS idTipoVehiculo,
                                                                MAX(accau.idAccidenteCausa) AS idAccidenteCausa
                                                                FROM accidentes AS acc
                                                                LEFT JOIN catMunicipios AS mun ON acc.idMunicipio = mun.idMunicipio 
                                                                LEFT JOIN catCarreteras AS car ON acc.idCarretera = car.idCarretera 
                                                                LEFT JOIN catTramos AS tra ON acc.idTramo = tra.idTramo 
                                                                LEFT JOIN conductoresVehiculosAccidente AS cva ON acc.idAccidente = cva.idAccidente  
                                                                LEFT JOIN personas AS cond ON cond.idPersona = cva.idPersona
                                                                LEFT JOIN vehiculos AS veh ON cva.idVehiculo = veh.idVehiculo
                                                                LEFT JOIN accidenteCausas AS accau ON acc.idAccidente = accau.idAccidente
                                                                WHERE acc.estatus = 1
                                                                GROUP BY acc.idAccidente;
                                                                ", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            BusquedaAccidentesModel accidente = new BusquedaAccidentesModel();

                            accidente.IdAccidente = reader["IdAccidente"] != DBNull.Value ? Convert.ToInt32(reader["IdAccidente"]) : 0;
                            accidente.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
                            accidente.municipio = reader["municipio"].ToString();
                            accidente.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
                            accidente.IdOficial = reader["idElabora"] != DBNull.Value ? Convert.ToInt32(reader["idElabora"]) : 0;
                            accidente.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
                            accidente.idTramo = reader["IdTramo"] != DBNull.Value ? Convert.ToInt32(reader["IdTramo"]) : 0;
                            accidente.idClasificacionAccidente = reader["idClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idClasificacionAccidente"]) : 0;
                            accidente.idTipoLicencia = reader["idTipoLicencia"] != DBNull.Value ? Convert.ToInt32(reader["idTipoLicencia"]) : 0;
                            accidente.idCausaAccidente = reader["idAccidenteCausa"] != DBNull.Value ? Convert.ToInt32(reader["idAccidenteCausa"]) : 0;
                            accidente.idFactorAccidente = reader["idFactorAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorAccidente"]) : 0;
                            accidente.idFactorOpcionAccidente = reader["idFactorOpcionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorOpcionAccidente"]) : 0;
                            accidente.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
                            accidente.IdTipoServicio = reader["idCatTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idCatTipoServicio"]) : 0;
                            accidente.idCausaAccidente = reader["idAccidenteCausa"] != DBNull.Value ? Convert.ToInt32(reader["idAccidenteCausa"]) : 0;

                            accidente.numeroReporte = reader["numeroReporte"] != DBNull.Value ? reader["numeroReporte"].ToString() : string.Empty;
                            accidente.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
                            accidente.hora = reader["hora"] != DBNull.Value ? reader.GetTimeSpan(reader.GetOrdinal("hora")) : TimeSpan.Zero;
                             


                            ListaAccidentes.Add(accidente);

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
            return ListaAccidentes;


        }

        public List<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente()
        {
            List<ListadoAccidentesPorAccidenteModel> modelList = new List<ListadoAccidentesPorAccidenteModel>();
            string strQuery = @"SELECT DISTINCT
                ac.numeroReporte as Numreporteaccidente
                ,ac.fecha as Fecha
                ,ac.hora as Hora
                ,del.delegacion as Delegacion
                ,mun.municipio
                ,tram.tramo
                ,ac.kilometro
                ,ac.latitud
                ,ac.longitud
                ,veh.vehiculos as Vehiculo
                ,CONCAT(ofic.nombre, ' ',ofic.apellidoPaterno,' ',ofic.apellidoMaterno) as NombredelOficial
                ,ac.montoCamino as Dañosalcamino
                ,ac.montoCarga as Dañosacarga
                ,ac.montoPropietarios as Dañosapropietario
                ,ac.montoOtros as Otrosdaños
                ,inv.Lesionados as Lesionados
                ,inv.Muertos as Muertos
                ,fac.factorAccidente as FactoresOpciones
                ,cauac.causaAccidente as Causas
                ,ac.descripcionCausas as CausasDescripcion
                FROM accidentes ac
                LEFT JOIN catDelegaciones del on del.idDelegacion = ac.idOficinaDelegacion
                LEFT JOIN catMunicipios mun on mun.idMunicipio = ac.idMunicipio
                LEFT JOIN catCarreteras car on car.idCarretera = ac.idCarretera
                LEFT JOIN catTramos tram on tram.idTramo = ac.idTramo
                LEFT JOIN catFactoresAccidentes fac on fac.idFactorAccidente = ac.idFactorAccidente
                LEFT JOIN catOficiales ofic on ofic.idOficial = ac.idElabora
                LEFT JOIN catCausasAccidentes cauac on cauac.idCausaAccidente = ac.idCausaAccidente
                LEFT JOIN (SELECT idAccidente, STRING_AGG(Vehiculos,'; \r\n ') vehiculos FROM (
                            SELECT ac.idAccidente
                            ,CONCAT('Vehículo ',(ROW_NUMBER() OVER (PARTITION BY ac.idAccidente Order by ac.idAccidente)),': ',mv.marcaVehiculo,' ',sm.nombreSubmarca,' ',veh.modelo,', TIPO: ',tv.tipoVehiculo,', Servicio: ',ts.tipoServicio,', Placa: ',veh.placas,', Serie: ',veh.serie,'') AS Vehiculos
                            FROM accidentes ac
                            left join vehiculosAccidente vehacc on vehacc.idAccidente = ac.idAccidente
                            left join vehiculos veh on veh.idVehiculo = vehacc.idVehiculo
                            left join catMarcasVehiculos mv ON mv.idMarcaVehiculo = veh.idMarcaVehiculo
                            left join catSubmarcasVehiculos sm ON sm.idSubmarca = veh.idSubmarca
                            left join catEntidades e ON e.idEntidad = veh.idEntidad
                            left join catColores cc ON cc.idColor = veh.idColor
                            left join catTiposVehiculo tv ON tv.idTipoVehiculo = veh.idTipoVehiculo
                            left join catTipoServicio ts ON ts.idCatTipoServicio = veh.idCatTipoServicio) as veh
                            GROUP by idAccidente
                ) veh on veh.idAccidente = ac.idAccidente
                left join (SELECT acc.idAccidente ,count(CASE WHEN invacc.idEstadoVictima = 1 THEN invacc.idEstadoVictima END) AS Lesionados,  count(CASE WHEN invacc.idEstadoVictima = 2 THEN invacc.idEstadoVictima END) AS Muertos
                FROM accidentes acc
                inner join involucradosAccidente invacc on invacc.idAccidente = acc.idAccidente
                GROUP by acc.idAccidente) inv on inv.idAccidente = ac.idAccidente";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ListadoAccidentesPorAccidenteModel model = new ListadoAccidentesPorAccidenteModel();
                            model.Numreporteaccidente = reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString();
                            model.Fecha = reader["Fecha"] == System.DBNull.Value ? default(string) : reader["Fecha"].ToString();
                            model.Hora = reader["Hora"] == System.DBNull.Value ? default(string) : reader["Hora"].ToString();
                            model.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
                            model.municipio = reader["municipio"] == System.DBNull.Value ? default(string) : reader["municipio"].ToString();
                            model.tramo = reader["tramo"] == System.DBNull.Value ? default(string) : reader["tramo"].ToString();
                            model.kilometro = reader["kilometro"] == System.DBNull.Value ? default(string) : reader["kilometro"].ToString();
                            model.latitud = reader["latitud"] == System.DBNull.Value ? default(string) : reader["latitud"].ToString();
                            model.longitud = reader["longitud"] == System.DBNull.Value ? default(string) : reader["longitud"].ToString();
                            model.Vehiculo = reader["Vehiculo"] == System.DBNull.Value ? default(string) : reader["Vehiculo"].ToString();
                            model.NombredelOficial = reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString();
                            model.Dañosalcamino = reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString();
                            model.Dañosacarga = reader["Dañosacarga"] == System.DBNull.Value ? default(string) : reader["Dañosacarga"].ToString();
                            model.Dañosapropietario = reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString();
                            model.Otrosdaños = reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString();
                            model.Lesionados = reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString();
                            model.Muertos = reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString();
                            model.FactoresOpciones = reader["FactoresOpciones"] == System.DBNull.Value ? default(string) : reader["FactoresOpciones"].ToString();
                            model.Causas = reader["Causas"] == System.DBNull.Value ? default(string) : reader["Causas"].ToString();
                            model.CausasDescripcion = reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString();
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
        public List<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo()
        {
            List<ListadoAccidentesPorVehiculoModel> modelList = new List<ListadoAccidentesPorVehiculoModel>();
            string strQuery = @"SELECT DISTINCT
                ac.numeroReporte		as Numreporteaccidente
                ,ac.fecha				as Fecha
                ,ac.hora				as Hora
                ,(ROW_NUMBER() OVER (PARTITION BY ac.idAccidente Order by ac.idAccidente))					as NumVeh							
                ,veh.placas				as PlacasVeh
                ,veh.serie				as SerieVeh
                ,CONCAT(prop.nombre, ' ',prop.apellidoPaterno,' ',prop.apellidoMaterno)		as PropietarioVeh
                ,tv.tipoVehiculo		as TipoVeh
                ,ts.tipoServicio		as ServicioVeh
                ,mv.marcaVehiculo		as Marca 
                ,sm.nombreSubmarca		as Submarca
                ,veh.modelo				as Modelo
                ,CONCAT(cond.nombre, ' ',cond.apellidoPaterno,' ',cond.apellidoMaterno) 	as ConductorVeh
                ,''						as DepositoVehículo
                ,del.delegacion			as Delegacion
                ,mun.municipio			as Municipio
                ,car.carretera			as Carretera
                ,tram.tramo				as Tramo
                ,ac.kilometro			as Kilómetro
                ,ac.latitud				as Latitud
                ,ac.longitud			as Longitud
                ,CONCAT(ofic.nombre, ' ',ofic.apellidoPaterno,' ',ofic.apellidoMaterno) 	as NombredelOficial
                ,ac.montoCamino			as Dañosalcamino
                ,ac.montoCarga			as Dañosacarga
                ,ac.montoPropietarios	as Dañosapropietario
                ,ac.montoOtros			as Otrosdaños
                ,inv.Lesionados			as Lesionados
                ,inv.Muertos			as Muertos
                ,cauac.causaAccidente	as Causas
                ,ac.descripcionCausas	as CausasDescripcion
                FROM accidentes ac
                LEFT JOIN catDelegaciones del on del.idDelegacion = ac.idOficinaDelegacion
                LEFT JOIN catMunicipios mun on mun.idMunicipio = ac.idMunicipio
                LEFT JOIN catCarreteras car on car.idCarretera = ac.idCarretera
                LEFT JOIN catTramos tram on tram.idTramo = ac.idTramo
                LEFT JOIN catFactoresAccidentes fac on fac.idFactorAccidente = ac.idFactorAccidente
                LEFT JOIN catOficiales ofic on ofic.idOficial = ac.idElabora
                LEFT JOIN catCausasAccidentes cauac on cauac.idCausaAccidente = ac.idCausaAccidente
                left join vehiculosAccidente vehacc on vehacc.idAccidente = ac.idAccidente
                left join vehiculos veh on veh.idVehiculo = vehacc.idVehiculo
                left join catMarcasVehiculos mv ON mv.idMarcaVehiculo = veh.idMarcaVehiculo
                left join catSubmarcasVehiculos sm ON sm.idSubmarca = veh.idSubmarca
                left join catEntidades e ON e.idEntidad = veh.idEntidad
                left join catColores cc ON cc.idColor = veh.idColor
                left join catTiposVehiculo tv ON tv.idTipoVehiculo = veh.idTipoVehiculo
                left join catTipoServicio ts ON ts.idCatTipoServicio = veh.idCatTipoServicio
                left join (SELECT acc.idAccidente ,count(CASE WHEN invacc.idEstadoVictima = 1 THEN invacc.idEstadoVictima END) AS Lesionados,  count(CASE WHEN invacc.idEstadoVictima = 2 THEN invacc.idEstadoVictima END) AS Muertos
                FROM accidentes acc
                inner join involucradosAccidente invacc on invacc.idAccidente = acc.idAccidente
                GROUP by acc.idAccidente) inv on inv.idAccidente = ac.idAccidente
                LEFT JOIN conductoresVehiculosAccidente cva ON cva.idAccidente = ac.idAccidente 
                LEFT JOIN personas cond ON cond.idPersona = cva.idPersona
                LEFT JOIN personas prop on prop.idPersona = veh.propietario";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ListadoAccidentesPorVehiculoModel model = new ListadoAccidentesPorVehiculoModel();
                            model.Numreporteaccidente = reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString();
                            model.NumVeh = reader["NumVeh"] == System.DBNull.Value ? default(string) : reader["NumVeh"].ToString();
                            model.PlacasVeh = reader["PlacasVeh"] == System.DBNull.Value ? default(string) : reader["PlacasVeh"].ToString();
                            model.SerieVeh = reader["SerieVeh"] == System.DBNull.Value ? default(string) : reader["SerieVeh"].ToString();
                            model.PropietarioVeh = reader["PropietarioVeh"] == System.DBNull.Value ? default(string) : reader["PropietarioVeh"].ToString();
                            model.TipoVeh = reader["TipoVeh"] == System.DBNull.Value ? default(string) : reader["TipoVeh"].ToString();
                            model.ServicioVeh = reader["ServicioVeh"] == System.DBNull.Value ? default(string) : reader["ServicioVeh"].ToString();
                            model.Marca = reader["Marca"] == System.DBNull.Value ? default(string) : reader["Marca"].ToString();
                            model.Submarca = reader["Submarca"] == System.DBNull.Value ? default(string) : reader["Submarca"].ToString();
                            model.Modelo = reader["Modelo"] == System.DBNull.Value ? default(string) : reader["Modelo"].ToString();
                            model.ConductorVeh = reader["ConductorVeh"] == System.DBNull.Value ? default(string) : reader["ConductorVeh"].ToString();
                            model.DepositoVehículo = reader["DepositoVehículo"] == System.DBNull.Value ? default(string) : reader["DepositoVehículo"].ToString();
                            model.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
                            model.Municipio = reader["Municipio"] == System.DBNull.Value ? default(string) : reader["Municipio"].ToString();
                            model.Carretera = reader["Carretera"] == System.DBNull.Value ? default(string) : reader["Carretera"].ToString();
                            model.Tramo = reader["Tramo"] == System.DBNull.Value ? default(string) : reader["Tramo"].ToString();
                            model.Kilómetro = reader["Kilómetro"] == System.DBNull.Value ? default(string) : reader["Kilómetro"].ToString();
                            model.Latitud = reader["Latitud"] == System.DBNull.Value ? default(string) : reader["Latitud"].ToString();
                            model.Longitud = reader["Longitud"] == System.DBNull.Value ? default(string) : reader["Longitud"].ToString();
                            model.NombredelOficial = reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString();
                            model.Dañosalcamino = reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString();
                            model.DañosaCarga = reader["DañosaCarga"] == System.DBNull.Value ? default(string) : reader["DañosaCarga"].ToString();
                            model.Dañosapropietario = reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString();
                            model.Otrosdaños = reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString();
                            model.Lesionados = reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString();
                            model.Muertos = reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString();
                            model.Causas = reader["Causas"] == System.DBNull.Value ? default(string) : reader["Causas"].ToString();
                            model.CausasDescripcion = reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString();
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

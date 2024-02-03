using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
																MAX(del.delegacion) AS delegacion,
                                                                MAX(acc.numeroReporte) AS numeroReporte,
                                                                MAX(acc.fecha) AS fecha,
                                                                MAX(acc.hora) AS hora,
                                                                MAX(veh.idVehiculo) AS idVehiculo,
                                                                MAX(veh.idCatTipoServicio) AS idCatTipoServicio,
                                                                MAX(veh.idSubtipoServicio) AS idSubtipoServicio,
                                                                MAX(veh.idTipoVehiculo) AS idTipoVehiculo,
                                                                MAX(accau.idCausaAccidente) AS idCausaAccidente
                                                                FROM accidentes AS acc
                                                                LEFT JOIN catMunicipios AS mun ON acc.idMunicipio = mun.idMunicipio 
                                                                LEFT JOIN catCarreteras AS car ON acc.idCarretera = car.idCarretera 
                                                                LEFT JOIN catTramos AS tra ON acc.idTramo = tra.idTramo 
                                                                LEFT JOIN conductoresVehiculosAccidente AS cva ON acc.idAccidente = cva.idAccidente  
                                                                LEFT JOIN personas AS cond ON cond.idPersona = cva.idPersona
                                                                LEFT JOIN vehiculos AS veh ON cva.idVehiculo = veh.idVehiculo
                                                                LEFT JOIN catDelegaciones AS del ON acc.idOficinaDelegacion = del.idDelegacion
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
                            accidente.municipio = reader["delegacion"].ToString();
                            accidente.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
                            accidente.IdOficial = reader["idElabora"] != DBNull.Value ? Convert.ToInt32(reader["idElabora"]) : 0;
                            accidente.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
                            accidente.idTramo = reader["IdTramo"] != DBNull.Value ? Convert.ToInt32(reader["IdTramo"]) : 0;
                            accidente.idClasificacionAccidente = reader["idClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idClasificacionAccidente"]) : 0;
                            accidente.idTipoLicencia = reader["idTipoLicencia"] != DBNull.Value ? Convert.ToInt32(reader["idTipoLicencia"]) : 0;
                            accidente.idCausaAccidente = reader["idCausaAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idCausaAccidente"]) : 0;
                            accidente.idFactorAccidente = reader["idFactorAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorAccidente"]) : 0;
                            accidente.idFactorOpcionAccidente = reader["idFactorOpcionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorOpcionAccidente"]) : 0;
                            accidente.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
                            accidente.IdTipoServicio = reader["idCatTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idCatTipoServicio"]) : 0;
							accidente.IdSubtipoServicio = reader["idSubtipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idSubtipoServicio"]) : 0;

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

        // public List<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente(BusquedaAccidentesModel recibido)
        public IEnumerable<ListadoAccidentesPorAccidenteModel> AccidentesPorAccidente(BusquedaAccidentesModel recibido)

        {
            List<ListadoAccidentesPorAccidenteModel> modelList = new List<ListadoAccidentesPorAccidenteModel>();
            var condicionesFiltro = new List<string>();

            if (recibido.idMunicipio > 0)
                condicionesFiltro.Add($"ac.idMunicipio = {recibido.idMunicipio}");

            if (recibido.idDelegacion > 0)
                condicionesFiltro.Add($"ac.idOficinaDelegacion = {recibido.idDelegacion}");

            if (recibido.IdOficial > 0)
                condicionesFiltro.Add($"ac.idElabora = {recibido.IdOficial}");

            if (recibido.idCarretera > 0)
                condicionesFiltro.Add($"ac.idCarretera = {recibido.idCarretera}");

            if (recibido.idTramo > 0)
                condicionesFiltro.Add($"ac.idTramo = {recibido.idTramo}");

            if (recibido.idClasificacionAccidente > 0)
                condicionesFiltro.Add($"ac.idClasificacionAccidente = {recibido.idClasificacionAccidente}");      

            if (recibido.idFactorAccidente > 0)
                condicionesFiltro.Add($"facAccSub.idFactor = {recibido.idFactorAccidente}");

            if (recibido.IdTipoVehiculo > 0)
                condicionesFiltro.Add($"veh.IdTipoVehiculo = {recibido.IdTipoVehiculo}");

            if (recibido.IdTipoServicio > 0)
                condicionesFiltro.Add($"veh.IdTipoServicio = {recibido.IdTipoServicio}");

			if (recibido.IdSubtipoServicio > 0)
				condicionesFiltro.Add($"veh.IdSubtipoServicio = {recibido.IdSubtipoServicio}");

			if (recibido.idCausaAccidente > 0)
                condicionesFiltro.Add($"accCau.idCausaAccidente = {recibido.idCausaAccidente}");

            if (recibido.idFactorOpcionAccidente > 0)
                condicionesFiltro.Add($"facAccSub.idFactorOpcion = {recibido.idFactorOpcionAccidente}");

			if (recibido.FechaInicio.HasValue)
			{
				condicionesFiltro.Add($"ac.fecha >= '{recibido.FechaInicio.Value.ToString("yyyy-MM-dd")}'");
			}

			if (recibido.FechaFin.HasValue)
			{
				condicionesFiltro.Add($"ac.fecha <= '{recibido.FechaFin.Value.ToString("yyyy-MM-dd")}'");
			}
			if (recibido.hora != TimeSpan.Zero)
                condicionesFiltro.Add($"ac.hora = '{recibido.hora}'");

			string condicionesSql = string.Join(" AND ", condicionesFiltro);

            string strQuery = @"SELECT DISTINCT
                                        ac.idAccidente,
                                        ac.numeroReporte AS Numreporteaccidente,
                                        CONVERT(varchar, ac.fecha, 103) AS Fecha,
                                        ac.hora AS Hora,
                                        ac.idMunicipio AS idMunicipio,
                                        ac.idOficinaDelegacion AS idOficinaDelegacion,
                                        ac.idElabora AS idOficial,
                                        ac.idCarretera AS idCarretera,
                                        ac.idTramo AS idTramo,
                                        ac.idClasificacionAccidente AS idClasificacionAccidente,
                                        del.delegacion AS Delegacion,
                                        mun.municipio,
                                        car.carretera,
                                        tram.tramo,
                                        ac.kilometro,
                                        ac.latitud,
                                        ac.longitud,
                                        veh.vehiculos AS Vehiculo,
                                        veh.idTipoVehiculo AS idTipoVehiculo,
                                        veh.idTipoServicio AS idTipoServicio,
                                        veh.idSubtipoServicio AS idSubtipoServicio,
                                        CONCAT(ofic.nombre, ' ', ofic.apellidoPaterno, ' ', ofic.apellidoMaterno) AS NombredelOficial,
                                        ac.montoCamino AS Dañosalcamino,
                                        ac.montoCarga AS Dañosacarga,
                                        ac.montoPropietarios AS Dañosapropietario,
                                        ac.montoOtros AS Otrosdaños,
                                        inv.Lesionados AS Lesionados,
                                        inv.Muertos AS Muertos,
                                        Factores.FactoresOpciones,
                                        Causas.CausasAccidente,
                                        ac.descripcionCausas AS CausasDescripcion
                                    FROM 
                                        accidentes ac 
                                    LEFT JOIN 
                                        catDelegaciones del ON del.idDelegacion = ac.idOficinaDelegacion
                                    LEFT JOIN 
                                        catMunicipios mun ON mun.idMunicipio = ac.idMunicipio
                                    LEFT JOIN 
                                        catCarreteras car ON car.idCarretera = ac.idCarretera
                                    LEFT JOIN 
                                        catTramos tram ON tram.idTramo = ac.idTramo
                                    LEFT JOIN 
                                        catOficiales ofic ON ofic.idOficial = ac.idElabora
                                    LEFT JOIN 
                                        accidenteCausas accCau ON accCau.idAccidente = ac.idAccidente 
                                    LEFT JOIN 
                                        catCausasAccidentes cauac ON cauac.idCausaAccidente = accCau.idCausaAccidente 
                                    LEFT JOIN 
                                        (
                                            SELECT 
                                                idAccidente,
                                                STUFF((
                                                    SELECT CHAR(13) + CHAR(10) + CONCAT(fac.factorAccidente, '/', facOp.factorOpcionAccidente) + ';'
                                                    FROM AccidenteFactoresOpciones facAccSub
                                                    LEFT JOIN catFactoresAccidentes fac ON fac.idFactorAccidente = facAccSub.idFactor
                                                    LEFT JOIN catFactoresOpcionesAccidentes facOp ON facOp.idFactorOpcionAccidente = facAccSub.idFactorOpcion
                                                    WHERE facAccSub.idAccidente = Accidentes.idAccidente AND facAccSub.estatus = 1
                                                    FOR XML PATH(''), TYPE
                                                ).value('.', 'VARCHAR(MAX)'), 1, 1, '') AS FactoresOpciones
                                            FROM accidentes AS Accidentes
                                            GROUP BY idAccidente
                                        ) AS Factores ON Factores.idAccidente = ac.idAccidente
                                    LEFT JOIN 
                                        (
                                            SELECT 
                                                idAccidente,
                                                STUFF((
                                                    SELECT CHAR(13) + CHAR(10) + CONCAT(cauac.causaAccidente, ':') + ','
                                                    FROM accidenteCausas accCau
                                                    LEFT JOIN catCausasAccidentes cauac ON cauac.idCausaAccidente = accCau.idCausaAccidente
                                                    WHERE accCau.idAccidente = Accidentes.idAccidente
                                                    FOR XML PATH(''), TYPE
                                                ).value('.', 'VARCHAR(MAX)'), 1, 1, '') AS CausasAccidente
                                            FROM accidentes AS Accidentes
                                            GROUP BY idAccidente
                                        ) AS Causas ON Causas.idAccidente = ac.idAccidente
                                    LEFT JOIN 
                                        (
                                            SELECT 
                                                idAccidente,
                                                STRING_AGG(Vehiculos, ';') AS vehiculos,
                                                MAX(idTipoVehiculo) AS idTipoVehiculo, 
                                                MAX(idCatTipoServicio) AS idTipoServicio,
                                                MAX(idSubtipoServicio) AS idSubtipoServicio

                                            FROM 
                                                (
                                                    SELECT 
                                                        ac.idAccidente,
                                                        veh.idTipoVehiculo,  
                                                        veh.idCatTipoServicio,
                                                        CONCAT(
                                                            '  Vehículo ', 
                                                            ROW_NUMBER() OVER (PARTITION BY ac.idAccidente ORDER BY ac.idAccidente),
                                                            ':  ', 
                                                            mv.marcaVehiculo, ' ', 
                                                            sm.nombreSubmarca, ' ', 
                                                            veh.modelo, ', TIPO: ', 
                                                            tv.tipoVehiculo, ', Servicio: ', 
                                                            ts.tipoServicio, ', Placa: ', 
                                                            veh.placas, ', Serie: ', 
                                                            veh.serie
                                                        ) AS Vehiculos
                                                    FROM 
                                                        accidentes ac
                                                        LEFT JOIN vehiculosAccidente vehacc ON vehacc.idAccidente = ac.idAccidente AND vehacc.estatus = 1 
                                                        LEFT JOIN vehiculos veh ON veh.idVehiculo = vehacc.idVehiculo
                                                        LEFT JOIN catMarcasVehiculos mv ON mv.idMarcaVehiculo = veh.idMarcaVehiculo
                                                        LEFT JOIN catSubmarcasVehiculos sm ON sm.idSubmarca = veh.idSubmarca
                                                        LEFT JOIN catEntidades e ON e.idEntidad = veh.idEntidad
                                                        LEFT JOIN catColores cc ON cc.idColor = veh.idColor
                                                        LEFT JOIN catTiposVehiculo tv ON tv.idTipoVehiculo = veh.idTipoVehiculo
                                                        LEFT JOIN catTipoServicio ts ON ts.idCatTipoServicio = veh.idCatTipoServicio
                                                ) AS veh
                                            GROUP BY 
                                                idAccidente
                                        ) veh ON veh.idAccidente = ac.idAccidente
                                    LEFT JOIN 
                                        (
                                            SELECT 
                                                acc.idAccidente,
                                                COUNT(CASE WHEN invacc.idEstadoVictima = 1 THEN invacc.idEstadoVictima END) AS Lesionados,
                                                COUNT(CASE WHEN invacc.idEstadoVictima = 2 THEN invacc.idEstadoVictima END) AS Muertos
                                            FROM 
                                                accidentes acc
                                            INNER JOIN 
                                                involucradosAccidente invacc ON invacc.idAccidente = acc.idAccidente
                                            GROUP BY 
                                                acc.idAccidente
                                        ) inv ON inv.idAccidente = ac.idAccidente
                                    WHERE 
                                        ac.estatus <> 0 ";

			                        if (!string.IsNullOrWhiteSpace(condicionesSql))
			                        {
				                        strQuery += " AND " + condicionesSql;
			                        }
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    int numeroSecuencial = 1; // Inicializa el número secuencial en 1
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ListadoAccidentesPorAccidenteModel model = new ListadoAccidentesPorAccidenteModel();
                            model.Numreporteaccidente = reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString();
							model.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
							model.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
							model.IdOficial = reader["idOficial"] != DBNull.Value ? Convert.ToInt32(reader["idOficial"]) : 0;
							model.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
							model.idTramo = reader["idTramo"] != DBNull.Value ? Convert.ToInt32(reader["idTramo"]) : 0;
							model.idClasificacionAccidente = reader["idClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idClasificacionAccidente"]) : 0;
							//model.idCausaAccidente = reader["idCausaAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idCausaAccidente"]) : 0;
							//model.idFactorAccidente = reader["idFactorAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorAccidente"]) : 0;
							//model.idFactorOpcionAccidente = reader["idFactorOpcionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorOpcionAccidente"]) : 0;
							model.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
							model.IdTipoServicio = reader["idTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idTipoServicio"]) : 0;
							model.IdSubtipoServicio = reader["idSubtipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idSubtipoServicio"]) : 0;

							model.Fecha = reader["Fecha"] == System.DBNull.Value ? default(string) : reader["Fecha"].ToString();
                            model.Hora = reader["Hora"] == System.DBNull.Value ? default(string) : reader["Hora"].ToString();
                            model.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
                            model.carretera = reader["carretera"] == System.DBNull.Value ? default(string) : reader["carretera"].ToString();
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
                            model.Causas = reader["CausasAccidente"] == System.DBNull.Value ? default(string) : reader["CausasAccidente"].ToString();
                            model.CausasDescripcion = reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString();
							if (!string.IsNullOrEmpty(model.FactoresOpciones))
							{
								string cleanedString = model.FactoresOpciones.Replace("\n", "").Replace("\r", "");

								string[] factoresOpcionesArray = cleanedString.Split(new[] { ": ", ":" }, StringSplitOptions.RemoveEmptyEntries);

                                for (int i = 0; i < factoresOpcionesArray.Length; i += 3)
                                {
                                    if (i + 2 < factoresOpcionesArray.Length)
                                    {
                                        if (int.TryParse(factoresOpcionesArray[i], out int idFactorAccidente) &&
                                            int.TryParse(factoresOpcionesArray[i + 1], out int idFactorOpcionAccidente))
                                        {
                                            model.idFactorAccidente = idFactorAccidente;
                                            model.idFactorOpcionAccidente = idFactorOpcionAccidente;


                                        }
                                    }
                                }
							}
							model.NumeroSecuencial = numeroSecuencial;
                            modelList.Add(model);
                            numeroSecuencial++;
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
        //public List<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo(BusquedaAccidentesModel model)
        public IEnumerable<ListadoAccidentesPorVehiculoModel> AccidentesPorVehiculo(BusquedaAccidentesModel model)

        {
            List<ListadoAccidentesPorVehiculoModel> modelList = new List<ListadoAccidentesPorVehiculoModel>();
            var condicionesFiltro = new List<string>();

            if (model.idMunicipio > 0)
                condicionesFiltro.Add($"ac.idMunicipio = {model.idMunicipio}");

            if (model.idDelegacion > 0)
                condicionesFiltro.Add($"ac.idOficinaDelegacion = {model.idDelegacion}");

            if (model.IdOficial > 0)
                condicionesFiltro.Add($"ac.idElabora = {model.IdOficial}");

            if (model.idCarretera > 0)
                condicionesFiltro.Add($"ac.idCarretera = {model.idCarretera}");

            if (model.idTramo > 0)
                condicionesFiltro.Add($"ac.idTramo = {model.idTramo}");

            if (model.idClasificacionAccidente > 0)
                condicionesFiltro.Add($"ac.idClasificacionAccidente = {model.idClasificacionAccidente}");

            if (model.idCausaAccidente > 0)
                condicionesFiltro.Add($"accCau.idCausaAccidente = {model.idCausaAccidente}");

            if (model.idFactorAccidente > 0)
                condicionesFiltro.Add($"factAcc.idFactor = {model.idFactorAccidente}");

            if (model.IdTipoVehiculo > 0)
                condicionesFiltro.Add($"veh.IdTipoVehiculo = {model.IdTipoVehiculo}");

            if (model.IdTipoServicio > 0)
                condicionesFiltro.Add($"veh.IdTipoServicio = {model.IdTipoServicio}");

			if (model.IdSubtipoServicio > 0)
				condicionesFiltro.Add($"veh.IdSubtipoServicio = {model.IdSubtipoServicio}");

			if (model.idFactorOpcionAccidente > 0)
                condicionesFiltro.Add($"factAcc.idFactorOpcion = {model.idFactorOpcionAccidente}");

			if (model.FechaInicio.HasValue)
			{
				condicionesFiltro.Add($"ac.fecha >= '{model.FechaInicio.Value.ToString("yyyy-MM-dd")}'");
			}

			if (model.FechaFin.HasValue)
			{
				condicionesFiltro.Add($"ac.fecha <= '{model.FechaFin.Value.ToString("yyyy-MM-dd")}'");
			}

	
			if (model.hora != TimeSpan.Zero)
                condicionesFiltro.Add($"ac.hora = '{model.hora}'");

            string condicionesSql = string.Join(" AND ", condicionesFiltro);

            string strQuery = @"SELECT DISTINCT  
                ac.numeroReporte		as Numreporteaccidente
                ,ac.fecha				as Fecha
                ,ac.hora				as Hora
                ,ac.idMunicipio         as idMunicipio
                ,ac.idOficinaDelegacion as idOficinaDelegacion
                ,ac.idElabora           as idOficial
                ,ac.idCarretera         as idCarretera
                ,ac.idTramo             as idTramo
                ,accCau.idCausaAccidente  as idCausaAccidente
                ,ac.idClasificacionAccidente   as idClasificacionAccidente
                ,factAcc.idFactor       as idFactorAccidente
                ,factAcc.idFactorOpcion  as idFactorOpcionAccidente
                ,veh.placas				as PlacasVeh
                ,veh.serie				as SerieVeh
                ,veh.idTipoVehiculo     as idTipoVehiculo 
                ,veh.idSubtipoServicio  as idSubtipoServicio
                ,veh.idCatTipoServicio  as idTipoServicio
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
                LEFT JOIN AccidenteFactoresOpciones factAcc on factAcc.idAccidente = ac.idAccidente
                LEFT JOIN catFactoresAccidentes fac on fac.idFactorAccidente = factAcc.idFactor
                LEFT JOIN catFactoresOpcionesAccidentes facOp on facOp.idFactorAccidente = factAcc.idFactorOpcion
                LEFT JOIN catOficiales ofic on ofic.idOficial = ac.idElabora
                LEFT JOIN accidenteCausas accCau on accCau.idAccidente = ac.idAccidente
                LEFT JOIN catCausasAccidentes cauac on cauac.idCausaAccidente = accCau.idCausaAccidente
                left join vehiculosAccidente vehacc on vehacc.idAccidente = ac.idAccidente AND vehacc.estatus = 1
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
                LEFT JOIN personas prop on prop.idPersona = veh.propietario
                WHERE ac.estatus <> 0";
			if (!string.IsNullOrWhiteSpace(condicionesSql))
			{
				strQuery += " AND " + condicionesSql;
			}

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    int numeroContinuo = 1;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ListadoAccidentesPorVehiculoModel vehiculo = new ListadoAccidentesPorVehiculoModel();
                            vehiculo.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
							vehiculo.idDelegacion = reader["idOficinaDelegacion"] != DBNull.Value ? Convert.ToInt32(reader["idOficinaDelegacion"]) : 0;
							vehiculo.IdOficial = reader["idOficial"] != DBNull.Value ? Convert.ToInt32(reader["idOficial"]) : 0;
							vehiculo.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
							vehiculo.idTramo = reader["idTramo"] != DBNull.Value ? Convert.ToInt32(reader["idTramo"]) : 0;
							vehiculo.idClasificacionAccidente = reader["idClasificacionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idClasificacionAccidente"]) : 0;
							vehiculo.idCausaAccidente = reader["idCausaAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idCausaAccidente"]) : 0;
							vehiculo.idFactorAccidente = reader["idFactorAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorAccidente"]) : 0;
							vehiculo.idFactorOpcionAccidente = reader["idFactorOpcionAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idFactorOpcionAccidente"]) : 0;
							vehiculo.IdTipoVehiculo = reader["idTipoVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idTipoVehiculo"]) : 0;
							vehiculo.IdTipoServicio = reader["idTipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idTipoServicio"]) : 0;
							vehiculo.IdSubtipoServicio = reader["idSubtipoServicio"] != DBNull.Value ? Convert.ToInt32(reader["idSubtipoServicio"]) : 0;

							vehiculo.Numreporteaccidente = reader["Numreporteaccidente"] == System.DBNull.Value ? default(string) : reader["Numreporteaccidente"].ToString();
                            //vehiculo.NumVeh = reader["NumVeh"] == System.DBNull.Value ? default(string) : reader["NumVeh"].ToString();
                            vehiculo.PlacasVeh = reader["PlacasVeh"] == System.DBNull.Value ? default(string) : reader["PlacasVeh"].ToString();
                            vehiculo.SerieVeh = reader["SerieVeh"] == System.DBNull.Value ? default(string) : reader["SerieVeh"].ToString();
                            vehiculo.PropietarioVeh = reader["PropietarioVeh"] == System.DBNull.Value ? default(string) : reader["PropietarioVeh"].ToString();
                            vehiculo.TipoVeh = reader["TipoVeh"] == System.DBNull.Value ? default(string) : reader["TipoVeh"].ToString();
                            vehiculo.ServicioVeh = reader["ServicioVeh"] == System.DBNull.Value ? default(string) : reader["ServicioVeh"].ToString();
                            vehiculo.Marca = reader["Marca"] == System.DBNull.Value ? default(string) : reader["Marca"].ToString();
                            vehiculo.Submarca = reader["Submarca"] == System.DBNull.Value ? default(string) : reader["Submarca"].ToString();
                            vehiculo.Modelo = reader["Modelo"] == System.DBNull.Value ? default(string) : reader["Modelo"].ToString();
                            vehiculo.ConductorVeh = reader["ConductorVeh"] == System.DBNull.Value ? default(string) : reader["ConductorVeh"].ToString();
                            vehiculo.DepositoVehículo = reader["DepositoVehículo"] == System.DBNull.Value ? default(string) : reader["DepositoVehículo"].ToString();
                            vehiculo.Delegacion = reader["Delegacion"] == System.DBNull.Value ? default(string) : reader["Delegacion"].ToString();
                            vehiculo.Municipio = reader["Municipio"] == System.DBNull.Value ? default(string) : reader["Municipio"].ToString();
                            vehiculo.Carretera = reader["Carretera"] == System.DBNull.Value ? default(string) : reader["Carretera"].ToString();
                            vehiculo.Tramo = reader["Tramo"] == System.DBNull.Value ? default(string) : reader["Tramo"].ToString();
                            vehiculo.Kilómetro = reader["Kilómetro"] == System.DBNull.Value ? default(string) : reader["Kilómetro"].ToString();
                            vehiculo.Latitud = reader["Latitud"] == System.DBNull.Value ? default(string) : reader["Latitud"].ToString();
                            vehiculo.Longitud = reader["Longitud"] == System.DBNull.Value ? default(string) : reader["Longitud"].ToString();
                            vehiculo.NombredelOficial = reader["NombredelOficial"] == System.DBNull.Value ? default(string) : reader["NombredelOficial"].ToString();
                            vehiculo.Dañosalcamino = reader["Dañosalcamino"] == System.DBNull.Value ? default(string) : reader["Dañosalcamino"].ToString();
                            vehiculo.DañosaCarga = reader["DañosaCarga"] == System.DBNull.Value ? default(string) : reader["DañosaCarga"].ToString();
                            vehiculo.Dañosapropietario = reader["Dañosapropietario"] == System.DBNull.Value ? default(string) : reader["Dañosapropietario"].ToString();
                            vehiculo.Otrosdaños = reader["Otrosdaños"] == System.DBNull.Value ? default(string) : reader["Otrosdaños"].ToString();
                            vehiculo.Lesionados = reader["Lesionados"] == System.DBNull.Value ? default(string) : reader["Lesionados"].ToString();
                            vehiculo.Muertos = reader["Muertos"] == System.DBNull.Value ? default(string) : reader["Muertos"].ToString();
                            vehiculo.Causas = reader["Causas"] == System.DBNull.Value ? default(string) : reader["Causas"].ToString();
                            vehiculo.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
                            vehiculo.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;
                            vehiculo.CausasDescripcion = reader["CausasDescripcion"] == System.DBNull.Value ? default(string) : reader["CausasDescripcion"].ToString();
                            vehiculo.NumeroContinuo = numeroContinuo;
                            modelList.Add(vehiculo);
                            numeroContinuo++;

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
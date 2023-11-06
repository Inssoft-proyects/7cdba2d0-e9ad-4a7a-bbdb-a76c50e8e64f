using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Text;

namespace GuanajuatoAdminUsuarios.Services
{
    public class BusquedaAccidentesService : IBusquedaAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public BusquedaAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<BusquedaAccidentesModel> BusquedaAccidentes(BusquedaAccidentesModel model, int idOficina)
        {
            //
            List<BusquedaAccidentesModel> ListaAccidentes = new List<BusquedaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa, a.idAutoriza, a.kilometro, a.idOficinaDelegacion, " +
                        "mun.municipio, " +
                        "car.carretera, " +
                        "tra.tramo, " +
                        "er.estatusReporte,er.idEstatusReporte, " +
                        "MAX(vea.placa) AS placa, MAX(vea.serie) AS serie, " +
                        "MAX(cond.idPersona) AS idConductor, " +
                        "MAX(vea.idPersona) AS idPropietario, " +
                        "ela.idOficial AS elabora, " +
                        "sup.idOficial AS supervisa, " +
                        "aut.idOficial AS autoriza " +
                        "FROM accidentes AS a " +
                        "LEFT JOIN vehiculosAccidente AS vea ON a.idAccidente = vea.idAccidente " +
                        "LEFT JOIN vehiculos AS v ON vea.idVehiculo = v.idVehiculo " +
                        "LEFT JOIN conductoresVehiculosAccidente AS cva ON a.idAccidente = cva.idAccidente " +
                        "LEFT JOIN personas AS cond ON cva.idPersona = cond.idPersona " +
                        "LEFT JOIN personas AS prop ON vea.idPersona = prop.idPersona " +
                        "LEFT JOIN catMunicipios AS mun ON a.idMunicipio = mun.idMunicipio " +
                        "LEFT JOIN catCarreteras AS car ON a.idCarretera = car.idCarretera " +
                        "LEFT JOIN catTramos AS tra ON a.idTramo = tra.idTramo " +
                        "LEFT JOIN catEstatusReporteAccidente AS er ON a.idEstatusReporte = er.idEstatusReporte " +
                        "LEFT JOIN catOficiales AS ela ON a.idElabora = ela.idOficial " +
                        "LEFT JOIN catOficiales AS sup ON a.idSupervisa = sup.idOficial " +
                        "LEFT JOIN catOficiales AS aut ON a.idAutoriza = aut.idOficial " +
                        "WHERE (vea.placa = @placasBusqueda OR a.fecha BETWEEN @fechaInicio AND @fechaFin " +
                        "OR UPPER(a.numeroReporte) = @oficioBusqueda " +
                        "OR a.idAutoriza = @idOficialBusqueda " +
                        "OR a.idSupervisa = @idOficialBusqueda " +
                        "OR a.idAutoriza = @idOficialBusqueda " +
                        "OR a.idCarretera = @idCarreteraBusqueda " +
                        "OR a.idTramo = @idTramoBusqueda " +
                        "OR prop.nombre = @propietarioBusqueda " +
                        "OR UPPER(prop.apellidoPaterno) = @propietarioBusqueda " +
                        "OR UPPER(prop.apellidoMaterno) = @propietarioBusqueda " +
                        "OR cond.nombre = @conductorBusqueda " +
                        "OR UPPER(cond.apellidoPaterno) = @conductorBusqueda " +
                        "OR UPPER(cond.apellidoMaterno) = @conductorBusqueda " +
                        "OR vea.serie = @serieBusqueda)" +
                        "AND a.idOficinaDelegacion = @idOficina AND a.estatus != 0 " +
                        "GROUP BY a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa,a. idAutoriza,a.kilometro,a.idOficinaDelegacion, " +
                        "mun.municipio, car.carretera, tra.tramo, er.estatusReporte,er.idEstatusReporte, ela.idOficial, sup.idOficial, aut.idOficial; ", connection);


                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = (model.FechaInicio == DateTime.MinValue) ? DBNull.Value : (object)model.FechaInicio;

                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = (model.FechaFin == DateTime.MinValue) ? DBNull.Value : (object)model.FechaFin;

                    command.Parameters.Add(new SqlParameter("@oficioBusqueda", SqlDbType.NVarChar)).Value = (object)model.folioBusqueda != null ? model.folioBusqueda.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDelegacionBusqueda", SqlDbType.Int)).Value = (object)model.IdDelegacionBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficialBusqueda", SqlDbType.Int)).Value = (object)model.IdOficialBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarreteraBusqueda", SqlDbType.Int)).Value = (object)model.IdCarreteraBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTramoBusqueda", SqlDbType.Int)).Value = (object)model.IdTramoBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@propietarioBusqueda", SqlDbType.NVarChar)).Value = (object)model.propietarioBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@conductorBusqueda", SqlDbType.NVarChar)).Value = (object)model.conductorBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placasBusqueda", SqlDbType.NVarChar)).Value = (object)model.placasBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@serieBusqueda", SqlDbType.NVarChar)).Value = (object)model.serieBusqueda ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        
                        
                        {
                            BusquedaAccidentesModel accidente = new BusquedaAccidentesModel();
                            accidente.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
                            accidente.idMunicipio = Convert.IsDBNull(reader["idMunicipio"]) ? 0 : Convert.ToInt32(reader["idMunicipio"]);
                            accidente.idCarretera = Convert.IsDBNull(reader["idCarretera"]) ? 0 : Convert.ToInt32(reader["idCarretera"]);
                            accidente.idTramo = Convert.IsDBNull(reader["idTramo"]) ? 0 : Convert.ToInt32(reader["idTramo"]);
                            accidente.kilometro = reader["kilometro"].ToString();
                            accidente.idEstatusReporte = Convert.IsDBNull(reader["idEstatusReporte"]) ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
                            accidente.estatusReporte = reader["estatusReporte"].ToString();
                            accidente.municipio = reader["municipio"].ToString();
                            accidente.carretera = reader["carretera"].ToString();
                            accidente.tramo = reader["tramo"].ToString();
                            accidente.idElabora = Convert.IsDBNull(reader["idElabora"]) ? 0 : Convert.ToInt32(reader["idElabora"]);
                            accidente.idSupervisa = Convert.IsDBNull(reader["idSupervisa"]) ? 0 : Convert.ToInt32(reader["idSupervisa"]);
                            accidente.idAutoriza = Convert.IsDBNull(reader["idAutoriza"]) ? 0 : Convert.ToInt32(reader["idAutoriza"]);
                            accidente.idConductor = Convert.IsDBNull(reader["idConductor"]) ? 0 : Convert.ToInt32(reader["idConductor"]);
                            accidente.idPropietario = Convert.IsDBNull(reader["idPropietario"]) ? 0 : Convert.ToInt32(reader["idPropietario"]);
                            accidente.numeroReporte = reader["numeroReporte"].ToString();
                            accidente.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
                            accidente.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;

                            ListaAccidentes.Add(accidente);

                        }

                    }

                }
                catch (SqlException ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            return ListaAccidentes;


        }
        public List<BusquedaAccidentesPDFModel> BusquedaAccidentes(BusquedaAccidentesPDFModel model, int idOficina)
        {
            //
            List<BusquedaAccidentesPDFModel> ListaAccidentes = new List<BusquedaAccidentesPDFModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa, a.idAutoriza, a.kilometro, a.idOficinaDelegacion, " +
                        "mun.municipio, " +
                        "car.carretera, " +
                        "tra.tramo, " +
                        "er.estatusReporte,er.idEstatusReporte, " +
                        "MAX(vea.placa) AS placa, MAX(vea.serie) AS serie, " +
                        "MAX(cond.idPersona) AS idConductor, " +
                        "MAX(vea.idPersona) AS idPropietario, " +
                        "ela.idOficial AS elabora, " +
                        "sup.idOficial AS supervisa, " +
                        "aut.idOficial AS autoriza " +
                        "FROM accidentes AS a " +
                        "LEFT JOIN vehiculosAccidente AS vea ON a.idAccidente = vea.idAccidente " +
                        "LEFT JOIN vehiculos AS v ON vea.idVehiculo = v.idVehiculo " +
                        "LEFT JOIN conductoresVehiculosAccidente AS cva ON a.idAccidente = cva.idAccidente " +
                        "LEFT JOIN personas AS cond ON cva.idPersona = cond.idPersona " +
                        "LEFT JOIN personas AS prop ON vea.idPersona = prop.idPersona " +
                        "LEFT JOIN catMunicipios AS mun ON a.idMunicipio = mun.idMunicipio " +
                        "LEFT JOIN catCarreteras AS car ON a.idCarretera = car.idCarretera " +
                        "LEFT JOIN catTramos AS tra ON a.idTramo = tra.idTramo " +
                        "LEFT JOIN catEstatusReporteAccidente AS er ON a.idEstatusReporte = er.idEstatusReporte " +
                        "LEFT JOIN catOficiales AS ela ON a.idElabora = ela.idOficial " +
                        "LEFT JOIN catOficiales AS sup ON a.idSupervisa = sup.idOficial " +
                        "LEFT JOIN catOficiales AS aut ON a.idAutoriza = aut.idOficial " +
                        "WHERE (vea.placa = @placasBusqueda " +
                        "OR UPPER(a.numeroReporte) = @oficioBusqueda " +
                        "OR a.idAutoriza = @idOficialBusqueda " +
                        "OR a.idSupervisa = @idOficialBusqueda " +
                        "OR a.idAutoriza = @idOficialBusqueda " +
                        "OR a.idCarretera = @idCarreteraBusqueda " +
                        "OR a.idTramo = @idTramoBusqueda " +
                        "OR prop.nombre = @propietarioBusqueda " +
                        "OR UPPER(prop.apellidoPaterno) = @propietarioBusqueda " +
                        "OR UPPER(prop.apellidoMaterno) = @propietarioBusqueda " +
                        "OR cond.nombre = @conductorBusqueda " +
                        "OR UPPER(cond.apellidoPaterno) = @conductorBusqueda " +
                        "OR UPPER(cond.apellidoMaterno) = @conductorBusqueda " +
                        "OR vea.serie = @serieBusqueda)" +
                        "AND a.idOficinaDelegacion = @idOficina AND  a.estatus != 0 " +
                       "AND ((@fechaInicio IS NULL AND @fechaFin IS NULL) OR (a.fecha BETWEEN @fechaInicio AND @fechaFin)) " +
                    "GROUP BY a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idTramo, a.idCarretera, a.idElabora, a.idSupervisa,a. idAutoriza,a.kilometro,a.idOficinaDelegacion, " +
                        "mun.municipio, car.carretera, tra.tramo, er.estatusReporte,er.idEstatusReporte, ela.idOficial, sup.idOficial, aut.idOficial; ", connection);


                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime)).Value = (model.FechaInicio == DateTime.MinValue) ? DBNull.Value : (object)model.FechaInicio;

                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime)).Value = (model.FechaFin == DateTime.MinValue) ? DBNull.Value : (object)model.FechaFin;

                    command.Parameters.Add(new SqlParameter("@oficioBusqueda", SqlDbType.NVarChar)).Value = (object)model.folio != null ? model.folioBusqueda.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idDelegacionBusqueda", SqlDbType.Int)).Value = (object)model.idDelegacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficialBusqueda", SqlDbType.Int)).Value = (object)model.IdOficial ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarreteraBusqueda", SqlDbType.Int)).Value = (object)model.idCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTramoBusqueda", SqlDbType.Int)).Value = (object)model.idTramo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@propietarioBusqueda", SqlDbType.NVarChar)).Value = (object)model.propietario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@conductorBusqueda", SqlDbType.NVarChar)).Value = (object)model.conductor ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placasBusqueda", SqlDbType.NVarChar)).Value = (object)model.placa ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@serieBusqueda", SqlDbType.NVarChar)).Value = (object)model.serie ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            BusquedaAccidentesPDFModel accidente = new BusquedaAccidentesPDFModel();
                            accidente.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
                            accidente.idMunicipio = Convert.IsDBNull(reader["idMunicipio"]) ? 0 : Convert.ToInt32(reader["idMunicipio"]);
                            accidente.idCarretera = Convert.IsDBNull(reader["idCarretera"]) ? 0 : Convert.ToInt32(reader["idCarretera"]);
                            accidente.idTramo = Convert.IsDBNull(reader["idTramo"]) ? 0 : Convert.ToInt32(reader["idTramo"]);
                            accidente.kilometro = reader["kilometro"].ToString();
                            accidente.idEstatusReporte = Convert.IsDBNull(reader["idEstatusReporte"]) ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
                            accidente.estatusReporte = reader["estatusReporte"].ToString();
                            accidente.municipio = reader["municipio"].ToString();
                            accidente.idElabora = Convert.IsDBNull(reader["idElabora"]) ? 0 : Convert.ToInt32(reader["idElabora"]);
                            accidente.idSupervisa = Convert.IsDBNull(reader["idSupervisa"]) ? 0 : Convert.ToInt32(reader["idSupervisa"]);
                            accidente.idAutoriza = Convert.IsDBNull(reader["idAutoriza"]) ? 0 : Convert.ToInt32(reader["idAutoriza"]);
                            accidente.idConductor = Convert.IsDBNull(reader["idConductor"]) ? 0 : Convert.ToInt32(reader["idConductor"]);
                            accidente.idPropietario = Convert.IsDBNull(reader["idPropietario"]) ? 0 : Convert.ToInt32(reader["idPropietario"]);
                            accidente.numeroReporte = reader["numeroReporte"].ToString();
                            accidente.fecha = reader["fecha"] != DBNull.Value ? reader["fecha"].ToString().Split(" ")[0] : string.Empty;
                            accidente.hora = reader["hora"] != DBNull.Value ? TimeSpan.Parse(reader["hora"].ToString()) : TimeSpan.MinValue;

                            ListaAccidentes.Add(accidente);

                        }

                    }

                }
                catch (SqlException ex)
                {

                }
                finally
                {
                    connection.Close();
                }
            return ListaAccidentes;


        }
        public BusquedaAccidentesPDFModel ObtenerAccidentePorId(int idAccidente)
        {
            BusquedaAccidentesPDFModel accidente = new BusquedaAccidentesPDFModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idCarretera, a.idTramo, a.kilometro, " +
                                                         "m.municipio, c.carretera, t.tramo, e.estatusDesc,va.Idpersona AS propietario,prop.nombre AS nombreProp, prop.apellidoPaterno AS apellidoPaternoProp, prop.apellidoMaterno AS apellidoMaternoProp , " +
                                                         "cond.nombre AS nombreCond, cond.apellidoPaterno AS apellidoPaternoCond, cond.apellidoMaterno AS apellidoMaternoCond " +
                                                         "FROM accidentes AS a JOIN catMunicipios AS m ON a.idMunicipio = m.idMunicipio " +
                                                         "JOIN catCarreteras AS c ON a.idCarretera = c.idCarretera " +
                                                         "JOIN catTramos AS t ON a.idTramo = t.idTramo " +
                                                         "JOIN estatus AS e ON a.estatus = e.estatus " +
                                                         "LEFT JOIN vehiculosAccidente AS va ON a.idAccidente = va.idAccidente " +
                                                         "LEFT JOIN personas AS prop ON prop.idPersona = va.idPersona " +
                                                         "LEFT JOIN conductoresVehiculosAccidente as cva ON a.idAccidente = cva.idAccidente " +
                                                         "LEFT JOIN personas AS cond ON cond.idPersona = cva.idPersona " +
                                                         "WHERE a.idAccidente = @idAccidente AND a.estatus = 1;", connection);

                    command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            accidente.IdAccidente = reader["idAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idAccidente"]) : 0;
                            accidente.numeroReporte = reader["numeroReporte"] != DBNull.Value ? reader["numeroReporte"].ToString() : string.Empty;
                            accidente.fecha = reader["fecha"] != DBNull.Value ? reader["fecha"].ToString().Split(" ")[0] : string.Empty;
                            accidente.hora = reader["hora"] != DBNull.Value ? reader.GetTimeSpan(reader.GetOrdinal("hora")) : TimeSpan.MinValue;
                            accidente.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
                            accidente.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;

                            accidente.municipio = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : string.Empty;
                            accidente.nombreConductor = reader["nombreCond"] != DBNull.Value ? reader["nombreCond"].ToString() : string.Empty;
                            accidente.apellidoPaternoConductor = reader["apellidoPaternoCond"] != DBNull.Value ? reader["apellidoPaternoCond"].ToString() : string.Empty;
                            accidente.apellidoMaternoConductor = reader["apellidoMaternoCond"] != DBNull.Value ? reader["apellidoMaternoCond"].ToString() : string.Empty;
                            accidente.nombreConductorCompleto = $"{reader["nombreCond"]} {reader["apellidoPaternoCond"]} {reader["apellidoMaternoCond"]}";
                            accidente.nombrePropietario = reader["nombreProp"] != DBNull.Value ? reader["nombreProp"].ToString() : string.Empty;
                            accidente.apellidoPaternoPropietario = reader["apellidoPaternoProp"] != DBNull.Value ? reader["apellidoPaternoProp"].ToString() : string.Empty;
                            accidente.apellidoMaternoPropietario = reader["apellidoMaternoProp"] != DBNull.Value ? reader["apellidoMaternoProp"].ToString() : string.Empty;
                            accidente.nombrePropietarioCompleto = $"{reader["nombreProp"]} {reader["apellidoPaternoProp"]} {reader["apellidoMaternoProp"]}";

                            accidente.municipio = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : string.Empty;



                            accidente.tramo = reader["tramo"] != DBNull.Value ? reader["tramo"].ToString() : string.Empty;
                            accidente.carretera = reader["carretera"] != DBNull.Value ? reader["carretera"].ToString() : string.Empty;
                            accidente.kilometro = reader["kilometro"] != DBNull.Value ? reader["kilometro"].ToString() : string.Empty;
                            accidente.idTramo = reader["idTramo"] != DBNull.Value ? Convert.ToInt32(reader["idTramo"]) : 0;


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

            return accidente;
        }

        public List<BusquedaAccidentesModel> ObtenerAccidentes(BusquedaAccidentesModel model)
        {
            //
            List<BusquedaAccidentesModel> ListaAccidentes = new List<BusquedaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(" SELECT acc.idAccidente, acc.idMunicipio, acc.idCarretera, acc.idTramo, acc.idEstatusReporte, acc.estatus,acc.numeroReporte,acc.fecha,acc.hora, " +
                                                        "mun.Municipio, car.Carretera, tra.Tramo, er.estatusReporte, " +
                                                        "MAX(prop.nombre) AS nombrePropietario, MAX(prop.apellidoPaterno) AS apellidoPaternoPropietario, MAX(prop.apellidoMaterno) AS apellidoMaternoPropietario, " +
                                                        "MAX(cond.nombre) AS nombreConductor, MAX(cond.apellidoPaterno) AS apellidoPaternoConductor, MAX(cond.apellidoMaterno) AS apellidoMaternoConductor " +
                                                        "FROM accidentes AS acc " +
                                                        "LEFT JOIN catMunicipios AS mun ON acc.idMunicipio = mun.idMunicipio " +
                                                        "LEFT JOIN catCarreteras AS car ON acc.idCarretera = car.idCarretera " +
                                                        "LEFT JOIN catTramos AS tra ON acc.idTramo = tra.idTramo " +
                                                        "LEFT JOIN vehiculosAccidente AS vea ON acc.idAccidente = vea.idAccidente " +
                                                        "LEFT JOIN personas AS prop ON prop.idPersona = vea.idPersona " +
                                                        "LEFT JOIN conductoresVehiculosAccidente AS cva ON acc.idAccidente = cva.idAccidente " +
                                                        "LEFT JOIN personas AS cond ON cond.idPersona = cva.idPersona " +
                                                        "LEFT JOIN catEstatusReporteAccidente AS er ON acc.idEstatusReporte = er.idEstatusReporte " +
                                                        "WHERE acc.estatus = 1 " +
                                                        "GROUP BY acc.idAccidente, acc.idMunicipio, acc.idCarretera, acc.idTramo, acc.idEstatusReporte, acc.estatus,acc.numeroReporte,acc.fecha,acc.hora, " +
                                                        "mun.Municipio, car.Carretera, tra.Tramo, er.estatusReporte;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            BusquedaAccidentesModel accidente = new BusquedaAccidentesModel();
                            accidente.IdAccidente = reader["IdAccidente"] != DBNull.Value ? Convert.ToInt32(reader["IdAccidente"]) : 0;
                            accidente.numeroReporte = reader["numeroReporte"] != DBNull.Value ? reader["numeroReporte"].ToString() : string.Empty;
                            accidente.fecha = reader["fecha"] != DBNull.Value ? Convert.ToDateTime(reader["fecha"]) : DateTime.MinValue;
                            accidente.hora = reader["hora"] != DBNull.Value ? reader.GetTimeSpan(reader.GetOrdinal("hora")) : TimeSpan.Zero;
                            accidente.idMunicipio = reader["idMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["idMunicipio"]) : 0;
                            accidente.idCarretera = reader["idCarretera"] != DBNull.Value ? Convert.ToInt32(reader["idCarretera"]) : 0;
                            accidente.idTramo = reader["IdTramo"] != DBNull.Value ? Convert.ToInt32(reader["IdTramo"]) : 0;
                            accidente.idEstatusReporte = reader["idEstatusReporte"] != DBNull.Value ? Convert.ToInt32(reader["idEstatusReporte"]) : 0;
                            accidente.municipio = reader["municipio"] != DBNull.Value ? reader["municipio"].ToString() : string.Empty;
                            accidente.tramo = reader["tramo"] != DBNull.Value ? reader["tramo"].ToString() : string.Empty;
                            accidente.carretera = reader["carretera"] != DBNull.Value ? reader["carretera"].ToString() : string.Empty;


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

    }
}

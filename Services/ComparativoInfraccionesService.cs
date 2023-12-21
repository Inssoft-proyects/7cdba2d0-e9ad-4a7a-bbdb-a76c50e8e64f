using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GuanajuatoAdminUsuarios.Services
{
    public class ComparativoInfraccionesService : IComparativoInfraccionesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public ComparativoInfraccionesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }               

        public List<ResultadoGeneral> BusquedaResultadosGenerales(ComparativoInfraccionesModel modelBusqueda)
        {
            List<ResultadoGeneral> modelList = new List<ResultadoGeneral>();
            string condiciones = "";

            condiciones += " AND YEAR(inf.fechaInfraccion) in (@año1,@año2) ";
            condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion == 0 ? "" : " AND inf.idDelegacion = @idDelegacion ";
            condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND inf.idOficial =@idOficial ";
            condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND inf.idCarretera = @idCarretera ";
            condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND inf.idTramo = @idTramo ";
            condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
            condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
            condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
            condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND inf.idMunicipio =@idMunicipio ";

            string query = @"SELECT YEAR(inf.fechaInfraccion) AS ANIO, COUNT(*) AS TOTAL
                FROM infracciones inf
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
                WHERE inf.estatus = 1 @WHERES
                GROUP BY YEAR(inf.fechaInfraccion)"
            ;

            string strQuery = query.Replace("@WHERES", condiciones);

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;

                    if (!modelBusqueda.idDelegacion.Equals(null) && modelBusqueda.idDelegacion != 0)
                        command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)modelBusqueda.idDelegacion ?? DBNull.Value;

                    if (!modelBusqueda.idOficial.Equals(null) && modelBusqueda.idOficial != 0)
                        command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = (object)modelBusqueda.idOficial ?? DBNull.Value;

                    if (!modelBusqueda.idCarretera.Equals(null) && modelBusqueda.idCarretera != 0)
                        command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)modelBusqueda.idCarretera ?? DBNull.Value;

                    if (!modelBusqueda.idTramo.Equals(null) && modelBusqueda.idTramo != 0)
                        command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)modelBusqueda.idTramo ?? DBNull.Value;

                    if (!modelBusqueda.idTipoVehiculo.Equals(null) && modelBusqueda.idTipoVehiculo != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoVehiculo ?? DBNull.Value;

                    if (!modelBusqueda.idTipoServicio.Equals(null) && modelBusqueda.idTipoServicio != 0)
                        command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoServicio ?? DBNull.Value;

                    if (!modelBusqueda.idTipoLicencia.Equals(null) && modelBusqueda.idTipoLicencia != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoLicencia ?? DBNull.Value;

                    if (!modelBusqueda.idMunicipio.Equals(null) && modelBusqueda.idMunicipio != 0)
                        command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)modelBusqueda.idMunicipio ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@año1", SqlDbType.Int)).Value = (object)modelBusqueda.año1 ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@año2", SqlDbType.Int)).Value = (object)modelBusqueda.año2 ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ResultadoGeneral model = new ResultadoGeneral();
                            model.año = reader["ANIO"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["ANIO"].ToString());
                            model.total = reader["TOTAL"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["TOTAL"].ToString());
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

        public List<DetallePorCausa> BusquedaDetallesPorCausas(ComparativoInfraccionesModel modelBusqueda)
        {
            List<DetallePorCausa> modelList = new List<DetallePorCausa>();
            string condiciones = "";

            condiciones += " AND YEAR(inf.fechaInfraccion) in (@año1,@año2) ";
            condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion == 0 ? "" : " AND inf.idDelegacion = @idDelegacion ";
            condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND inf.idOficial =@idOficial ";
            condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND inf.idCarretera = @idCarretera ";
            condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND inf.idTramo = @idTramo ";
            condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
            condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
            condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
            condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND inf.idMunicipio =@idMunicipio ";

            string query = @"SELECT catac.causaAccidente AS CAUSA, COUNT(*) AS CANTIDAD, YEAR(inf.fechaInfraccion) AS ANIO 
                FROM infracciones inf
                INNER JOIN infraccionesAccidente ia on ia.idInfraccion = inf.idInfraccion
                INNER JOIN accidenteCausas ac on ac.idAccidente = ia.idAccidente
                INNER JOIN catCausasAccidentes catac on catac.idCausaAccidente = ac.idAccidenteCausa
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
                WHERE inf.estatus = 1 @WHERES
                GROUP BY catac.causaAccidente, YEAR(inf.fechaInfraccion)"
            ;

            string strQuery = query.Replace("@WHERES", condiciones);

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;

                    if (!modelBusqueda.idDelegacion.Equals(null) && modelBusqueda.idDelegacion != 0)
                        command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)modelBusqueda.idDelegacion ?? DBNull.Value;

                    if (!modelBusqueda.idOficial.Equals(null) && modelBusqueda.idOficial != 0)
                        command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = (object)modelBusqueda.idOficial ?? DBNull.Value;

                    if (!modelBusqueda.idCarretera.Equals(null) && modelBusqueda.idCarretera != 0)
                        command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)modelBusqueda.idCarretera ?? DBNull.Value;

                    if (!modelBusqueda.idTramo.Equals(null) && modelBusqueda.idTramo != 0)
                        command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)modelBusqueda.idTramo ?? DBNull.Value;

                    if (!modelBusqueda.idTipoVehiculo.Equals(null) && modelBusqueda.idTipoVehiculo != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoVehiculo ?? DBNull.Value;

                    if (!modelBusqueda.idTipoServicio.Equals(null) && modelBusqueda.idTipoServicio != 0)
                        command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoServicio ?? DBNull.Value;

                    if (!modelBusqueda.idTipoLicencia.Equals(null) && modelBusqueda.idTipoLicencia != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoLicencia ?? DBNull.Value;

                    if (!modelBusqueda.idMunicipio.Equals(null) && modelBusqueda.idMunicipio != 0)
                        command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)modelBusqueda.idMunicipio ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@año1", SqlDbType.Int)).Value = (object)modelBusqueda.año1 ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@año2", SqlDbType.Int)).Value = (object)modelBusqueda.año2 ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            DetallePorCausa model = new DetallePorCausa();
                            model.causa = reader["CAUSA"] == System.DBNull.Value ? default(string) : reader["CAUSA"].ToString();
                            model.cantidad = reader["CANTIDAD"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["CANTIDAD"].ToString());
                            model.año = reader["ANIO"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["ANIO"].ToString());
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

        public List<DesgloseTotalInfraccion> DesgloseTotalesInfracciones(ComparativoInfraccionesModel modelBusqueda)
        {
            List<DesgloseTotalInfraccion> modelList = new List<DesgloseTotalInfraccion>();
            string condiciones = "";

            condiciones += " AND YEAR(inf.fechaInfraccion) in (@año1,@año2) ";
            condiciones += modelBusqueda.idDelegacion.Equals(null) || modelBusqueda.idDelegacion == 0 ? "" : " AND inf.idDelegacion = @idDelegacion ";
            condiciones += modelBusqueda.idOficial.Equals(null) || modelBusqueda.idOficial == 0 ? "" : " AND inf.idOficial =@idOficial ";
            condiciones += modelBusqueda.idCarretera.Equals(null) || modelBusqueda.idCarretera == 0 ? "" : " AND inf.idCarretera = @idCarretera ";
            condiciones += modelBusqueda.idTramo.Equals(null) || modelBusqueda.idTramo == 0 ? "" : " AND inf.idTramo = @idTramo ";
            condiciones += modelBusqueda.idTipoVehiculo.Equals(null) || modelBusqueda.idTipoVehiculo == 0 ? "" : " AND veh.idTipoVehiculo = @idTipoVehiculo ";
            condiciones += modelBusqueda.idTipoServicio.Equals(null) || modelBusqueda.idTipoServicio == 0 ? "" : " AND veh.idCatTipoServicio  = @idCatTipoServicio ";
            condiciones += modelBusqueda.idTipoLicencia.Equals(null) || modelBusqueda.idTipoLicencia == 0 ? "" : " AND gar.idTipoLicencia = @idTipoLicencia ";
            condiciones += modelBusqueda.idMunicipio.Equals(null) || modelBusqueda.idMunicipio == 0 ? "" : " AND inf.idMunicipio =@idMunicipio ";

            string query = @"SELECT C.NUMERO_MOTIVO AS NUMERO_MOTIVO, COUNT(C.idInfraccion) AS TOTAL_INFRACCIONES, 0 AS TOTAL_CONTAB, C.FECHA AS ANIO 
                    FROM (
	                    SELECT COUNT(MI.idMotivoInfraccion) CUENTA, CONCAT('CON ',COUNT(MI.idMotivoInfraccion), ' MOTIVO(S)') AS NUMERO_MOTIVO, 
	                    inf.idInfraccion,
	                    YEAR(inf.fechaInfraccion) AS FECHA
	                    FROM infracciones inf
	                    LEFT JOIN motivosInfraccion MI ON MI.idInfraccion = inf.idInfraccion
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
	                    WHERE inf.estatus = 1 @WHERES
	                    GROUP BY inf.idInfraccion, YEAR(inf.fechaInfraccion)
                    ) C
                    GROUP BY C.CUENTA, C.NUMERO_MOTIVO, C.FECHA
                    ORDER BY FECHA, C.CUENTA ASC "
            ;
            string strQuery = query.Replace("@WHERES", condiciones);

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;

                    if (!modelBusqueda.idDelegacion.Equals(null) && modelBusqueda.idDelegacion != 0)
                        command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = (object)modelBusqueda.idDelegacion ?? DBNull.Value;

                    if (!modelBusqueda.idOficial.Equals(null) && modelBusqueda.idOficial != 0)
                        command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = (object)modelBusqueda.idOficial ?? DBNull.Value;

                    if (!modelBusqueda.idCarretera.Equals(null) && modelBusqueda.idCarretera != 0)
                        command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)modelBusqueda.idCarretera ?? DBNull.Value;

                    if (!modelBusqueda.idTramo.Equals(null) && modelBusqueda.idTramo != 0)
                        command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)modelBusqueda.idTramo ?? DBNull.Value;

                    if (!modelBusqueda.idTipoVehiculo.Equals(null) && modelBusqueda.idTipoVehiculo != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoVehiculo ?? DBNull.Value;

                    if (!modelBusqueda.idTipoServicio.Equals(null) && modelBusqueda.idTipoServicio != 0)
                        command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoServicio ?? DBNull.Value;

                    if (!modelBusqueda.idTipoLicencia.Equals(null) && modelBusqueda.idTipoLicencia != 0)
                        command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)modelBusqueda.idTipoLicencia ?? DBNull.Value;

                    if (!modelBusqueda.idMunicipio.Equals(null) && modelBusqueda.idMunicipio != 0)
                        command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)modelBusqueda.idMunicipio ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@año1", SqlDbType.Int)).Value = (object)modelBusqueda.año1 ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@año2", SqlDbType.Int)).Value = (object)modelBusqueda.año2 ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            DesgloseTotalInfraccion model = new DesgloseTotalInfraccion();
                            model.numeroMotivo = reader["NUMERO_MOTIVO"] == System.DBNull.Value ? default(string) : reader["NUMERO_MOTIVO"].ToString();
                            model.totalInfracciones = reader["TOTAL_INFRACCIONES"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["TOTAL_INFRACCIONES"].ToString());
                            model.totalContabiliza = reader["TOTAL_CONTAB"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["TOTAL_CONTAB"].ToString());
                            model.año = reader["ANIO"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["ANIO"].ToString());
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

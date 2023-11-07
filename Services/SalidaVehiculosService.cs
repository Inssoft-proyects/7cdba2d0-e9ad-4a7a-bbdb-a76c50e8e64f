﻿using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class SalidaVehiculosService : ISalidaVehiculosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public SalidaVehiculosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<SalidaVehiculosModel> ObtenerIngresos(SalidaVehiculosModel model)
        {
            List<SalidaVehiculosModel> modelList = new List<SalidaVehiculosModel>();
            string strQuery = @"SELECT d.idDeposito,d.idVehiculo,d.numeroInventario,d.idSolicitud,
	                                    d.idMarca,d.placa,d.serie,d.idPension,d.fechaIngreso,
	                                    v.modelo,v.idSubmarca,
	                                    v.idColor,v.idPersona,
	                                    mv.marcaVehiculo,co.color,sol.fechaSolicitud,
										smv.nombreSubmarca,per.nombre,per.apellidoPaterno,per.apellidoMaterno,
										pen.pension
                                    FROM depositos AS d
                                    LEFT JOIN vehiculos AS v ON d.idVehiculo = v.idVehiculo
                                    LEFT JOIN catMarcasVehiculos AS mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo
                                    LEFT JOIN catSubmarcasVehiculos AS smv ON v.idSubmarca = smv.idSubmarca
								    LEFT JOIN catColores AS co ON v.idColor = co.idColor
					   		        LEFT JOIN personas AS per ON v.idPersona = per.idPersona
                                    LEFT JOIN solicitudes AS sol ON d.idSolicitud = sol.idSolicitud
	                                LEFT JOIN pensiones AS pen ON d.idPension = pen.idPension
                                    WHERE d.idMarca = @idMarca OR d.serie = @serie OR d.numeroInventario = @numeroInventario
                                    OR d.placa = @placa";
                                    if (model.fechaIngreso != DateTime.MinValue)
                                    {
                                        strQuery += " OR fechaIngreso = @fechaIngreso";
                                    }
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = (object)model.idDeposito ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMarca", SqlDbType.Int)).Value = (object)model.idMarca ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@serie", SqlDbType.VarChar)).Value = (object)model.serie ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroInventario", SqlDbType.VarChar)).Value = (object)model.folioInventario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placa", SqlDbType.VarChar)).Value = (object)model.placa ?? DBNull.Value;
                    if (model.fechaIngreso != DateTime.MinValue)
                    {
                        command.Parameters.Add(new SqlParameter("@fechaIngreso", SqlDbType.DateTime)).Value = model.fechaIngreso;
                    }
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            SalidaVehiculosModel deposito = new SalidaVehiculosModel();
                            deposito.idDeposito = reader["idDeposito"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idDeposito"].ToString());
                            deposito.idMarca = reader["idMarca"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idMarca"].ToString());
                            deposito.folioInventario = reader["numeroInventario"].ToString();
                            deposito.marca = reader["marcaVehiculo"].ToString();
                            deposito.placa = reader["placa"].ToString();
                            deposito.submarca = reader["nombreSubmarca"].ToString();
                            deposito.serie = reader["serie"].ToString();
                            deposito.propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            deposito.pension = reader["pension"].ToString();
                            deposito.color = reader["color"].ToString();
                            deposito.fechaIngreso = reader["fechaIngreso"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaIngreso"].ToString());


                            modelList.Add(deposito);
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
        public SalidaVehiculosModel DetallesDeposito(int iDp)
        {
            SalidaVehiculosModel model = new SalidaVehiculosModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                                            @"SELECT d.idVehiculo,d.numeroInventario,d.idSolicitud,d.idDeposito,d.fechaIngreso,
                                            v.modelo,v.idMarcaVehiculo,v.idTipoVehiculo,v.idColor,v.idPersona,
                                            mv.marcaVehiculo,tv.tipoVehiculo,c.color,
                                            per.nombre,per.apellidoPaterno,per.apellidoMaterno,
                                            sol.solicitanteNombre,sol.solicitanteAp,sol.solicitanteAm,
                                            sol.idPropietarioGrua,sol.idEvento,sol.fechaSolicitud,
                                            sol.idTramoUbicacion,sol.idCarreteraUbicacion,sol.vehiculoKm,sol.vehiculoColonia,
                                            sol.vehiculoCalle,sol.vehiculoNumero,sol.idMunicipioUbicacion,sol.vehiculoInterseccion,
                                            de.descripcionEvento,tra.tramo,car.carretera,mun.municipio,con.concesionario,ga.fechaFinal,
											ga.idGrua,ga.costoTotal,g.noEconomico,g.idTipoGrua,ctg.TipoGrua                                         
										    FROM depositos AS d
                                            LEFT JOIN vehiculos AS v ON d.idVehiculo = v.idVehiculo
                                            LEFT JOIN catMarcasVehiculos AS mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo
                                            LEFT JOIN catTiposVehiculo AS tv ON tv.idTipoVehiculo = v.idTipoVehiculo
                                            LEFT JOIN catColores AS c ON c.idColor = v.idColor
                                            LEFT JOIN personas AS per ON per.idPersona = v.idPersona
                                            LEFT JOIN solicitudes AS sol ON sol.idSolicitud = d.idSolicitud
                                            LEFT JOIN catDescripcionesEvento AS de ON sol.idEvento = de.idDescripcion
                                            LEFT JOIN catTramos AS tra ON sol.idTramoUbicacion = tra.idTramo
                                            LEFT JOIN catCarreteras AS car ON sol.idCarreteraUbicacion = car.idCarretera
                                            LEFT JOIN catMunicipios AS mun ON sol.idMunicipioUbicacion = mun.idMunicipio
                                            LEFT JOIN concesionarios AS con ON sol.idPropietarioGrua = con.idConcesionario
                                            LEFT JOIN gruasAsignadas AS ga ON d.idDeposito = ga.idDeposito
                                            LEFT JOIN gruas AS g ON ga.idGrua = g.idGrua
										    LEFT JOIN catTipoGrua AS ctg ON g.idTipoGrua = ctg.IdTipoGrua
											WHERE d.idDeposito = @idDeposito";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = iDp;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            model.idDeposito = reader["idDeposito"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idDeposito"]);
                            model.idVehiculo = reader["idVehiculo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idVehiculo"]);
                            model.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            model.marca = reader["marcaVehiculo"].ToString();
                            model.modelo = reader["modelo"].ToString();
                            model.color = reader["color"].ToString();
                            model.propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            model.solicitante = $"{reader["solicitanteNombre"]} {reader["solicitanteAp"]} {reader["solicitanteAm"]}";
                            model.folioInventario = reader["numeroInventario"].ToString();
                            model.evento = reader["descripcionEvento"].ToString();
                            model.propietarioGrua = reader["concesionario"].ToString();
                            model.fechaSolicitud = reader["fechaSolicitud"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fechaSolicitud"]);
                            model.fechaIngreso = reader["fechaIngreso"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fechaIngreso"]);
                            model.fechaFinal = reader["fechaFinal"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fechaFinal"]);
                            model.tramo = reader["tramo"].ToString();
                            model.carretera = reader["carretera"].ToString();
                            model.kilometro = reader["vehiculoKm"].ToString();
                            model.colonia = reader["vehiculoColonia"].ToString();
                            model.calle = reader["vehiculoCalle"].ToString();
                            model.numero = reader["vehiculoNumero"].ToString();
                            model.municipio = reader["municipio"].ToString();
                            model.grua = reader["noEconomico"].ToString();
                            model.tipoGrua = reader["TipoGrua"].ToString();
                            model.interseccion = reader["vehiculoInterseccion"].ToString();
                            model.costoTotalPorGrua = reader["costoTotal"] == DBNull.Value ? 0.0f : Convert.ToSingle(reader["costoTotal"]);



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
        public List<GruasSalidaVehiculosModel> ObtenerDatosGridGruas(int iDp)
        {
            //
            List<GruasSalidaVehiculosModel> ListaGruas = new List<GruasSalidaVehiculosModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ga.idDeposito,ga.idGrua,ga.abanderamiento,ga.arrastre, " +
                                                        "ga.salvamento,ga.costoTotal,g.noEconomico,tg.TipoGrua " +
                                                        "From gruasAsignadas AS ga " +
                                                        "LEFT JOIN gruas AS g ON g.idGrua = ga.idGrua " +
                                                        "LEFT JOIN catTipoGrua AS tg ON tg.IdTipoGrua = g.idTipoGrua " +
                                                        "WHERE ga.idDeposito = @idDeposito", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = iDp;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            GruasSalidaVehiculosModel grua = new GruasSalidaVehiculosModel();
                            grua.idDeposito = Convert.ToInt32(reader["idDeposito"]?.ToString() ?? "0");
                            grua.idGrua = Convert.ToInt32(reader["idGrua"]?.ToString() ?? "0");
                            grua.grua = reader["noEconomico"]?.ToString();
                            grua.costoTotalPorGrua = float.TryParse(reader["costoTotal"]?.ToString(), out float costoTotal) ? costoTotal : 0.0f;
                            grua.abanderamiento = Convert.ToInt32(reader["abanderamiento"]?.ToString() ?? "0");
                            grua.salvamento = Convert.ToInt32(reader["salvamento"]?.ToString() ?? "0");
                            grua.arrastre = Convert.ToInt32(reader["arrastre"]?.ToString() ?? "0");
                            grua.tipoGrua = reader["TipoGrua"]?.ToString();



                            ListaGruas.Add(grua);

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
            return ListaGruas;


        }
        public CostosServicioModel CostosServicio(int idDeposito)
        {
            CostosServicioModel model = new CostosServicioModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                                            @"SELECT ga.idDeposito,ga.idGrua,ga.abanderamiento,ga.arrastre,  
                                                                ga.salvamento,ga.costoTotal,ga.costoAbanderamiento,ga.costoArrastre,ga.costoBanderazo,ga.costoSalvamento
                                                                From gruasAsignadas AS ga  
                                                                WHERE ga.idDeposito = @idDeposito";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = idDeposito;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            model.idDeposito = Convert.ToInt32(reader["idDeposito"]?.ToString() ?? "0");
                            model.costoArrastre = float.TryParse(reader["costoArrastre"]?.ToString(), out float costoArrastre) ? costoArrastre : 0.0f;
                            model.costoSalvamento = float.TryParse(reader["costoSalvamento"]?.ToString(), out float costoSalvamento) ? costoSalvamento : 0.0f;
                            model.costoBanderazo = float.TryParse(reader["costoBanderazo"]?.ToString(), out float costoBanderazo) ? costoBanderazo : 0.0f;

                            model.costoAbanderamiento = float.TryParse(reader["costoAbanderamiento"]?.ToString(), out float costoAbanderamiento) ? costoAbanderamiento : 0.0f;
                            model.costoTotalPorGrua = float.TryParse(reader["costoTotal"]?.ToString(), out float costoTotal) ? costoTotal : 0.0f;
                            model.abanderamiento = Convert.ToInt32(reader["abanderamiento"]?.ToString() ?? "0");
                            model.salvamento = Convert.ToInt32(reader["salvamento"]?.ToString() ?? "0");
                            model.arrastre = Convert.ToInt32(reader["arrastre"]?.ToString() ?? "0");
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
        public int ActualizarCostos(CostosServicioModel model)

        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update gruasAsignadas set " +
                                   "costoAbanderamiento = @costoAbanderamiento, " +
                                   "costoArrastre = @costoArrastre, " +
                                   "costoBanderazo = @costoBanderazo, " +
                                   "costoSalvamento = @costoSalvamento, " +
                                   "costoTotal = @costoTotal " +
                                   "where idDeposito=@idDeposito",
                        connection);
                    
                    sqlCommand.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = model.idDeposito;
                    sqlCommand.Parameters.Add(new SqlParameter("@costoAbanderamiento", SqlDbType.Float)).Value = model.costoAbanderamiento;
                    sqlCommand.Parameters.Add(new SqlParameter("@costoArrastre", SqlDbType.Float)).Value = model.costoArrastre;
                    sqlCommand.Parameters.Add(new SqlParameter("@costoBanderazo", SqlDbType.Float)).Value = model.costoBanderazo;
                    sqlCommand.Parameters.Add(new SqlParameter("@costoSalvamento", SqlDbType.Float)).Value = model.costoSalvamento;
                    sqlCommand.Parameters.Add(new SqlParameter("@costoTotal", SqlDbType.Float)).Value = model.costoTotalPorGrua;


                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    //---Log
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;


        }
        public int GuardarInforSalida(SalidaVehiculosModel model)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string mergeQuery =
                        @"MERGE INTO serviciosDepositos AS target
                USING (SELECT @idDeposito AS idDeposito) AS source
                ON target.idDeposito = source.idDeposito
                WHEN MATCHED THEN
                    UPDATE SET
                        fechaIngreso = @fechaIngreso,
                        fechaSalida = @fechaSalida,
                        diasResguardo = @diasResguardo,
                        costoDeposito = @costoDeposito,
                        costoTotalGruas = @costoTotalGruas,
                        nombreRecibe = @nombreRecibe,
                        nombreEntrega = @nombreEntrega,
                        observaciones = @observaciones,
                        estatus = @estatus,
                        actualizadoPor = @actualizadoPor,
                        fechaActualizacion = @fechaActualizacion
                WHEN NOT MATCHED THEN
                    INSERT (idDeposito, fechaIngreso, fechaSalida, diasResguardo, costoDeposito, costoTotalGruas, nombreRecibe, nombreEntrega, observaciones, estatus, actualizadoPor, fechaActualizacion)
                    VALUES (@idDeposito, @fechaIngreso, @fechaSalida, @diasResguardo, @costoDeposito, @costoTotalGruas, @nombreRecibe, @nombreEntrega, @observaciones, @estatus, @actualizadoPor, @fechaActualizacion);";

                    SqlCommand command = new SqlCommand(mergeQuery, connection);

                    // Asigna los parámetros como lo hacías en tu consulta original
                    command.Parameters.AddWithValue("@idDeposito", model.idDeposito);
                    command.Parameters.AddWithValue("@fechaIngreso", model.fechaIngreso);
                    command.Parameters.AddWithValue("@fechaSalida", model.fechaSalida);
                    command.Parameters.AddWithValue("@diasResguardo", model.diasResguardo);
                    command.Parameters.AddWithValue("@costoDeposito", model.costoDeposito);
                    command.Parameters.AddWithValue("@costoTotalGruas", model.costoTotalTodasGruas);
                    command.Parameters.AddWithValue("@nombreRecibe", model.recibe);
                    command.Parameters.AddWithValue("@nombreEntrega", model.entrega);
                    command.Parameters.AddWithValue("@observaciones", model.observaciones);
                    command.Parameters.AddWithValue("@estatus", 1);
                    command.Parameters.AddWithValue("@actualizadoPor", 1);
                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);

                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }

                return result;
            }
        }


        public List<SalidaVehiculosModel> ObtenerTotal(int iDp)
        {
            List<SalidaVehiculosModel> resultados = new List<SalidaVehiculosModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"SELECT ga.idDeposito, ga.costoTotal
                FROM gruasAsignadas AS ga
                WHERE ga.idDeposito = @idDeposito";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = iDp;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            SalidaVehiculosModel resultado = new SalidaVehiculosModel
                            {
                                idDeposito = reader["idDeposito"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idDeposito"]),
                                costoTotalPorGrua = reader["costoTotal"] == DBNull.Value ? 0 : Convert.ToInt32(reader["costoTotal"])
                            };
                            resultados.Add(resultado);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Manejar la excepción, por ejemplo, guardarla en un log de errores
                    // ex
                }
                finally
                {
                    connection.Close();
                }
            }
            return resultados;
        }

    }
}

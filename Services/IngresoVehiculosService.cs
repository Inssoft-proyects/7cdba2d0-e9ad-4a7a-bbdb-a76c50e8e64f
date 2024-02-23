using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class IngresoVehiculosService : IIngresarVehiculosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public IngresoVehiculosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<IngresoVehiculosModel> ObtenerDepositos(IngresoVehiculosModel model, int idPension)
        {
            List<IngresoVehiculosModel> modelList = new List<IngresoVehiculosModel>();
            string condiciones = "";
            condiciones += model.idMarca.HasValue ? $" AND d.idMarca = {model.idMarca}" : "";
            condiciones += model.serie.IsNullOrEmpty() ? "" : " AND d.serie = @serie";
            condiciones += model.folioInventario.IsNullOrEmpty() ? "" : " AND d.numeroInventario LIKE '%' + @numeroInventario + '%' ";
            condiciones += model.placa.IsNullOrEmpty() ? "" : " AND d.placa = @placa";
            string strQuery = @"SELECT d.idDeposito,d.idVehiculo,d.numeroInventario,
	                                    d.idMarca,d.placa,d.serie,d.liberado,
	                                    v.modelo,v.motor,v.numeroEconomico,
	                                    v.idColor,v.idTipoVehiculo,
	                                    mv.marcaVehiculo,tv.tipoVehiculo,co.color,sol.fechaSolicitud
                                    FROM depositos AS d
                                    LEFT JOIN vehiculos AS v ON d.idVehiculo = v.idVehiculo
                                    LEFT JOIN catMarcasVehiculos AS mv ON d.idMarca = mv.idMarcaVehiculo
                                    LEFT JOIN catColores AS co ON v.idColor = co.idColor
                                    LEFT JOIN catTiposVehiculo AS tv ON v.idTipoVehiculo = tv.idTipoVehiculo
                                    LEFT JOIN solicitudes AS sol ON d.idSolicitud = sol.idSolicitud
                                    WHERE d.idPension = @idPension AND d.liberado = 0 AND d.estatusSolicitud = 4 " + condiciones;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                 

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = (object)model.idDeposito ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = (object)idPension ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@idMarca", SqlDbType.Int)).Value = (object)model.idMarca ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@serie", SqlDbType.VarChar)).Value = (object)model.serie ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroInventario", SqlDbType.VarChar)).Value = (object)model.folioInventario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placa", SqlDbType.VarChar)).Value = (object)model.placa ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            IngresoVehiculosModel deposito = new IngresoVehiculosModel();
                            deposito.idDeposito = reader["idDeposito"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idDeposito"].ToString());
                            deposito.idMarca = reader["idMarca"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idMarca"].ToString());
                            deposito.folioInventario = reader["numeroInventario"].ToString();
                            deposito.marca = reader["marcaVehiculo"].ToString();
                            deposito.placa = reader["placa"].ToString();
                            deposito.modelo = reader["modelo"].ToString();
                            deposito.serie = reader["serie"].ToString();
                            deposito.motor = reader["motor"].ToString();
                            deposito.numeroEconomicoVehiculo = reader["numeroEconomico"].ToString();
                            deposito.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            deposito.color = reader["color"].ToString();
                            deposito.fechaServicio = reader["fechaSolicitud"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaSolicitud"].ToString());


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
        public IngresoVehiculosModel DetallesDeposito(int idDeposito)
        {
            IngresoVehiculosModel model = new IngresoVehiculosModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                                            @"SELECT d.idVehiculo,d.numeroInventario,d.idSolicitud,d.idDeposito,
                                            v.modelo,v.idMarcaVehiculo,v.idTipoVehiculo,v.idColor,v.idPersona,
                                            mv.marcaVehiculo,tv.tipoVehiculo,c.color,
                                            per.nombre,per.apellidoPaterno,per.apellidoMaterno,
                                            sol.solicitanteNombre,sol.solicitanteAp,sol.solicitanteAm,
                                            sol.idPropietarioGrua,sol.fechaSolicitud,sol.idEvento,
                                            sol.idTramoUbicacion,sol.idCarreteraUbicacion,sol.vehiculoKm,sol.vehiculoColonia,
                                            sol.vehiculoCalle,sol.vehiculoNumero,sol.idMunicipioUbicacion,sol.vehiculoInterseccion,
                                            de.descripcionEvento,tra.tramo,car.carretera,mun.municipio,con.concesionario,ga.fechaFinal
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
                                            WHERE d.idDeposito = @idDeposito";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = (object)idDeposito ?? DBNull.Value;
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
                            model.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            model.Solicitante = $"{reader["solicitanteNombre"]} {reader["solicitanteAp"]} {reader["solicitanteAm"]}";
                            model.folioInventario = reader["numeroInventario"].ToString();
                            model.evento = reader["descripcionEvento"].ToString();
                            model.propietarioGrua = reader["concesionario"].ToString();
                            model.fechaSolicitud = reader["fechaSolicitud"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fechaSolicitud"]);
                            model.fechaFinal = reader["fechaFinal"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["fechaFinal"]);
                            model.tramo = reader["tramo"].ToString();
                            model.carretera = reader["carretera"].ToString();
                            model.kilometro = reader["vehiculoKm"].ToString();
                            model.colonia = reader["vehiculoColonia"].ToString();
                            model.calle = reader["vehiculoCalle"].ToString();
                            model.numero = reader["vehiculoNumero"].ToString();
                            model.municipio = reader["municipio"].ToString();
                            model.interseccion = reader["vehiculoInterseccion"].ToString();


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
        public int GuardarFechaIngreso(DatosIngresoModel model)
        {
            int depositoModificado = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE depositos SET fechaIngreso = @fechaIngreso, idDependenciaGenera = @idDependenciaGenera, imagenDeposito = @imagenDeposito WHERE idDeposito = @idDeposito";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@fechaIngreso", model.fechaIngreso);
                    command.Parameters.AddWithValue("@idDeposito", model.IdDeposito);
                    command.Parameters.AddWithValue("@idDependenciaGenera", 1);
                    command.Parameters.Add(new SqlParameter("@imagenDeposito", SqlDbType.Image)).Value = (object)model.AnexarImagen1 ?? DBNull.Value;

                    command.ExecuteScalar();
                }

                catch (SqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    connection.Close();
                }
            }

            return depositoModificado;
        }
    }
}

using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;
using Microsoft.Extensions.DependencyModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Controllers;
using System.Windows.Input;
using Microsoft.IdentityModel.Tokens;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using System.Globalization;

namespace GuanajuatoAdminUsuarios.Services
{

    public class CapturaAccidentesService : ICapturaAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CapturaAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CapturaAccidentesModel> ObtenerAccidentes(int idOficina)
        {
            //
            List<CapturaAccidentesModel> ListaAccidentes = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"SELECT acc.IdAccidente,acc.NumeroReporte,acc.Fecha,acc.Hora,acc.IdMunicipio,acc.IdCarretera,
                        acc.IdTramo,acc.idEstatusReporte,
                        mun.Municipio, car.Carretera, tra.Tramo, er.estatusReporte,er.idEstatusReporte
                        FROM accidentes AS acc  
                        LEFT JOIN catMunicipios AS mun ON acc.idMunicipio = mun.idMunicipio 
                        LEFT JOIN catCarreteras AS car ON acc.idCarretera = car.idCarretera  
                        LEFT JOIN catTramos AS tra ON acc.idTramo = tra.idTramo  
                        LEFT JOIN catEstatusReporteAccidente AS er ON acc.idEstatusReporte = er.idEstatusReporte
                        WHERE acc.estatus = 1 AND acc.idOficinaDelegacion = @idOficina and acc.idEstatusReporte != 3;", connection);
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel accidente = new CapturaAccidentesModel();
                            accidente.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
                            accidente.NumeroReporte = reader["NumeroReporte"].ToString();
                            accidente.Fecha = Convert.ToDateTime(reader["Fecha"].ToString());
                            accidente.Hora = reader.GetTimeSpan(reader.GetOrdinal("Hora"));
                            accidente.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            accidente.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
                            accidente.IdTramo = Convert.ToInt32(reader["IdTramo"].ToString());
                            accidente.idEstatusReporte = Convert.ToInt32(reader["idEstatusReporte"].ToString());
                            accidente.EstatusReporte = reader["estatusReporte"].ToString();
                            accidente.Municipio = reader["Municipio"].ToString();
                            accidente.Tramo = reader["Tramo"].ToString();
                            accidente.Carretera = reader["Carretera"].ToString();

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

        public CapturaAccidentesModel ObtenerAccidentePorId(int idAccidente, int idOficina)
        {
            CapturaAccidentesModel accidente = new CapturaAccidentesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                        SELECT DISTINCT 
                        a.idAccidente,
                        a.numeroReporte,
                        a.fecha, a.hora,
                        a.idMunicipio, 
                        a.idCarretera, 
                        a.idTramo, 
                        a.kilometro, 
                        a.idClasificacionAccidente, 
                        a.idFactorAccidente,
                        a.IdFactorOpcionAccidente,
                        a.idOficinaDelegacion,
                        a.descripcionCausas, m.municipio, c.carretera, t.tramo, e.estatusDesc, 
                        ac.idCausaAccidente, d.nombreOficina,d.jefeOficina
                        FROM accidentes AS a                         
                        JOIN catMunicipios AS m ON a.idMunicipio = m.idMunicipio
                        JOIN catDelegacionesOficinasTransporte as d on d.idOficinaTransporte= a.idOficinaDelegacion
                        JOIN catCarreteras AS c ON a.idCarretera = c.idCarretera 
                        JOIN catTramos AS t ON a.idTramo = t.idTramo 
                        JOIN estatus AS e ON a.estatus = e.estatus 
                        LEFT JOIN accidenteCausas AS ac ON ac.idAccidente = a.idAccidente 
                        WHERE a.idAccidente = @idAccidente AND a.estatus = 1 AND a.idOficinaDelegacion = @idOficina
                    ", connection);
                    command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            accidente.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
                            accidente.NumeroReporte = reader["NumeroReporte"].ToString();
                            accidente.Fecha = Convert.ToDateTime(reader["Fecha"].ToString());
                            accidente.Hora = reader.GetTimeSpan(reader.GetOrdinal("Hora"));
                            accidente.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            accidente.IdCarretera = Convert.ToInt32(reader["IdCarretera"].ToString());
                            accidente.IdClasificacionAccidente = reader["IdClasificacionAccidente"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["IdClasificacionAccidente"].ToString());
                            accidente.IdFactorAccidente = reader["IdFactorAccidente"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            accidente.IdFactorOpcionAccidente = reader["IdFactorOpcionAccidente"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["IdFactorOpcionAccidente"].ToString());
                            accidente.IdCausaAccidente = reader["idCausaAccidente"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idCausaAccidente"].ToString());
                            accidente.Municipio = reader["Municipio"].ToString();
                            accidente.Tramo = reader["Tramo"].ToString();
                            accidente.DescripcionCausa = reader["descripcionCausas"].ToString();
                            accidente.Carretera = reader["Carretera"].ToString();
                            accidente.Kilometro = reader["Kilometro"].ToString();
                            accidente.IdTramo = Convert.ToInt32(reader["IdTramo"].ToString());
							accidente.DelegacionOficina = reader["nombreOficina"].ToString();
                            accidente.jefeOficina = reader["jefeOficina"].ToString();
                            accidente.Fecha=accidente.Fecha.Value.Add(accidente.Hora.Value);
                            var tt = accidente.Hora.Value.ToString(@"hh\:mm",new CultureInfo("en-US"));
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

        public int GuardarParte1(CapturaAccidentesModel model,int idOficina)
        
        {
            int result = 0;
            int lastInsertedId = 0;
            string strQuery = @"INSERT INTO accidentes( 
                                         [Hora]
                                        ,[idOficinaDelegacion]
                                        ,[idMunicipio]
                                        ,[idTramo]
                                        ,[Fecha]
                                        ,[idCarretera]
                                        ,[kilometro]
                                        ,[idEstatusReporte]
                                        ,[fechaActualizacion]
                                        ,[actualizadoPor]
                                        ,[estatus])
                                VALUES (
                                         @Hora
                                        ,@idOficina
                                        ,@idMunicipio
                                        ,@idTramo
                                        ,@Fecha
                                        ,@idCarretera
                                        ,@kilometro
                                        ,@idEstatusReporte
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
                    command.Parameters.Add(new SqlParameter("@Hora", SqlDbType.Time)).Value = (object)model.Hora ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@kilometro", SqlDbType.NVarChar)).Value = (object)model.Kilometro ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEstatusReporte", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)model.IdTramo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Fecha", SqlDbType.DateTime)).Value = (object)model.Fecha ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    result = Convert.ToInt32(command.ExecuteScalar()); // Valor de IdAccidente de este mismo registro
                    lastInsertedId = result; // Almacena el valor en la variable lastInsertedId
                }
                catch (SqlException ex)
                {
                    return lastInsertedId;
                }
                finally
                {
                    connection.Close();
                }
            }
            return lastInsertedId;
        }

        public List<CapturaAccidentesModel> BuscarPorParametro(string Placa, string Serie, string Folio)
        {
            List<CapturaAccidentesModel> Vehiculo = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command;

                    if (!string.IsNullOrEmpty(Serie))
                    {
                        command = new SqlCommand(
                       "SELECT v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio, p.nombre, p.apellidoPaterno, p.apellidoMaterno " +
                        "FROM vehiculos v " +
                        "left JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                        "left JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                        "left JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                        "left JOIN catColores cc ON v.idColor = cc.idColor " +
                        "left JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                        "left JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                        "JOIN personas p ON v.idPersona = p.idPersona " +
                        "WHERE v.estatus = 1 AND v.serie LIKE '%' + @Serie + '%';", connection);

                        command.Parameters.AddWithValue("@Serie", Serie);
                    }
                    else if (!string.IsNullOrEmpty(Placa))
                    {
                        command = new SqlCommand("SELECT v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio,p.nombre, p.apellidoPaterno, p.apellidoMaterno " +
                            "FROM vehiculos v " +
                            "left JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                            "left JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                            "left JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                            "left JOIN catColores cc ON v.idColor = cc.idColor " +
                            "left JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                            "left JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                            "JOIN personas p ON v.idPersona = p.idPersona " +
                            "WHERE v.estatus = 1 AND v.placas LIKE '%' + @Placa + '%';", connection);
                        command.Parameters.AddWithValue("@Placa", Placa);
                    }
                    else if (!string.IsNullOrEmpty(Folio))
                    {
                        command = new SqlCommand("SELECT v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio,p.nombre, p.apellidoPaterno, p.apellidoMaterno " + 
                            "FROM vehiculos v " +
                            "JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo" +
                            "left JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                            "left JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                            "left JOIN catColores cc ON v.idColor = cc.idColor " +
                            "left JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                            "left JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                            "JOIN personas p ON v.idPersona = p.idPersona " +
                            "WHERE v.estatus = 1 AND v.placas LIKE '%' + @Folio + '%';", connection);
                        command.Parameters.AddWithValue("@Folio", Folio);
                    }
                    else
                    {
                        // No se proporcionó ningún parámetro válido
                        return Vehiculo;
                    }

                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel vehiculo = new CapturaAccidentesModel();
                            vehiculo.IdVehiculo = Convert.IsDBNull(reader["IdVehiculo"]) ? 0 : Convert.ToInt32(reader["IdVehiculo"]);
                            vehiculo.IdMarcaVehiculo = Convert.IsDBNull(reader["IdMarcaVehiculo"]) ? 0 : Convert.ToInt32(reader["IdMarcaVehiculo"]);
                            vehiculo.IdSubmarca = Convert.IsDBNull(reader["IdSubmarca"]) ? 0 : Convert.ToInt32(reader["IdSubmarca"]);
                            vehiculo.IdEntidad = Convert.IsDBNull(reader["IdEntidad"]) ? 0 : Convert.ToInt32(reader["IdEntidad"]);
                            vehiculo.IdColor = Convert.IsDBNull(reader["IdColor"]) ? 0 : Convert.ToInt32(reader["IdColor"]);
                            vehiculo.IdTipoVehiculo = Convert.IsDBNull(reader["IdTipoVehiculo"]) ? 0 : Convert.ToInt32(reader["IdTipoVehiculo"]);
                            vehiculo.IdCatTipoServicio = Convert.IsDBNull(reader["IdCatTipoServicio"]) ? 0 : Convert.ToInt32(reader["IdCatTipoServicio"]);
                            vehiculo.IdPersona = Convert.IsDBNull(reader["IdPersona"]) ? 0 : Convert.ToInt32(reader["IdPersona"]);
                            vehiculo.Marca = reader["marcaVehiculo"].ToString();
                            vehiculo.Submarca = reader["nombreSubmarca"].ToString();
                            vehiculo.Modelo = reader["Modelo"].ToString();
                            vehiculo.Placa = reader["Placas"].ToString();
                            vehiculo.Tarjeta = reader["Tarjeta"].ToString();
                            vehiculo.VigenciaTarjeta = Convert.IsDBNull(reader["VigenciaTarjeta"]) ? DateTime.MinValue : Convert.ToDateTime(reader["VigenciaTarjeta"]);
                            vehiculo.Serie = reader["serie"].ToString();
                            vehiculo.EntidadRegistro = reader["nombreEntidad"].ToString();
                            vehiculo.Color = reader["color"].ToString();
                            vehiculo.TipoServicio = reader["tipoServicio"].ToString();
                            vehiculo.TipoVehiculo = reader["tipoVehiculo"].ToString();
                            vehiculo.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";


                            Vehiculo.Add(vehiculo);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    return Vehiculo;
                }
                finally
                {
                    connection.Close();
                }
            }

            return Vehiculo;
        }




        public int ActualizarConVehiculo(int idVehiculo, int idAccidente,int IdPersona, string Placa, string Serie)
        {
            int idVehiculoInsertado = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO vehiculosAccidente (idAccidente, idVehiculo, idPersona, placa, serie) OUTPUT INSERTED.idVehiculo VALUES (@idAccidente, @idVehiculo,@idPersona, @Placa ,@Serie)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idVehiculo", idVehiculo);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idPersona", IdPersona);
                    command.Parameters.AddWithValue("@Placa", Placa);
                    command.Parameters.AddWithValue("@Serie", Serie);

                    object insertedId = command.ExecuteScalar();

                    if (insertedId != null && int.TryParse(insertedId.ToString(), out idVehiculoInsertado))
                    {
                        // El valor de idVehiculoInsertado es el ID del vehículo insertado en la tabla
                    }
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

            return idVehiculoInsertado;
        }
        public int BorrarVehiculoAccidente(int idVehiculo, int idAccidente)
        {

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM vehiculosAccidente " +
                                                        "WHERE idAccidente = @idAccidente AND idVehiculo = @idVehiculo ", connection);
                    command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
                    command.Parameters.Add(new SqlParameter("@idVehiculo", SqlDbType.Int)).Value = idVehiculo;
                    command.CommandType = CommandType.Text;

                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return idVehiculo;
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al borrar el vehículo del accidente: " + ex.Message);
                    return -1;
                }

            }
        }
        public CapturaAccidentesModel InvolucradoSeleccionado(int idAccidente, int IdVehiculoInvolucrado, int IdPropietarioInvolucrado)
        {
            CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                                                        "SELECT cva.*, COALESCE(cva.idPersona, pcv.idPersona) AS idConductor,cva.idTipoCarga,cva.poliza,ctc.tipoCarga,v.placas, v.tarjeta, v.serie, v.idMarcaVehiculo, " +
                        "v.idSubmarca,v.idEntidad, v.idTipoVehiculo,acc.numeroReporte,v.idPersona AS idPropietario, v.modelo, v.idColor, v.idCatTipoServicio, v.motor, v.capacidad, " +
                        "cm.marcaVehiculo, csv.nombreSubmarca, tv.tipoVehiculo, COALESCE(p.nombre, pcv.nombre) AS nombre, COALESCE(p.apellidoPaterno, pcv.apellidoPaterno) AS apellidoPaterno, " +
                        "p.apellidoMaterno,pcv.RFC,pcv.CURP,CONVERT(varchar, p.fechaNacimiento, 103) AS fechaNacimiento, c.color, ts.tipoServicio, pcv.nombre AS nombreConductor, pcv.apellidoPaterno AS apellidoPConductor, pcv.apellidoMaterno AS apellidoMConductor, " +
                        "tc.tipoCarga,v.vigenciaTarjeta,tp.tipoPersona, pen.pension, ft.formaTraslado, cent.nombreEntidad,va.montoVehiculo " +
                        "FROM conductoresVehiculosAccidente AS cva INNER JOIN vehiculos AS v ON cva.idVehiculo = v.idVehiculo " +
                        "LEFT JOIN catMarcasVehiculos AS cm ON v.idMarcaVehiculo = cm.idMarcaVehiculo " +
                        "LEFT JOIN catTiposcarga AS ctc ON cva.idTipoCarga = ctc.idTipoCarga " +
                        "LEFT JOIN catSubmarcasVehiculos AS csv ON v.idSubmarca = csv.idSubmarca " +
                        "LEFT JOIN catTiposVehiculo AS tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                        "LEFT JOIN personas AS p ON v.idPersona = p.idPersona " +
                        "LEFT JOIN catColores AS c ON v.idColor = c.idColor " +
                        "LEFT JOIN catTiposcarga AS tc ON cva.idTipoCarga = tc.idTipoCarga " +
                        "LEFT JOIN pensiones AS pen ON cva.idPension = pen.idPension " +
                        "LEFT JOIN vehiculosAccidente AS va ON cva.idVehiculo = va.idVehiculo AND cva.idAccidente = va.idAccidente " +
                        "LEFT JOIN catFormasTraslado AS ft ON cva.idFormaTraslado = ft.idFormaTraslado " +
                        "LEFT JOIN catTipoServicio AS ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                        "LEFT JOIN accidentes AS acc ON cva.idAccidente = acc.idAccidente " +
                        "LEFT JOIN catEntidades AS cent ON v.idEntidad = cent.idEntidad " +
                        "LEFT JOIN personas AS pcv ON cva.idPersona = pcv.idPersona " +
                        "LEFT JOIN catTipoPersona AS tp ON p.idCatTipoPersona = tp.idCatTipoPersona " +
                        "WHERE cva.idAccidente = @idAccidente AND cva.idPersona = @idPersona AND cva.idVehiculo = @IdVehiculoInvolucrado AND cva.idAccidente > 0;", connection);

                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idPersona", IdPropietarioInvolucrado);
                    command.Parameters.AddWithValue("@IdVehiculoInvolucrado", IdVehiculoInvolucrado);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            involucrado.IdPropietarioInvolucrado = reader["idPersona"] != DBNull.Value ? Convert.ToInt32(reader["idPersona"].ToString()) : 0;
                            involucrado.IdAccidente = reader["idAccidente"] != DBNull.Value ? Convert.ToInt32(reader["idAccidente"].ToString()) : 0;
                            involucrado.IdVehiculoInvolucrado = reader["idVehiculo"] != DBNull.Value ? Convert.ToInt32(reader["idVehiculo"].ToString()) : 0;
                            involucrado.IdTipoCarga = reader["IdTipoCarga"] != DBNull.Value ? Convert.ToInt32(reader["IdTipoCarga"].ToString()) : 0;
                            involucrado.IdPension = reader["IdPension"] != DBNull.Value ? Convert.ToInt32(reader["IdPension"].ToString()) : 0;
                            involucrado.IdFormaTrasladoInvolucrado = reader["idFormaTraslado"] != DBNull.Value ? Convert.ToInt32(reader["idFormaTraslado"].ToString()) : 0;
                            involucrado.idPersonaInvolucrado = reader["IdConductor"] != DBNull.Value ? Convert.ToInt32(reader["IdConductor"].ToString()) : 0;
                            involucrado.Placa = reader["placas"] != DBNull.Value ? reader["placas"].ToString() : string.Empty;
                            involucrado.fechaNacimiento = reader["fechaNacimiento"] != DBNull.Value ? Convert.ToDateTime(reader["fechaNacimiento"]) : DateTime.MinValue;
                            involucrado.Tarjeta = reader["tarjeta"] != DBNull.Value ? reader["tarjeta"].ToString() : string.Empty;
                            involucrado.TipoCarga = reader["tipoCarga"] != DBNull.Value ? reader["tipoCarga"].ToString() : string.Empty;
                            involucrado.Poliza = reader["poliza"] != DBNull.Value ? reader["poliza"].ToString() : string.Empty;
                            involucrado.Serie = reader["serie"] != DBNull.Value ? reader["serie"].ToString() : string.Empty;
                            involucrado.Entidad = reader["nombreEntidad"] != DBNull.Value ? reader["nombreEntidad"].ToString() : string.Empty;
                            involucrado.Marca = reader["marcaVehiculo"] != DBNull.Value ? reader["marcaVehiculo"].ToString() : string.Empty;
                            involucrado.Submarca = reader["nombreSubmarca"] != DBNull.Value ? reader["nombreSubmarca"].ToString() : string.Empty;
                            involucrado.TipoVehiculo = reader["tipoVehiculo"] != DBNull.Value ? reader["tipoVehiculo"].ToString() : string.Empty;
                            involucrado.PropietarioInvolucrado = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            involucrado.NumeroReporte = reader["numeroReporte"] != DBNull.Value ? reader["numeroReporte"].ToString() : string.Empty;
                            involucrado.Modelo = reader["modelo"] != DBNull.Value ? reader["modelo"].ToString() : string.Empty;
                            involucrado.Motor = reader["motor"] != DBNull.Value ? reader["motor"].ToString() : string.Empty;
                            involucrado.Capacidad = reader["capacidad"] != DBNull.Value ? reader["capacidad"].ToString() : string.Empty;
                            involucrado.Pension = reader["pension"] != DBNull.Value ? reader["pension"].ToString() : string.Empty;
                            involucrado.Modelo = reader["modelo"] != DBNull.Value ? reader["modelo"].ToString() : string.Empty;
                            involucrado.Color = reader["color"] != DBNull.Value ? reader["color"].ToString() : string.Empty;
                            involucrado.RFC = reader["RFC"] != DBNull.Value ? reader["RFC"].ToString() : string.Empty;
                            involucrado.CURP = reader["CURP"] != DBNull.Value ? reader["CURP"].ToString() : string.Empty;
                            involucrado.TipoPersona = reader["tipoPersona"] != DBNull.Value ? reader["tipoPersona"].ToString() : string.Empty;
                            involucrado.TipoServicio = reader["tipoServicio"] != DBNull.Value ? reader["tipoServicio"].ToString() : string.Empty;
                            involucrado.VigenciaTarjeta = reader["vigenciaTarjeta"] != DBNull.Value ? (DateTime)reader["vigenciaTarjeta"] : DateTime.MinValue;


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
            return involucrado;
        }

        public CapturaAccidentesModel ObtenerConductorPorId(int IdPersona)
        {
            CapturaAccidentesModel model = new CapturaAccidentesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                                            @"SELECT p.idPersona,p.idCatTipoPersona,p.nombre,p.apellidoPaterno,p.apellidoMaterno,
                                            p.rfc,p.curp,CONVERT(varchar, p.fechaNacimiento, 103) AS fechaNacimiento,p.vigenciaLicencia,p.numeroLicencia,p.idGenero
                                            ,ctp.tipoPersona,v.idVehiculo,v.modelo
                                            ,pd.telefono
                                            ,pd.correo,pd.idEntidad,pd.idMunicipio,pd.colonia,pd.calle,pd.numero
							                ,tl.tipoLicencia,tv.tipoVehiculo
                                            ,mun.Municipio,cent.nombreEntidad,cg.genero
                                            ,v.vigenciaTarjeta


                                            FROM personas AS p                                           
                                            LEFT JOIN catTipoPersona AS ctp ON p.idCatTipoPersona = ctp.idCatTipoPersona
                                            LEFT JOIN vehiculos AS v ON p.idPersona = v.idPersona
                                            LEFT JOIN personasDirecciones AS pd ON p.idPersona = pd.idPersona
                                            LEFT JOIN catGeneros cg on p.idGenero = cg.idGenero
                                            LEFT JOIN catTipoLicencia tl ON p.idTipoLicencia = tl.idTipoLicencia
                                            LEFT JOIN catTiposVehiculo AS tv ON v.idTipoVehiculo = tv.idTipoVehiculo
                                            LEFT JOIN catMunicipios AS mun ON mun.idMunicipio = pd.idMunicipio 
                                            LEFT JOIN catEntidades AS cent ON pd.idEntidad = cent.idEntidad 
                                            WHERE p.idPersona = @IdPersona AND p.estatus = 1";
                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdPersona", SqlDbType.Int)).Value = (object)IdPersona ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            model.IdPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.IdVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idVehiculo"].ToString());
                            model.IdCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.nombre = reader["nombre"].ToString();
                            model.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            model.RFC = reader["rfc"].ToString();
                            model.CURP = reader["curp"].ToString();
                            model.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.Genero = reader["genero"].ToString();
                            model.TipoPersona = reader["tipoPersona"].ToString();
                            model.FormatDateNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["fechaNacimiento"].ToString());
                            model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
                            model.idGenero = reader["idGenero"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"]);
							model.Telefono = reader["telefono"].ToString();
							model.Correo = reader["correo"].ToString();
							model.Modelo= reader["modelo"].ToString();
							model.Colonia = reader["colonia"].ToString();
							model.Numero = reader["numero"].ToString();
							model.Calle = reader["calle"].ToString();
							model.IdEntidad= reader["IdEntidad"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdEntidad"].ToString());
							model.IdMunicipio = reader["IdMunicipio"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdMunicipio"].ToString());
							model.TipoLicencia = reader["tipoLicencia"].ToString();
							model.TipoVehiculo = reader["tipoVehiculo"].ToString();
							model.Entidad = reader["nombreEntidad"].ToString();
							model.Municipio = reader["Municipio"].ToString();


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
        public int InsertarConductor(int IdVehiculo, int idAccidente, int IdPersona)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO conductoresVehiculosAccidente (idAccidente,idVehiculo, idPersona, estatus) values(@idAccidente, @idVehiculo,@idPersona,@estatus) ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idPersona", IdPersona);
                    command.Parameters.AddWithValue("@estatus", 1);



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

		public int ActualizaInfoAccidente(int idAccidente, DateTime Fecha, TimeSpan Hora, int IdMunicipio, int IdCarretera, int IdTramo, int Kilometro)
		{
			int result = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = "UPDATE accidentes SET fecha = @fecha, hora = @hora, idMunicipio = @idMunicipio, idCarretera = @idCarretera, idTramo = @idTramo, kilometro = @kilometro WHERE idAccidente = @idAccidente";

					SqlCommand command = new SqlCommand(query, connection);

					command.Parameters.Add(new SqlParameter("@fecha", SqlDbType.Date)).Value = (object)Fecha ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@hora", SqlDbType.Time)).Value = (object)Hora ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = IdMunicipio;
					command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = IdCarretera;
					command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = IdTramo;
					command.Parameters.Add(new SqlParameter("@kilometro", SqlDbType.Int)).Value = Kilometro;
					command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;

					result = command.ExecuteNonQuery();
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

		public int AgregarValorClasificacion(int IdClasificacionAccidente, int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidentes SET idClasificacionAccidente = @IdClasificacionAccidente WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@IdClasificacionAccidente", IdClasificacionAccidente);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

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
        public List<CapturaAccidentesModel> ObtenerDatosGrid(int idAccidente)
        {
            //
            List<CapturaAccidentesModel> ListaClasificacion = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.*, ca.nombreClasificacion FROM accidentes a JOIN catClasificacionAccidentes ca ON a.idClasificacionAccidente = ca.idClasificacionAccidente WHERE a.idAccidente = @idAccidente AND a.idClasificacionAccidente > 0;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel clasificacion = new CapturaAccidentesModel();
                            clasificacion.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
                            clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["IdClasificacionAccidente"].ToString());
                            clasificacion.NombreClasificacion = reader["NombreClasificacion"].ToString();


                            ListaClasificacion.Add(clasificacion);

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
            return ListaClasificacion;


        }

        public List<CapturaAccidentesModel> AccidentePorID(int IdAccidente)
        {
            List<CapturaAccidentesModel> ListaAccidentePorID = new List<CapturaAccidentesModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT* FROM accidentes WHERE idAccidente = @idAccidente;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", IdAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel clasificacion = new CapturaAccidentesModel();
                            clasificacion.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
                            clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["IdClasificacionAccidente"].ToString());


                            ListaAccidentePorID.Add(clasificacion);

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
            return ListaAccidentePorID;


        }
        public int ClasificacionEliminar(int IdAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidentes SET idClasificacionAccidente = 0 WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAccidente", IdAccidente);

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

        public int AgregarValorFactorYOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente, int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO AccidenteFactoresOpciones (idFactor, idFactorOpcion, idAccidente,fechaActualizacion,actualizadoPor,estatus) " +
                                   "VALUES (@IdFactorAccidente, @IdFactorOpcionAccidente,@idAccidente,@fechaActualizacion,@actualizadoPor,@estatus);";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@IdFactorAccidente", IdFactorAccidente);
                    command.Parameters.AddWithValue("@IdFactorOpcionAccidente", IdFactorOpcionAccidente);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
                    command.Parameters.AddWithValue("@actualizadoPor", 1);
                    command.Parameters.AddWithValue("@estatus", 1);


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
        public int EliminarValorFactorYOpcion(int IdAccidenteFactorOpcion)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE AccidenteFactoresOpciones SET estatus = 0 " +
                        "WHERE idAccidenteFactorOpcion = @IdAccidenteFactorOpcion";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@IdAccidenteFactorOpcion", IdAccidenteFactorOpcion);
              
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
        public int EditarFactorOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente,int IdAccidenteFactorOpcion)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE AccidenteFactoresOpciones SET idFactor = @IdFactor, idFactorOpcion = @IdFactorOpcion  " +
                                    "WHERE idAccidenteFactorOpcion = @IdAccidenteFactorOpcion";
                    
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idFactor", IdFactorAccidente);
                    command.Parameters.AddWithValue("@idFactorOpcion", IdFactorOpcionAccidente);
                    command.Parameters.AddWithValue("@IdAccidenteFactorOpcion", IdAccidenteFactorOpcion);

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

        public List<CapturaAccidentesModel> ObtenerDatosGridFactor(int idAccidente)
        {
            //
            List<CapturaAccidentesModel> ListaGridFactor = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT afo.idAccidenteFactorOpcion, afo.idFactor,afo.idFactorOpcion,afo.idAccidente,cfa.FactorAccidente,cfoa.FactorOpcionAccidente " +
                        "FROM AccidenteFactoresOpciones AS afo " +
                        "LEFT JOIN catFactoresAccidentes AS cfa ON afo.idFactor = cfa.idFactorAccidente " +
                        "LEFT JOIN catFactoresOpcionesAccidentes AS cfoa ON afo.idFactorOpcion = cfoa.idFactorOpcionAccidente " +
                        "WHERE afo.idAccidente = @IdAccidente AND afo.estatus = 1 ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel factorOpcion = new CapturaAccidentesModel();
                            factorOpcion.IdAccidenteFactorOpcion = Convert.ToInt32(reader["idAccidenteFactorOpcion"].ToString());
                            factorOpcion.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
                            factorOpcion.IdFactorAccidente = Convert.ToInt32(reader["idFactor"].ToString());
                            factorOpcion.IdFactorOpcionAccidente = Convert.ToInt32(reader["idFactorOpcion"].ToString());
                            factorOpcion.FactorAccidente = reader["FactorAccidente"].ToString();
                            factorOpcion.FactorOpcionAccidente = reader["FactorOpcionAccidente"].ToString();


                            ListaGridFactor.Add(factorOpcion);

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
            return ListaGridFactor;


        }

        public int AgregarValorCausa(int IdCausaAccidente, int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
					connection.Open();
                    string query = @"INSERT into accidenteCausas(idAccidente,idCausaAccidente,indice) values(@idAccidente, @idCausaAccidente,
                    (SELECT isnull(Max(indice),0)+1 FROM accidenteCausas where idAccidente = @idAccidente and idCausaAccidente <> 0))";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idCausaAccidente", IdCausaAccidente);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

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

        public void ActualizaIndiceCuasa(int idAccidenteCausa, int indice)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidenteCausas SET indice = @indice WHERE idAccidenteCausa = @idCausaAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idCausaAccidente", idAccidenteCausa);
                    command.Parameters.AddWithValue("@indice", indice);

                    command.ExecuteNonQuery();
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
        }

        public int EditarValorCausa(int IdCausaAccidente, int idAccidenteCausa)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidenteCausas SET idCausaAccidente = @idCausaAccidente WHERE idAccidenteCausa = @idAccidenteCausa ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idCausaAccidente", IdCausaAccidente);
                    command.Parameters.AddWithValue("@idAccidenteCausa", idAccidenteCausa);

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

        public void RecalcularIndex(int IdAccidente)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = @"
                    With UpdateData  As
                    (
                    SELECT idAccidenteCausa,idAccidente, idCausaAccidente, row_number() OVER (PARTITION BY idAccidente ORDER BY indice) indexs 
                    FROM accidenteCausas 
                    WHERE idAccidente = @idAccidente
                    and idCausaAccidente > 0
                    )
                    UPDATE accidenteCausas SET indice = indexs
                    FROM accidenteCausas
                    INNER JOIN UpdateData ON accidenteCausas.idAccidenteCausa = UpdateData.idAccidenteCausa and accidenteCausas.idAccidente = UpdateData.idAccidente ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAccidente", IdAccidente);

                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        public int EliminarCausaBD(int IdCausaAccidente, int idAccidente, int idAccidenteCausa)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidenteCausas SET idCausaAccidente = 0 WHERE idAccidente = @idAccidente AND idCausaAccidente = @idCausaAccidente AND idAccidenteCausa = @idAccidenteCausa ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idCausaAccidente", IdCausaAccidente);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idAccidenteCausa", idAccidenteCausa);

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
        public int GuardarDescripcion(int idAccidente, string descripcionCausa)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidentes SET idEstatusReporte = @idEstatusReporte";

                    if (descripcionCausa != null)
                    {
                        query += ", descripcionCausas = @DescripcionCausa";
                    }

                    query += " WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idEstatusReporte", 2);

                    if (descripcionCausa != null)
                    {
                        command.Parameters.AddWithValue("@DescripcionCausa", descripcionCausa);
                    }

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

        public List<CapturaAccidentesModel> ObtenerDatosGridCausa(int idAccidente)
        
        {
            //
            List<CapturaAccidentesModel> ListaGridCausa = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
					int numeroContinuo = 1;
					connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ac.*,a.descripcionCausas, c.causaAccidente, ac.idAccidenteCausa, ac.indice FROM accidenteCausas ac " +
                                                        "JOIN catCausasAccidentes c ON ac.idCausaAccidente = c.idCausaAccidente " +
                                                        "LEFT JOIN accidentes AS a ON ac.idAccidente = a.idAccidente " +
                                                        "WHERE ac.idAccidente = @idAccidente AND ac.idCausaAccidente > 0 Order By ac.indice", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel causa = new CapturaAccidentesModel();
                            causa.idAccidenteCausa = Convert.ToInt32(reader["idAccidenteCausa"].ToString());

                            causa.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
                            causa.IdCausaAccidente = Convert.ToInt32(reader["IdCausaAccidente"].ToString());
                            causa.CausaAccidente = reader["causaAccidente"].ToString();
                            causa.DescripcionCausa = reader["descripcionCausas"].ToString();
                            string indx = reader["indice"].ToString();
                            causa.indice = Convert.ToInt32((indx == string.Empty? "0": indx));
                            causa.NumeroContinuo = numeroContinuo;
							ListaGridCausa.Add(causa);
							numeroContinuo++;


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
            return ListaGridCausa;


        }
        public List<CapturaAccidentesModel> BusquedaPersonaInvolucrada(BusquedaInvolucradoModel model, string server = null)
        {
            //
            List<CapturaAccidentesModel> ListaInvolucrados = new List<CapturaAccidentesModel>();

            string condiciones = "";
            condiciones += string.IsNullOrEmpty(model.licencia) ? "" : " AND numeroLicencia LIKE '%' + @numeroLicencia + '%' ";
            condiciones += string.IsNullOrEmpty(model.curpBusqueda) ? "" : " AND curp LIKE '%' + @curp + '%'";
            condiciones += string.IsNullOrEmpty(model.rfcBusqueda) ? "" : " AND rfc LIKE '%' + @rfc + '%'";
            condiciones += string.IsNullOrEmpty(model.nombre) ? "" : " AND nombre LIKE '%' + @nombre + '%' ";
            condiciones += string.IsNullOrEmpty(model.apellidoPaterno) ? "" : " AND apellidoPaterno LIKE '%' + @apellidoPaterno + '%' ";
            condiciones += string.IsNullOrEmpty(model.apellidoMaterno) ? "" : " AND apellidoMaterno LIKE '%' + @apellidoMaterno + '%' ";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open(); 

                    string SqlTransact = @"SELECT * FROM personas WHERE estatus = 1 "+ condiciones+ " ORDER BY nombre";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    if (!string.IsNullOrEmpty(model.licencia))
                        command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.licencia ?? DBNull.Value;

                    if (!string.IsNullOrEmpty(model.curpBusqueda))
                        command.Parameters.Add(new SqlParameter("@curp", SqlDbType.NVarChar)).Value = (object)model.curpBusqueda ?? DBNull.Value;

                    if (!string.IsNullOrEmpty(model.rfcBusqueda))
                        command.Parameters.Add(new SqlParameter("@rfc", SqlDbType.NVarChar)).Value = (object)model.rfcBusqueda ?? DBNull.Value;

                    if (!string.IsNullOrEmpty(model.nombre))
                        command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar)).Value = (object)model.nombre ?? DBNull.Value;

                    if (!string.IsNullOrEmpty(model.apellidoPaterno))
                        command.Parameters.Add(new SqlParameter("@apellidoPaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoPaterno ?? DBNull.Value;

                    if (!string.IsNullOrEmpty(model.apellidoMaterno))
                        command.Parameters.Add(new SqlParameter("@apellidoMaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoMaterno ?? DBNull.Value;
                     
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
                            involucrado.IdPersona = Convert.ToInt32(reader["idPersona"].ToString());
                            involucrado.nombre = reader["nombre"].ToString();
                            involucrado.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            involucrado.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            involucrado.RFC = reader["rfc"].ToString();
                            involucrado.CURP = reader["curp"].ToString();
                            involucrado.numeroLicencia = reader["numeroLicencia"].ToString();
                            string fechaNac = reader["fechaNacimiento"].ToString();
                            if (!fechaNac.IsNullOrEmpty())
                                involucrado.fechaNacimiento = Convert.ToDateTime(fechaNac);

							ListaInvolucrados.Add(involucrado); 
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

            return ListaInvolucrados;
        }
        public int AgregarPersonaInvolucrada(int idPersonaInvolucrado, int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT into involucradosAccidente(idAccidente,idPersona) values(@idAccidente, @idPersonaInvolucrado)";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idPersonaInvolucrado", idPersonaInvolucrado);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

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

        public int EliminarInvolucradoAcc(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado, int IdAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE conductoresVehiculosAccidente SET estatus = 0 WHERE idAccidente = @IdAccidente AND idPersona = @IdPropietarioInvolucrado AND idVehiculo = @IdVehiculoInvolucrado;";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAccidente", IdAccidente);
                    command.Parameters.AddWithValue("@IdPropietarioInvolucrado", IdPropietarioInvolucrado);
                    command.Parameters.AddWithValue("@IdVehiculoInvolucrado", IdVehiculoInvolucrado);


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
        public List<CapturaAccidentesModel> VehiculosInvolucrados(int IdAccidente)
        {
            //
            List<CapturaAccidentesModel> ListaVehiculosInvolucrados = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                       SELECT cva.*, COALESCE(cva.idPersona, pcv.idPersona) AS idConductor,cva.idTipoCarga,cva.poliza,ctc.tipoCarga,v.placas, v.tarjeta, v.serie, v.idMarcaVehiculo,  
                        v.idSubmarca,v.idEntidad, v.idTipoVehiculo,acc.numeroReporte,v.idPersona AS idPropietario, v.modelo, v.idColor, v.idCatTipoServicio, v.motor, v.capacidad,  
                        cm.marcaVehiculo, csv.nombreSubmarca, tv.tipoVehiculo, COALESCE(p.nombre, pcv.nombre) AS nombre, COALESCE(p.apellidoPaterno, pcv.apellidoPaterno) AS apellidoPaterno,  
                        p.apellidoMaterno,p.RFC,p.CURP, CONVERT(varchar, p.fechaNacimiento, 103) AS fechaNacimiento, c.color, ts.tipoServicio, pcv.nombre AS nombreConductor, pcv.apellidoPaterno AS apellidoPConductor, pcv.apellidoMaterno AS apellidoMConductor,  
                        tc.tipoCarga, pen.pension, ft.formaTraslado, cent.nombreEntidad,va.montoVehiculo ,p.vigenciaLicencia ,
						isnull(pd.colonia,'')+' '+isnull(pd.codigoPostal,'')+' '+ isnull(pd.calle,'')+' '+isnull(pd.numero,'') as direccion,
						isnull(pdc.colonia,'')+' '+isnull(pdc.codigoPostal,'')+' '+ isnull(pdc.calle,'')+' '+isnull(pdc.numero,'') as direccionc,
						p.nombre,pcv.nombre, GC.genero,pcv.numeroLicencia,tl.tipoLicencia
                        FROM conductoresVehiculosAccidente AS cva 
						INNER JOIN vehiculos AS v ON cva.idVehiculo = v.idVehiculo  
                        LEFT JOIN catMarcasVehiculos AS cm ON v.idMarcaVehiculo = cm.idMarcaVehiculo  
                        LEFT JOIN catTiposcarga AS ctc ON cva.idTipoCarga = ctc.idTipoCarga  
                        LEFT JOIN catSubmarcasVehiculos AS csv ON v.idSubmarca = csv.idSubmarca  
                        LEFT JOIN catTiposVehiculo AS tv ON v.idTipoVehiculo = tv.idTipoVehiculo  
                        LEFT JOIN personas AS p ON v.idPersona = p.idPersona  
                        LEFT JOIN catColores AS c ON v.idColor = c.idColor  
                        LEFT JOIN catTiposcarga AS tc ON cva.idTipoCarga = tc.idTipoCarga  
                        LEFT JOIN pensiones AS pen ON cva.idPension = pen.idPension  
                        LEFT JOIN vehiculosAccidente AS va ON cva.idVehiculo = va.idVehiculo AND cva.idAccidente = va.idAccidente  
                        LEFT JOIN catFormasTraslado AS ft ON cva.idFormaTraslado = ft.idFormaTraslado  
                        LEFT JOIN catTipoServicio AS ts ON v.idCatTipoServicio = ts.idCatTipoServicio  
                        LEFT JOIN accidentes AS acc ON cva.idAccidente = acc.idAccidente  
                        LEFT JOIN catEntidades AS cent ON v.idEntidad = cent.idEntidad  
                        LEFT JOIN personas AS pcv ON cva.idPersona = pcv.idPersona  
						left join personasDirecciones pd on pd.idPersona=p.idPersona
						left join personasDirecciones pdc on pdc.idPersona=pcv.idPersona
						left join catGeneros GC on GC.idGenero=pcv.idGenero
						left join catTipoLicencia tl on pcv.idTipoLicencia=tl.idTipoLicencia

                        WHERE cva.idAccidente = @idAccidente AND cva.idAccidente > 0 AND cva.estatus = 1;
                        ", connection);

                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", IdAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel vehiculo = new CapturaAccidentesModel();
                            vehiculo.IdPropietarioInvolucrado = Convert.IsDBNull(reader["idPersona"]) ? 0 : Convert.ToInt32(reader["idPersona"]);
                            vehiculo.IdAccidente = Convert.IsDBNull(reader["idAccidente"]) ? 0 : Convert.ToInt32(reader["idAccidente"]);
                            vehiculo.IdVehiculoInvolucrado = Convert.IsDBNull(reader["idVehiculo"]) ? 0 : Convert.ToInt32(reader["idVehiculo"]);
                            vehiculo.IdTipoCarga = Convert.IsDBNull(reader["IdTipoCarga"]) ? 0 : Convert.ToInt32(reader["IdTipoCarga"]);
                            vehiculo.IdPension = Convert.IsDBNull(reader["IdPension"]) ? 0 : Convert.ToInt32(reader["IdPension"]);
                            vehiculo.IdFormaTrasladoInvolucrado = Convert.IsDBNull(reader["idFormaTraslado"]) ? 0 : Convert.ToInt32(reader["idFormaTraslado"]);
                            vehiculo.idPersonaInvolucrado = Convert.IsDBNull(reader["IdConductor"]) ? 0 : Convert.ToInt32(reader["IdConductor"]);
                            vehiculo.Placa = reader["placas"].ToString();
                            vehiculo.fechaNacimiento = Convert.IsDBNull(reader["fechaNacimiento"]) ? DateTime.MinValue : Convert.ToDateTime(reader["fechaNacimiento"]);
                            vehiculo.Tarjeta = reader["tarjeta"].ToString();
                            vehiculo.TipoCarga = reader["tipoCarga"].ToString();
                            vehiculo.Poliza = reader["poliza"].ToString();
                            vehiculo.Serie = reader["serie"].ToString();
                            vehiculo.Entidad = reader["nombreEntidad"].ToString();
                            vehiculo.Marca = reader["marcaVehiculo"].ToString();
                            vehiculo.Submarca = reader["nombreSubmarca"].ToString();
                            vehiculo.TipoVehiculo = reader["tipoVehiculo"].ToString();
                            vehiculo.PropietarioInvolucrado = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            vehiculo.NumeroReporte = reader["numeroReporte"].ToString();
                            vehiculo.Modelo = reader["modelo"].ToString();
                            vehiculo.Motor = reader["motor"].ToString();
                            vehiculo.Capacidad = reader["capacidad"].ToString();
                            vehiculo.Pension = reader["pension"].ToString();
                            vehiculo.Modelo = reader["modelo"].ToString();
                            vehiculo.Color = reader["color"].ToString();
                            vehiculo.RFC = reader["RFC"].ToString();
                            vehiculo.CURP = reader["CURP"].ToString();
                            vehiculo.TipoServicio = reader["tipoServicio"].ToString();
                            vehiculo.FormaTrasladoInvolucrado = reader["formaTraslado"].ToString();
                            vehiculo.Direccion = reader["direccion"].ToString();
							vehiculo.DireccionConductor = reader["direccionc"].ToString();
                            vehiculo.Sexo = reader["genero"].ToString();
                            vehiculo.numeroLicencia = reader["numeroLicencia"].ToString();
                            vehiculo.TipoLicencia = reader["tipoLicencia"].ToString();
							vehiculo.ConductorInvolucrado = $"{reader["nombreConductor"]} {reader["apellidoPConductor"]} {reader["apellidoMConductor"]}";
                            vehiculo.vigenciaLicencia = reader["vigenciaLicencia"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)reader["vigenciaLicencia"];
                            string montoVehiculoString = reader["montoVehiculo"].ToString();
                            float montoVehiculo;

                            if (!string.IsNullOrEmpty(montoVehiculoString) && float.TryParse(montoVehiculoString, out montoVehiculo))
                            {
                                vehiculo.montoVehiculo = montoVehiculo;
                            }
                            else
                            {
                                vehiculo.montoVehiculo = 0.0f; 
                            }

                            ListaVehiculosInvolucrados.Add(vehiculo);

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
            return ListaVehiculosInvolucrados;


        }
        public int GuardarComplementoVehiculo(CapturaAccidentesModel model, int IdVehiculo, int idAccidente)
        {
            int result = 0;
            string strQuery = @"
                                IF EXISTS (SELECT 1 FROM conductoresVehiculosAccidente WHERE idVehiculo = @IdVehiculo AND idAccidente = @IdAccidente)
                                    UPDATE conductoresVehiculosAccidente
                                    SET idTipoCarga = @IdTipoCarga,
                                        poliza = @Poliza,
                                        idDelegacion = @IdDelegacion,
                                        idPension = @IdPension,
                                        idFormaTraslado = @IdFormaTraslado,
                                        fechaActualizacion = @fechaActualizacion,
                                        actualizadoPor = @actualizadoPor,
                                        estatus = @estatus
                                    WHERE idVehiculo = @IdVehiculo AND idAccidente = @IdAccidente;";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@IdTipoCarga", SqlDbType.Int)).Value = (object)model.IdTipoCarga ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Poliza", SqlDbType.NVarChar)).Value = (object)model.Poliza ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdDelegacion", SqlDbType.Int)).Value = (object)model.IdDelegacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdPension", SqlDbType.Int)).Value = (object)model.IdPension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdFormaTraslado", SqlDbType.Int)).Value = (object)model.IdFormaTraslado ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdVehiculo", SqlDbType.Int)).Value = (object)IdVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdAccidente", SqlDbType.Int)).Value = (object)idAccidente ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;

                    result = command.ExecuteNonQuery();
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

            return result;
        }

        public int AgregarMontoV(MontoModel model)

        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update vehiculosAccidente set montoVehiculo = @montoVehiculo where idAccidente=@idAccidente AND idvehiculo = @idVehiculo",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@montoVehiculo", SqlDbType.Float)).Value = model.montoVehiculo;
                    sqlCommand.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = model.IdAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@idVehiculo", SqlDbType.Int)).Value = model.IdVehiculoInvolucrado;
                    sqlCommand.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = model.IdPropietarioInvolucrado;

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
        public List<CapturaAccidentesModel> InfraccionesVehiculosAccidete(int idAccidente)
        {
            //
            List<CapturaAccidentesModel> ListaVehiculosInfracciones = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("(SELECT v.idVehiculo, v.placas, propietario.idPersona AS propietario, i.folioInfraccion, i.fechaInfraccion,i.idInfraccion,i.idPersona,i.idPersonaInfraccion, conductor.idPersona AS conductor, ent.nombreEntidad, " +
                        "propietario.nombre AS propietario_nombre, propietario.apellidoPaterno AS propietario_apellidoPaterno, propietario.apellidoMaterno AS propietario_apellidoMaterno, conductor.nombre AS conductor_nombre, conductor.apellidoPaterno AS conductor_apellidoPaterno, conductor.apellidoMaterno AS conductor_apellidoMaterno " +
                        "FROM conductoresVehiculosAccidente AS cva " +
                        "LEFT JOIN vehiculos AS v ON cva.idVehiculo = v.idVehiculo " +
                        "LEFT JOIN catEntidades AS ent ON v.idEntidad = ent.idEntidad " +
                        "LEFT JOIN infracciones AS i ON cva.idVehiculo = i.idVehiculo " +
                        "LEFT JOIN personas AS propietario ON v.idPersona = propietario.idPersona " +
                        "LEFT JOIN personas AS conductor ON i.idPersonaInfraccion= conductor.idPersona " +
                        "WHERE cva.idAccidente = @idAccidente " +
                        "AND EXISTS(SELECT 1 FROM infracciones AS i WHERE i.idVehiculo = v.idVehiculo))", connection);



                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel elemnto = new CapturaAccidentesModel();
                            elemnto.IdVehiculo = Convert.ToInt32(reader["IdVehiculo"].ToString());
                            elemnto.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
                            elemnto.Placa = reader["placas"].ToString();
                            elemnto.folioInfraccion = reader["folioInfraccion"].ToString();
                            elemnto.Fecha = Convert.ToDateTime(reader["fechaInfraccion"].ToString());
                            elemnto.ConductorInvolucrado = reader["conductor_nombre"].ToString() + " " + reader["conductor_apellidoPaterno"].ToString() + " " + reader["conductor_apellidoMaterno"].ToString();
                            elemnto.Propietario = reader["propietario_nombre"].ToString() + " " + reader["propietario_apellidoPaterno"].ToString() + " " + reader["propietario_apellidoMaterno"].ToString();
                            elemnto.EntidadRegistro = reader["nombreEntidad"].ToString();


                            ListaVehiculosInfracciones.Add(elemnto);

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
            return ListaVehiculosInfracciones;


        }
        public int RelacionAccidenteInfraccion(int IdVehiculo, int idAccidente, int IdInfraccion)
        {
            int infraccionAgregada = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO infraccionesAccidente (idVehiculo, idAccidente,idInfraccion) VALUES (@IdVehiculo, @idAccidente, @IdInfraccion)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idInfraccion", IdInfraccion);

                    command.ExecuteNonQuery();
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

            return infraccionAgregada;
        }
        public List<CapturaAccidentesModel> InfraccionesDeAccidente(int idAccidente)
        {
            //
            List<CapturaAccidentesModel> ListaInfracciones = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ia.idInf_Acc, ia.idAccidente, ia.idVehiculo, " +
                        "v.idMarcaVehiculo, v.idSubmarca, v.placas, v.modelo, " +
                        "a.idEstatusReporte, " +
                        "i.folioInfraccion, " +
                        "cei.estatusInfraccion, " +
                        "i.idEstatusInfraccion, "+
						"mv.marcaVehiculo, sv.nombreSubmarca, i.idInfraccion " +
						"FROM infraccionesAccidente AS ia JOIN vehiculos AS v ON ia.idVehiculo = v.idVehiculo " +
                        "JOIN accidentes AS a ON ia.idAccidente = a.idAccidente " +
                        "JOIN infracciones AS i ON ia.idInfraccion = i.idInfraccion " +
                        "JOIN catEstatusInfraccion AS cei ON cei.idEstatusInfraccion = i.idEstatusInfraccion " +
                        "JOIN catMarcasVehiculos AS mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                        "JOIN catSubmarcasVehiculos AS sv ON v.idSubmarca = sv.idSubmarca " +
                        "WHERE ia.idAccidente = @idAccidente;", connection);



                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel elemnto = new CapturaAccidentesModel();
							elemnto.IdInfraccion = Convert.IsDBNull(reader["idInfraccion"]) ? 0 : Convert.ToInt32(reader["idInfraccion"]);
							elemnto.IdInfAcc = Convert.IsDBNull(reader["IdInf_Acc"]) ? 0 : Convert.ToInt32(reader["IdInf_Acc"]);
                            elemnto.IdAccidente = Convert.IsDBNull(reader["IdAccidente"]) ? 0 : Convert.ToInt32(reader["IdAccidente"]);
                            elemnto.IdVehiculoInvolucrado = Convert.IsDBNull(reader["IdVehiculo"]) ? 0 : Convert.ToInt32(reader["IdVehiculo"]);
                            elemnto.Placa = reader["placas"].ToString();
                            elemnto.EstatusInfraccion = reader["estatusInfraccion"].ToString();
                            elemnto.folioInfraccion = reader["folioInfraccion"].ToString();
                           // elemnto.EstatusReporte = reader["estatusReporte"].ToString();
                            elemnto.Vehiculo = $"{reader["marcaVehiculo"]} {reader["nombreSubmarca"]} {reader["placas"]} {reader["modelo"]}";




                            ListaInfracciones.Add(elemnto);

                        }

                    }

                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error al ejecutar la consulta SQL: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            return ListaInfracciones;


        }
        public int RelacionPersonaVehiculo(int IdPersona, int idAccidente, int IdVehiculoInvolucrado)


        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT into involucradosAccidente(idPersona,idAccidente,idVehiculo) values(@idPersona, @idAccidente,@idVehiculoInvolucrado)";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idPersona", IdPersona);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idVehiculoInvolucrado", IdVehiculoInvolucrado);

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
        public int ActualizarInvolucrado(CapturaAccidentesModel model, int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    string consultaExistencia = @"SELECT COUNT(*) FROM InvolucradosAccidente
                                     WHERE idAccidente = @idAccidente
                                     AND idPersona = @idPersona
                                     AND idVehiculo = @idVehiculo";

                    string consultaUpdate = @"UPDATE InvolucradosAccidente
                                  SET idTipoInvolucrado = @idTipoInvolucrado,
                                      idEstadoVictima = @idEstadoVictima,
                                      idHospital = @idHospital,
                                      idInstitucionTraslado = @idInstitucionTraslado,
                                      idAsiento = @idAsiento,
                                      idCinturon = @idCinturon
                                  WHERE idAccidente = @idAccidente
                                    AND idPersona = @idPersona
                                    AND idVehiculo = @idVehiculo";

                    string consultaInsert = @"INSERT INTO InvolucradosAccidente
                                  (idAccidente, idPersona, idVehiculo, idTipoInvolucrado, idEstadoVictima, idHospital, idInstitucionTraslado, idAsiento, idCinturon)
                                  VALUES
                                  (@idAccidente, @idPersona, @idVehiculo, @idTipoInvolucrado, @idEstadoVictima, @idHospital, @idInstitucionTraslado, @idAsiento, @idCinturon)";

                    SqlCommand comandoExistencia = new SqlCommand(consultaExistencia, connection);
                    comandoExistencia.Parameters.AddWithValue("@idAccidente", idAccidente);
                    comandoExistencia.Parameters.AddWithValue("@idPersona", model.IdPersona);
                    comandoExistencia.Parameters.AddWithValue("@idVehiculo", model.IdVehiculo);

                    connection.Open();
                    int existencia = (int)comandoExistencia.ExecuteScalar();

                    if (existencia > 0)
                    {
                        // Ejecutar la consulta de actualización
                        SqlCommand comandoUpdate = new SqlCommand(consultaUpdate, connection);
                        comandoUpdate.Parameters.AddWithValue("@idAccidente", idAccidente);
                        comandoUpdate.Parameters.AddWithValue("@idPersona", model.IdPersona);
                        comandoUpdate.Parameters.AddWithValue("@idVehiculo", model.IdVehiculo != null ? model.IdVehiculo : 0);
                        comandoUpdate.Parameters.AddWithValue("@idTipoInvolucrado", model.IdTipoInvolucrado);
                        comandoUpdate.Parameters.AddWithValue("@idEstadoVictima", model.IdEstadoVictima);
                        comandoUpdate.Parameters.AddWithValue("@idHospital", model.IdHospital);
                        comandoUpdate.Parameters.AddWithValue("@idInstitucionTraslado", model.IdInstitucionTraslado);
                        comandoUpdate.Parameters.AddWithValue("@idAsiento", model.IdAsiento);
                        comandoUpdate.Parameters.AddWithValue("@idCinturon", model.IdCinturon);

                        result = comandoUpdate.ExecuteNonQuery();
                    }
                    else
                    {
                        // Ejecutar la consulta de inserción
                        SqlCommand comandoInsert = new SqlCommand(consultaInsert, connection);
                        comandoInsert.Parameters.AddWithValue("@idAccidente", idAccidente);
                        comandoInsert.Parameters.AddWithValue("@idPersona", model.IdPersona);
                        comandoInsert.Parameters.AddWithValue("@idVehiculo", model.IdVehiculo != null ? model.IdVehiculo : 0);
                        comandoInsert.Parameters.AddWithValue("@idTipoInvolucrado", model.IdTipoInvolucrado);
                        comandoInsert.Parameters.AddWithValue("@idEstadoVictima", model.IdEstadoVictima);
                        comandoInsert.Parameters.AddWithValue("@idHospital", model.IdHospital);
                        comandoInsert.Parameters.AddWithValue("@idInstitucionTraslado", model.IdInstitucionTraslado);
                        comandoInsert.Parameters.AddWithValue("@idAsiento", model.IdAsiento);
                        comandoInsert.Parameters.AddWithValue("@idCinturon", model.IdCinturon);

                        result = comandoInsert.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    // Manejar excepciones
                }
            }

            return result;
        
       }
        public List<CapturaAccidentesModel> InvolucradosAccidente(int idAccidente)
        {
            //
            List<CapturaAccidentesModel> ListaInvolucrados = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try


                {
                    var nombre = @"
hola
";

                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT " +
											 "MAX(p.nombre) AS nombre, " +
											 "MAX(p.apellidoPaterno) AS apellidoPaterno, " +
											 "MAX(p.apellidoMaterno) AS apellidoMaterno, " +
											 "MAX(p.rfc) AS rfc, " +
											 "MAX(p.curp) AS curp, " +
											 "MAX(p.idTipoLicencia) AS idTipoLicencia, " +
											 "MAX(CONVERT(varchar, p.fechaNacimiento, 103)) AS fechaNacimiento, " +
											 "MAX(tl.tipoLicencia) AS tipoLicencia, " +
											 "MAX(ia.idAccidente) AS idAccidente," +
											 "MAX(ia.idPersona) AS idPersona, " +
											 "MAX(ia.idVehiculo) AS idVehiculo, " +
											 "MAX(v.idTipoVehiculo) AS idTipoVehiculo, " +
											 "MAX(tv.tipoVehiculo) AS tipoVehiculo, " +
											 "MAX(ia.idEstadoVictima) AS idEstadoVictima, " +
											 "MAX(ev.estadoVictima) AS estadoVictima, " +
											 "MAX(ia.idInstitucionTraslado) AS idInstitucionTraslado, " +
											 "MAX(it.institucionTraslado) AS institucionTraslado, " +
											 "MAX(ia.idHospital) AS idHospital, " +
											 "MAX(h.nombreHospital) AS nombreHospital, " +
											 "MAX(ia.idAsiento) AS idAsiento, " +
											 "MAX(ia.fechaIngreso) AS fechaIngreso, " +
											 "MAX(ia.horaIngreso) AS horaIngreso, " +
											 "MAX(ca.asiento) AS asiento, " +
											 "MAX(ia.idCinturon) AS idCinturon, " +
											 "MAX(ev.EstadoVictima) AS EstadoVictima," +
											 "MAX(v.modelo) AS modelo," +
											 "MAX(v.placas) AS placas," +
											 "MAX(cg.genero) AS genero," +
											 "MAX(cm.marcaVehiculo) AS marcaVehiculo," +
											 "MAX(csv.nombreSubmarca) AS nombreSubmarca," +
											 "MAX(pd.telefono) AS telefono," +
											 "MAX(pd.correo) AS correo," +
											 "MAX(mun.municipio) AS municipio," +
											 "MAX(e.nombreEntidad) AS nombreEntidad," +
											 "MAX(concat (pd.colonia,' ', pd.calle,' ', pd.numero,' ', pd.codigoPostal)) as Direccion," +
											 "MAX(va.idAccidente) AS NoAccidente," +
											 "MAX(ct.tipoInvolucrado) AS tipoInvolucrado," +
											 "MAX(cc.cinturon) AS cinturon " +
											 "FROM involucradosAccidente ia " +
											 "LEFT JOIN accidentes a ON ia.idAccidente = a.idAccidente " +
                                             "LEFT JOIN personas p ON ia.idPersona = p.idPersona " +
                                             "LEFT JOIN catTipoLicencia tl ON p.idTipoLicencia = tl.idTipoLicencia " +
                                             "LEFT JOIN vehiculos v ON ia.idVehiculo = v.idVehiculo " +
                                             "LEFT JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                                             "LEFT JOIN catEstadoVictima ev ON ia.idEstadoVictima = ev.idEstadoVictima " +
                                             "LEFT JOIN catInstitucionesTraslado it ON ia.idInstitucionTraslado = it.idInstitucionTraslado " +
                                             "LEFT JOIN catHospitales h ON ia.idHospital = h.idHospital " +
                                             "LEFT JOIN catAsientos ca ON ia.idAsiento = ca.idAsiento " +
                                             "LEFT JOIN catCinturon cc ON ia.idCinturon = cc.idCinturon " +
											 "LEFT JOIN catGeneros AS cg ON cg.idGenero = p.idGenero " +
											 "LEFT JOIN catMarcasVehiculos AS cm ON v.idMarcaVehiculo = cm.idMarcaVehiculo " +
											 "LEFT JOIN catSubmarcasVehiculos AS csv ON v.idSubmarca = csv.idSubmarca " +
                                             "LEFT JOIN catEntidades AS e ON v.idEntidad = e.idEntidad " +
											 "LEFT JOIN personasDirecciones AS pd ON p.idPersona = pd.idPersona " +
											 "LEFT JOIN catMunicipios AS mun ON mun.idMunicipio = pd.idMunicipio " +
											 "LEFT JOIN vehiculosAccidente AS va ON  va.idAccidente = a.idAccidente " +
											 "LEFT JOIN catTipoInvolucrado ct ON ct.idTipoInvolucrado = ia.idTipoInvolucrado " +
											 "WHERE ia.idAccidente = @idAccidente group by ia.idPersona;", connection);



                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
							involucrado.IdAccidente = reader["idAccidente"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idAccidente"].ToString());
							involucrado.IdTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
							involucrado.IdTipoVehiculo = reader["IdTipoVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdTipoVehiculo"].ToString());
							involucrado.IdPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
							involucrado.IdVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idVehiculo"].ToString());
							involucrado.IdEstadoVictima = reader["idEstadoVictima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idEstadoVictima"].ToString());
							involucrado.IdInstitucionTraslado = reader["idInstitucionTraslado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInstitucionTraslado"].ToString());
							involucrado.IdHospital = reader["idHospital"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idHospital"].ToString());
							involucrado.IdAsiento = reader["idAsiento"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idAsiento"].ToString());
							involucrado.IdCinturon = reader["idCinturon"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCinturon"].ToString());
							involucrado.nombre = reader["nombre"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombre"].ToString());
							involucrado.apellidoPaterno = reader["apellidoPaterno"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["apellidoPaterno"].ToString());
							involucrado.apellidoMaterno = reader["apellidoMaterno"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["apellidoMaterno"].ToString());
							involucrado.RFC = reader["rfc"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["rfc"].ToString());
							involucrado.CURP = reader["curp"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["curp"].ToString());
							involucrado.TipoLicencia = reader["tipoLicencia"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoLicencia"].ToString());
							involucrado.TipoVehiculo = reader["tipoVehiculo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoVehiculo"].ToString());
							involucrado.EstadoVictima = reader["estadoVictima"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["estadoVictima"].ToString());
							involucrado.NombreHospital = reader["nombreHospital"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreHospital"].ToString());
							involucrado.Asiento = reader["asiento"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["asiento"].ToString());
							involucrado.InstitucionTraslado = reader["institucionTraslado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["institucionTraslado"].ToString());
							involucrado.Placa = reader["placas"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["placas"].ToString());
							involucrado.Sexo = reader["genero"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["genero"].ToString());
							involucrado.Marca = reader["marcaVehiculo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["marcaVehiculo"].ToString());
							involucrado.Submarca = reader["nombreSubmarca"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreSubmarca"].ToString());
							involucrado.Direccion = reader["Direccion"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["Direccion"].ToString());
							involucrado.Municipio = reader["municipio"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["municipio"].ToString());
							involucrado.Telefono = reader["telefono"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["telefono"].ToString());
							involucrado.Correo = reader["correo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["correo"].ToString());
							involucrado.Entidad = reader["nombreEntidad"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["nombreEntidad"].ToString());
							involucrado.Modelo = reader["modelo"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["modelo"].ToString());
							involucrado.ConductorInvolucrado = reader["tipoInvolucrado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoInvolucrado"].ToString());
							involucrado.Cinturon = reader["cinturon"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["cinturon"].ToString());
							involucrado.NumeroEconomico = reader["cinturon"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["cinturon"].ToString());
							involucrado.NoAccidente = reader["NoAccidente"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["NoAccidente"].ToString());

							involucrado.FormatDateNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["fechaNacimiento"].ToString());

                            
                            if (reader["fechaIngreso"] != System.DBNull.Value)
                            {
                                involucrado.FechaIngreso = Convert.ToDateTime(reader["fechaIngreso"].ToString());
                            }
                            else
                            {
                                involucrado.FechaIngreso = default(DateTime);
                            }
                            if (reader["horaIngreso"] != System.DBNull.Value)
                            {
                                involucrado.HoraIngreso = reader.GetTimeSpan(reader.GetOrdinal("horaIngreso"));
                            }
                            else
                            {
                                involucrado.HoraIngreso = default(TimeSpan);
                            }


                            ListaInvolucrados.Add(involucrado);

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
            return ListaInvolucrados;


        }
        public int AgregarFechaHoraIngreso(FechaHoraIngresoModel model,int idAccidente)

        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("UPDATE involucradosAccidente SET fechaIngreso = @fechaIngreso, horaIngreso = @horaIngreso WHERE idAccidente = @idAccidente AND idPersona = @idPersona;",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaIngreso", SqlDbType.DateTime)).Value = (object)model.FechaIngreso ?? DBNull.Value;
                    sqlCommand.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
                    sqlCommand.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = model.IdPersona;
                    sqlCommand.Parameters.Add(new SqlParameter("@horaIngreso", SqlDbType.Time)).Value = (object)model.HoraIngreso ?? DBNull.Value;

                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

        public int AgregarDatosFinales(DatosAccidenteModel datosAccidente, int armasValue, int drogasValue, int valoresValue, int prendasValue, int otrosValue, int idAccidente, int convenioValue)
        {
            int result = 0;
            string qryUpdate = "";
            qryUpdate += !datosAccidente.montoCamino.Equals(null) ? " , montoCamino = @MontoCamino " : "";
            qryUpdate += !datosAccidente.montoCarga.Equals(null) ? " , montoCarga = @MontoCarga " : "";
            qryUpdate += !datosAccidente.montoPropietarios.Equals(null) ? " , montoPropietarios = @MontoPropietarios " : "";
            qryUpdate += !datosAccidente.montoOtros.Equals(null) ? " , montoOtros = @MontoOtros " : "";
            qryUpdate += !datosAccidente.Latitud.Equals(null) ? " , latitud = @Latitud " : "";
            qryUpdate += !datosAccidente.Longitud.Equals(null) ? " , longitud = @Longitud " : "";
            qryUpdate += !datosAccidente.IdCertificado.Equals(null) ? " , idCertificado = @IdCertificado " : "";
            qryUpdate += !convenioValue.Equals(null) ? " , convenio = @convenioValue " : "";
            qryUpdate += !armasValue.Equals(null) ? " , armas = @armasValue " : "";
            qryUpdate += !drogasValue.Equals(null) ? " , drogas = @drogasValue " : "";
            qryUpdate += !valoresValue.Equals(null) ? " , valores = @valoresValue " : "";
            qryUpdate += !prendasValue.Equals(null) ? " , prendas = @prendasValue " : "";
            qryUpdate += !otrosValue.Equals(null) ? " , otros = @otrosValue " : ""; 
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.entregaObjetos) ? " , entregaObjetos = @entregaObjetos " : ""; 
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.entregaOtros) ? " , entregaOtros = @entregaOtros " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.consignacionHechos) ? " , consignacionHechos = @consignacionHechos " : "";
            qryUpdate += !datosAccidente.IdCiudad.Equals(null) && datosAccidente.IdCiudad > 0 ? " , idCiudad = @idCiudad " : "";
            qryUpdate += !datosAccidente.IdAutoridadEntrega.Equals(null) ? " , idAutoridadEntrega = @IdAutoridadEntrega " : "";
            qryUpdate += !datosAccidente.IdAutoridadDisposicion.Equals(null) ? " , idAutoridadDisposicion = @IdAutoridadDisposicion " : "";
            qryUpdate += !datosAccidente.IdElaboraConsignacion.Equals(null) ? " , idElaboraConsignacion = @IdElaboraConsignacion " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.numeroOficio) ? " , numeroOficio = @numeroOficio " : "";
            qryUpdate += !datosAccidente.IdAgenciaMinisterio.Equals(null) ? " , idAgenciaMinisterio = @IdAgenciaMinisterio " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.RecibeMinisterio) ? " , recibeMinisterio = @RecibeMinisterio " : "";
            qryUpdate += !datosAccidente.IdElabora.Equals(null) ? " , idElabora = @IdElabora " : "";
            qryUpdate += !datosAccidente.IdAutoriza.Equals(null) ? " , idAutoriza = @IdAutoriza " : "";
            qryUpdate += !datosAccidente.IdSupervisa.Equals(null) ? " , idSupervisa = @IdSupervisa " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.ArmasTexto) ? " , armasTexto = @armasTexto " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.DrogasTexto) ? " , drogasTexto = @drogasTexto " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.ValoresTexto) ? " , valoresTexto = @valoresTexto " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.PrendasTexto) ? " , prendasTexto = @prendasTexto " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.OtrosTexto) ? " , otrosTexto = @otrosTexto " : "";

            qryUpdate += !string.IsNullOrEmpty(datosAccidente.observacionesConvenio) ? " , observacionesConvenio = @observacionesConvenio " : "";

            qryUpdate += !datosAccidente.IdEntidadCompetencia.Equals(null) ? " , idEntidadCompetencia = @IdEntidadCompetencia " : "";


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = @"UPDATE accidentes SET idEstatusReporte = @idEstatusReporte "+qryUpdate+" WHERE idAccidente = @IdAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@MontoCamino", datosAccidente.montoCamino);
                    command.Parameters.AddWithValue("@MontoCarga", datosAccidente.montoCarga);
                    command.Parameters.AddWithValue("@MontoPropietarios", datosAccidente.montoPropietarios);
                    command.Parameters.AddWithValue("@MontoOtros", datosAccidente.montoOtros);
                    command.Parameters.AddWithValue("@Latitud", datosAccidente.Latitud);
                    command.Parameters.AddWithValue("@Longitud", datosAccidente.Longitud);
                    command.Parameters.AddWithValue("@IdCertificado", datosAccidente.IdCertificado);
					command.Parameters.Add(new SqlParameter("@convenioValue", SqlDbType.Bit)).Value = (object)convenioValue ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@armasValue", SqlDbType.Bit)).Value = (object)armasValue ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@drogasValue", SqlDbType.Bit)).Value = (object)drogasValue ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@valoresValue", SqlDbType.Bit)).Value = (object)valoresValue ?? DBNull.Value;
					command.Parameters.Add(new SqlParameter("@prendasValue", SqlDbType.Bit)).Value = (object)prendasValue ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@otrosValue", SqlDbType.Bit)).Value = (object)otrosValue ?? DBNull.Value;
                   
                    if (!string.IsNullOrEmpty(datosAccidente.ArmasTexto))
                        command.Parameters.Add(new SqlParameter("@armasTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.ArmasTexto ?? DBNull.Value;
                    
                    if (!string.IsNullOrEmpty(datosAccidente.DrogasTexto))
                        command.Parameters.Add(new SqlParameter("@drogasTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.DrogasTexto ?? DBNull.Value;
                    
                    if (!string.IsNullOrEmpty(datosAccidente.PrendasTexto))
                        command.Parameters.Add(new SqlParameter("@prendasTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.PrendasTexto ?? DBNull.Value;
                   
                    if (!string.IsNullOrEmpty(datosAccidente.ValoresTexto))
                        command.Parameters.Add(new SqlParameter("@valoresTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.ValoresTexto ?? DBNull.Value;
                   
                    if (!string.IsNullOrEmpty(datosAccidente.OtrosTexto))
                        command.Parameters.Add(new SqlParameter("@otrosTexto", SqlDbType.NVarChar)).Value = (object)datosAccidente.OtrosTexto ?? DBNull.Value;
                    
                    if (!string.IsNullOrEmpty(datosAccidente.observacionesConvenio))
                        command.Parameters.Add(new SqlParameter("@observacionesConvenio", SqlDbType.NVarChar)).Value = (object)datosAccidente.observacionesConvenio ?? DBNull.Value;


                    if (!string.IsNullOrEmpty(datosAccidente.entregaObjetos))
                        command.Parameters.Add(new SqlParameter("@entregaObjetos", SqlDbType.NVarChar)).Value = (object)datosAccidente.entregaObjetos ?? DBNull.Value;

                    if (!string.IsNullOrEmpty(datosAccidente.entregaObjetos))
                        command.Parameters.Add(new SqlParameter("@entregaOtros", SqlDbType.NVarChar)).Value = (object)datosAccidente.entregaOtros ?? DBNull.Value;
                    
                    if (!string.IsNullOrEmpty(datosAccidente.consignacionHechos))
                        command.Parameters.Add(new SqlParameter("@consignacionHechos", SqlDbType.NVarChar)).Value = (object)datosAccidente.consignacionHechos ?? DBNull.Value;
                    
                    if(datosAccidente.IdCiudad > 0)
                        command.Parameters.AddWithValue("@idCiudad", datosAccidente.IdCiudad);
                    
                    command.Parameters.AddWithValue("@IdAutoridadEntrega", datosAccidente.IdAutoridadEntrega);
                    command.Parameters.AddWithValue("@IdAutoridadDisposicion", datosAccidente.IdAutoridadDisposicion);
                    command.Parameters.AddWithValue("@IdElaboraConsignacion", datosAccidente.IdElaboraConsignacion); 

                    if (!string.IsNullOrEmpty(datosAccidente.numeroOficio))
                        command.Parameters.Add(new SqlParameter("@numeroOficio", SqlDbType.NVarChar)).Value = (object)datosAccidente.numeroOficio ?? DBNull.Value;

                    command.Parameters.AddWithValue("@IdAgenciaMinisterio", datosAccidente.IdAgenciaMinisterio);

                    if (!string.IsNullOrEmpty(datosAccidente.RecibeMinisterio))
                        command.Parameters.Add(new SqlParameter("@RecibeMinisterio", SqlDbType.NVarChar)).Value = (object)datosAccidente.RecibeMinisterio ?? DBNull.Value;

                    command.Parameters.AddWithValue("@IdElabora", datosAccidente.IdElabora);
                    command.Parameters.AddWithValue("@IdAutoriza", datosAccidente.IdAutoriza);
                    command.Parameters.AddWithValue("@IdSupervisa", datosAccidente.IdSupervisa);
                    command.Parameters.AddWithValue("@IdEntidadCompetencia", datosAccidente.IdEntidadCompetencia);
                    command.Parameters.AddWithValue("@idEstatusReporte", 3);
                    
                    result = command.ExecuteNonQuery();
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
        public int RegistrarInfraccion(NuevaInfraccionModel model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO infracciones
                                            (fechaInfraccion
                                            ,folioInfraccion
                                            ,idOficial
                                            ,idMunicipio
                                            ,idCarretera
                                            ,idTramo
                                            ,kmCarretera
                                            ,idVehiculo
                                            ,idPersona
                                            ,idPersonaInfraccion
                                            ,placasVehiculo
                                            ,NumTarjetaCirculacion
                                            ,idEstatusInfraccion
                                            ,fechaActualizacion
                                            ,actualizadoPor
                                            ,estatus)
                                     VALUES (@fechaInfraccion
                                            ,@folioInfraccion
                                            ,@idOficial
                                            ,@idMunicipio
                                            ,@idCarretera
                                            ,@idTramo
                                            ,@kmCarretera
                                            ,@idVehiculo
                                            ,@idPersona
                                            ,@idPersonaInfraccion
                                            ,@placasVehiculo
                                            ,@NumTarjetaCirculacion
                                            ,1
                                            ,@fechaActualizacion
                                            ,@actualizadoPor
                                            ,@estatus);SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion ?? "-";
                    command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? 0;
                    command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? 0;
                    command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera ?? 0;
                    command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.IdTramo ?? 0;
                    command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.Float)).Value = (object)model.Kilometro ?? 0;
                    command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.IdVehiculo ?? 0;
                    command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.IdPersona ?? 0;
                    command.Parameters.Add(new SqlParameter("idPersonaInfraccion", SqlDbType.Int)).Value = (object)model.idPersonaInfraccion ?? 0;
                    command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.NVarChar)).Value = (object)model.Placa ?? "-";
                    command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.NVarChar)).Value = (object)model.Tarjeta ?? "-";



                    command.Parameters.Add(new SqlParameter("fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("estatus", SqlDbType.Int)).Value = (object)1;


                    result = Convert.ToInt32(command.ExecuteScalar());
                }
                catch (SqlException ex)
                {
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

        public string ObtenerDescripcionCausaDesdeBD(int idAccidente)
        {
            string descripcionCausa = null;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                string query = "SELECT descripcionCausas FROM accidentes WHERE idAccidente = @idAccidente"; 
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            descripcionCausa = reader["descripcionCausas"].ToString();
                        }

                        reader.Close();
                    }
                }

                return descripcionCausa;
            }
        }


        public DatosAccidenteModel ObtenerDatosFinales(int idAccidente)
        {

            DatosAccidenteModel datosFinales = new DatosAccidenteModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                string query = @"SELECT idEstatusReporte,
                                    montoCamino, montoCarga, montoPropietarios, montoOtros,
                                    latitud ,longitud ,idCertificado ,
                                    armas ,drogas ,valores ,prendas ,otros ,entregaObjetos, entregaOtros ,
                                    consignacionHechos, idCiudad , convenio ,
                                    idAutoridadEntrega , idAutoridadDisposicion , idElaboraConsignacion , 
                                    numeroOficio , idAgenciaMinisterio ,recibeMinisterio , 
                                    idElabora , idAutoriza , idSupervisa,armasTexto,drogasTexto,valoresTexto,prendasTexto,otrosTexto,
                                    observacionesConvenio, idEntidadCompetencia
                                    FROM accidentes a 
                                    WHERE idAccidente = @IdAccidente
                                    ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            datosFinales.montoCamino = reader["montoCamino"] == DBNull.Value ? "": reader["montoCamino"].ToString();
                            datosFinales.montoCarga = reader["montoCarga"] == DBNull.Value ? "" : reader["montoCarga"].ToString();
                            datosFinales.montoPropietarios = reader["montoPropietarios"] == DBNull.Value ? "" : reader["montoPropietarios"].ToString();
                            datosFinales.montoOtros = reader["montoOtros"] == DBNull.Value ? "" : reader["montoOtros"].ToString();
                            datosFinales.Latitud = reader["latitud"] == DBNull.Value ? 0 : float.Parse(reader["latitud"].ToString());
                            datosFinales.Longitud = reader["longitud"] == DBNull.Value ? 0 : float.Parse(reader["longitud"].ToString());
                            datosFinales.IdCertificado = reader["idCertificado"] == DBNull.Value ? 0 : int.Parse(reader["idCertificado"].ToString());
                            datosFinales.EstadoArmas = reader["armas"] == DBNull.Value ? 0 : int.Parse(reader["armas"].ToString()); 
                            datosFinales.EstadoDrogas = reader["drogas"] == DBNull.Value ? 0 : int.Parse(reader["drogas"].ToString());
                            datosFinales.EstadoValores = reader["valores"] == DBNull.Value ? 0 : int.Parse(reader["valores"].ToString());
                            datosFinales.EstadoPrendas = reader["prendas"] == DBNull.Value ? 0 : int.Parse(reader["prendas"].ToString());
                            datosFinales.EstadoOtros = reader["otros"] == DBNull.Value ? 0 : int.Parse(reader["otros"].ToString());
                            datosFinales.EstadoConvenio = reader["convenio"] == DBNull.Value ? 0 : int.Parse(reader["convenio"].ToString());
                            datosFinales.consignacionHechos = reader["consignacionHechos"] == DBNull.Value ? "" : reader["consignacionHechos"].ToString();
                            datosFinales.IdCiudad = reader["idCiudad"] == DBNull.Value ? 0 : int.Parse(reader["idCiudad"].ToString());
                            datosFinales.entregaObjetos = reader["entregaObjetos"] == DBNull.Value ? "" : reader["entregaObjetos"].ToString();
                            datosFinales.entregaOtros = reader["entregaOtros"] == DBNull.Value ? "" : reader["entregaOtros"].ToString();
                            datosFinales.IdAutoridadEntrega = reader["idAutoridadEntrega"] == DBNull.Value ? 0 : int.Parse(reader["idAutoridadEntrega"].ToString());
                            datosFinales.IdAutoridadDisposicion = reader["idAutoridadDisposicion"] == DBNull.Value ? 0 : int.Parse(reader["idAutoridadDisposicion"].ToString());
                            datosFinales.IdElaboraConsignacion = reader["idElaboraConsignacion"] == DBNull.Value ? 0 : int.Parse(reader["idElaboraConsignacion"].ToString());
                            datosFinales.numeroOficio = reader["numeroOficio"] == DBNull.Value ?"" : reader["numeroOficio"].ToString();
                            datosFinales.IdAgenciaMinisterio = reader["idAgenciaMinisterio"] == DBNull.Value ? 0 : int.Parse(reader["idAgenciaMinisterio"].ToString());
                            datosFinales.RecibeMinisterio = reader["recibeMinisterio"] == DBNull.Value ? "" : reader["recibeMinisterio"].ToString();
                            datosFinales.IdElabora = reader["idElabora"] == DBNull.Value ? 0 : int.Parse(reader["idElabora"].ToString());
                            datosFinales.IdAutoriza = reader["idAutoriza"] == DBNull.Value ? 0 : int.Parse(reader["idAutoriza"].ToString());
                            datosFinales.IdSupervisa = reader["idSupervisa"] == DBNull.Value ? 0 : int.Parse(reader["idSupervisa"].ToString());
                            datosFinales.IdEstatusReporte = reader["idEstatusReporte"] == DBNull.Value ? 0 : int.Parse(reader["idEstatusReporte"].ToString());
                            datosFinales.ArmasTexto = reader["armasTexto"] == DBNull.Value ? "" : reader["armasTexto"].ToString();
                            datosFinales.DrogasTexto = reader["drogasTexto"] == DBNull.Value ? "" : reader["drogasTexto"].ToString();
                            datosFinales.ValoresTexto = reader["valoresTexto"] == DBNull.Value ? "" : reader["valoresTexto"].ToString();
                            datosFinales.PrendasTexto = reader["prendasTexto"] == DBNull.Value ? "" : reader["prendasTexto"].ToString();
                            datosFinales.OtrosTexto = reader["otrosTexto"] == DBNull.Value ? "" : reader["otrosTexto"].ToString();
                            datosFinales.observacionesConvenio = reader["observacionesConvenio"] == DBNull.Value ? "" : reader["observacionesConvenio"].ToString();
                            datosFinales.IdEntidadCompetencia = reader["idEntidadCompetencia"] == DBNull.Value ? 0 : int.Parse(reader["idEntidadCompetencia"].ToString());
						}


                        reader.Close();
                    }
                }

                return datosFinales;
            } 
        }  
    }
}

















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
                    SqlCommand command = new SqlCommand("SELECT acc.*, mun.Municipio, car.Carretera, tra.Tramo, er.estatusReporte FROM accidentes AS acc " +
                        "LEFT JOIN catMunicipios AS mun ON acc.idMunicipio = mun.idMunicipio " +
                        "LEFT JOIN catCarreteras AS car ON acc.idCarretera = car.idCarretera " +
                        "LEFT JOIN catTramos AS tra ON acc.idTramo = tra.idTramo " +
                        "LEFT JOIN catEstatusReporteAccidente AS er ON acc.idEstatusReporte = er.idEstatusReporte " +
                        "WHERE acc.estatus = 1 AND acc.idOficinaDelegacion = @idOficina and acc.idEstatusReporte != 3;", connection);
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
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idCarretera, a.idTramo, a.kilometro,a.idClasificacionAccidente, " +
                        "                                a.idFactorAccidente, a.IdFactorOpcionAccidente,a.idOficinaDelegacion,m.municipio, c.carretera, t.tramo, e.estatusDesc " +
                                                        "FROM accidentes AS a JOIN catMunicipios AS m ON a.idMunicipio = m.idMunicipio " +
                                                        "JOIN catCarreteras AS c ON a.idCarretera = c.idCarretera " +
                                                        "JOIN catTramos AS t ON a.idTramo = t.idTramo " +
                                                        "JOIN estatus AS e ON a.estatus = e.estatus " +
                                                        "WHERE a.idAccidente = @idAccidente AND a.estatus = 1 AND a.idOficinaDelegacion = @idOficina", connection);
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
                            accidente.Municipio = reader["Municipio"].ToString();
                            accidente.Tramo = reader["Tramo"].ToString();
                            accidente.Carretera = reader["Carretera"].ToString();
                            accidente.Kilometro = reader["Kilometro"].ToString();
                            accidente.IdTramo = Convert.ToInt32(reader["IdTramo"].ToString());




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
                        "JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                        "JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                        "JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                        "JOIN catColores cc ON v.idColor = cc.idColor " +
                        "JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                        "JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                        "JOIN personas p ON v.idPersona = p.idPersona " +
                        "WHERE v.estatus = 1 AND v.serie LIKE '%' + @Serie + '%';", connection);

                        command.Parameters.AddWithValue("@Serie", Serie);
                    }
                    else if (!string.IsNullOrEmpty(Placa))
                    {
                        command = new SqlCommand("SELECT v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio,p.nombre, p.apellidoPaterno, p.apellidoMaterno " +
                            "FROM vehiculos v " +
                            "JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                            "JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                            "JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                            "JOIN catColores cc ON v.idColor = cc.idColor " +
                            "JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                            "JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                            "JOIN personas p ON v.idPersona = p.idPersona " +
                            "WHERE v.estatus = 1 AND v.placas LIKE '%' + @Placa + '%';", connection);
                        command.Parameters.AddWithValue("@Placa", Placa);
                    }
                    else if (!string.IsNullOrEmpty(Folio))
                    {
                        command = new SqlCommand("SELECT v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio,p.nombre, p.apellidoPaterno, p.apellidoMaterno " + 
                            "FROM vehiculos v " +
                            "JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo" +      
                            "JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                            "JOIN catEntidades e ON v.idEntidad = e.idEntidad " + 
                            "JOIN catColores cc ON v.idColor = cc.idColor " +
                            "JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +    
                            "JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
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
                        "p.apellidoMaterno,p.RFC,p.CURP, p.fechaNacimiento, c.color, ts.tipoServicio, pcv.nombre AS nombreConductor, pcv.apellidoPaterno AS apellidoPConductor, pcv.apellidoMaterno AS apellidoMConductor, " +
                        "tc.tipoCarga, pen.pension, ft.formaTraslado, cent.nombreEntidad,va.montoVehiculo " +
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
                        "WHERE cva.idAccidente = @idAccidente AND cva.idPersona = @idPersona AND cva.idVehiculo = @IdVehiculoInvolucrado AND cva.idAccidente > 0;",connection);

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
                            involucrado.rfc = reader["RFC"] != DBNull.Value ? reader["RFC"].ToString() : string.Empty;
                            involucrado.curp = reader["CURP"] != DBNull.Value ? reader["CURP"].ToString() : string.Empty;
                            involucrado.TipoServicio = reader["tipoServicio"] != DBNull.Value ? reader["tipoServicio"].ToString() : string.Empty;



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
                                            @"SELECT p.*, ctp.tipoPersona,v.idVehiculo
                                            FROM personas AS p                                           
                                            INNER JOIN catTipoPersona AS ctp ON p.idCatTipoPersona = ctp.idCatTipoPersona
                                            INNER JOIN vehiculos AS v ON p.idPersona = v.idPersona
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
                            model.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            model.rfc = reader["rfc"].ToString();
                            model.curp = reader["curp"].ToString();
                            model.TipoPersona = reader["tipoPersona"].ToString();
                            model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());

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
                    string query = "UPDATE accidentes SET idFactorAccidente = @IdFactorAccidente, idFactorOpcionAccidente = @IdFactorOpcionAccidente WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@IdFactorAccidente", IdFactorAccidente);
                    command.Parameters.AddWithValue("@IdFactorOpcionAccidente", IdFactorOpcionAccidente);
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
        public int EliminarValorFactorYOpcion(int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidentes SET idFactorAccidente = 0, idFactorOpcionAccidente = 0 WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

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

        public List<CapturaAccidentesModel> ObtenerDatosGridFactor(int idAccidente)
        {
            //
            List<CapturaAccidentesModel> ListaGridFactor = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.*, fa.factorAccidente, op.factorOpcionAccidente FROM accidentes a JOIN catFactoresAccidentes fa ON a.idFactorAccidente = fa.idFactorAccidente JOIN catFactoresOpcionesAccidentes op ON a.idFactorOpcionAccidente = op.idFactorOpcionAccidente WHERE a.idAccidente = @idAccidente AND a.idFactorAccidente > 0;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel factorOpcion = new CapturaAccidentesModel();
                            factorOpcion.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
                            factorOpcion.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            factorOpcion.IdFactorOpcionAccidente = Convert.ToInt32(reader["IdFactorOpcionAccidente"].ToString());
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
                    string query = "INSERT into accidenteCausas(idAccidente,idCausaAccidente) values(@idAccidente, @idCausaAccidente)";

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


        public int EditarValorCausa(int IdCausaAccidente, int idAccidente, int IdCausaAccidenteEdit)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidenteCausas SET idCausaAccidente = @idCausaAccidenteEdit WHERE idAccidente = @idAccidente AND idCausaAccidente = @idCausaAccidente ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idCausaAccidenteEdit", IdCausaAccidenteEdit);
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
        public int EliminarCausaBD(int IdCausaAccidente, int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidenteCausas SET idCausaAccidente = 0 WHERE idAccidente = @idAccidente AND idCausaAccidente = @idCausaAccidente ";

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
        public int GuardarDescripcion(int idAccidente, string descripcionCausa)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidentes SET descripcionCausas = @DescripcionCausa, idEstatusReporte = @idEstatusReporte WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@DescripcionCausa", descripcionCausa);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idEstatusReporte", 2);

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
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ac.*, c.causaAccidente FROM accidenteCausas ac JOIN catCausasAccidentes c ON ac.idCausaAccidente = c.idCausaAccidente WHERE ac.idAccidente = @idAccidente AND ac.idCausaAccidente > 0;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel causa = new CapturaAccidentesModel();
                            causa.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
                            causa.IdCausaAccidente = Convert.ToInt32(reader["IdCausaAccidente"].ToString());
                            causa.CausaAccidente = reader["causaAccidente"].ToString();

                            ListaGridCausa.Add(causa);

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
        public List<CapturaAccidentesModel> BusquedaPersonaInvolucrada(BusquedaInvolucradoModel model)
        {
            //
            List<CapturaAccidentesModel> ListaInvolucrados = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();


                    const string SqlTransact = @"SELECT * FROM personas WHERE (numeroLicencia LIKE '%' + @numeroLicencia + '%' OR curp LIKE '%' + @curp " +
                                                "OR rfc LIKE '%' + @rfc + '%' OR nombre LIKE '%' + @nombre OR apellidoPaterno LIKE '%' + @apellidoPaterno + '%' OR apellidoMaterno LIKE '%' + @apellidoMaterno) " +
                                                "AND estatus = 1;";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.licencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@curp", SqlDbType.NVarChar)).Value = (object)model.curpBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@rfc", SqlDbType.NVarChar)).Value = (object)model.rfcBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar)).Value = (object)model.nombre ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoPaterno ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoMaterno ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
                            involucrado.idPersonaInvolucrado = Convert.ToInt32(reader["idPersona"].ToString());
                            involucrado.nombre = reader["nombre"].ToString();
                            involucrado.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            involucrado.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            involucrado.rfc = reader["rfc"].ToString();
                            involucrado.curp = reader["curp"].ToString();
                            involucrado.licencia = reader["numeroLicencia"].ToString();

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
                    SqlCommand command = new SqlCommand("SELECT cva.*, COALESCE(cva.idPersona, pcv.idPersona) AS idConductor,cva.idTipoCarga,cva.poliza,ctc.tipoCarga,v.placas, v.tarjeta, v.serie, v.idMarcaVehiculo, " +
                        "v.idSubmarca,v.idEntidad, v.idTipoVehiculo,acc.numeroReporte,v.idPersona AS idPropietario, v.modelo, v.idColor, v.idCatTipoServicio, v.motor, v.capacidad, " +
                        "cm.marcaVehiculo, csv.nombreSubmarca, tv.tipoVehiculo, COALESCE(p.nombre, pcv.nombre) AS nombre, COALESCE(p.apellidoPaterno, pcv.apellidoPaterno) AS apellidoPaterno, " +
                        "p.apellidoMaterno,p.RFC,p.CURP, p.fechaNacimiento, c.color, ts.tipoServicio, pcv.nombre AS nombreConductor, pcv.apellidoPaterno AS apellidoPConductor, pcv.apellidoMaterno AS apellidoMConductor, " +
                        "tc.tipoCarga, pen.pension, ft.formaTraslado, cent.nombreEntidad,va.montoVehiculo " +
                        "FROM conductoresVehiculosAccidente AS cva LEFT JOIN vehiculos AS v ON cva.idVehiculo = v.idVehiculo " +
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
                        "WHERE cva.idAccidente = @idAccidente AND cva.idAccidente > 0 AND cva.estatus = 1;", connection);

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
                            vehiculo.rfc = reader["RFC"].ToString();
                            vehiculo.curp = reader["CURP"].ToString();
                            vehiculo.TipoServicio = reader["tipoServicio"].ToString();
                            vehiculo.FormaTrasladoInvolucrado = reader["formaTraslado"].ToString();
                            vehiculo.ConductorInvolucrado = $"{reader["nombreConductor"]} {reader["apellidoPConductor"]} {reader["apellidoMConductor"]}";
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
                        SqlCommand("Update vehiculosAccidente set montoVehiculo = @montoVehiculo where idAccidente=@idAccidente AND idvehiculo = @idvehiculo AND idPersona = @idPersona",
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
                        "JOIN vehiculos AS v ON cva.idVehiculo = v.idVehiculo " +
                        "JOIN catEntidades AS ent ON v.idEntidad = ent.idEntidad " +
                        "JOIN infracciones AS i ON cva.idVehiculo = i.idVehiculo " +
                        "JOIN personas AS propietario ON v.idPersona = propietario.idPersona " +
                        "JOIN personas AS conductor ON i.idPersonaInfraccion= conductor.idPersona " +
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
                        "mv.marcaVehiculo, sv.nombreSubmarca " +
                        "FROM infraccionesAccidente AS ia LEFT JOIN vehiculos AS v ON ia.idVehiculo = v.idVehiculo " +
                        "LEFT JOIN accidentes AS a ON ia.idAccidente = a.idAccidente " +
                        "LEFT JOIN infracciones AS i ON ia.idInfraccion = i.idInfraccion " +
                        "LEFT JOIN catEstatusInfraccion AS cei ON cei.idEstatusInfraccion = i.idEstatusInfraccion " +
                        "LEFT JOIN catMarcasVehiculos AS mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                        "LEFT JOIN catSubmarcasVehiculos AS sv ON v.idSubmarca = sv.idSubmarca " +
                        "WHERE ia.idAccidente = @idAccidente;", connection);



                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel elemnto = new CapturaAccidentesModel();
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
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT " +
                                             "p.nombre, " +
                                             "p.apellidoPaterno, " +
                                             "p.apellidoMaterno, " +
                                             "p.rfc, " +
                                             "p.curp, " +
                                             "p.idTipoLicencia, " +
                                             "p.fechaNacimiento, " +
                                             "tl.tipoLicencia, " +
                                             "ia.idAccidente," +
                                             "ia.idPersona, " +
                                             "ia.idVehiculo, " +
                                             "v.idTipoVehiculo, " +
                                             "tv.tipoVehiculo, " +
                                             "ia.idEstadoVictima, " +
                                             "ev.estadoVictima, " +
                                             "ia.idInstitucionTraslado, " +
                                             "it.institucionTraslado, " +
                                             "ia.idHospital, " +
                                             "h.nombreHospital, " +
                                             "ia.idAsiento, " +
                                             "ia.fechaIngreso, "+
                                             "ia.horaIngreso, "+
                                             "ca.asiento, " +
                                             "ia.idCinturon, " +
                                             "cc.cinturon " +
                                             "FROM involucradosAccidente ia " +
                                             "LEFT JOIN personas p ON ia.idPersona = p.idPersona " +
                                             "LEFT JOIN catTipoLicencia tl ON p.idTipoLicencia = tl.idTipoLicencia " +
                                             "LEFT JOIN vehiculos v ON ia.idVehiculo = v.idVehiculo " +
                                             "LEFT JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                                             "LEFT JOIN catEstadoVictima ev ON ia.idEstadoVictima = ev.idEstadoVictima " +
                                             "LEFT JOIN catInstitucionesTraslado it ON ia.idInstitucionTraslado = it.idInstitucionTraslado " +
                                             "LEFT JOIN catHospitales h ON ia.idHospital = h.idHospital " +
                                             "LEFT JOIN catAsientos ca ON ia.idAsiento = ca.idAsiento " +
                                             "LEFT JOIN catCinturon cc ON ia.idCinturon = cc.idCinturon " +
                                             "WHERE ia.idAccidente = @idAccidente;", connection);



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
                            involucrado.nombre = reader["nombre"].ToString();
                            involucrado.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            involucrado.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            involucrado.rfc = reader["rfc"].ToString();
                            involucrado.curp = reader["curp"].ToString();
                            involucrado.TipoLicencia = reader["tipoLicencia"].ToString();
                            involucrado.TipoVehiculo= reader["tipoVehiculo"].ToString();
                            involucrado.EstadoVictima = reader["estadoVictima"].ToString();
                            involucrado.NombreHospital = reader["nombreHospital"].ToString();
                            involucrado.Asiento = reader["asiento"].ToString();
                            involucrado.InstitucionTraslado = reader["institucionTraslado"].ToString();

                            involucrado.Cinturon = reader["cinturon"].ToString();
                            involucrado.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
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

        public int AgregarDatosFinales(DatosAccidenteModel datosAccidente, int armasValue, int drogasValue, int valoresValue, int prendasValue, int otrosValue, int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidentes SET idEstatusReporte = @idEstatusReporte, montoCamino = @MontoCamino, montoCarga = @MontoCarga, montoPropietarios = @MontoPropietarios, montoOtros = @MontoOtros, latitud = @Latitud, longitud = @Longitud, idCertificado = @IdCertificado, armas = @armasValue, drogas = @drogasValue, valores = @valoresValue, prendas = @prendasValue, otros = @otrosValue, entregaObjetos = @entregaObjetos, entregaOtros = @entregaOtros, idAutoridadEntrega = @IdAutoridadEntrega, idAutoridadDisposicion = @IdAutoridadDisposicion, idElaboraConsignacion = @IdElaboraConsignacion, numeroOficio = @numeroOficio, idAgenciaMinisterio = @IdAgenciaMinisterio, recibeMinisterio = @RecibeMinisterio, idElabora = @IdElabora, idAutoriza = @IdAutoriza, idSupervisa = @IdSupervisa WHERE idAccidente = @IdAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@MontoCamino", datosAccidente.montoCamino);
                    command.Parameters.AddWithValue("@MontoCarga", datosAccidente.montoCarga);
                    command.Parameters.AddWithValue("@MontoPropietarios", datosAccidente.montoPropietarios);
                    command.Parameters.AddWithValue("@MontoOtros", datosAccidente.montoOtros);
                    command.Parameters.AddWithValue("@Latitud", datosAccidente.Latitud);
                    command.Parameters.AddWithValue("@Longitud", datosAccidente.Longitud);
                    command.Parameters.AddWithValue("@IdCertificado", datosAccidente.IdCertificado);
                    command.Parameters.AddWithValue("@armasValue", armasValue);
                    command.Parameters.AddWithValue("@drogasValue", drogasValue);
                    command.Parameters.AddWithValue("@valoresValue", valoresValue);
                    command.Parameters.AddWithValue("@prendasValue", prendasValue);
                    command.Parameters.AddWithValue("@otrosValue", otrosValue);
                    command.Parameters.AddWithValue("@entregaObjetos", datosAccidente.entregaObjetos);
                    command.Parameters.AddWithValue("@entregaOtros", datosAccidente.entregaOtros);
                    command.Parameters.AddWithValue("@IdAutoridadEntrega", datosAccidente.IdAutoridadEntrega);
                    command.Parameters.AddWithValue("@IdAutoridadDisposicion", datosAccidente.IdAutoridadDisposicion);
                    command.Parameters.AddWithValue("@IdElaboraConsignacion", datosAccidente.IdElaboraConsignacion);
                    command.Parameters.AddWithValue("@numeroOficio", datosAccidente.numeroOficio);
                    command.Parameters.AddWithValue("@IdAgenciaMinisterio", datosAccidente.IdAgenciaMinisterio);
                    command.Parameters.AddWithValue("@RecibeMinisterio", datosAccidente.RecibeMinisterio);
                    command.Parameters.AddWithValue("@IdElabora", datosAccidente.IdElabora);
                    command.Parameters.AddWithValue("@IdAutoriza", datosAccidente.IdAutoriza);
                    command.Parameters.AddWithValue("@IdSupervisa", datosAccidente.IdSupervisa);
                    command.Parameters.AddWithValue("@idEstatusReporte", 3);



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
                    command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion;
                    command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial;
                    command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio;

                    command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera;
                    command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.IdTramo;
                    command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.Int)).Value = (object)model.Kilometro;
                    
                    command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.IdVehiculo;
                    command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.IdPersona;
                    command.Parameters.Add(new SqlParameter("idPersonaInfraccion", SqlDbType.Int)).Value = (object)model.idPersonaInfraccion;
                    command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.NVarChar)).Value = (object)model.Placa;
                    command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.NVarChar)).Value = (object)model.Tarjeta;
                 

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


    }
}

















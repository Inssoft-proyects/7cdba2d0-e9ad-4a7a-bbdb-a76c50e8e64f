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

namespace GuanajuatoAdminUsuarios.Services
{

    public class CapturaAccidentesService : ICapturaAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CapturaAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CapturaAccidentesModel> ObtenerAccidentes()
        {
            //
            List<CapturaAccidentesModel> ListaAccidentes = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT acc.*, mun.Municipio, car.Carretera, tra.Tramo FROM accidentes AS acc INNER JOIN catMunicipios AS mun ON acc.idMunicipio = mun.idMunicipio INNER JOIN catCarreteras AS car ON acc.idCarretera = car.idCarretera INNER JOIN catTramos AS tra ON acc.idTramo = tra.idTramo;", connection);
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
                            accidente.EstatusReporte = Convert.ToInt32(reader["EstatusReporte"].ToString());
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

        public CapturaAccidentesModel ObtenerAccidentePorId(int idAccidente)
        {
            CapturaAccidentesModel accidente = new CapturaAccidentesModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT a.idAccidente, a.numeroReporte, a.fecha, a.hora, a.idMunicipio, a.idCarretera, a.idTramo, a.kilometro,a.idClasificacionAccidente, " +
                        "                                a.idFactorAccidente, a.IdFactorOpcionAccidente,a.IdCausaAccidente,m.municipio, c.carretera, t.tramo, e.estatusDesc " +
                                                        "FROM accidentes AS a JOIN catMunicipios AS m ON a.idMunicipio = m.idMunicipio " +
                                                        "JOIN catCarreteras AS c ON a.idCarretera = c.idCarretera " +
                                                        "JOIN catTramos AS t ON a.idTramo = t.idTramo " +
                                                        "JOIN estatus AS e ON a.estatus = e.estatus " +
                                                        "WHERE a.idAccidente = @idAccidente AND a.estatus = 1", connection);
                    command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
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
                            accidente.IdClasificacionAccidente = Convert.ToInt32(reader["IdClasificacionAccidente"].ToString());
                            accidente.IdCausaAccidente = Convert.ToInt32(reader["IdCausaAccidente"].ToString());
                            accidente.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            accidente.IdFactorOpcionAccidente = Convert.ToInt32(reader["IdFactorOpcionAccidente"].ToString());
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

        public int GuardarParte1(CapturaAccidentesModel model)
        {
            int result = 0;
            int lastInsertedId = 0;
            string strQuery = @"INSERT INTO accidentes ([EstatusReporte]
                                        ,[Hora]
                                        ,[idMunicipio]
                                        ,[idTramo]
                                        ,[Fecha]
                                        ,[idCarretera]
                                        ,[kilometro]
                                        ,[fechaActualizacion]
                                        ,[actualizadoPor]
                                        ,[estatus])
                                VALUES (@EstatusReporte
                                        ,@Hora
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
                    command.Parameters.Add(new SqlParameter("@kilometro", SqlDbType.NVarChar)).Value = (object)model.Kilometro ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@EstatusReporte", SqlDbType.Int)).Value = 1;
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
                           "SELECT v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio,p.nombre, p.apellidoPaterno, p.apellidoMaterno " +
                            "FROM vehiculos v " +
                            "JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                            "JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                            "JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                            "JOIN catColores cc ON v.idColor = cc.idColor " +
                            "JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                            "JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                            "JOIN personas p ON v.idPersona = p.idPersona " +
                            "WHERE v.estatus = 1 AND v.placas LIKE '%' + @Serie%';", connection);
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
                        command = new SqlCommand("SELECT * FROM vehiculos WHERE Folio = @Folio;", connection);
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
                            vehiculo.IdVehiculo = Convert.ToInt32(reader["IdVehiculo"].ToString());
                            vehiculo.IdMarcaVehiculo = Convert.ToInt32(reader["IdMarcaVehiculo"].ToString());
                            vehiculo.IdSubmarca = Convert.ToInt32(reader["IdSubmarca"].ToString());
                            vehiculo.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            vehiculo.IdColor = Convert.ToInt32(reader["IdColor"].ToString());
                            vehiculo.IdTipoVehiculo = Convert.ToInt32(reader["IdTipoVehiculo"].ToString());
                            vehiculo.IdCatTipoServicio = Convert.ToInt32(reader["IdCatTipoServicio"].ToString());
                            vehiculo.IdPersona = Convert.ToInt32(reader["IdPersona"].ToString());
                            vehiculo.Marca = reader["marcaVehiculo"].ToString();
                            vehiculo.Submarca = reader["nombreSubmarca"].ToString();
                            vehiculo.Modelo = reader["Modelo"].ToString();
                            vehiculo.Placa = reader["Placas"].ToString();
                            vehiculo.Tarjeta = reader["Tarjeta"].ToString();
                            vehiculo.VigenciaTarjeta = Convert.ToDateTime(reader["VigenciaTarjeta"].ToString());
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




        public int ActualizarConVehiculo(int idVehiculo, int idAccidente)
        {
            int idVehiculoInsertado = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO vehiculosAccidente (idAccidente, idVehiculo) OUTPUT INSERTED.idVehiculo VALUES (@idAccidente, @idVehiculo)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idVehiculo", idVehiculo);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

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
                    string query = "INSERT INTO conductoresVehiculosAccidente (idAccidente,idVehiculo, idPersona) values(@idAccidente, @idVehiculo,@idPersona) ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idPersona", IdPersona);


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
                    command.Parameters.AddWithValue("@idAccidente",IdAccidente);

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
                    string query = "UPDATE accidentes SET descripcionCausas = @DescripcionCausa WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@DescripcionCausa", descripcionCausa);
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
                    command.Parameters.Add(new SqlParameter("@curp", SqlDbType.NVarChar)).Value = (object)model.curp ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@rfc", SqlDbType.NVarChar)).Value = (object)model.rfc ?? DBNull.Value;
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
        public List<CapturaAccidentesModel> VehiculosInvolucrados(int IdAccidente)
        {
            //
            List<CapturaAccidentesModel> ListaVehiculosInvolucrados = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT cva.*, COALESCE(cva.idPersona, pcv.idPersona) AS idConductor, v.placas, v.tarjeta, v.serie, v.idMarcaVehiculo, " +
                        "v.idSubmarca, v.idTipoVehiculo, v.idPersona AS idPropietario, v.modelo, v.idColor, v.idCatTipoServicio, v.motor, v.capacidad, " +
                        "cm.marcaVehiculo, csv.nombreSubmarca, tv.tipoVehiculo, COALESCE(p.nombre, pcv.nombre) AS nombre, COALESCE(p.apellidoPaterno, pcv.apellidoPaterno) AS apellidoPaterno, " +
                        "p.apellidoMaterno, c.color, ts.tipoServicio, pcv.nombre AS nombreConductor, pcv.apellidoPaterno AS apellidoPConductor, pcv.apellidoMaterno AS apellidoMConductor, " +
                        "tc.tipoCarga, pen.pension, ft.formaTraslado " +
                        "FROM conductoresVehiculosAccidente AS cva INNER JOIN vehiculos AS v ON cva.idVehiculo = v.idVehiculo " +
                        "INNER JOIN catMarcasVehiculos AS cm ON v.idMarcaVehiculo = cm.idMarcaVehiculo " +
                        "INNER JOIN catSubmarcasVehiculos AS csv ON v.idSubmarca = csv.idSubmarca " +
                        "INNER JOIN catTiposVehiculo AS tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                        "INNER JOIN personas AS p ON v.idPersona = p.idPersona " +
                        "INNER JOIN catColores AS c ON v.idColor = c.idColor " +
                        "INNER JOIN catTiposcarga AS tc ON cva.idTipoCarga = tc.idTipoCarga " +
                        "INNER JOIN pensiones AS pen ON cva.idPension = pen.idPension " +
                        "INNER JOIN catFormasTraslado AS ft ON cva.idFormaTraslado = ft.idFormaTraslado " +
                        "INNER JOIN catTipoServicio AS ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                        "LEFT JOIN personas AS pcv ON cva.idPersona = pcv.idPersona " +
                        "WHERE idAccidente = @idAccidente AND idAccidente > 0;", connection);

                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", IdAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel vehiculo = new CapturaAccidentesModel();
                            vehiculo.IdPropietarioInvolucrado = Convert.ToInt32(reader["IdPersona"].ToString());
                            vehiculo.IdAccidente = Convert.ToInt32(reader["IdAccidente"].ToString());
                            vehiculo.IdVehiculoInvolucrado = Convert.ToInt32(reader["idVehiculo"].ToString());
                            vehiculo.IdTipoCarga = Convert.ToInt32(reader["IdTipoCarga"].ToString());
                            vehiculo.IdPension = Convert.ToInt32(reader["IdPension"].ToString());
                            vehiculo.IdFormaTrasladoInvolucrado = Convert.ToInt32(reader["idFormaTraslado"].ToString());
                            vehiculo.idPersonaInvolucrado = Convert.ToInt32(reader["IdConductor"].ToString());
                            vehiculo.Placa = reader["placas"].ToString();
                            vehiculo.Tarjeta = reader["tarjeta"].ToString();
                            vehiculo.Serie = reader["serie"].ToString();
                            vehiculo.Marca = reader["marcaVehiculo"].ToString();
                            vehiculo.Submarca = reader["nombreSubmarca"].ToString();
                            vehiculo.TipoVehiculo = reader["tipoVehiculo"].ToString();
                            vehiculo.PropietarioInvolucrado = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            vehiculo.Modelo = reader["modelo"].ToString();
                            vehiculo.Motor = reader["motor"].ToString();
                            vehiculo.Capacidad = reader["capacidad"].ToString();
                            vehiculo.Pension = reader["pension"].ToString();
                            vehiculo.Modelo = reader["modelo"].ToString();
                            vehiculo.Color = reader["color"].ToString();
                            vehiculo.TipoServicio = reader["tipoServicio"].ToString();
                            vehiculo.FormaTrasladoInvolucrado = reader["formaTraslado"].ToString();
                            vehiculo.ConductorInvolucrado = $"{reader["nombreConductor"]} {reader["apellidoPConductor"]} {reader["apellidoMConductor"]}";


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


    }
}
        













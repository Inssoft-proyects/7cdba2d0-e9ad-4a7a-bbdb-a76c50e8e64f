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
                            "WHERE v.estatus = 1 and v.serie = @Serie;", connection);
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
                            "WHERE v.estatus = 1 and v.placas = @Placa;", connection);
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




        public int ActualizarConVehiculo(int IdVehiculo, int idAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE accidentes SET idVehiculo = @IdVehiculo WHERE idAccidente = @idAccidente";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
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


    }
}
        













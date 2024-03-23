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
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Linq;

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
                    //command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;
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
                        left JOIN catMunicipios AS m ON a.idMunicipio = m.idMunicipio
                        left JOIN catDelegacionesOficinasTransporte as d on d.idOficinaTransporte= a.idOficinaDelegacion
                        left JOIN catCarreteras AS c ON a.idCarretera = c.idCarretera 
                        left JOIN catTramos AS t ON a.idTramo = t.idTramo 
                        left JOIN estatus AS e ON a.estatus = e.estatus 
                        LEFT JOIN accidenteCausas AS ac ON ac.idAccidente = a.idAccidente 
                        WHERE a.idAccidente = @idAccidente AND a.estatus = 1
                    ", connection);
                    command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.Int)).Value = idAccidente;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;
                   // command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;

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
                            
                            var valueDec = 0;
                            var km = reader["kilometro"] == System.DBNull.Value ? default(string) : Decimal.Parse((string)reader["kilometro"]).ToString("G29");
                            //if (reader["Kilometro"].ToString().Split('.').Length>1)
                            //    valueDec = Convert.ToInt32(reader["Kilometro"].ToString().Split('.')[1]);

                            //if (valueDec > 0)
                            //    km = reader["Kilometro"].ToString().Split('.')[0] + "." + valueDec.ToString();
                            //else
                            //    km = reader["Kilometro"].ToString().Split('.')[0];

							accidente.Kilometro = km;
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






        public int GuardarParte1(CapturaAccidentesModel model,int idOficina,string abreviaturaMunicipio, int anio, string oficina = "NRA")
        
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
                                        ,[estatus]
                                        )
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
                                        ,@estatus
                                         );
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
                   // command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = (object)idDependencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@kilometro", SqlDbType.NVarChar)).Value = (object)model.Kilometro ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEstatusReporte", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = (object)model.IdTramo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Fecha", SqlDbType.DateTime)).Value = (object)model.Fecha ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    result = Convert.ToInt32(command.ExecuteScalar());
                    lastInsertedId = result;
                    //Se busca el ultimo consecutivo
                    command = new SqlCommand("select id,consecutivo from foliosAccidentes where abreviaturaMunicipio =@abreviaturaMunicipio and anio=@anio and idDelegacion=@idDelegacion", connection)
                    {
                        CommandType = CommandType.Text
                    };
                    command.Parameters.Add(new SqlParameter("@abreviaturaMunicipio", SqlDbType.VarChar)).Value = abreviaturaMunicipio;
                    command.Parameters.Add(new SqlParameter("@anio", SqlDbType.VarChar)).Value = anio;
                    command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.VarChar)).Value = idOficina;

                    int consecutivo = -1;
                    int idFolioSolicitud = -1;


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Intenta leer un registro del resultado
                        {
                            idFolioSolicitud = reader["id"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["id"]);
                            consecutivo = reader["consecutivo"] == System.DBNull.Value ? -1 : Convert.ToInt32(reader["consecutivo"]);
                        }
                    }

                    if (consecutivo == -1)
                        throw new Exception("No se pudo crear el folio ya que no se encontraron registros con los datos de usuario");

                    //Se incrementa en 1 el consecutivo
                    consecutivo++;

					command = new SqlCommand("select abreviatura from catMunicipios where idMunicipio =@idMunicipio", connection)
					{
						CommandType = CommandType.Text
					};
					command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.VarChar)).Value = model.IdMunicipio;
                    string abreviaturaMun = "";

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read()) // Intenta leer un registro del resultado
						{
							abreviaturaMun = reader["abreviatura"].ToString();

						}
					}
					//Se completa con ceros a la izquierda
					string consecutivoConCeros = consecutivo.ToString("D5");
                    string anio2 = "/" + (anio % 100);

                    string newFolio = $"{abreviaturaMun}{consecutivoConCeros}{anio2}";

                    //Se actualiza el consecutivo en la tabla de foliosSolicitud
                    command = new SqlCommand(@"update foliosAccidentes set consecutivo=@consecutivo where id=@id", connection);
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = idFolioSolicitud;
                    command.Parameters.Add(new SqlParameter("@consecutivo", SqlDbType.Int)).Value = consecutivo;
                    command.CommandType = CommandType.Text;
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected <= 0)
                        throw new Exception("No se pudo crear el folio.");



                    SqlCommand command2 = new SqlCommand(@"
                            update accidentes set numeroReporte=@folio where idAccidente=@id
                                        ", connection);
                    command2.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = (object)lastInsertedId ?? DBNull.Value;
                    command2.Parameters.Add(new SqlParameter("@folio", SqlDbType.VarChar)).Value = (object)newFolio ?? DBNull.Value;
                    command2.CommandType = CommandType.Text;
                    rowsAffected = command2.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return lastInsertedId;
                    }
                    else
                    {
                        throw new Exception("No se pudo crear el folio.");
                    }
                }
                catch (SqlException ex)
                {

                    Console.WriteLine("Error de SQL: " + ex.Message);
                    return lastInsertedId;
                }
                finally
                {
                    connection.Close();
                }
            }
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
                        "SELECT TOP 200 v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio, " +
                        "p.CURP, p.RFC, p.nombre, p.apellidoPaterno, p.apellidoMaterno " +
                        "FROM vehiculos v " +
                        "left JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                        "left JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                        "left JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                        "left JOIN catColores cc ON v.idColor = cc.idColor " +
                        "left JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                        "left JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                        "LEFT JOIN personas p ON v.idPersona = p.idPersona " +
                        "WHERE v.estatus = 1 AND v.serie = @Serie  ORDER BY v.idVehiculo DESC", connection);

                        command.Parameters.AddWithValue("@Serie", Serie);
                    }
                    else if (!string.IsNullOrEmpty(Placa))
                    {
                        command = new SqlCommand("SELECT TOP 200 v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio, " +
                            "p.CURP, p.RFC,p.nombre, p.apellidoPaterno, p.apellidoMaterno " +
                            "FROM vehiculos v " +
                            "left JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                            "left JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                            "left JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                            "left JOIN catColores cc ON v.idColor = cc.idColor " +
                            "left JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                            "left JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                            "LEFT JOIN personas p ON v.idPersona = p.idPersona " +
                            "WHERE v.estatus = 1 AND v.placas LIKE '%' + @Placa + '%' ORDER BY v.idVehiculo DESC", connection);
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
                            "LEFT JOIN personas p ON v.idPersona = p.idPersona " +
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
                            vehiculo.RFC = reader["RFC"].ToString();
                            vehiculo.CURP = reader["CURP"].ToString();
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



        public List<CapturaAccidentesModel> BuscarPorParametroid(string id)
        {
            List<CapturaAccidentesModel> Vehiculo = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command;                  
                        command = new SqlCommand("SELECT v.*, " +
                            "mv.marcaVehiculo, " +
                            "sm.nombreSubmarca, " +
                            "e.nombreEntidad, " +
                            "cc.color, " +
                            "tv.tipoVehiculo, " +
                            "ts.tipoServicio," +
                            "p.nombre, " +
                            "p.apellidoPaterno, " +
                            "p.apellidoMaterno " +
                            "FROM vehiculos v " +
                            "JOIN catMarcasVehiculos mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                            "left JOIN catSubmarcasVehiculos sm ON v.idSubmarca = sm.idSubmarca " +
                            "left JOIN catEntidades e ON v.idEntidad = e.idEntidad " +
                            "left JOIN catColores cc ON v.idColor = cc.idColor " +
                            "left JOIN catTiposVehiculo tv ON v.idTipoVehiculo = tv.idTipoVehiculo " +
                            "left JOIN catTipoServicio ts ON v.idCatTipoServicio = ts.idCatTipoServicio " +
                            "LEFT JOIN personas p ON v.idPersona = p.idPersona " +
                            "WHERE v.estatus = 1 AND idVehiculo=@Folio;", connection);
                        command.Parameters.AddWithValue("@Folio", Convert.ToInt32(id));

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
                 //   connection.Close();
                }
            }

            return Vehiculo;
        }







        public int ActualizarConVehiculo(int idVehiculo, int idAccidente, int IdPersona, string Placa, string Serie)
		{
			int idVehiculoInsertado = 0;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();
					string query = @"
                            DECLARE @exist INT
                            SET @exist = (SELECT COUNT(*) FROM vehiculosAccidente WHERE idVehiculo = @idVehiculo AND idAccidente = @idAccidente AND estatus = 1)

                            IF @exist = 0
                            BEGIN
                                INSERT INTO vehiculosAccidente (idAccidente, idVehiculo, idPersona, placa, serie)
                                OUTPUT INSERTED.idVehiculo
                                VALUES (@idAccidente, @idVehiculo, @idPersona, @Placa, @Serie)
                                SET @idVehiculoInsertado = 1
                            END
                            ELSE
                            BEGIN
                                SET @idVehiculoInsertado = 0
                            END";

					SqlCommand command = new SqlCommand(query, connection);
					command.Parameters.AddWithValue("@idVehiculo", idVehiculo);
					command.Parameters.AddWithValue("@idAccidente", idAccidente);
					command.Parameters.AddWithValue("@idPersona", IdPersona);
					command.Parameters.AddWithValue("@Placa", Placa);
					command.Parameters.AddWithValue("@Serie", Serie);

					// Agregamos el parámetro de salida para capturar el valor de @idVehiculoInsertado
					SqlParameter idVehiculoInsertadoParam = new SqlParameter("@idVehiculoInsertado", SqlDbType.Int);
					idVehiculoInsertadoParam.Direction = ParameterDirection.Output;
					command.Parameters.Add(idVehiculoInsertadoParam);

					// Ejecutamos el comando
					command.ExecuteNonQuery();

					// Capturamos el valor de @idVehiculoInsertado después de ejecutar el comando
					idVehiculoInsertado = (int)idVehiculoInsertadoParam.Value;
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
                                                        @"SELECT
                                                            cva.*,
                                                            COALESCE(cva.idPersona, pcv.idPersona) AS idConductor,
                                                            cva.idTipoCarga,
                                                            cva.poliza,
                                                            ctc.tipoCarga,
                                                            v.placas,
                                                            v.tarjeta,
                                                            v.serie,
                                                            v.idMarcaVehiculo,
                                                            v.idSubmarca,
                                                            v.idEntidad,
                                                            v.idTipoVehiculo,
                                                            acc.numeroReporte,
                                                            v.idPersona AS idPropietario,
                                                            v.modelo,
                                                            v.idColor,
                                                            v.idCatTipoServicio,
                                                            v.motor,
                                                            v.capacidad,
                                                            cm.marcaVehiculo,
                                                            csv.nombreSubmarca,
                                                            tv.tipoVehiculo,
                                                            COALESCE(p.nombre, pcv.nombre) AS nombre,
                                                            COALESCE(p.apellidoPaterno, pcv.apellidoPaterno) AS apellidoPaterno,
                                                            p.apellidoMaterno,
	                                                        pdir.idEntidad AS IdEntidadPropetario,
	                                                        pent.nombreEntidad AS EntidadPropietario,
	                                                        pdir.idMunicipio AS IdMunicipioPropietario,
	                                                        mun.municipio AS MunicipioPropietario,
	                                                        pdir.colonia AS ColoniaPropietario,
	                                                        pdir.calle AS CallePropietario,
	                                                        pdir.numero AS NumeroPropietario,
	                                                        pdir.telefono AS TelefonoPropietario,
	                                                        pdir.correo AS CorreoPropietario,
                                                            pcv.RFC,
                                                            pcv.CURP,
                                                            FORMAT(p.fechaNacimiento, 'dd/MM/yyyy') AS fechaNacimiento,
                                                            c.color,
                                                            ts.tipoServicio,
                                                            pcv.nombre AS nombreConductor,
                                                            pcv.apellidoPaterno AS apellidoPConductor,
                                                            pcv.apellidoMaterno AS apellidoMConductor,
                                                            tc.tipoCarga,
                                                            v.vigenciaTarjeta,
                                                            v.otros,
                                                            tp.tipoPersona,
                                                            pen.pension,
                                                            ft.formaTraslado,
                                                            cent.nombreEntidad,
                                                            va.montoVehiculo
                                                        FROM
                                                            conductoresVehiculosAccidente AS cva
                                                        INNER JOIN
                                                            vehiculos AS v ON cva.idVehiculo = v.idVehiculo
                                                        LEFT JOIN
                                                            catMarcasVehiculos AS cm ON v.idMarcaVehiculo = cm.idMarcaVehiculo
                                                        LEFT JOIN
                                                            catTiposcarga AS ctc ON cva.idTipoCarga = ctc.idTipoCarga
                                                        LEFT JOIN
                                                            catSubmarcasVehiculos AS csv ON v.idSubmarca = csv.idSubmarca
                                                        LEFT JOIN
                                                            catTiposVehiculo AS tv ON v.idTipoVehiculo = tv.idTipoVehiculo
                                                        LEFT JOIN
                                                            personas AS p ON v.idPersona = p.idPersona
                                                        LEFT JOIN
                                                            catColores AS c ON v.idColor = c.idColor
                                                        LEFT JOIN
                                                            catTiposcarga AS tc ON cva.idTipoCarga = tc.idTipoCarga
                                                        LEFT JOIN
                                                            pensiones AS pen ON cva.idPension = pen.idPension
                                                        LEFT JOIN
                                                            vehiculosAccidente AS va ON cva.idVehiculo = va.idVehiculo AND cva.idAccidente = va.idAccidente
                                                        LEFT JOIN
                                                            catFormasTraslado AS ft ON cva.idFormaTraslado = ft.idFormaTraslado
                                                        LEFT JOIN
                                                            catTipoServicio AS ts ON v.idCatTipoServicio = ts.idCatTipoServicio
                                                        LEFT JOIN
                                                            accidentes AS acc ON cva.idAccidente = acc.idAccidente
                                                        LEFT JOIN
                                                            catEntidades AS cent ON v.idEntidad = cent.idEntidad
                                                        LEFT JOIN
                                                            personas AS pcv ON cva.idPersona = pcv.idPersona
                                                        LEFT JOIN
                                                            catTipoPersona AS tp ON p.idCatTipoPersona = tp.idCatTipoPersona
                                                        LEFT JOIN
                                                            personasDirecciones AS pdir ON pdir.idPersona = v.idPersona
                                                        LEFT JOIN
                                                            catEntidades AS pent ON pent.idEntidad = pdir.idEntidad
                                                        LEFT JOIN
                                                            catMunicipios AS mun ON mun.idMunicipio = pdir.idMunicipio
                                                        WHERE cva.idAccidente = @idAccidente AND cva.idPersona = @idPersona AND cva.idVehiculo = @IdVehiculoInvolucrado AND cva.estatus > 0;", connection);

                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idPersona", IdPropietarioInvolucrado);
                    command.Parameters.AddWithValue("@IdVehiculoInvolucrado", IdVehiculoInvolucrado);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            involucrado.IdPropietarioInvolucrado = reader["idPropietario"] != DBNull.Value ? Convert.ToInt32(reader["idPropietario"].ToString()) : 0;
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
                            involucrado.EntidadPropietario = reader["EntidadPropietario"] != DBNull.Value ? reader["EntidadPropietario"].ToString() : string.Empty;
                            involucrado.MunicipioPropietario = reader["MunicipioPropietario"] != DBNull.Value ? reader["MunicipioPropietario"].ToString() : string.Empty;
                            involucrado.ColoniaPropietario = reader["ColoniaPropietario"] != DBNull.Value ? reader["ColoniaPropietario"].ToString() : string.Empty;
                            involucrado.CallePropietario = reader["CallePropietario"] != DBNull.Value ? reader["CallePropietario"].ToString() : string.Empty;
                            involucrado.NumeroPropietario = reader["NumeroPropietario"] != DBNull.Value ? reader["NumeroPropietario"].ToString() : string.Empty;
                            involucrado.TelefonoPropietario = reader["TelefonoPropietario"] != DBNull.Value ? reader["TelefonoPropietario"].ToString() : string.Empty;
                            involucrado.CorreoPropietario = reader["CorreoPropietario"] != DBNull.Value ? reader["CorreoPropietario"].ToString() : string.Empty;
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
                            involucrado.Otros = reader["otros"] != DBNull.Value ? reader["otros"].ToString() : string.Empty;


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
                            model.VigenciaTarjeta = reader["vigenciaTarjeta"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaTarjeta"].ToString());
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
                    string query = "asp_GenerateClasificacionAccidente";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.CommandType = CommandType.StoredProcedure;
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
                    SqlCommand command = new SqlCommand(@"
                        SELECT a.*, ca.nombreClasificacion 
                            FROM AccidenteClasificacion a 
                            JOIN catClasificacionAccidentes ca ON a.idCatClasificacionAccidentes = ca.idClasificacionAccidente 
                            WHERE a.idAccidente = @idAccidente 
                            and a.estatus=1 

                    ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel clasificacion = new CapturaAccidentesModel();
                            clasificacion.IdAccidente = Convert.ToInt32(reader["idAccidente"].ToString());
                            clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["idCatClasificacionAccidentes"].ToString());
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
        public int ClasificacionEliminar(int IdAccidente, int IdClasificacionAccidente)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = @"UPDATE AccidenteClasificacion SET estatus = 0 WHERE idAccidente = @idAccidente and idCatClasificacionAccidentes=@claf";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAccidente", IdAccidente);
                    command.Parameters.AddWithValue("@claf", IdClasificacionAccidente);

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
                        "WHERE afo.idAccidente = @IdAccidente AND afo.estatus = 1 ORDER BY afo.fechaActualizacion DESC", connection);
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





		CapturaAccidentesModel ICapturaAccidentesService.ObtenerDetallePersona(int id)
		{
			CapturaAccidentesModel involucrado = new CapturaAccidentesModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT " +
											 "MAX(ia.idPersona) AS idPersona," +
											 "MAX(p.nombre) AS nombre, " +
											 "MAX(p.apellidoPaterno) AS apellidoPaterno, " +
											 "MAX(p.apellidoMaterno) AS apellidoMaterno, " +
											 "MAX(p.rfc) AS rfc, " +
											 "MAX(p.curp) AS curp, " +
											 "MAX(CONVERT(varchar, p.fechaNacimiento, 103)) AS fechaNacimiento, " +											 
											 "MAX(tv.tipoVehiculo) AS tipoVehiculo, " +
											 "MAX(ia.idEstadoVictima) AS idEstadoVictima, " +
											 "MAX(ev.estadoVictima) AS estadoVictima, " +											
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
											 "MAX(p.numeroLicencia) AS numeroLicencia," +
											 "MAX(pd.colonia) AS colonia," +
                                             "MAX(pd.numero) AS numero,"  +
                                             "MAX(pd.calle) AS calle," +
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
											 "WHERE p.idpersona = @idpersona and ia.estatus = 1 group by ia.idPersona;", connection);

					command.Parameters.Add(new SqlParameter("@idpersona", SqlDbType.NVarChar)).Value = id;
					command.CommandType = CommandType.Text;
					using (
						SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdAsiento = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);
							involucrado.IdPersona = reader.IsDBNull(reader.GetOrdinal("idPersona")) ? default(int) : Convert.ToInt32(reader["idPersona"]);

							involucrado.nombre = reader["nombre"].ToString();
							involucrado.apellidoPaterno = reader["apellidoPaterno"].ToString();
							involucrado.apellidoMaterno = reader["apellidoMaterno"].ToString();
							involucrado.RFC = reader["rfc"].ToString();
							involucrado.CURP = reader["curp"].ToString();
							involucrado.Calle = reader["calle"].ToString();
							involucrado.numeroLicencia = reader["numeroLicencia"].ToString();
							involucrado.Numero = reader["numero"].ToString();
							involucrado.Colonia = reader["colonia"].ToString();
							involucrado.Correo = reader["correo"].ToString();
							involucrado.FormatDateNacimiento = reader["fechaNacimiento"].ToString();

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

			return involucrado;

		}

		CapturaAccidentesModel ICapturaAccidentesService.DatosInvolucradoEdicion(int id, int idAccidente,int IdInvolucrado)
		{
			CapturaAccidentesModel involucrado = new CapturaAccidentesModel();

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
				try

				{
					connection.Open();
					SqlCommand command = new SqlCommand(@"SELECT p.idPersona,p.nombre,p.apellidoPaterno,p.apellidoMaterno,p.RFC,p.CURP
                                                        ,p.numeroLicencia,p.fechaNacimiento,pd.calle,pd.numero,pd.colonia,pd.telefono,pd.correo,inv.idTipoInvolucrado,
														inv.idEstadoVictima,inv.idHospital,inv.idInstitucionTraslado,inv.idAsiento,inv.idCinturon, inv.idVehiculo,inv.idInvolucradosAccidente
														,v.placas,v.tarjeta,v.serie,v.idTipoVehiculo,tv.tipoVehiculo,v.idMarcaVehiculo,mv.marcaVehiculo
														,v.idSubmarca,sm.nombreSubmarca,v.modelo,v.idPersona AS Prop,prop.nombre AS propietarioNombre
														,prop.apellidoPaterno AS apPaternoPropietario,prop.apellidoMaterno AS apMaternoPropietario
														,cm.municipio,ce.nombreEntidad,gen.genero
                                                        From personas p														
                                                        LEFT JOIN personasDirecciones AS pd ON pd.idPersona = p.idPersona
														LEFT JOIN involucradosAccidente AS inv ON inv.idPersona = p.idPersona
														LEFT JOIN vehiculos v ON v.idVehiculo = inv.idVehiculo
														LEFT JOIN personas prop ON prop.idPersona = v.idPersona
														LEFT JOIN catTiposVehiculo tv ON  tv.idTipoVehiculo = v.idTipoVehiculo
														LEFT JOIN catMarcasVehiculos mv ON  mv.idMarcaVehiculo = v.idMarcaVehiculo
														LEFT JOIN catSubmarcasVehiculos sm ON  sm.idSubmarca = v.idSubmarca
														LEFT JOIN catMunicipios cm ON cm.idMunicipio= pd.idMunicipio
														LEFT JOIN catEntidades ce ON ce.idEntidad = pd.idEntidad
														LEFT JOIN catGeneros gen ON gen.idGenero = p.idGenero
                                                        WHERE p.idPersona = @idpersona AND inv.idAccidente = @idAccidente AND inv.idInvolucradosAccidente = @idInvolucradosAccidente", connection);

					command.Parameters.Add(new SqlParameter("@idpersona", SqlDbType.NVarChar)).Value = id;
					command.Parameters.Add(new SqlParameter("@idAccidente", SqlDbType.NVarChar)).Value = idAccidente;
					command.Parameters.Add(new SqlParameter("@idInvolucradosAccidente", SqlDbType.NVarChar)).Value = IdInvolucrado;

					command.CommandType = CommandType.Text;
					using (
						SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
					{
						while (reader.Read())
						{
							involucrado.IdInvolucrado = reader["idInvolucradosAccidente"] != DBNull.Value ?
								Convert.ToInt32(reader["idInvolucradosAccidente"]) :
								0;
							involucrado.IdVehiculo = reader["idVehiculo"] != DBNull.Value ?
								Convert.ToInt32(reader["idVehiculo"]) :
								0;
							involucrado.IdPersona = Convert.ToInt32(reader["idPersona"].ToString());
							involucrado.IdAsiento = Convert.ToInt32(reader["idAsiento"].ToString());
							if (involucrado.IdAsiento == 0)
							{
								involucrado.IdAsiento = null; 
							}

							involucrado.nombre = reader["nombre"].ToString();
							involucrado.apellidoPaterno = reader["apellidoPaterno"].ToString();
							involucrado.apellidoMaterno = reader["apellidoMaterno"].ToString();
							involucrado.RFC = reader["rfc"].ToString();
							involucrado.CURP = reader["curp"].ToString();
							involucrado.Calle = reader["calle"].ToString();
							involucrado.numeroLicencia = reader["numeroLicencia"].ToString();
							involucrado.Numero = reader["numero"].ToString();
							involucrado.Colonia = reader["colonia"].ToString();
							involucrado.Placa = reader["placas"].ToString();
							involucrado.Tarjeta = reader["tarjeta"].ToString();
							involucrado.Serie = reader["serie"].ToString();
							involucrado.TipoVehiculo = reader["tipoVehiculo"].ToString();
							involucrado.Marca = reader["marcaVehiculo"].ToString();
							involucrado.Submarca = reader["nombreSubmarca"].ToString();
							involucrado.Municipio = reader["municipio"].ToString();
							involucrado.Entidad = reader["nombreEntidad"].ToString();
							involucrado.Genero = reader["genero"].ToString();
							involucrado.Telefono = reader["telefono"].ToString();

							involucrado.Modelo = reader["modelo"].ToString();
							involucrado.Propietario = $"{reader["propietarioNombre"]} {reader["apPaternoPropietario"]} {reader["apMaternoPropietario"]}";					
							involucrado.Correo = reader["correo"].ToString();
                            int fechaNacimientoIndex = reader.GetOrdinal("fechaNacimiento");

                            // Verificar si el valor del campo es DBNull.Value
                            if (!reader.IsDBNull(fechaNacimientoIndex))
                            {
                                DateTime fechaNacimiento = reader.GetDateTime(fechaNacimientoIndex);
                                involucrado.FormatDateNacimiento = fechaNacimiento.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                involucrado.FormatDateNacimiento = ""; // O asignar cualquier otro valor predeterminado
                            }

                            involucrado.IdInstitucionTraslado = Convert.ToInt32(reader["idInstitucionTraslado"].ToString());
							involucrado.IdTipoInvolucrado = Convert.ToInt32(reader["idTipoInvolucrado"].ToString());
							involucrado.IdEstadoVictima = Convert.ToInt32(reader["idEstadoVictima"].ToString());
							involucrado.IdHospital = Convert.ToInt32(reader["idHospital"].ToString());
							involucrado.IdCinturon = Convert.ToInt32(reader["idCinturon"].ToString());
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

			return involucrado;

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
					string query = "DELETE FROM conductoresVehiculosAccidente WHERE idAccidente = @IdAccidente AND idPersona = @IdPropietarioInvolucrado AND idVehiculo = @IdVehiculoInvolucrado;";

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
					int numeroConsecutivo = 1;

					connection.Open();
                    SqlCommand command = new SqlCommand("usp_ObtieneVehiculosInvolucrados", connection);

                    command.CommandType = CommandType.StoredProcedure;
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
                            vehiculo.Grua = reader["grua"].ToString();
                            vehiculo.TipoLicencia = reader["tipoLicencia"].ToString();
							vehiculo.ConductorInvolucrado = $"{reader["nombreConductor"]} {reader["apellidoPConductor"]} {reader["apellidoMConductor"]}";
                            vehiculo.vigenciaLicencia = reader["vigenciaLicencia"].GetType() == typeof(DBNull) ? DateTime.MinValue : (DateTime)reader["vigenciaLicencia"];
                            vehiculo.NumeroEconomico = reader["numeroeconomico"].GetType() == typeof(DBNull) ? "" : reader["numeroeconomico"].ToString();
                            vehiculo.IdVehiculo = Convert.IsDBNull(reader["idVehiculo"]) ? 0 : Convert.ToInt32(reader["idVehiculo"]);
                            //vehiculo.MontoDanos = Convert.ToDecimal(reader["MontoDanos"]);
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
							vehiculo.numeroConsecutivo = numeroConsecutivo;

							ListaVehiculosInvolucrados.Add(vehiculo);
							numeroConsecutivo++;
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
                                        fechaActualizacion = getdate(),
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
                    /*                connection.Open();

                                    SqlCommand command = new SqlCommand("SELECT " +
                                        "CASE WHEN i.infraccionCortesia = 1 THEN 'Cortesia' ELSE 'No Cortesia' END TipoCortesia," +
                                        "v.idVehiculo, v.placas, propietario.idPersona AS propietario, i.folioInfraccion, i.fechaInfraccion,i.idInfraccion,i.idPersona,i.idPersonaInfraccion, conductor.idPersona AS conductor, ent.nombreEntidad, " +
                                        "propietario.nombre AS propietario_nombre, propietario.apellidoPaterno AS propietario_apellidoPaterno, propietario.apellidoMaterno AS propietario_apellidoMaterno, conductor.nombre AS conductor_nombre, conductor.apellidoPaterno AS conductor_apellidoPaterno, conductor.apellidoMaterno AS conductor_apellidoMaterno " +
                                        "FROM conductoresVehiculosAccidente AS cva " +
                                        "LEFT JOIN vehiculos AS v ON cva.idVehiculo = v.idVehiculo " +
                                        "LEFT JOIN catEntidades AS ent ON v.idEntidad = ent.idEntidad " +
                                        "LEFT JOIN infracciones AS i ON cva.idVehiculo = i.idVehiculo " +
                                        "LEFT JOIN infraccionesAccidente AS infAcc ON i.idInfraccion = infAcc.idInfraccion " +
                                        "LEFT JOIN personas AS propietario ON v.idPersona = propietario.idPersona " +
                                        "LEFT JOIN personas AS conductor ON i.idPersonaInfraccion= conductor.idPersona " +
                                        "WHERE infAcc.estatus = 1 AND cva.idAccidente = @idAccidente " +
                                        "AND EXISTS(SELECT 1 FROM infracciones AS i1 WHERE i1.idVehiculo = v.idVehiculo) " +
                                        " order by i.idInfraccion desc", connection);



                                    command.CommandType = CommandType.Text;
                                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                                   // command.Parameters.AddWithValue("@idDependencia", idDependencia);

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
                                            elemnto.Cortesia = reader["TipoCortesia"].ToString();

                                            ListaVehiculosInfracciones.Add(elemnto);

                                        }

                                    }
                    */
                    // en caso de no haber por accidente se mostraran las infracciones de los vehiculo
                    if (ListaVehiculosInfracciones.Count == 0) {
						connection.Open();
                        SqlCommand command = new SqlCommand("SELECT " +
                        "       i.horainfraccion,           " +
						"CASE WHEN i.infraccionCortesia = 2 THEN 'Cortesia' ELSE 'No Cortesia' END TipoCortesia," +
						"v.idVehiculo, v.placas, propietario.idPersona AS propietario, i.folioInfraccion, i.fechaInfraccion,i.idInfraccion,i.idPersona,i.idPersonaInfraccion, conductor.idPersona AS conductor, ent.nombreEntidad, " +
						"propietario.nombre AS propietario_nombre, propietario.apellidoPaterno AS propietario_apellidoPaterno, propietario.apellidoMaterno AS propietario_apellidoMaterno, conductor.nombre AS conductor_nombre, conductor.apellidoPaterno AS conductor_apellidoPaterno, conductor.apellidoMaterno AS conductor_apellidoMaterno " +
						"FROM conductoresVehiculosAccidente AS cva " +
						"LEFT JOIN vehiculos AS v ON cva.idVehiculo = v.idVehiculo " +
						"LEFT JOIN catEntidades AS ent ON v.idEntidad = ent.idEntidad " +
						"LEFT JOIN infracciones AS i ON cva.idVehiculo = i.idVehiculo " +						
						"LEFT JOIN personas AS propietario ON v.idPersona = propietario.idPersona " +
						"LEFT JOIN personas AS conductor ON i.idPersonaInfraccion= conductor.idPersona " +
						"WHERE cva.idAccidente = @idAccidente " +
						" AND EXISTS(SELECT 1 FROM infracciones AS i1 WHERE i1.idVehiculo = v.idVehiculo) " +
                        " AND NOT EXISTS (SELECT 1 FROM infraccionesAccidente infAc WHERE infAc.idVehiculo = v.idVehiculo and infAc.idInfraccion = i.idInfraccion) " +
                        " order by i.idInfraccion desc", connection);



						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@idAccidente", idAccidente);
						// command.Parameters.AddWithValue("@idDependencia", idDependencia);

						using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
						{
							while (reader.Read())
							{
								CapturaAccidentesModel elemnto = new CapturaAccidentesModel();
								elemnto.IdVehiculo = Convert.ToInt32(reader["IdVehiculo"].ToString());
								elemnto.IdInfraccion = Convert.ToInt32(reader["IdInfraccion"].ToString());
								elemnto.Placa = reader["placas"].ToString();
								elemnto.folioInfraccion = reader["folioInfraccion"].ToString();

                                var aux = reader["horainfraccion"].ToString();
                                var timeint = Convert.ToInt32(aux);
                                var Min = (timeint % 100);
                                var hour = (timeint - Min) / 100;

                                elemnto.Fecha = Convert.ToDateTime(reader["fechaInfraccion"].ToString());

                                elemnto.Fecha=elemnto.Fecha.Value.AddHours((double)hour).AddMinutes((double)Min);


                                elemnto.ConductorInvolucrado = reader["conductor_nombre"].ToString() + " " + reader["conductor_apellidoPaterno"].ToString() + " " + reader["conductor_apellidoMaterno"].ToString();
								elemnto.Propietario = reader["propietario_nombre"].ToString() + " " + reader["propietario_apellidoPaterno"].ToString() + " " + reader["propietario_apellidoMaterno"].ToString();
								elemnto.EntidadRegistro = reader["nombreEntidad"].ToString();
								elemnto.Cortesia = reader["TipoCortesia"].ToString();

								ListaVehiculosInfracciones.Add(elemnto);

							}

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

            ListaVehiculosInfracciones= ListaVehiculosInfracciones.OrderByDescending(x => x.Fecha).ToList();    

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
                    string query = "INSERT INTO infraccionesAccidente (idVehiculo, idAccidente,idInfraccion,estatus) VALUES (@IdVehiculo, @idAccidente, @IdInfraccion, @estatus)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idVehiculo", IdVehiculo);
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@idInfraccion", IdInfraccion);
                    command.Parameters.AddWithValue("@estatus", 1);

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
                        "a.numeroReporte, " +
                        "i.folioInfraccion, " +
                        "cei.estatusInfraccion, " +
                        "i.idEstatusInfraccion, "+
						"mv.marcaVehiculo, sv.nombreSubmarca, i.idInfraccion, ISNULL(gr.garantia,'') garantia " +
						"FROM infraccionesAccidente AS ia JOIN vehiculos AS v ON ia.idVehiculo = v.idVehiculo " +
                        "LEFT JOIN accidentes AS a ON ia.idAccidente = a.idAccidente " +
                        "LEFT JOIN infracciones AS i ON ia.idInfraccion = i.idInfraccion " +
                        "LEFT JOIN catEstatusInfraccion AS cei ON cei.idEstatusInfraccion = i.idEstatusInfraccion " +
                        "LEFT JOIN catMarcasVehiculos AS mv ON v.idMarcaVehiculo = mv.idMarcaVehiculo " +
                        "LEFT JOIN catSubmarcasVehiculos AS sv ON v.idSubmarca = sv.idSubmarca " +
						"LEFT JOIN garantiasInfraccion AS gi ON gi.idInfraccion = ia.idInfraccion " +
						"LEFT JOIN catGarantias AS gr ON gr.idGarantia = gi.idCatGarantia " +
						"WHERE ia.idAccidente = @idAccidente AND ia.estatus != 0 ORDER BY  ia.idInf_Acc DESC;", connection);



                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                   // command.Parameters.AddWithValue("@idDependencia", idDependencia);


                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel elemnto = new CapturaAccidentesModel();
							elemnto.IdInfraccion = Convert.IsDBNull(reader["idInfraccion"]) ? 0 : Convert.ToInt32(reader["idInfraccion"]);
							elemnto.IdInfAcc = Convert.IsDBNull(reader["IdInf_Acc"]) ? 0 : Convert.ToInt32(reader["IdInf_Acc"]);
                            elemnto.IdAccidente = Convert.IsDBNull(reader["IdAccidente"]) ? 0 : Convert.ToInt32(reader["IdAccidente"]);
                            elemnto.IdVehiculoInvolucrado = Convert.IsDBNull(reader["IdVehiculo"]) ? 0 : Convert.ToInt32(reader["IdVehiculo"]);
                            elemnto.NumeroReporte = reader["numeroReporte"].ToString();
                            elemnto.Placa = reader["placas"].ToString();
                            elemnto.EstatusInfraccion = reader["estatusInfraccion"].ToString();
                            elemnto.folioInfraccion = reader["folioInfraccion"].ToString();
                            elemnto.garantia = reader["garantia"].ToString();
							// elemnto.EstatusReporte = reader["estatusReporte"].ToString();
							elemnto.Vehiculo = $"{reader["marcaVehiculo"]} {reader["nombreSubmarca"]} {reader["placas"]} {reader["modelo"]}";
                            elemnto.IdVehiculo = Convert.IsDBNull(reader["IdVehiculo"]) ? 0 : Convert.ToInt32(reader["IdVehiculo"]);


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
		public int RelacionPersonaVehiculo(int IdPersona, int idAccidente, int IdVehiculoInvolucrado, int IdInvolucrado)
		{
            int result = 0;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				try
				{
					connection.Open();

					// Verificar si IdInvolucrado no es nulo y si existe en la tabla
					if (IdInvolucrado != 0)
					{
						string queryExistencia = "SELECT COUNT(*) FROM involucradosAccidente WHERE IdInvolucradosAccidente = @IdInvolucrado";
						SqlCommand commandExistencia = new SqlCommand(queryExistencia, connection);
						commandExistencia.Parameters.AddWithValue("@IdInvolucrado", IdInvolucrado);
						int count = (int)commandExistencia.ExecuteScalar();

						if (count > 0)
						{
							// Si existe, ejecutar un UPDATE en lugar de un INSERT
							string queryUpdate = @"UPDATE involucradosAccidente SET idPersona = @idPersona, idAccidente = @idAccidente, idVehiculo = @idVehiculoInvolucrado WHERE IdInvolucradosAccidente = @IdInvolucrado";
							SqlCommand commandUpdate = new SqlCommand(queryUpdate, connection);
							commandUpdate.Parameters.AddWithValue("@idPersona", IdPersona);
							commandUpdate.Parameters.AddWithValue("@idAccidente", idAccidente);
							commandUpdate.Parameters.AddWithValue("@idVehiculoInvolucrado", IdVehiculoInvolucrado);
							commandUpdate.Parameters.AddWithValue("@IdInvolucrado", IdInvolucrado);

							commandUpdate.ExecuteNonQuery();
						}
						else
						{
							// Si no existe, continuar con el INSERT
							string queryInsert = "INSERT into involucradosAccidente(idPersona,idAccidente,idVehiculo) values(@idPersona, @idAccidente, @idVehiculoInvolucrado)";
							SqlCommand commandInsert = new SqlCommand(queryInsert, connection);
							commandInsert.Parameters.AddWithValue("@idPersona", IdPersona);
							commandInsert.Parameters.AddWithValue("@idAccidente", idAccidente);
							commandInsert.Parameters.AddWithValue("@idVehiculoInvolucrado", IdVehiculoInvolucrado);

							commandInsert.ExecuteNonQuery();
						}
					}
					else
					{
						// Si IdInvolucrado es null, continuar con el INSERT
						string queryInsert = "INSERT into involucradosAccidente(idPersona,idAccidente,idVehiculo) values(@idPersona, @idAccidente, @idVehiculoInvolucrado)";
						SqlCommand commandInsert = new SqlCommand(queryInsert, connection);
						commandInsert.Parameters.AddWithValue("@idPersona", IdPersona);
						commandInsert.Parameters.AddWithValue("@idAccidente", idAccidente);
						commandInsert.Parameters.AddWithValue("@idVehiculoInvolucrado", IdVehiculoInvolucrado);

						commandInsert.ExecuteNonQuery();
					}
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
                                      idCinturon = @idCinturon, 
                                      estatus = @estatus
                                  WHERE idAccidente = @idAccidente
                                    AND idPersona = @idPersona
                                    AND idVehiculo = @idVehiculo";

                    string consultaInsert = @"INSERT INTO InvolucradosAccidente
                                  (idAccidente, idPersona, idVehiculo, idTipoInvolucrado, idEstadoVictima, idHospital, idInstitucionTraslado, idAsiento, idCinturon,estatus)
                                  VALUES
                                  (@idAccidente, @idPersona, @idVehiculo, @idTipoInvolucrado, @idEstadoVictima, @idHospital, @idInstitucionTraslado, @idAsiento, @idCinturon,@estatus)";

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
						comandoUpdate.Parameters.AddWithValue("@idAsiento", model.IdAsiento ?? 0);
						comandoUpdate.Parameters.AddWithValue("@idCinturon", model.IdCinturon);
                        comandoUpdate.Parameters.AddWithValue("@estatus", 1);

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
						comandoInsert.Parameters.AddWithValue("@idAsiento", model.IdAsiento ?? 0);
						comandoInsert.Parameters.AddWithValue("@idCinturon", model.IdCinturon);
                        comandoInsert.Parameters.AddWithValue("@estatus", 1);

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
					int numeroConsecutivo = 1;

					connection.Open();
                    SqlCommand command = new SqlCommand("SELECT " +
                                             "MAX (ia.idInvolucradosAccidente) AS idInvolucrado, " +
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
											 "MAX(p.numeroLicencia) AS numeroLicencia," +
											 "MAX(cc.cinturon) AS cinturon, " +
                                             "MAX(ct.idTipoInvolucrado) as idTipoInvolucrado " +
                                            // "MAX(ct.tipoInvolucrado) as tipoInvolucrado " +
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
											 "LEFT JOIN personasDirecciones AS pd ON p.idPersona = pd.idPersona " +
										     "LEFT JOIN catEntidades AS e ON e.idEntidad = pd.idEntidad " +
											 "LEFT JOIN catMunicipios AS mun ON mun.idMunicipio = pd.idMunicipio " +
											 "LEFT JOIN vehiculosAccidente AS va ON  va.idAccidente = a.idAccidente " +
											 "LEFT JOIN catTipoInvolucrado ct ON ct.idTipoInvolucrado = ia.idTipoInvolucrado " +
                                             "WHERE ia.idAccidente = @idAccidente and ia.estatus = 1 group by ia.idPersona;", connection);



                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@idAccidente", idAccidente);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CapturaAccidentesModel involucrado = new CapturaAccidentesModel();
							involucrado.IdInvolucrado = reader["idInvolucrado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInvolucrado"].ToString());

							involucrado.IdAccidente = reader["idAccidente"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idAccidente"].ToString());
							involucrado.IdTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
							involucrado.IdTipoVehiculo = reader["IdTipoVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["IdTipoVehiculo"].ToString());
							involucrado.IdPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
							involucrado.IdVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idVehiculo"].ToString());
							involucrado.IdEstadoVictima = reader["idEstadoVictima"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idEstadoVictima"].ToString());
							involucrado.IdInstitucionTraslado = reader["idInstitucionTraslado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInstitucionTraslado"].ToString());
							involucrado.IdHospital = reader["idHospital"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idHospital"].ToString());
							involucrado.IdAsiento = Convert.ToInt32(reader["idAsiento"].ToString());
							if (involucrado.IdAsiento == 0)
							{
								involucrado.IdAsiento = null;
							}
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
							involucrado.Asiento = reader["idAsiento"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["idAsiento"].ToString());
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
                            involucrado.IdTipoInvolucrado = reader["idTipoInvolucrado"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoInvolucrado"].ToString()); 
							involucrado.TipoInvolucrado = reader["tipoInvolucrado"] == System.DBNull.Value ? string.Empty : Convert.ToString(reader["tipoInvolucrado"].ToString());
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

							involucrado.numeroConsecutivo = numeroConsecutivo;

							ListaInvolucrados.Add(involucrado);
							numeroConsecutivo++;
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
        public int EditarInvolucrado(CapturaAccidentesModel model)
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

                  //  command.Parameters.AddWithValue("@idFactor", IdFactorAccidente);

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
        public int EliminarInvolucrado(int IdInvolucrado)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE involucradosAccidente SET estatus = 0 WHERE idInvolucradosAccidente = @IdInvolucrado";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@IdInvolucrado", IdInvolucrado);

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
            //  qryUpdate += !datosAccidente.montoCamino.Equals(null) ? " , montoCamino = @MontoCamino " : "";
            //qryUpdate += !datosAccidente.montoCarga.Equals(null) ? " , montoCarga = @MontoCarga " : "";
            // qryUpdate += !datosAccidente.montoPropietarios.Equals(null) ? " , montoPropietarios = @MontoPropietarios " : "";
            //qryUpdate += !datosAccidente.montoOtros.Equals(null) ? " , montoOtros = @MontoOtros " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.montoCamino) ? " , montoCamino = @MontoCamino " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.montoCarga) ? " , montoCarga = @MontoCarga " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.montoPropietarios) ? " , montoPropietarios = @MontoPropietarios " : "";
            qryUpdate += !string.IsNullOrEmpty(datosAccidente.montoOtros) ? " , montoOtros = @MontoOtros " : "";
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
                    command.Parameters.AddWithValue("@MontoCamino", datosAccidente.montoCamino ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MontoCarga", datosAccidente.montoCarga ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MontoPropietarios", datosAccidente.montoPropietarios ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MontoOtros", datosAccidente.montoOtros ?? (object)DBNull.Value);

                    //command.Parameters.AddWithValue("@MontoCarga", datosAccidente.montoCarga);
                    //command.Parameters.AddWithValue("@MontoPropietarios", datosAccidente.montoPropietarios);
                    //command.Parameters.AddWithValue("@MontoOtros", datosAccidente.montoOtros);
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

        public int GuardarDatosPrevioInfraccion(int idAccidente, string montoCamino, string montoCarga, string montoPropietarios, string montoOtros)
        {
            int result = 0;       
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = @"UPDATE accidentes 
                                            SET montoCamino = @MontoCamino, 
                                                montoCarga = @MontoCarga, 
                                                montoPropietarios = @MontoPropietarios, 
                                                montoOtros = @MontoOtros
                                            WHERE idAccidente = @idAccidente
                                            ";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAccidente", idAccidente);
                    command.Parameters.AddWithValue("@MontoCamino", montoCamino ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MontoCarga", montoCarga ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MontoPropietarios", montoPropietarios ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MontoOtros", montoOtros ?? (object)DBNull.Value);
                                     
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
        public bool ValidarFolio(string folioInfraccion, int idDependencia)
		{
			int folio = 0;


			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
			{
				connection.Open();

				string query = "SELECT COUNT(*) AS Result FROM infracciones WHERE folioInfraccion = @folioInfraccion and  year(fechaInfraccion) = year(getdate())";

				using (SqlCommand command = new SqlCommand(query, connection))
				{

					command.Parameters.AddWithValue("@folioInfraccion", folioInfraccion);
					//command.Parameters.AddWithValue("@idDependencia", idDependencia);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{

							folio = reader["Result"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["Result"]);
						}
					}
				}
			}
			return folio > 0;
		}
		public int RegistrarInfraccion(NuevaInfraccionModel model, int idDependencia)
        {
            int result = 0;
            string strQuery = @"INSERT INTO infracciones
                                            (fechaInfraccion
                                            ,horaInfraccion    
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
                                            ,estatus
                                            ,transito)
                                     VALUES (@fechaInfraccion
                                            ,@horaInfraccion
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
                                            ,@estatus
                                            ,@idDependencia
                                            );SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    //   command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    //  command.Parameters.Add(new SqlParameter("horaInfraccion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("fechaInfraccion", SqlDbType.DateTime)).Value = (object)model.fechaInfraccion;
                    DateTime fechaInfraccion = model.fechaInfraccion;
                    TimeSpan horaInfraccion = fechaInfraccion.TimeOfDay;
                    string horaFormateada = horaInfraccion.ToString("hhmm");
                    string horaInfraccionString = horaFormateada;
                    command.Parameters.Add(new SqlParameter("horaInfraccion", SqlDbType.NVarChar)).Value = horaInfraccionString;


                    command.Parameters.Add(new SqlParameter("folioInfraccion", SqlDbType.NVarChar)).Value = (object)model.folioInfraccion ?? "-";
                    command.Parameters.Add(new SqlParameter("idOficial", SqlDbType.Int)).Value = (object)model.idOficial ?? 0;
                    command.Parameters.Add(new SqlParameter("idMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? 0;
                    command.Parameters.Add(new SqlParameter("idCarretera", SqlDbType.Int)).Value = (object)model.IdCarretera ?? 0;
                    command.Parameters.Add(new SqlParameter("idTramo", SqlDbType.Int)).Value = (object)model.IdTramo ?? 0;
                    command.Parameters.Add(new SqlParameter("kmCarretera", SqlDbType.Float)).Value = (object)model.Kilometro ?? 0;
                    command.Parameters.Add(new SqlParameter("idVehiculo", SqlDbType.Int)).Value = (object)model.IdVehiculo ?? 0;
                    command.Parameters.Add(new SqlParameter("idPersona", SqlDbType.Int)).Value = (object)model.IdPersona ?? 0;
                    command.Parameters.Add(new SqlParameter("idPersonaInfraccion", SqlDbType.Int)).Value = (object)model.IdPersona ?? 0;
                    command.Parameters.Add(new SqlParameter("placasVehiculo", SqlDbType.NVarChar)).Value = (object)model.Placa ?? "-";
                    command.Parameters.Add(new SqlParameter("NumTarjetaCirculacion", SqlDbType.NVarChar)).Value = (object)model.Tarjeta ?? "-";
                    command.Parameters.Add(new SqlParameter("@idDependencia", SqlDbType.Int)).Value = idDependencia;



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
                                    montoCamino, montoCarga, CASE WHEN montoPropietarios = 0 THEN B.montoVehiculo ELSE montoPropietarios END montoPropietarios, montoOtros,
                                    latitud ,longitud ,idCertificado ,
                                    armas ,drogas ,valores ,prendas ,otros ,entregaObjetos, entregaOtros ,
                                    consignacionHechos, idCiudad , convenio ,
                                    idAutoridadEntrega , idAutoridadDisposicion , idElaboraConsignacion , 
                                    numeroOficio , idAgenciaMinisterio ,recibeMinisterio , 
                                    idElabora , idAutoriza , idSupervisa,armasTexto,drogasTexto,valoresTexto,prendasTexto,otrosTexto,
                                    observacionesConvenio, idEntidadCompetencia
                                    FROM accidentes a LEFT JOIN vehiculosAccidente b ON A.idAccidente = B.idAccidente 
                                    WHERE A.idAccidente = @IdAccidente
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
        public int EliminarRegistroInfraccion(int IdInfraccion)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE infraccionesAccidente SET estatus = 0 " +
                        "WHERE idInfraccion = @IdInfraccion";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@IdInfraccion", IdInfraccion);

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



        public List<CapturaAccidentesModel> ObtenerAccidentesPagination(int idOficina, Pagination pagination)
        {
            List<CapturaAccidentesModel> ListaAccidentes = new List<CapturaAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_ObtieneTodosLosAccidentes", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        cmd.Parameters.AddWithValue("@IdOficina", idOficina);
                        if (pagination.Filter.Trim() != "")
                            cmd.Parameters.AddWithValue("@Filter", pagination.Filter);

                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                CapturaAccidentesModel accidente = new CapturaAccidentesModel();
                                accidente.IdAccidente = reader["IdAccidente"] is DBNull ? 0 : Convert.ToInt32(reader["IdAccidente"]);
                                accidente.NumeroReporte = reader["NumeroReporte"] is DBNull ? string.Empty : reader["NumeroReporte"].ToString();
                                accidente.Fecha = reader["Fecha"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["Fecha"]);
                                accidente.Hora = reader["Hora"] is DBNull ? TimeSpan.MinValue : reader.GetTimeSpan(reader.GetOrdinal("Hora"));
                                accidente.IdMunicipio = reader["IdMunicipio"] is DBNull ? 0 : Convert.ToInt32(reader["IdMunicipio"]);
                                accidente.IdCarretera = reader["IdCarretera"] is DBNull ? 0 : Convert.ToInt32(reader["IdCarretera"]);
                                accidente.IdTramo = reader["IdTramo"] is DBNull ? 0 : Convert.ToInt32(reader["IdTramo"]);
                                accidente.idEstatusReporte = reader["idEstatusReporte"] is DBNull ? 0 : Convert.ToInt32(reader["idEstatusReporte"]);
                                accidente.EstatusReporte = reader["estatusReporte"] is DBNull ? string.Empty : reader["estatusReporte"].ToString();
                                accidente.Municipio = reader["Municipio"] is DBNull ? string.Empty : reader["Municipio"].ToString();
                                accidente.Tramo = reader["Tramo"] is DBNull ? string.Empty : reader["Tramo"].ToString();
                                accidente.Carretera = reader["Carretera"] is DBNull ? string.Empty : reader["Carretera"].ToString();
                                accidente.Total = Convert.ToInt32(reader["Total"]);
                                ListaAccidentes.Add(accidente);

                            }

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
}

















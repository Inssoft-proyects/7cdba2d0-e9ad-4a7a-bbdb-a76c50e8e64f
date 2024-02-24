using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Drawing;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;
using GuanajuatoAdminUsuarios.Util;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Newtonsoft.Json;
using GuanajuatoAdminUsuarios.RESTModels;
using Microsoft.Extensions.Options;
using static GuanajuatoAdminUsuarios.RESTModels.CotejarDatosResponseModel;
using GuanajuatoAdminUsuarios.Framework;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GuanajuatoAdminUsuarios.Services
{
    public class VehiculosService : IVehiculosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly AppSettings _appSettings;
        private readonly ICotejarDocumentosClientService _cotejarDocumentosClientService;
        private readonly IRepuveService _repuveService;
        private readonly ICatDictionary _catDictionary;
        private readonly IColores _coloresService;
        private readonly ICatMarcasVehiculosService _catMarcasVehiculosService;
        private readonly ICatSubmarcasVehiculosService _catSubmarcasVehiculosService;
        private readonly ICatEntidadesService _catEntidadesService;








        public VehiculosService(ISqlClientConnectionBD sqlClientConnectionBD, IOptions<AppSettings> appSettings, IRepuveService repuveService, ICotejarDocumentosClientService cotejarDocumentosClientService,
            ICatDictionary catDictionary, IColores coloresService, ICatMarcasVehiculosService catMarcasVehiculosService, ICatSubmarcasVehiculosService catSubmarcasVehiculosService,
            ICatEntidadesService catEntidadesService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _appSettings = appSettings.Value;
            _repuveService = repuveService;
            _cotejarDocumentosClientService = cotejarDocumentosClientService;
            _catDictionary = catDictionary;
            _coloresService = coloresService;
            _catMarcasVehiculosService = catMarcasVehiculosService;
            _catSubmarcasVehiculosService = catSubmarcasVehiculosService;
            _catEntidadesService = catEntidadesService;




        }

        public IEnumerable<VehiculoModel> GetAllVehiculos()
        {
            List<VehiculoModel> modelList = new List<VehiculoModel>();
            string strQuery = @"SELECT 
                                 v.idVehiculo
                                ,v.placas
                                ,v.serie
                                ,v.tarjeta
                                ,v.vigenciaTarjeta
                                ,v.idMarcaVehiculo
                                ,v.idSubmarca
                                ,v.idTipoVehiculo
                                ,v.modelo
                                ,v.idColor
                                ,v.idEntidad
                                ,v.idSubtipoServicio
                                ,v.propietario
                                ,v.numeroEconomico
                                ,v.paisManufactura
                                ,v.idPersona
                                ,v.fechaActualizacion
                                ,v.actualizadoPor
                                ,v.estatus
                                ,v.motor
                                ,p.idPersona
                                ,p.numeroLicencia
                                ,p.CURP
                                ,p.RFC
                                ,p.nombre
                                ,p.apellidoPaterno
                                ,p.apellidoMaterno
                                ,p.fechaActualizacion
                                ,p.actualizadoPor
                                ,p.estatus
                                ,p.idCatTipoPersona
								,cmv.marcaVehiculo
								,csv.nombreSubmarca
								,ce.nombreEntidad
								,cv.tipoVehiculo
								,cc.color
                                ,catTS.servicio
                                FROM vehiculos v
								LEFT JOIN catColores cc
								on v.idColor = cc.idColor AND cc.estatus = 1
								LEFT JOIN catTiposVehiculo cv
								on v.idTipoVehiculo = cv.idTipoVehiculo AND cv.estatus = 1
								LEFT JOIN catEntidades ce
								on v.idEntidad = ce.idEntidad AND ce.estatus = 1
                                LEFT JOIN personas p
                                on v.idPersona = p.idPersona AND p.estatus = 1
								LEFT JOIN catMarcasVehiculos cmv
								on v.idMarcaVehiculo = cmv.idMarcaVehiculo and cmv.estatus = 1
								LEFT JOIN catSubmarcasVehiculos csv
								on v.idSubmarca = csv.idSubmarca and csv.estatus = 1
                                LEFT JOIN catSubtipoServicio catTS on v.idSubtipoServicio = catTS.idSubtipoServicio 
                                WHERE v.estatus = 1
                                order by v.idVehiculo desc";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    int numeroSecuencial = 1;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            VehiculoModel model = new VehiculoModel();
                            model.Persona = new PersonaModel();
                            model.idVehiculo = Convert.ToInt32(reader["idVehiculo"]);
                            model.placas = reader["placas"].ToString();
                            model.serie = reader["serie"].ToString();
                            model.tarjeta = reader["tarjeta"].ToString();
                            model.vigenciaTarjeta = reader["vigenciaTarjeta"].GetType() == typeof(DBNull) ? null : Convert.ToDateTime(reader["vigenciaTarjeta"].ToString());
                            model.idMarcaVehiculo = Convert.ToInt32(reader["idMarcaVehiculo"].ToString());
                            model.idSubmarca = Convert.ToInt32(reader["idSubmarca"].ToString());
                            model.idTipoVehiculo = Convert.ToInt32(reader["idTipoVehiculo"].ToString());
                            model.modelo = reader["modelo"].ToString();
                            model.idColor = Convert.ToInt32(reader["idColor"].ToString());
                            model.idEntidad = Convert.ToInt32(reader["idEntidad"].ToString());
                            model.idSubtipoServicio = reader["idSubtipoServicio"].GetType() == typeof(DBNull) ? 0 : Convert.ToInt32(reader["idSubtipoServicio"].ToString());
                            model.numeroEconomico = reader["numeroEconomico"].ToString();
                            model.subTipoServicio = reader["servicio"].GetType() == typeof(DBNull) ?  "" : reader["servicio"].ToString();
                            model.fechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = Convert.ToInt32(reader["estatus"].ToString());
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : (int?)reader["idPersona"];
                            model.Persona.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : (int)reader["idPersona"];
                            model.Persona.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.Persona.CURP = reader["CURP"].ToString();
                            model.Persona.RFC = reader["RFC"].ToString();
                            model.motor = reader["motor"].ToString();
                            model.Persona.nombre = reader["nombre"].ToString();
                            model.Persona.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.Persona.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.Persona.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.Persona.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.Persona.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.Persona.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.marca = reader["marcaVehiculo"].ToString();
                            model.submarca = reader["nombreSubmarca"].ToString();
                            model.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            model.entidadRegistro = reader["nombreEntidad"].ToString();
                            model.color = reader["color"].ToString();
                            model.propietario = model.Persona.nombre + " " + model.Persona.apellidoPaterno + " " + model.Persona.apellidoMaterno;
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
        public int BuscarPorParametro(string Placa, string Serie, string Folio)
        {
            var Vehiculo = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command;

                    var query = @"SELECT count(*) result
                        FROM vehiculos v 
                        WHERE v.estatus = 1 AND {0}";


                    if (!string.IsNullOrEmpty(Serie))
                    {
                       query= string.Format(query, "(v.serie = @Serie  )");
                    }
                    else if (!string.IsNullOrEmpty(Placa))
                    {
                        query= string.Format(query, "( v.placas = + @placas  )");
                    }
                    else
                    {
                        query = string.Format(query, "");
                    }


                    command = new SqlCommand(
                        query, connection);

                    if (!string.IsNullOrEmpty(Serie))
                    {
                        command.Parameters.AddWithValue("@Serie", Serie);
                    }
                    else if (!string.IsNullOrEmpty(Placa))
                    {
                        command.Parameters.AddWithValue("@placas", Placa);
                    }




                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                           
                            Vehiculo = (int)reader["result"];
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

        public VehiculoModel GetVehiculoById(int idVehiculo)
        {
            List<VehiculoModel> modelList = new List<VehiculoModel>();
            string strQuery = @"SELECT 
                                 v.idVehiculo
                                ,v.placas
                                ,v.serie
                                ,v.tarjeta
                                ,v.vigenciaTarjeta
                                ,v.idMarcaVehiculo
                                ,v.idSubmarca
                                ,v.idTipoVehiculo
                                ,v.modelo
                                ,v.idColor
                                ,v.idEntidad
                                ,v.idCatTipoServicio
                                ,v.propietario
                                ,v.numeroEconomico
                                ,v.paisManufactura
                                ,v.idPersona
                                ,v.fechaActualizacion
                                ,v.actualizadoPor
                                ,v.estatus
                                ,v.motor,v.capacidad,v.poliza,v.otros, v.carga
                                ,p.idPersona
                                ,p.numeroLicencia
                                ,p.CURP
                                ,p.RFC
                                ,p.nombre
                                ,p.apellidoPaterno
                                ,p.apellidoMaterno
                                ,p.fechaActualizacion
                                ,p.actualizadoPor
                                ,p.estatus
                                ,p.idCatTipoPersona
								,cmv.marcaVehiculo
								,csv.nombreSubmarca
								,ce.nombreEntidad
								,cv.tipoVehiculo
								,cc.color
                                ,v.idSubtipoServicio
                                FROM vehiculos v
								LEFT JOIN catColores cc
								on v.idColor = cc.idColor AND cc.estatus = 1
								LEFT JOIN catTiposVehiculo cv
								on v.idTipoVehiculo = cv.idTipoVehiculo AND cv.estatus = 1
								LEFT JOIN catEntidades ce
								on v.idEntidad = ce.idEntidad AND ce.estatus = 1
                                LEFT JOIN personas p
                                on v.idPersona = p.idPersona AND p.estatus = 1
								LEFT JOIN catMarcasVehiculos cmv
								on v.idMarcaVehiculo = cmv.idMarcaVehiculo and cmv.estatus = 1
								LEFT JOIN catSubmarcasVehiculos csv
								on v.idSubmarca = csv.idSubmarca and csv.estatus = 1
                                WHERE v.estatus = 1
                                AND v.idVehiculo = @idVehiculo";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idVehiculo", SqlDbType.Int)).Value = (object)idVehiculo ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            VehiculoModel model = new VehiculoModel();
                            model.Persona = new PersonaModel();
                            model.idVehiculo = Convert.ToInt32(reader["idVehiculo"]);
                            model.placas = reader["placas"].GetType() == typeof(DBNull) ? "" : reader["placas"].ToString();
                            model.serie = reader["serie"].ToString();
                            model.tarjeta = reader["tarjeta"].ToString();
                            model.vigenciaTarjeta = reader["vigenciaTarjeta"].GetType() == typeof(DBNull) ? null : Convert.ToDateTime(reader["vigenciaTarjeta"].ToString());
                            model.idMarcaVehiculo = Convert.ToInt32(reader["idMarcaVehiculo"].ToString());
                            model.idSubmarca = Convert.ToInt32(reader["idSubmarca"].ToString());
                            model.idTipoVehiculo = Convert.ToInt32(reader["idTipoVehiculo"].ToString());
                            model.modelo = reader["modelo"].ToString();
                            model.idColor = Convert.ToInt32(reader["idColor"].ToString());
                            model.idEntidad = Convert.ToInt32(reader["idEntidad"].ToString());
                            model.idCatTipoServicio = Convert.ToInt32(reader["idCatTipoServicio"].ToString());
                            model.numeroEconomico = reader["numeroEconomico"].ToString();
                            model.fechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = Convert.ToInt32(reader["estatus"].ToString());
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : (int?)reader["idPersona"];
                            model.Persona.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : (int)reader["idPersona"];
                            model.Persona.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.Persona.CURP = reader["CURP"].ToString();
                            model.Persona.RFC = reader["RFC"].ToString();
                            model.Persona.nombre = reader["nombre"].ToString();
                            model.Persona.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.Persona.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.Persona.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.Persona.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.Persona.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.Persona.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.marca = reader["marcaVehiculo"].ToString();
                            model.submarca = reader["nombreSubmarca"].ToString();
                            model.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            model.entidadRegistro = reader["nombreEntidad"].ToString();
                            model.color = reader["color"].ToString();
                            model.propietario = model.Persona.nombre + " " + model.Persona.apellidoPaterno + " " + model.Persona.apellidoMaterno;
                            model.paisManufactura = reader["paisManufactura"].ToString();
                            model.motor = reader["motor"].ToString();
                            model.capacidad = reader["capacidad"] == System.DBNull.Value ? default(int?) : (int?)reader["capacidad"];
                            model.poliza = reader["poliza"].ToString();
                            model.carga = reader["carga"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["carga"].ToString());
                            model.otros = reader["otros"].ToString();
                            model.idSubtipoServicio = reader["idSubtipoServicio"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idSubtipoServicio"].ToString());
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

            VehiculoModel model22 = new VehiculoModel();
            model22.Persona = new PersonaModel();

            return modelList.FirstOrDefault()??model22;
        }

        public VehiculoModel GetVehiculoToAnexo(VehiculoBusquedaModel modelSearch)
        {
            VehiculoModel model = new VehiculoModel();
            //ToDo: Logica para buscar primero en el registro estatal
            //Busqueda en Sitteg:
            bool encontradoEnRegistroEstatal = false;

            string strQuery = @"SELECT
                                v.idVehiculo, v.placas, v.serie, v.tarjeta, v.vigenciaTarjeta, v.idMarcaVehiculo
                                ,v.idSubmarca, v.idTipoVehiculo, v.modelo, v.idColor, v.idEntidad, v.idCatTipoServicio
                                ,v.propietario, v.numeroEconomico, v.paisManufactura, v.idPersona
                                ,v.motor,v.capacidad,v.poliza,v.otros, v.carga
                                ,catMV.marcaVehiculo, catTV.tipoVehiculo, catSV.nombreSubmarca, catTS.tipoServicio
                                ,catE.nombreEntidad, catC.color  
                                FROM vehiculos v
                                INNER JOIN catMarcasVehiculos catMV on v.idMarcaVehiculo = catMV.idMarcaVehiculo 
                                left JOIN catTiposVehiculo catTV on v.idTipoVehiculo = catTV.idTipoVehiculo 
                                left JOIN catSubmarcasVehiculos catSV on v.idSubmarca = catSV.idSubmarca 
                                left JOIN catTipoServicio catTS on v.idCatTipoServicio = catTS.idCatTipoServicio 
                                left JOIN catEntidades catE on v.idEntidad = catE.idEntidad  
                                left JOIN catColores catC on v.idColor = catC.idColor  
                                WHERE v.estatus = 1
                                AND 
                                (
                                (v.idEntidad = @idEntidad  and v.serie= @Serie)
                                OR v.serie= @Serie
                                OR v.placas= @Placas 
                                )";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)modelSearch.IdEntidadBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Placas", SqlDbType.NVarChar)).Value = (object)modelSearch.PlacasBusqueda != null ? modelSearch.PlacasBusqueda.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Serie", SqlDbType.NVarChar)).Value = (object)modelSearch.SerieBusqueda != null ? modelSearch.SerieBusqueda.ToUpper() : DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            model.idVehiculo = Convert.ToInt32(reader["idVehiculo"]);
                            model.placas = reader["placas"].ToString();

                            model.serie = reader["serie"].ToString();
                            model.tarjeta = reader["tarjeta"].ToString();
                            model.vigenciaTarjeta = reader["vigenciaTarjeta"].GetType() == typeof(DBNull) ? null : Convert.ToDateTime(reader["vigenciaTarjeta"].ToString());
                            model.idMarcaVehiculo = Convert.ToInt32(reader["idMarcaVehiculo"]);
                            model.idSubmarca = Convert.ToInt32(reader["idSubmarca"]);
                            model.idTipoVehiculo = Convert.ToInt32(reader["idTipoVehiculo"]);

                            model.modelo = reader["modelo"].ToString();
                            model.idColor = Convert.ToInt32(reader["idColor"]);
                            model.idEntidad = Convert.ToInt32(reader["idEntidad"]);
                            model.idCatTipoServicio = Convert.ToInt32(reader["idCatTipoServicio"]);
                            model.propietario = reader["propietario"].ToString();
                            model.numeroEconomico = reader["numeroEconomico"].ToString();
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : (int?)reader["idPersona"];
                            model.paisManufactura = reader["paisManufactura"].ToString();
                            model.numeroEconomico = reader["numeroEconomico"].ToString();

                            model.marca = reader["marcaVehiculo"].ToString();
                            model.submarca = reader["nombreSubmarca"].ToString();
                            model.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            model.color = reader["color"].ToString();
                            model.entidadRegistro = reader["nombreEntidad"].ToString();
                            model.tipoServicio = reader["tipoServicio"].ToString();

                            model.motor = reader["motor"].ToString();
                            model.capacidad = reader["capacidad"] == System.DBNull.Value ? default(int?) : (int?)reader["capacidad"];
                            model.poliza = reader["poliza"].ToString();
                            model.carga = reader["carga"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["carga"].ToString());
                            model.otros = reader["otros"].ToString();

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

            if (encontradoEnRegistroEstatal)
            {
                model.encontradoEn = (int)EstatusBusquedaVehiculo.RegistroEstatal;
            }
            else
            if (model.idVehiculo != 0)
            {
                model.encontradoEn = (int)EstatusBusquedaVehiculo.Sitteg;
            }
            else
            {
                model.encontradoEn = (int)EstatusBusquedaVehiculo.NoEncontrado;
                model.serie = modelSearch.SerieBusqueda;
            }
            return model;
        }

        public List<VehiculoModel> GetVehiculos(VehiculoBusquedaModel modelSearch)
        {
            List<VehiculoModel> ListVehiculos = new List<VehiculoModel>();
            //ToDo: Revisar si se buscar primero en el registro estatal
            //Busqueda en Sitteg:


            string sqlCondiciones = "";
            sqlCondiciones += (object)modelSearch.IdEntidadBusqueda == null ? "" : " v.idEntidad = @idEntidad AND \n";
            sqlCondiciones += (object)modelSearch.SerieBusqueda == null ? "" : " v.serie  LIKE '%' + @Serie + '%' AND \n";
            sqlCondiciones += (object)modelSearch.PlacasBusqueda == null ? "" : " v.placas LIKE '%' + @Placas + '%'  AND \n";
            sqlCondiciones += (object)modelSearch.tarjeta == null ? "" : " v.tarjeta LIKE '%' + @Tarjeta + '%' AND \n";
            sqlCondiciones += (object)modelSearch.motor == null ? "" : " v.motor LIKE '%' + @Motor + '%' AND \n";
            sqlCondiciones += (object)modelSearch.modelo == null ? "" : " v.modelo LIKE '%' + @Modelo + '%' AND \n";
            sqlCondiciones += (object)modelSearch.numeroEconomico == null ? "" : " v.numeroEconomico LIKE '%' + @NumeroEconomico + '%' AND \n";
            sqlCondiciones += (object)modelSearch.propietario == null ? "" : " v.propietario LIKE '%' + @Propietario + '%' AND \n";
            sqlCondiciones += (object)modelSearch.idMarca == null ? "" : " v.idMarcaVehiculo = @idMarca AND \n";
            sqlCondiciones += (object)modelSearch.idSubMarca == null ? "" : " v.idSubMarca = @idSubMarca AND \n";
            sqlCondiciones += (object)modelSearch.idTipoVehiculo == null ? "" : " v.idTipoVehiculo = @idTipoVehiculo AND \n";
            sqlCondiciones += (object)modelSearch.idSubtipoServicio == null ? "" : " v.idSubtipoServicio = @idSubTipoServicio AND \n";
            sqlCondiciones += (object)modelSearch.idColor == null ? "" : " v.idColor = @idColor AND \n";

            if (sqlCondiciones.Length > 0)
            {
                sqlCondiciones = sqlCondiciones.Remove(sqlCondiciones.Length - 5);
                sqlCondiciones = "AND( " + sqlCondiciones + " )";

            }

            string strQuery = string.Format(@"SELECT TOP 200
                                v.idVehiculo, v.placas, v.serie, v.tarjeta, v.vigenciaTarjeta, v.idMarcaVehiculo
                                ,v.idSubmarca, v.idTipoVehiculo, v.modelo, v.idColor, v.idEntidad, v.idCatTipoServicio
                                ,v.propietario, v.numeroEconomico, v.paisManufactura, v.idPersona
                                ,v.motor,v.capacidad,v.poliza,v.otros, v.carga
                                ,catMV.marcaVehiculo, catTV.tipoVehiculo, catSV.nombreSubmarca, catTS.servicio
                                ,catE.nombreEntidad, catC.color,p.nombre
                                ,p.apellidoPaterno
                                ,p.apellidoMaterno  
                                ,'' TipoServicio
                                FROM vehiculos v
                                LEFT JOIN catMarcasVehiculos catMV on v.idMarcaVehiculo = catMV.idMarcaVehiculo 
                                LEFT JOIN catTiposVehiculo catTV on v.idTipoVehiculo = catTV.idTipoVehiculo 
                                LEFT JOIN catSubmarcasVehiculos catSV on v.idSubmarca = catSV.idSubmarca 
                                LEFT JOIN catSubtipoServicio catTS on v.idSubtipoServicio = catTS.idSubtipoServicio 
                                LEFT JOIN personas p on v.idPersona = p.idPersona 
                                LEFT JOIN catEntidades catE on v.idEntidad = catE.idEntidad  
                                LEFT JOIN catColores catC on v.idColor = catC.idColor  
                                WHERE v.estatus = 1
                                    {0} ORDER BY v.idVehiculo DESC;
                                ", sqlCondiciones);
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    int numeroSecuencial = 1;
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)modelSearch.IdEntidadBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Placas", SqlDbType.NVarChar)).Value = (object)modelSearch.PlacasBusqueda != null ? modelSearch.PlacasBusqueda.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Serie", SqlDbType.NVarChar)).Value = (object)modelSearch.SerieBusqueda != null ? modelSearch.SerieBusqueda.ToUpper() : DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@Tarjeta", SqlDbType.NVarChar)).Value = (object)modelSearch.tarjeta != null ? modelSearch.tarjeta.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Motor", SqlDbType.NVarChar)).Value = (object)modelSearch.motor != null ? modelSearch.motor.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Modelo", SqlDbType.NVarChar)).Value = (object)modelSearch.modelo != null ? modelSearch.modelo.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@NumeroEconomico", SqlDbType.NVarChar)).Value = (object)modelSearch.numeroEconomico != null ? modelSearch.numeroEconomico.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Propietario", SqlDbType.NVarChar)).Value = (object)modelSearch.propietario != null ? modelSearch.propietario.ToUpper() : DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@idMarca", SqlDbType.Int)).Value = (object)modelSearch.idMarca ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSubMarca", SqlDbType.Int)).Value = (object)modelSearch.idSubMarca ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)modelSearch.idTipoVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSubTipoServicio", SqlDbType.Int)).Value = (object)modelSearch.idSubtipoServicio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idColor", SqlDbType.Int)).Value = (object)modelSearch.idColor ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            VehiculoModel model = new VehiculoModel();
                            model.Persona = new PersonaModel();
                            model.idVehiculo = Convert.ToInt32(reader["idVehiculo"]);
                            model.placas = reader["placas"].ToString();

                            model.serie = reader["serie"].ToString();
                            model.tarjeta = reader["tarjeta"].ToString();
                            model.vigenciaTarjeta = reader["vigenciaTarjeta"] == DBNull.Value
                                ? (DateTime?)null 
                                : Convert.ToDateTime(reader["vigenciaTarjeta"].ToString());
                            model.idMarcaVehiculo = Convert.ToInt32(reader["idMarcaVehiculo"]);
                            model.idSubmarca = Convert.ToInt32(reader["idSubmarca"]);
                            model.idTipoVehiculo = Convert.ToInt32(reader["idTipoVehiculo"]);

                            model.modelo = reader["modelo"].ToString();
                            model.idColor = Convert.ToInt32(reader["idColor"]);
                            model.idEntidad = Convert.ToInt32(reader["idEntidad"]);
                            model.idCatTipoServicio = Convert.ToInt32(reader["idCatTipoServicio"]);
                            model.numeroEconomico = reader["numeroEconomico"].ToString();
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : (int?)reader["idPersona"];
                            model.paisManufactura = reader["paisManufactura"].ToString();
                            model.numeroEconomico = reader["numeroEconomico"].ToString();
                            model.marca = reader["marcaVehiculo"].ToString();
                            model.submarca = reader["nombreSubmarca"].ToString();
                            model.tipoVehiculo = reader["tipoVehiculo"].ToString();
                            model.color = reader["color"].ToString();
                            model.entidadRegistro = reader["nombreEntidad"].ToString();
                            model.tipoServicio = reader["tipoServicio"].ToString();
                            model.subTipoServicio = reader["servicio"].ToString();
                            model.Persona.nombre = reader["nombre"].ToString();
                            model.Persona.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.Persona.apellidoMaterno = reader["apellidoMaterno"].ToString(); model.motor = reader["motor"].ToString();
                            model.propietario = model.Persona.nombre + " " + model.Persona.apellidoPaterno + " " + model.Persona.apellidoMaterno;
                            model.capacidad = reader["capacidad"] == System.DBNull.Value ? default(int?) : (int?)reader["capacidad"];
                            model.poliza = reader["poliza"].ToString();
                            model.carga = reader["carga"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["carga"].ToString());
                            model.otros = reader["otros"].ToString();
                            model.encontradoEn = (int)EstatusBusquedaVehiculo.Sitteg;
                            model.NumeroSecuencial = numeroSecuencial;
                            ListVehiculos.Add(model);

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
            return ListVehiculos;
        }

        public int CreateVehiculo(VehiculoModel model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO vehiculos(
                                 placas
                                ,serie
                                ,tarjeta
                                ,vigenciaTarjeta
                                ,idMarcaVehiculo
                                ,idSubmarca
                                ,idTipoVehiculo
                                ,modelo
                                ,idColor
                                ,idEntidad
                                ,idCatTipoServicio
                                ,propietario
                                ,numeroEconomico
                                ,paisManufactura
                                ,idPersona
                                ,fechaActualizacion
                                ,actualizadoPor
                                ,estatus
                                ,motor
                                ,capacidad
                                ,poliza
                                ,carga
                                ,otros
                                ,idSubtipoServicio
                                ) VALUES (
                                @placas
                                ,@serie
                                ,@tarjeta
                                ,@vigenciaTarjeta
                                ,@idMarcaVehiculo
                                ,@idSubmarca
                                ,@idTipoVehiculo
                                ,@modelo
                                ,@idColor
                                ,@idEntidad
                                ,@idCatTipoServicio
                                ,@propietario
                                ,@numeroEconomico
                                ,@paisManufactura
                                ,@idPersona
                                ,@fechaActualizacion
                                ,@actualizadoPor
                                ,@estatus
                                ,@motor
                                ,@capacidad
                                ,@poliza
                                ,@carga
                                ,@otros
                                ,@idSubtipoServicio
                                );select CAST (SCOPE_IDENTITY() As int)";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {

                    DateTime? t = model.vigenciaTarjeta != null && model.vigenciaTarjeta.Value.Year > 1 ? model.vigenciaTarjeta : null;

                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);

                    command.Parameters.Add(new SqlParameter("@placas", SqlDbType.NVarChar)).Value = (object)model.placas ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@serie", SqlDbType.NVarChar)).Value = (object)model.serie ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@tarjeta", SqlDbType.NVarChar)).Value = (object)model.tarjeta ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@vigenciaTarjeta", SqlDbType.DateTime)).Value = (object)t ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMarcaVehiculo", SqlDbType.Int)).Value = (object)model.idMarcaVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSubmarca", SqlDbType.Int)).Value = (object)model.idSubmarca ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)model.idTipoVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@modelo", SqlDbType.NVarChar)).Value = (object)model.modelo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idColor", SqlDbType.Int)).Value = (object)model.idColor ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.idEntidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.Int)).Value = (object)model.idCatTipoServicio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@propietario", SqlDbType.NVarChar)).Value = (object)model.propietario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroEconomico", SqlDbType.NVarChar)).Value = (object)model.numeroEconomico ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@paisManufactura", SqlDbType.NVarChar)).Value = (object)model.paisManufactura ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)model.Persona.idPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@motor", SqlDbType.NVarChar)).Value = (object)model.motor ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@capacidad", SqlDbType.Int)).Value = (object)model.capacidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@poliza", SqlDbType.NVarChar)).Value = (object)model.poliza ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@carga", SqlDbType.Int)).Value = (object)model.cargaInt ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@otros", SqlDbType.NVarChar)).Value = (object)model.otros ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSubtipoServicio", SqlDbType.Int)).Value = (object)model.idSubtipoServicio ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    result = Convert.ToInt32(command.ExecuteScalar());
                    //result = command.ExecuteNonQuery();
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

        public int UpdateVehiculo(VehiculoModel model)
        {
            int result = 0;
            string strQuery = @"Update vehiculos
                                set				   
                                placas			   = @placas
                                ,serie			   = @serie
                                ,tarjeta		   = @tarjeta
                                ,vigenciaTarjeta   = @vigenciaTarjeta
                                ,idMarcaVehiculo   = @idMarcaVehiculo
                                ,idSubmarca		   = @idSubmarca
                                ,idTipoVehiculo	   = @idTipoVehiculo
                                ,modelo			   = @modelo
                                ,idColor		   = @idColor
                                ,idEntidad		   = @idEntidad
                                ,idCatTipoServicio = @idCatTipoServicio
                                ,propietario	   = @propietario
                                ,numeroEconomico   = @numeroEconomico
                                ,paisManufactura   = @paisManufactura
                                ,idPersona		   = @idPersona
                                ,fechaActualizacion= @fechaActualizacion
                                ,actualizadoPor	   = @actualizadoPor
                                ,estatus		   = @estatus
                                ,motor			   = @motor
                                ,capacidad		   = @capacidad
                                ,poliza			   = @poliza
                                ,carga			   = @carga
                                ,otros			   = @otros
                                ,idSubtipoServicio = @idSubtipoServicio
                                where idVehiculo= @idVehiculo
                                ";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idVehiculo", SqlDbType.Int)).Value = (object)model.idVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placas", SqlDbType.NVarChar)).Value = (object)model.placas ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@serie", SqlDbType.NVarChar)).Value = (object)model.serie ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@tarjeta", SqlDbType.NVarChar)).Value = (object)model.tarjeta ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@vigenciaTarjeta", SqlDbType.DateTime)).Value = (object)model.vigenciaTarjeta ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMarcaVehiculo", SqlDbType.Int)).Value = (object)model.idMarcaVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSubmarca", SqlDbType.Int)).Value = (object)model.idSubmarca ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoVehiculo", SqlDbType.Int)).Value = (object)model.idTipoVehiculo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@modelo", SqlDbType.NVarChar)).Value = (object)model.modelo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idColor", SqlDbType.Int)).Value = (object)model.idColor ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.idEntidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idCatTipoServicio", SqlDbType.Int)).Value = (object)model.idCatTipoServicio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@propietario", SqlDbType.NVarChar)).Value = (object)model.propietario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroEconomico", SqlDbType.NVarChar)).Value = (object)model.numeroEconomico ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@paisManufactura", SqlDbType.NVarChar)).Value = (object)model.paisManufactura ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)model.Persona.idPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@motor", SqlDbType.NVarChar)).Value = (object)model.motor ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@capacidad", SqlDbType.Int)).Value = (object)model.capacidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@poliza", SqlDbType.NVarChar)).Value = (object)model.poliza ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@carga", SqlDbType.Bit)).Value = (object)model.carga ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@otros", SqlDbType.NVarChar)).Value = (object)model.otros ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSubtipoServicio", SqlDbType.Int)).Value = (object)model.idSubtipoServicio ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    //result = Convert.ToInt32(command.ExecuteScalar());
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
            }
            return result;
        }


        // HMG - 16 01 2024
        /// <summary>
        /// Paginado para la carga de vehículos
        /// </summary>
        /// <param name="pagination">Objeto con el numero de pagina y registros a regresar</param>
        /// <returns></returns>
        public IEnumerable<VehiculoModel> GetAllVehiculosPagination(Pagination pagination)
        {
            List<VehiculoModel> modelList = new List<VehiculoModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    int numeroSecuencial = 1;
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_ObtieneTodosLosVehiculos", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        if (pagination.Filter.Trim() != "")
                            cmd.Parameters.AddWithValue("@Filter", pagination.Filter);

                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                VehiculoModel model = new VehiculoModel();
                                model.Persona = new PersonaModel();
                                model.idVehiculo = Convert.ToInt32(reader["idVehiculo"]);
                                model.placas = reader["placas"].ToString();
                                model.serie = reader["serie"].ToString();
                                model.tarjeta = reader["tarjeta"].ToString();
                                model.vigenciaTarjeta = reader["vigenciaTarjeta"].GetType() == typeof(DBNull) ? null : Convert.ToDateTime(reader["vigenciaTarjeta"].ToString());
                                model.idMarcaVehiculo = Convert.ToInt32(reader["idMarcaVehiculo"].ToString());
                                model.idSubmarca = Convert.ToInt32(reader["idSubmarca"].ToString());
                                model.idTipoVehiculo = Convert.ToInt32(reader["idTipoVehiculo"].ToString());
                                model.modelo = reader["modelo"].ToString();
                                model.idColor = Convert.ToInt32(reader["idColor"].ToString());
                                model.idEntidad = Convert.ToInt32(reader["idEntidad"].ToString());
                                model.idSubtipoServicio = reader["idSubtipoServicio"].GetType() == typeof(DBNull) ? 0 : Convert.ToInt32(reader["idSubtipoServicio"].ToString());
                                model.numeroEconomico = reader["numeroEconomico"].ToString();
                                model.subTipoServicio = reader["servicio"].GetType() == typeof(DBNull) ? "" : reader["servicio"].ToString();
                                model.fechaActualizacion = Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                                model.actualizadoPor = Convert.ToInt32(reader["actualizadoPor"].ToString());
                                model.estatus = Convert.ToInt32(reader["estatus"].ToString());
                                model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : (int?)reader["idPersona"];
                                model.Persona.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : (int)reader["idPersona"];
                                model.Persona.numeroLicencia = reader["numeroLicencia"].ToString();
                                model.Persona.CURP = reader["CURP"].ToString();
                                model.Persona.RFC = reader["RFC"].ToString();
                                model.motor = reader["motor"].ToString();
                                model.Persona.nombre = reader["nombre"].ToString();
                                model.Persona.apellidoPaterno = reader["apellidoPaterno"].ToString();
                                model.Persona.apellidoMaterno = reader["apellidoMaterno"].ToString();
                                model.Persona.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                                model.Persona.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                                model.Persona.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                                model.Persona.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                                model.marca = reader["marcaVehiculo"].ToString();
                                model.submarca = reader["nombreSubmarca"].ToString();
                                model.tipoVehiculo = reader["tipoVehiculo"].ToString();
                                model.entidadRegistro = reader["nombreEntidad"].ToString();
                                model.color = reader["color"].ToString();
                                model.propietario = model.Persona.nombre + " " + model.Persona.apellidoPaterno + " " + model.Persona.apellidoMaterno;
                                model.NumeroSecuencial = numeroSecuencial;
                                model.total = Convert.ToInt32(reader["Total"]);
                                modelList.Add(model);
                                numeroSecuencial++;

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
            }
            return modelList;
        }


        // HMG - 25 01 2024
        /// <summary>
        /// Paginado para la carga de vehículos - Modulo - Modificar vehículo
        /// </summary>
        /// <param name="pagination">Objeto con el numero de pagina y registros a regresar</param>
        /// <returns></returns>
        public List<VehiculoModel> GetVehiculosPagination(VehiculoBusquedaModel modelSearch, Pagination pagination)
        {
            List<VehiculoModel> ListVehiculos = new List<VehiculoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    int numeroSecuencial = 1;
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_ObtieneTodosLosVehiculosFiltros", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PageIndex", pagination.PageIndex);
                        cmd.Parameters.AddWithValue("@PageSize", pagination.PageSize);
                        cmd.Parameters.AddWithValue("@idEntidad", modelSearch.IdEntidadBusqueda);
                        cmd.Parameters.AddWithValue("@Serie", modelSearch.SerieBusqueda);
                        cmd.Parameters.AddWithValue("@Placas", modelSearch.PlacasBusqueda);
                        cmd.Parameters.AddWithValue("@Tarjeta", modelSearch.tarjeta);
                        cmd.Parameters.AddWithValue("@Motor", modelSearch.motor);
                        cmd.Parameters.AddWithValue("@Modelo", modelSearch.modelo);
                        cmd.Parameters.AddWithValue("@NumeroEconomico", modelSearch.numeroEconomico);
                        cmd.Parameters.AddWithValue("@Propietario", modelSearch.propietario);
                        cmd.Parameters.AddWithValue("@idMarca", modelSearch.idMarca);
                        cmd.Parameters.AddWithValue("@idSubMarca", modelSearch.idSubMarca);
                        cmd.Parameters.AddWithValue("@idTipoVehiculo", modelSearch.idTipoVehiculo);
                        cmd.Parameters.AddWithValue("@idSubTipoServicio", modelSearch.idSubtipoServicio);
                        cmd.Parameters.AddWithValue("@idColor", modelSearch.idColor);

                        using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                VehiculoModel model = new VehiculoModel();
                                model.Persona = new PersonaModel();
                                model.idVehiculo = Convert.ToInt32(reader["idVehiculo"]);
                                model.placas = reader["placas"].ToString();

                                model.serie = reader["serie"].ToString();
                                model.tarjeta = reader["tarjeta"].ToString();
                                model.vigenciaTarjeta = reader["vigenciaTarjeta"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader["vigenciaTarjeta"].ToString());
                                model.idMarcaVehiculo = Convert.ToInt32(reader["idMarcaVehiculo"]);
                                model.idSubmarca = Convert.ToInt32(reader["idSubmarca"]);
                                model.idTipoVehiculo = Convert.ToInt32(reader["idTipoVehiculo"]);

                                model.modelo = reader["modelo"].ToString();
                                model.idColor = Convert.ToInt32(reader["idColor"]);
                                model.idEntidad = Convert.ToInt32(reader["idEntidad"]);
                                model.idCatTipoServicio = Convert.ToInt32(reader["idCatTipoServicio"]);
                                model.numeroEconomico = reader["numeroEconomico"].ToString();
                                model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : (int?)reader["idPersona"];
                                model.paisManufactura = reader["paisManufactura"].ToString();
                                model.numeroEconomico = reader["numeroEconomico"].ToString();
                                model.marca = reader["marcaVehiculo"].ToString();
                                model.submarca = reader["nombreSubmarca"].ToString();
                                model.tipoVehiculo = reader["tipoVehiculo"].ToString();
                                model.color = reader["color"].ToString();
                                model.entidadRegistro = reader["nombreEntidad"].ToString();
                                model.tipoServicio = reader["tipoServicio"].ToString();
                                model.subTipoServicio = reader["servicio"].ToString();
                                model.Persona.nombre = reader["nombre"].ToString();
                                model.Persona.apellidoPaterno = reader["apellidoPaterno"].ToString();
                                model.Persona.apellidoMaterno = reader["apellidoMaterno"].ToString(); model.motor = reader["motor"].ToString();
                                model.propietario = model.Persona.nombre + " " + model.Persona.apellidoPaterno + " " + model.Persona.apellidoMaterno;
                                model.capacidad = reader["capacidad"] == System.DBNull.Value ? default(int?) : (int?)reader["capacidad"];
                                model.poliza = reader["poliza"].ToString();
                                model.carga = reader["carga"] == System.DBNull.Value ? default(bool?) : Convert.ToBoolean(reader["carga"].ToString());
                                model.otros = reader["otros"].ToString();
                                model.encontradoEn = (int)EstatusBusquedaVehiculo.Sitteg;
                                model.NumeroSecuencial = numeroSecuencial;
                                model.total = Convert.ToInt32(reader["Total"]);
                                ListVehiculos.Add(model);

                                numeroSecuencial++;
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
            }
            return ListVehiculos;
        }


        private bool ValidarRobo(RepuveConsgralRequestModel repuveGralModel)
        {
            var estatus = false;

            var repuveConsRoboResponse = _repuveService.ConsultaRobo(repuveGralModel)?.FirstOrDefault() ?? new RepuveConsRoboResponseModel();

            estatus = repuveConsRoboResponse.estatus == "1";

            return estatus;
        }

        private int ObtenerIdEntidadRepuve(string entidad)
        {
            int idEntidad = 0;
            var Entidad = _catDictionary.GetCatalog("CatEntidades", "0");
            idEntidad = Entidad.CatalogList
                .Where(w => RemoveDiacritics(w.Text.ToLower()).Contains(RemoveDiacritics(entidad.ToLower())))
                .Select(s => s.Id)
                .FirstOrDefault();
            return (idEntidad);
        }
        private  string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        private int ObtenerIdColor(string color)
        {
            string colorLimpio = Regex.Replace(color, "[0-9-]", "").Trim();
            var idColor = _coloresService.obtenerIdPorColor(colorLimpio);
            return (idColor);
        }
        private int ObtenerIdMarcaRepuve(string marca)
        {

            string marcaLimpio = marca.Trim();

            var idMarca = _catMarcasVehiculosService.obtenerIdPorMarca(marcaLimpio);
            return idMarca;


        }
        private int ObtenerIdSubmarcaRepuve(string submarca)
        {

            string submarcaLimpio = submarca.Trim();

            var idMarca = _catSubmarcasVehiculosService.obtenerIdPorSubmarca(submarcaLimpio);
            return idMarca;


        }
        private int ObtenerIdTipoServicioRepuve(string servicio)
        {
            int TipoServicio = 0;

            var Tipo = _catDictionary.GetCatalog("CatTipoServicio", "0");

            TipoServicio = Tipo.CatalogList.Where(w => servicio.ToLower().Contains(w.Text.ToLower())).Select(s => s.Id).FirstOrDefault();

            return (int)TipoServicio;
        }
        private int ObtenerIdTipoVehiculo(string categoria)
        {

            int idTipo = 0;

            var tipoVehiculo = _catDictionary.GetCatalog("CatTiposVehiculo", "0");

            idTipo = tipoVehiculo.CatalogList.Where(w => categoria.ToLower().Contains(w.Text.ToLower())).Select(s => s.Id).FirstOrDefault();

            return (idTipo);

        }

        private int ObtenerIdMunicipioDesdeBD(string municipio)
        {
            int idMunicipio = 0;

            var municipioStr = _catDictionary.GetCatalog("CatMunicipios", "0");

            idMunicipio = municipioStr.CatalogList
                            .Where(w => RemoveDiacritics(w.Text.ToLower()).Contains(RemoveDiacritics(municipio.ToLower())))
                            .Select(s => s.Id)
                            .FirstOrDefault();
            return (idMunicipio);
        }

        private int ObtenerIdEntidadDesdeBD(string entidad)
        {
            var idEntidad = _catEntidadesService.obtenerIdPorEntidad(entidad);
            return (idEntidad);
        }
        private int ObtenerIdMarca(string marca)
        {
            string[] partes = marca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string marcaLimpio = partes[1].Trim();

                var idMarca = _catMarcasVehiculosService.obtenerIdPorMarca(marcaLimpio);
                return idMarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }

        private int ObtenerIdSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                var idMarca = _catSubmarcasVehiculosService.obtenerIdPorSubmarca(submarcaLimpio);
                return idMarca;
            }

            return 0; // Valor predeterminado en caso de no encontrar el guión
        }



        private string ObtenerSubmarca(string submarca)
        {
            string[] partes = submarca.Split(new[] { '-' }, 2);

            if (partes.Length > 1)
            {
                string submarcaLimpio = partes[1].Trim();

                return submarcaLimpio;
            }

            return "NA"; // Valor predeterminado en caso de no encontrar el guión
        }

        private long LimpiarValorTelefono(string telefono)
        {
            telefono = telefono.Replace(" ", "");

            long telefonoValido;

            if (long.TryParse(telefono, out telefonoValido))
            {
                return telefonoValido;
            }
            else
            {
                return 0; // O algún otro valor que indique que no es válido
            }
        }

        private bool ConvertirBool(string carga)
        {
            bool cargaBool = false;

            if (carga.Trim() == "1.00")
            {
                cargaBool = true;
            }
            else if (carga.Trim() == "0.00")
            {
                cargaBool = false;
            }
            return (cargaBool);
        }

        private bool ConvertirGeneroBool(string sexo)
        {
            if (sexo == "2")
            {
                return true;
            }
            else if (sexo == "1")
            {
                return false;
            }
            else
            {
                return false;
            }

        }

        private int ObtenerIdTipoServicio(string servicio)
        {
            int servicioNumero = int.Parse(servicio.TrimStart('0'));
            var idTipoVehiculo = _catDictionary.GetCatalog("CatTipoServicio", "0");

            var tipoServicio = idTipoVehiculo.CatalogList.FirstOrDefault(item => item.Id == servicioNumero)?.Id;

            return (int)tipoServicio;
        }

        private VehiculoModel GetVEiculoModelFromFinanzas(RootCotejarDatosRes result)
        {
            var vehiculoEncontradoData = result.MT_CotejarDatos_res.tb_vehiculo[0];
            var vehiculoDireccionData = result.MT_CotejarDatos_res.tb_direccion[0];
            var vehiculoInterlocutorData = result.MT_CotejarDatos_res;
            var idMunicipio = !string.IsNullOrEmpty(vehiculoDireccionData.municipio)
                  ? ObtenerIdMunicipioDesdeBD(vehiculoDireccionData.municipio)
                  : 0;

            var idEntidad = !string.IsNullOrEmpty(vehiculoDireccionData.entidadreg)
                ? ObtenerIdEntidadDesdeBD(vehiculoDireccionData.entidadreg)
                : 0;


            var idColor = !string.IsNullOrEmpty(vehiculoEncontradoData.color)
                ? ObtenerIdColor(vehiculoEncontradoData.color)
                : 0;

            var idMarca = !string.IsNullOrEmpty(vehiculoEncontradoData.marca)
                ? ObtenerIdMarca(vehiculoEncontradoData.marca)
                : 0;

            var idSubmarca = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                ? ObtenerIdSubmarca(vehiculoEncontradoData.linea)
                : 0;
            var submarcaLimpio = !string.IsNullOrEmpty(vehiculoEncontradoData.linea)
                ? ObtenerSubmarca(vehiculoEncontradoData.linea)
                : "NA";
            var telefonoValido = !string.IsNullOrEmpty(vehiculoDireccionData.telefono)
                ? LimpiarValorTelefono(vehiculoDireccionData.telefono)
                : 0;
            var cargaBool = ConvertirBool(vehiculoEncontradoData.carga);
            var generoBool = ConvertirGeneroBool(vehiculoInterlocutorData.es_per_fisica?.sexo);

            var idTipo = !string.IsNullOrEmpty(vehiculoEncontradoData.categoria)
             ? ObtenerIdTipoVehiculo(vehiculoEncontradoData.categoria)
             : 0;
            var idTipoServicio = !string.IsNullOrEmpty(vehiculoEncontradoData.servicio)
            ? ObtenerIdTipoServicio(vehiculoEncontradoData.servicio)
            : 0;
            var vehiculoEncontrado = new VehiculoModel
            {
                placas = vehiculoEncontradoData.no_placa,
                serie = vehiculoEncontradoData.no_serie,
                tarjeta = vehiculoEncontradoData.no_tarjeta,
                motor = vehiculoEncontradoData.no_motor,
                otros = vehiculoEncontradoData.otros,
                idColor = idColor,
                idEntidad = idEntidad,
                idMarcaVehiculo = idMarca,
                idSubmarca = idSubmarca,
                submarca = submarcaLimpio,
                idTipoVehiculo = idTipo,
                modelo = vehiculoEncontradoData.modelo,
                capacidad = vehiculoEncontradoData.numpersona,
                carga = cargaBool,
                idCatTipoServicio = idTipoServicio,
                idTipoPersona = vehiculoInterlocutorData.es_per_fisica != null ? 1 : 2,

                Persona = new PersonaModel
                {
                    nombreFisico = vehiculoInterlocutorData.es_per_fisica?.Nombre,
                    apellidoPaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_paterno,
                    apellidoMaternoFisico = vehiculoInterlocutorData.es_per_fisica?.Ape_materno,
                    fechaNacimiento = vehiculoInterlocutorData.es_per_fisica?.Fecha_nacimiento,
                    CURPFisico = vehiculoInterlocutorData.es_per_fisica?.Nro_curp,
                    generoBool = generoBool,
                    nombre = vehiculoInterlocutorData.es_per_moral?.name_org1,
                    RFC = vehiculoInterlocutorData.Nro_rfc,


                    PersonaDireccion = new PersonaDireccionModel
                    {

                        telefono = vehiculoInterlocutorData.es_per_moral != null ? telefonoValido.ToString() : null,
                        telefonoFisico = vehiculoInterlocutorData.es_per_fisica != null ? telefonoValido.ToString() : null,
                        colonia = vehiculoInterlocutorData.es_per_moral != null ? vehiculoDireccionData.colonia : null,
                        coloniaFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.colonia : null,
                        calle = vehiculoInterlocutorData.es_per_moral != null ? vehiculoDireccionData.calle : null,
                        calleFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.calle : null,
                        numero = vehiculoInterlocutorData.es_per_moral != null ? vehiculoDireccionData.nro_exterior : null,
                        numeroFisico = vehiculoInterlocutorData.es_per_fisica != null ? vehiculoDireccionData.nro_exterior : null,
                        idMunicipio = vehiculoInterlocutorData.es_per_moral != null ? idMunicipio : null,
                        idMunicipioFisico = vehiculoInterlocutorData.es_per_fisica != null ? idMunicipio : null,
                        idEntidad = vehiculoInterlocutorData.es_per_moral != null ? idEntidad : null,
                        idEntidadFisico = vehiculoInterlocutorData.es_per_fisica != null ? idEntidad : null,
                    }
                },

                PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel
                {
                    PersonasMorales = new List<PersonaModel>()
                }
            };

            return vehiculoEncontrado;

        }






        public VehiculoModel GetModles(VehiculoBusquedaModel model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
            Logger.Debug("Infracciones - ajax_BuscarVehiculo - Request:" + request);
            var vehiculosModel = new VehiculoModel();

            RepuveConsgralRequestModel repuveGralModel = new RepuveConsgralRequestModel(model.PlacasBusqueda, model.SerieBusqueda);
            Logger.Debug("Infracciones - ajax_BuscarVehiculo - ValidarRobo ");
            vehiculosModel.ReporteRobo = ValidarRobo(repuveGralModel);

            var allowSistem = _appSettings.AllowWebServicesRepuve;

            Logger.Debug("Infracciones - ajax_BuscarVehiculo - GetVehiculoToAnexo");
            vehiculosModel =GetVehiculoToAnexo(model);
            vehiculosModel.idSubmarcaUpdated = vehiculosModel.idSubmarca;
            vehiculosModel.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel();
            vehiculosModel.PersonaMoralBusquedaModel.PersonasMorales = new List<PersonaModel>();

            if (vehiculosModel.idVehiculo > 0)
            {
                    return vehiculosModel;

            }

            if (allowSistem)
            {
                CotejarDatosRequestModel cotejarDatosRequestModel = new CotejarDatosRequestModel();
                cotejarDatosRequestModel.Tp_folio = "4";
                cotejarDatosRequestModel.Folio = model.PlacasBusqueda;
                cotejarDatosRequestModel.tp_consulta = "3";
                var endPointName = "CotejarDatosEndPoint";
                Logger.Debug("Infracciones - ajax_BuscarVehiculo - CotejarDatos");
                var result = _cotejarDocumentosClientService.CotejarDatos(cotejarDatosRequestModel, endPointName);
                if (result.MT_CotejarDatos_res != null && result.MT_CotejarDatos_res.Es_mensaje != null && result.MT_CotejarDatos_res.Es_mensaje.TpMens.ToString().Equals("I", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Debug("Infracciones - ajax_BuscarVehiculo - GetVEiculoModelFromFinanzas - Response - " + JsonConvert.SerializeObject(result));
                    vehiculosModel = GetVEiculoModelFromFinanzas(result);

                    vehiculosModel.ErrorRepube = string.IsNullOrEmpty(vehiculosModel.placas) ? "No" : "";
                    //Se establece el origen de datos
                    vehiculosModel.origenDatos = "Padrón Estatal";

                    return vehiculosModel;
                }
            }

            if (allowSistem)
            {
                Logger.Debug("Infracciones - ajax_BuscarVehiculo - ConsultaGeneral - REPUVE");
                var repuveConsGralResponse = _repuveService.ConsultaGeneral(repuveGralModel).FirstOrDefault();
                Logger.Debug(" - Response - " + JsonConvert.SerializeObject(repuveConsGralResponse));
                var idEntidad = !string.IsNullOrEmpty(repuveConsGralResponse.entidad_expide)
                      ? ObtenerIdEntidadRepuve(repuveConsGralResponse.entidad_expide)
                      : 0;
                var idColor = !string.IsNullOrEmpty(repuveConsGralResponse.color)
                    ? ObtenerIdColor(repuveConsGralResponse.color)
                    : 0;

                var idMarca = !string.IsNullOrEmpty(repuveConsGralResponse.marca_padron)
                    ? ObtenerIdMarcaRepuve(repuveConsGralResponse.marca_padron)
                    : 0;

                var idSubmarca = !string.IsNullOrEmpty(repuveConsGralResponse.submarca_padron)
                    ? ObtenerIdSubmarcaRepuve(repuveConsGralResponse.submarca_padron)
                    : 0;
                var submarcaLimpio = !string.IsNullOrEmpty(repuveConsGralResponse.submarca_padron)
                    ? ObtenerIdSubmarcaRepuve(repuveConsGralResponse.submarca_padron)
                    : 0;

                var idTipo = !string.IsNullOrEmpty(repuveConsGralResponse.tipo_vehiculo_padron)
                 ? ObtenerIdTipoVehiculo(repuveConsGralResponse.tipo_vehiculo_padron)
                 : 0;
                var idTipoServicio = !string.IsNullOrEmpty(repuveConsGralResponse.tipo_uso_padron)
                ? ObtenerIdTipoServicioRepuve(repuveConsGralResponse.tipo_uso_padron)
                : 0;

                var vehiculoEncontrado = new VehiculoModel
                {
                    placas = repuveConsGralResponse.placa,
                    serie = repuveConsGralResponse.niv_padron,
                    // numeroEconomico = repuveConsGralResponse.tnia,
                    motor = repuveConsGralResponse.motor,
                    //otros = repuveConsGralResponse.
                    idColor = idColor,
                    idEntidad = idEntidad,
                    idMarcaVehiculo = idMarca,
                    idSubmarca = idSubmarca,
                    //submarca = submarcaLimpio,
                    idTipoVehiculo = idTipo,
                    modelo = repuveConsGralResponse.modelo,
                    idCatTipoServicio = idTipoServicio,
                    //carga = repuveConsGralResponse.ca,

                    Persona = new PersonaModel(),

                    PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel(),
                };

                vehiculoEncontrado.ErrorRepube = string.IsNullOrEmpty(vehiculoEncontrado.placas) ? "No" : "";

                //Se establece el origen de datos
                vehiculoEncontrado.origenDatos = string.IsNullOrEmpty(vehiculoEncontrado.placas) ? null : "REPUVE";
                return vehiculoEncontrado;

            }
            else
            {
                Logger.Debug("Infracciones - ajax_BuscarVehiculo - ConsultaGeneral - REPUVE (BANDERA DESACTIVADA)");
            }
            vehiculosModel.ErrorRepube = string.IsNullOrEmpty(vehiculosModel.placas) ? "No" : "";


            return vehiculosModel;
        }
            catch (Exception ex)
            {
                Logger.Error("Infracciones - ajax_BuscarVehiculo: " + ex.Message);
                return null;
            }
         }

    }
}

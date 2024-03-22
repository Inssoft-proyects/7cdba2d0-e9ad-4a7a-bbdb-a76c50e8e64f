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
using Microsoft.IdentityModel.Tokens;
using Kendo.Mvc.Extensions;

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
                        query = string.Format(query, "(v.serie = @Serie  )");
                    }
                    else if (!string.IsNullOrEmpty(Placa))
                    {
                        query = string.Format(query, "( v.placas = + @placas  )");
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
                            model.idColor = reader["idColor"] is DBNull ? 0: Convert.ToInt32(reader["idColor"].ToString());
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

            return modelList.FirstOrDefault() ?? model22;
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
            string strQuery = @"
        IF NOT EXISTS (
            SELECT 1 FROM vehiculos 
            WHERE placas = @placas OR serie = @serie
        )
        BEGIN
            INSERT INTO vehiculos (
                placas,
                serie,
                tarjeta,
                vigenciaTarjeta,
                idMarcaVehiculo,
                idSubmarca,
                idTipoVehiculo,
                modelo,
                idColor,
                idEntidad,
                idCatTipoServicio,
                propietario,
                numeroEconomico,
                paisManufactura,
                idPersona,
                fechaActualizacion,
                actualizadoPor,
                estatus,
                motor,
                capacidad,
                poliza,
                carga,
                otros,
                idSubtipoServicio
            ) 
            VALUES (
                @placas,
                @serie,
                @tarjeta,
                @vigenciaTarjeta,
                @idMarcaVehiculo,
                @idSubmarca,
                @idTipoVehiculo,
                @modelo,
                @idColor,
                @idEntidad,
                @idCatTipoServicio,
                @propietario,
                @numeroEconomico,
                @paisManufactura,
                @idPersona,
                @fechaActualizacion,
                @actualizadoPor,
                @estatus,
                @motor,
                @capacidad,
                @poliza,
                @carga,
                @otros,
                @idSubtipoServicio
            );
            SELECT CAST (SCOPE_IDENTITY() AS int)
        END
        ELSE
        BEGIN
            SELECT -1
        END";
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
                    command.Parameters.Add(new SqlParameter("@idColor", SqlDbType.Int)).Value = (object)model.idColor ?? (object)34;
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


        /// <summary>
        /// Busca en la base de datos si hay vehiculos que coincidan con los parametros de busqueda
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<VehiculoModel> GetVehiculoPropietario(VehiculoBusquedaModel modelSearch)
        {
            List<VehiculoModel> listaVehiculos = new();


            string strQuery = @"SELECT
                                v.idVehiculo, v.placas, v.serie, v.tarjeta, v.vigenciaTarjeta, v.idMarcaVehiculo
                                ,v.idSubmarca, v.idTipoVehiculo, v.modelo, v.idColor, v.idEntidad, v.idCatTipoServicio
                                ,v.propietario, v.numeroEconomico, v.paisManufactura, v.idPersona
                                ,v.motor,v.capacidad,v.poliza,v.otros, v.carga
                                ,catMV.marcaVehiculo, catTV.tipoVehiculo, catSV.nombreSubmarca, catTS.tipoServicio
                                ,catE.nombreEntidad, catC.color
                                ,p.numeroLicencia,p.curp,p.rfc,nombre,p.apellidoPaterno,p.apellidoMaterno,p.idCatTipoPersona,p.idGenero,p.fechaNacimiento,p.idTipoLicencia,p.vigenciaLicencia
                                FROM vehiculos v
                                INNER JOIN catMarcasVehiculos catMV on v.idMarcaVehiculo = catMV.idMarcaVehiculo 
                                left JOIN catTiposVehiculo catTV on v.idTipoVehiculo = catTV.idTipoVehiculo 
                                left JOIN catSubmarcasVehiculos catSV on v.idSubmarca = catSV.idSubmarca 
                                left JOIN catTipoServicio catTS on v.idCatTipoServicio = catTS.idCatTipoServicio 
                                left JOIN catEntidades catE on v.idEntidad = catE.idEntidad  
                                left JOIN catColores catC on v.idColor = catC.idColor  
                                left join personas p on v.idPersona=p.idPersona
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
                            VehiculoModel model = new()
                            {
                                Persona = new PersonaModel(),
                                idVehiculo = Convert.ToInt32(reader["idVehiculo"]),
                                placas = reader["placas"].ToString(),

                                serie = reader["serie"].ToString(),
                                tarjeta = reader["tarjeta"].ToString(),
                                vigenciaTarjeta = reader["vigenciaTarjeta"].GetType() == typeof(DBNull) ? null : Convert.ToDateTime(reader["vigenciaTarjeta"].ToString()),
                                idMarcaVehiculo = Convert.ToInt32(reader["idMarcaVehiculo"]),
                                idSubmarca = Convert.ToInt32(reader["idSubmarca"]),
                                idTipoVehiculo = Convert.ToInt32(reader["idTipoVehiculo"]),

                                modelo = reader["modelo"].ToString(),
                                idColor = Convert.ToInt32(reader["idColor"]),
                                idEntidad = Convert.ToInt32(reader["idEntidad"]),
                                idCatTipoServicio = Convert.ToInt32(reader["idCatTipoServicio"]),
                                propietario = reader["propietario"].ToString(),
                                numeroEconomico = reader["numeroEconomico"].ToString(),
                                idPersona = reader["idPersona"] == System.DBNull.Value ? default(int?) : (int?)reader["idPersona"],
                                paisManufactura = reader["paisManufactura"].ToString()
                            };
                            model.numeroEconomico = reader["numeroEconomico"].ToString();

                            model.marca = model.otros = reader["otros"].ToString(); ;
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

                            model.idSubmarcaUpdated = model.idSubmarca;
                            model.PersonaMoralBusquedaModel = new PersonaMoralBusquedaModel
                            {
                                PersonasMorales = new List<PersonaModel>()
                            };

                            //Propietario
                            model.Persona.idPersona = reader["idPersona"] == System.DBNull.Value ? default : (int?)reader["idPersona"];
                            model.Persona.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.Persona.CURP = reader["curp"].ToString();
                            model.Persona.RFC = reader["rfc"].ToString();
                            model.Persona.nombre = reader["nombre"].ToString();
                            model.Persona.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.Persona.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.Persona.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default : (int?)reader["idCatTipoPersona"];
                            model.Persona.idGenero = reader["idGenero"] == System.DBNull.Value ? default : (int?)reader["idGenero"];
                            model.Persona.fechaNacimiento = reader["fechaNacimiento"].GetType() == typeof(DBNull) ? null : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            model.Persona.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default : (int?)reader["idTipoLicencia"];
                            model.Persona.vigenciaLicenciaFisico = reader["vigenciaLicencia"].GetType() == typeof(DBNull) ? null : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
                            model.Persona.vigenciaLicencia = reader["vigenciaLicencia"].GetType() == typeof(DBNull) ? null : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());

                            if (model.idVehiculo != 0)
                            {
                                model.encontradoEn = (int)EstatusBusquedaVehiculo.Sitteg;
                            }
                            else
                            {
                                model.encontradoEn = (int)EstatusBusquedaVehiculo.NoEncontrado;
                                model.serie = modelSearch.SerieBusqueda;
                            }
                            listaVehiculos.Add(model);

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
            return listaVehiculos;
        }
    }
}

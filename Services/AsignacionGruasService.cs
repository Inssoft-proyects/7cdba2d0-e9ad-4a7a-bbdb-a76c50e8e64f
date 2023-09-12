using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class AsignacionGruasService : IAsignacionGruasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IVehiculosService _vehiculosService;
        private readonly IPersonasService _personasService;
        public AsignacionGruasService(ISqlClientConnectionBD sqlClientConnectionBD
                                  , IVehiculosService vehiculosService
                                  , IPersonasService personasService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _vehiculosService = vehiculosService;
            _personasService = personasService;
        }

        public List<AsignacionGruaModel> BuscarSolicitudes(AsignacionGruaModel model)
        {
            //
            List<AsignacionGruaModel> ListaSolicitudes = new List<AsignacionGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT sol.idSolicitud,sol.folio,sol.fechaSolicitud,sol.idMunicipioUbicacion,sol.idCarreteraUbicacion, " + "" +
                        "sol.idEntidadUbicacion,sol.vehiculoCalle,sol.vehiculoCarretera,sol.vehiculoColonia,sol.idOficial,sol.idTipoUsuario,sol.idPension,sol.idPropietarioGrua, " +
                        "mun.municipio, " +
                        "car.carretera, " +
                        "ent.nombreEntidad, " +
                        "tip_us.tipoUsuario, " +
                        "ofi.nombre,ofi.apellidoPaterno,ofi.apellidoMaterno " +
                        "From solicitudes AS sol " +
                        "LEFT JOIN catMunicipios AS mun ON sol.idMunicipioUbicacion = mun.idMunicipio " +
                        "LEFT JOIN catCarreteras AS car ON sol.idCarreteraUbicacion = car.idCarretera " +
                        "LEFT JOIN catEntidades AS ent ON sol.idEntidadUbicacion = ent.idEntidad " +
                        "LEFT JOIN catTiposUsuario AS tip_us ON sol.idTipoUsuario = tip_us.idTipoUsuario " +
                        "LEFT JOIN catOficiales AS ofi ON sol.idOficial = ofi.idOficial " +
                        "WHERE sol.folio = @folioBusqueda OR sol.fechaSolicitud = @fechaSolicitud AND sol.estatus = 1", connection);


                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@folioBusqueda", SqlDbType.NVarChar)).Value = model.FolioSolicitud;
                    command.Parameters.Add(new SqlParameter("@fechaSolicitud", SqlDbType.Date)).Value = model.fecha;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AsignacionGruaModel solicitud = new AsignacionGruaModel();
                            solicitud.idSolicitud = reader["idSolicitud"] != DBNull.Value ? Convert.ToInt32(reader["idSolicitud"]) : 0;
                            solicitud.FolioSolicitud = reader["folio"].ToString();
                            solicitud.fechaSolicitud = reader["fechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(reader["fechaSolicitud"]) : DateTime.MinValue;
                            solicitud.idPension = reader["idPension"] != DBNull.Value ? Convert.ToInt32(reader["idPension"]) : 0;
                            solicitud.idPropietarioGrua = reader["idPropietarioGrua"] != DBNull.Value ? Convert.ToInt32(reader["idPropietarioGrua"]) : 0;
                            solicitud.vehiculoCalle = reader["vehiculoCalle"].ToString();
                            solicitud.vehiculoColonia = reader["vehiculoColonia"].ToString();
                            solicitud.municipio = reader["municipio"].ToString();
                            solicitud.carretera = reader["carretera"].ToString();
                            solicitud.nombreEntidad = reader["nombreEntidad"].ToString();
                            solicitud.tipoUsuario = reader["tipoUsuario"].ToString();
                            solicitud.nombreOficial = reader["nombre"].ToString();
                            solicitud.apellidoPaternoOficial = reader["apellidoPaterno"].ToString();
                            solicitud.apellidoPaternoOficial = reader["apellidoMaterno"].ToString();


                            ListaSolicitudes.Add(solicitud);

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
            return ListaSolicitudes;


        }
        public List<AsignacionGruaModel> ObtenerTodasSolicitudes()
        {
            //
            List<AsignacionGruaModel> ListaSolicitudes = new List<AsignacionGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT sol.idSolicitud,sol.folio,sol.fechaSolicitud,sol.idMunicipioUbicacion,sol.idCarreteraUbicacion, " + "" +
                        "sol.idEntidadUbicacion,sol.vehiculoCalle,sol.vehiculoCarretera,sol.vehiculoColonia,sol.idOficial,sol.idTipoUsuario, " +
                        "mun.municipio, " +
                        "car.carretera, " +
                        "ent.nombreEntidad, " +
                        "tip_us.tipoUsuario, " +
                        "ofi.nombre,ofi.apellidoPaterno,ofi.apellidoMaterno " +
                        "From solicitudes AS sol " +
                        "LEFT JOIN catMunicipios AS mun ON sol.idMunicipioUbicacion = mun.idMunicipio " +
                        "LEFT JOIN catCarreteras AS car ON sol.idCarreteraUbicacion = car.idCarretera " +
                        "LEFT JOIN catEntidades AS ent ON sol.idEntidadUbicacion = ent.idEntidad " +
                        "LEFT JOIN catTiposUsuario AS tip_us ON sol.idTipoUsuario = tip_us.idTipoUsuario " +
                        "LEFT JOIN catOficiales AS ofi ON sol.idOficial = ofi.idOficial " +
                        "WHERE sol.estatus = 1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AsignacionGruaModel solicitud = new AsignacionGruaModel();
                            solicitud.idSolicitud = reader["idSolicitud"] != DBNull.Value ? Convert.ToInt32(reader["idSolicitud"]) : 0;
                            solicitud.FolioSolicitud = reader["folio"].ToString();
                            solicitud.fechaSolicitud = reader["fechaSolicitud"] != DBNull.Value ? Convert.ToDateTime(reader["fechaSolicitud"]) : DateTime.MinValue;

                            solicitud.vehiculoCalle = reader["vehiculoCalle"].ToString();
                            solicitud.vehiculoColonia = reader["vehiculoColonia"].ToString();
                            solicitud.municipio = reader["municipio"].ToString();
                            solicitud.carretera = reader["carretera"].ToString();
                            solicitud.nombreEntidad = reader["nombreEntidad"].ToString();
                            solicitud.tipoUsuario = reader["tipoUsuario"].ToString();
                            solicitud.nombreOficial = reader["nombre"].ToString();
                            solicitud.apellidoPaternoOficial = reader["apellidoPaterno"].ToString();
                            solicitud.apellidoPaternoOficial = reader["apellidoMaterno"].ToString();


                            ListaSolicitudes.Add(solicitud);

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
            return ListaSolicitudes;


        }

        public List<AsignacionGruaModel> BuscarPorParametro(string Placa, string Serie)
        {
            List<AsignacionGruaModel> Vehiculo = new List<AsignacionGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command;

                    if (!string.IsNullOrEmpty(Serie))
                    {
                        command = new SqlCommand(
                       "SELECT v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio, p.nombre, p.apellidoPaterno, p.apellidoMaterno,p.CURP,p.RFC, " +
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
                        command = new SqlCommand("SELECT v.*, mv.marcaVehiculo, sm.nombreSubmarca, e.nombreEntidad, cc.color, tv.tipoVehiculo, ts.tipoServicio,p.nombre, p.apellidoPaterno, p.apellidoMaterno, " +
                            "p.CURP,p.RFC " +
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
                            AsignacionGruaModel vehiculo = new AsignacionGruaModel();
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
                            vehiculo.Motor = reader["motor"].ToString();
                            vehiculo.NumeroEconomico = reader["numeroEconomico"].ToString();
                            vehiculo.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            vehiculo.CURP = reader["CURP"].ToString();
                            vehiculo.RFC = reader["RFC"].ToString();

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

        public AsignacionGruaModel BuscarSolicitudPord(int iSo)
        {
            AsignacionGruaModel solicitud = new AsignacionGruaModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();

                    // Consulta para buscar si el idSolicitud ya existe en la tabla depositos
                    SqlCommand searchCommand = new SqlCommand("SELECT idSolicitud,folio FROM depositos WHERE idSolicitud = @idSolicitud", connection);
                    searchCommand.Parameters.Add(new SqlParameter("@idSolicitud", SqlDbType.Int)).Value = iSo;

                    // Ejecutar la consulta de búsqueda
                    using (SqlDataReader searchReader = searchCommand.ExecuteReader())
                    {
                        if (searchReader.Read())  // Si hay resultado, el idSolicitud ya existe, no es necesario hacer la inserción
                        {
                            solicitud.idSolicitud = iSo;
                            solicitud.FolioSolicitud = searchReader["folio"].ToString();
                            solicitud.IdDeposito = -1;
                            return solicitud;
                        }
                    }

                    // Continuar con la consulta y la inserción
                    SqlCommand command = new SqlCommand("SELECT sol.idSolicitud,sol.folio,sol.idPropietarioGrua,sol.idPension,sol.idTramoUbicacion, " +
                                                        "sol.vehiculoKm " +
                                                        "FROM solicitudes AS sol " +
                                                        "WHERE idSolicitud = @idSolicitud", connection);
                    command.Parameters.Add(new SqlParameter("@idSolicitud", SqlDbType.Int)).Value = iSo;

                    command.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            solicitud.idSolicitud = reader["idSolicitud"] != DBNull.Value ? Convert.ToInt32(reader["idSolicitud"]) : 0;
                            solicitud.idPropietarioGrua = reader["idPropietarioGrua"] != DBNull.Value ? Convert.ToInt32(reader["idPropietarioGrua"]) : 0;
                            solicitud.FolioSolicitud = reader["folio"].ToString();
                            solicitud.idPension = reader["idPension"] != DBNull.Value ? Convert.ToInt32(reader["idPension"]) : 0;
                            solicitud.idTramoUbicacion = reader["idTramoUbicacion"] != DBNull.Value ? Convert.ToInt32(reader["idTramoUbicacion"]) : 0;
                            solicitud.kilometro = reader["vehiculoKm"].ToString();
                        }
                    }

                    int idDeposito = -1;
                    using (SqlConnection insertConnection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                    {
                        insertConnection.Open();
                        SqlCommand insertCommand = new SqlCommand("INSERT INTO depositos " +
                                                                "(idSolicitud,folio,idTramo,idPension,km,liberado,IdConcesionario,estatus) " +
                                                                "VALUES (@idSolicitud,@folio,@idTramo,@idPension,@km,@liberado,@idPropietarioGruas,@estatus);" +
                                                                "SELECT SCOPE_IDENTITY()", insertConnection);
                        insertCommand.Parameters.Add(new SqlParameter("@idSolicitud", SqlDbType.Int)).Value = solicitud.idSolicitud;
                        insertCommand.Parameters.Add(new SqlParameter("@folio", SqlDbType.VarChar, 50)).Value = solicitud.FolioSolicitud;
                        insertCommand.Parameters.Add(new SqlParameter("@idTramo", SqlDbType.Int)).Value = solicitud.idTramoUbicacion;
                        insertCommand.Parameters.Add(new SqlParameter("@idPropietarioGruas", SqlDbType.Int)).Value = solicitud.idPropietarioGrua;
                        insertCommand.Parameters.Add(new SqlParameter("@idPension", SqlDbType.Int)).Value = solicitud.idPension;
                        insertCommand.Parameters.Add(new SqlParameter("@km", SqlDbType.NVarChar)).Value = solicitud.kilometro;
                        insertCommand.Parameters.Add(new SqlParameter("@liberado", SqlDbType.Int)).Value = 0;
                        insertCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                        idDeposito = Convert.ToInt32(insertCommand.ExecuteScalar());
                        solicitud.IdDeposito = idDeposito;

                    }

                }
                catch (Exception ex)
                {
                    // Manejar las excepciones adecuadamente
                }
                finally
                {
                    connection.Close();
                }
            }

            return solicitud;
        }


        public int ActualizarDatos(AsignacionGruaModel selectedRowData, int iSo)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update depositos set idMarca = @idMarca,idSubmarca =@idSubmarca, idColor = @idColor,serie = @Serie,placa = @Placa where idSolicitud=@idSolicitud", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idMarca", SqlDbType.Int)).Value = selectedRowData.IdMarcaVehiculo;
                    sqlCommand.Parameters.Add(new SqlParameter("@idSubmarca", SqlDbType.Int)).Value = selectedRowData.IdSubmarca;
                    sqlCommand.Parameters.Add(new SqlParameter("@idColor", SqlDbType.Int)).Value = selectedRowData.IdColor;
                    sqlCommand.Parameters.Add(new SqlParameter("@Serie", SqlDbType.NVarChar)).Value = selectedRowData.Serie;
                    sqlCommand.Parameters.Add(new SqlParameter("@Placa", SqlDbType.NVarChar)).Value = selectedRowData.Placa;
                    sqlCommand.Parameters.Add(new SqlParameter("@idSolicitud", SqlDbType.Int)).Value = iSo;

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
        public List<AsignacionGruaModel> ObtenerInfracciones(string folioInfraccion)
        {
            List<AsignacionGruaModel> modelList = new List<AsignacionGruaModel>();
            string strQuery = @"SELECT inf.idInfraccion, inf.idVehiculo,inf.idPersona,inf.folioInfraccion,inf.fechaInfraccion,
                                        v.placas,v.serie,v.idMarcaVehiculo,v.idSubmarca,v.modelo,v.idPersona,
                                        p.nombre,p.apellidoPaterno,p.apellidoMaterno,p.CURP,p.RFC,
                                        cmv.marcaVehiculo,csv.nombreSubmarca,col.color
                                        FROM infracciones AS inf
                                        LEFT JOIN vehiculos AS v ON inf.idVehiculo = v.idVehiculo
                                        LEFT JOIN personas AS p ON v.idPersona = p.idPersona
                                        LEFT JOIN catMarcasVehiculos as cmv ON v.idMarcaVehiculo = cmv.idMarcaVehiculo
                                        LEFT JOIN catSubmarcasVehiculos as csv ON v.idSubmarca = csv.idSubmarca
                                        LEFT JOIN catColores AS col ON v.idColor = col.idColor
                                        WHERE inf.estatus = 1 AND inf.folioInfraccion = @folioInfraccion";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    // command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@folioInfraccion", SqlDbType.NVarChar)).Value = (object)folioInfraccion ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AsignacionGruaModel model = new AsignacionGruaModel();
                            model.idInfraccion = reader["idInfraccion"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idInfraccion"].ToString());
                            model.idVehiculo = reader["idVehiculo"] == System.DBNull.Value ? default(int?) : Convert.ToInt32(reader["idVehiculo"].ToString());
                            model.folioInfraccion = reader["folioInfraccion"].ToString();
                            model.fechaInfraccion = reader["fechaInfraccion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaInfraccion"].ToString());
                            model.IdMarcaVehiculo = Convert.IsDBNull(reader["IdMarcaVehiculo"]) ? 0 : Convert.ToInt32(reader["IdMarcaVehiculo"]);
                            model.IdSubmarca = Convert.IsDBNull(reader["IdSubmarca"]) ? 0 : Convert.ToInt32(reader["IdSubmarca"]);
                            model.Marca = reader["marcaVehiculo"].ToString();
                            model.Submarca = reader["nombreSubmarca"].ToString();
                            model.Modelo = reader["Modelo"].ToString();
                            model.Color = reader["color"].ToString();
                            model.Placa = reader["placas"].ToString();
                            model.Serie = reader["serie"].ToString();
                            model.Propietario = $"{reader["nombre"]} {reader["apellidoPaterno"]} {reader["apellidoMaterno"]}";
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();

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
        public int UpdateDatosGrua(IFormCollection formData, int abanderamiento, int arrastre, int salvamento, int iSo)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO gruasAsignadas (fechaArribo,fechaInicio,fechaFinal,operadorGrua,abanderamiento,arrastre,salvamento,idGrua,minutosManiobra,idSolicitud,actualizadoPor,fechaActualizacion,estatus)" +
                                    "VALUES(@fechaArribo,@fechaInicio,@fechaFinal,@operadorGrua,@abanderamiento,@arrastre,@salvamento,@idGrua,@minutosManiobra,@idSolicitud,@actualizadoPor,@fechaActualizacion,@estatus)";


                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@fechaArribo", DateTime.Parse(formData["fechaArribo"].ToString()));
                    command.Parameters.AddWithValue("@fechaInicio", DateTime.Parse(formData["fechaInicio"].ToString()));
                    command.Parameters.AddWithValue("@fechaFinal", DateTime.Parse(formData["fechaFinal"].ToString()));
                    command.Parameters.AddWithValue("@operadorGrua", formData["operadorGrua"].ToString());
                    command.Parameters.AddWithValue("@abanderamiento", abanderamiento);
                    command.Parameters.AddWithValue("@arrastre", arrastre);
                    command.Parameters.AddWithValue("@salvamento", salvamento);
                    command.Parameters.AddWithValue("@minutosManiobra", int.Parse(formData["tiempoManiobras"].ToString()));
                    command.Parameters.AddWithValue("@idSolicitud", iSo);
                    command.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
                    command.Parameters.AddWithValue("@actualizadoPor", 1);
                    command.Parameters.AddWithValue("@estatus", 1);
                    int idGrua;
                    if (int.TryParse(formData["idGrua"].ToString(), out idGrua))
                    {
                        command.Parameters.AddWithValue("@idGrua", idGrua);
                    }
                    else
                    {
                      
                        command.Parameters.AddWithValue("@idGrua", DBNull.Value); 
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
        public List<SeleccionGruaModel> BusquedaGruaTabla(int iSo)
        {
            List<SeleccionGruaModel> ListaSolicitudes = new List<SeleccionGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ga.idAsignacion, ga.idGrua, ga.operadorGrua, ga.abanderamiento, ga.arrastre, ga.salvamento, " +
                                                          "ga.fechaArribo, ga.fechaInicio, ga.fechaFinal,ga.minutosManiobra, " +
                                                          "g.noEconomico, g.idTipoGrua, g.placas, ctg.TipoGrua " +
                                                          "FROM gruasAsignadas AS ga " +
                                                          "LEFT JOIN gruas AS g ON ga.idGrua = g.idGrua " +
                                                          "LEFT JOIN catTipoGrua AS ctg ON g.idTipoGrua = ctg.IdTipoGrua " +
                                                          "WHERE ga.idSolicitud = @idSolicitud AND ga.estatus = 1", connection);



                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idSolicitud", SqlDbType.Int)).Value = iSo;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            SeleccionGruaModel solicitud = new SeleccionGruaModel();
                            solicitud.idAsignacion = reader["idAsignacion"] != DBNull.Value ? Convert.ToInt32(reader["idAsignacion"]) : 0;
                            solicitud.idGrua = reader["idGrua"] != DBNull.Value ? Convert.ToInt32(reader["idGrua"]) : 0;
                            solicitud.EstadoAbanderamiento = reader["abanderamiento"] != DBNull.Value ? Convert.ToInt32(reader["abanderamiento"]) : 0;
                            solicitud.EstadoArrastre = reader["arrastre"] != DBNull.Value ? Convert.ToInt32(reader["arrastre"]) : 0;
                            solicitud.EstadoSalvamento = reader["salvamento"] != DBNull.Value ? Convert.ToInt32(reader["salvamento"]) : 0;
                            solicitud.fechaArribo = reader["fechaArribo"] != DBNull.Value ? Convert.ToDateTime(reader["fechaArribo"]) : DateTime.MinValue;
                            solicitud.fechaInicio = reader["fechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["fechaInicio"]) : DateTime.MinValue;
                            solicitud.fechaFinal = reader["fechaFinal"] != DBNull.Value ? Convert.ToDateTime(reader["fechaFinal"]) : DateTime.MinValue;
                            solicitud.operadorGrua = reader["operadorGrua"].ToString();
                            solicitud.numeroEconomico = reader["noEconomico"].ToString();
                            solicitud.Placas = reader["placas"].ToString();
                            solicitud.TipoGrua = reader["TipoGrua"].ToString();

                            ListaSolicitudes.Add(solicitud);

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
            return ListaSolicitudes;


        }
        public int AgregarObs(AsignacionGruaModel formData, int iSo)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE depositos SET " +
                                    "observaciones=@observaciones " +
                                    "Where depositos.idSolicitud = @idsolicitud";


                    SqlCommand command = new SqlCommand(query, connection);


                    command.Parameters.AddWithValue("@observaciones", formData.observaciones);
                    command.Parameters.AddWithValue("@idSolicitud", iSo);


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
        public int InsertarInventario(byte[] imageData, int iSo, string numeroInventario)
        {
            int result = 0;
            string strQuery = @"UPDATE depositos
                   SET inventario = @inventario,
                       numeroInventario = @numeroInventario
                   WHERE idSolicitud = @idSolicitud";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idSolicitud", SqlDbType.Int)).Value = iSo;
                    command.Parameters.Add(new SqlParameter("@numeroInventario", SqlDbType.NVarChar)).Value = numeroInventario;
                    command.Parameters.Add(new SqlParameter("@inventario", SqlDbType.VarBinary)).Value = imageData;

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
        public SeleccionGruaModel ObtenerAsignacionPorId(int idAsignacion)
        {
            SeleccionGruaModel asignacion = new SeleccionGruaModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM gruasAsignadas WHERE idAsignacion = @idAsignacion", connection);
                    command.Parameters.Add(new SqlParameter("@idAsignacion", SqlDbType.Int)).Value = idAsignacion;
                    //command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;

                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            SeleccionGruaModel solicitud = new SeleccionGruaModel();
                            asignacion.idAsignacion = reader["idAsignacion"] != DBNull.Value ? Convert.ToInt32(reader["idAsignacion"]) : 0;
                            asignacion.idGrua = reader["idGrua"] != DBNull.Value ? Convert.ToInt32(reader["idGrua"]) : 0;
                            asignacion.EstadoAbanderamiento = reader["abanderamiento"] != DBNull.Value ? Convert.ToInt32(reader["abanderamiento"]) : 0;
                            asignacion.EstadoArrastre = reader["arrastre"] != DBNull.Value ? Convert.ToInt32(reader["arrastre"]) : 0;
                            asignacion.EstadoSalvamento = reader["salvamento"] != DBNull.Value ? Convert.ToInt32(reader["salvamento"]) : 0;
                            asignacion.fechaArribo = reader["fechaArribo"] != DBNull.Value ? Convert.ToDateTime(reader["fechaArribo"]) : DateTime.MinValue;
                            asignacion.fechaInicio = reader["fechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["fechaInicio"]) : DateTime.MinValue;
                            asignacion.fechaFinal = reader["fechaFinal"] != DBNull.Value ? Convert.ToDateTime(reader["fechaFinal"]) : DateTime.MinValue;
                            asignacion.operadorGrua = reader["operadorGrua"].ToString();
                            asignacion.numeroEconomico = reader["noEconomico"].ToString();
                            asignacion.Placas = reader["placas"].ToString();
                            asignacion.TipoGrua = reader["TipoGrua"].ToString();




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

            return asignacion;
        }

        public int EliminarGrua(int idAsignacion)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE gruasAsignadas SET estatus = 0 WHERE idAsignacion = @idAsignacion";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@idAsignacion", idAsignacion);

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

        public int EditarDatosGrua(IFormCollection formData, int abanderamiento, int arrastre, int salvamento)
        {
            int result = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE gruasAsignadas " +
                                   "SET fechaArribo = @fechaArribo, " +
                                   "fechaInicio = @fechaInicio, " +
                                   "fechaFinal = @fechaFinal, " +
                                   "operadorGrua = @operadorGrua, " +
                                   "abanderamiento = @abanderamiento, " +
                                   "arrastre = @arrastre, " +
                                   "salvamento = @salvamento, " +
                                   "idGrua = @idGrua, " +
                                   "minutosManiobra = @minutosManiobra, " +
                                   "actualizadoPor = @actualizadoPor, " +
                                   "fechaActualizacion = @fechaActualizacion, " +
                                   "estatus = @estatus " +
                                   "WHERE idAsignacion = @idAsignacion";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@fechaArribo", DateTime.Parse(formData["fechaArribo"].ToString()));
                    command.Parameters.AddWithValue("@fechaInicio", DateTime.Parse(formData["fechaInicio"].ToString()));
                    command.Parameters.AddWithValue("@fechaFinal", DateTime.Parse(formData["fechaFinal"].ToString()));
                    command.Parameters.AddWithValue("@operadorGrua", formData["operadorGrua"].ToString());
                    command.Parameters.AddWithValue("@abanderamiento", abanderamiento);
                    command.Parameters.AddWithValue("@arrastre", arrastre);
                    command.Parameters.AddWithValue("@salvamento", salvamento);
                    command.Parameters.AddWithValue("@idGrua", int.Parse(formData["idGrua"].ToString()));
                    command.Parameters.AddWithValue("@minutosManiobra", int.Parse(formData["tiempoManiobras"].ToString()));
                    command.Parameters.AddWithValue("@idAsignacion", int.Parse(formData["idAsignacion"].ToString()));
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
    }
}
  



    


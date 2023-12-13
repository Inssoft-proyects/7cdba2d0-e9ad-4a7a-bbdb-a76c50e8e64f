using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using static GuanajuatoAdminUsuarios.Utils.CatalogosEnums;

namespace GuanajuatoAdminUsuarios.Services
{
    public class PersonasService : IPersonasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly ICatEntidadesService _catEntidadesService;
        public PersonasService(ISqlClientConnectionBD sqlClientConnectionBD, ICatEntidadesService catEntidadesService)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _catEntidadesService = catEntidadesService; 
        }

        public IEnumerable<PersonaModel> GetAllPersonas()
        {
            //quitar el top
            List<PersonaModel> modelList = new List<PersonaModel>();
            string strQuery = @"SELECT 
                                p.idPersona
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
							   ,p.idTipoLicencia
							   ,p.fechaNacimiento
							   ,p.idGenero
							   ,p.vigenciaLicencia
                               ,ctp.tipoPersona
							   ,cl.tipoLicencia
							   ,cg.genero
                               FROM personas p
                               LEFT JOIN catTipoPersona ctp
                               on p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1
							   LEFT JOIN catTipoLicencia cl
							   on p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1
							   LEFT JOIN catGeneros cg
							   on p.idGenero = cg.idGenero AND cg.estatus = 1
                               WHERE p.estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaModel model = new PersonaModel();
                            model.PersonaDireccion = new PersonaDireccionModel();
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();
                            model.nombre = reader["nombre"].ToString();
                            model.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.tipoPersona = reader["tipoPersona"].ToString();
                            model.idGenero = reader["idGenero"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"].ToString());
                            model.genero = reader["genero"].ToString();
                            model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            model.tipoLicencia = reader["tipoLicencia"].ToString();
                            model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
                            model.PersonaDireccion = GetPersonaDireccionByIdPersona((int)model.idPersona);
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
        public PersonaModel GetPersonaById(int idPersona)
        {
            List<PersonaModel> modelList = new List<PersonaModel>();
            string strQuery = @"SELECT
                                p.idPersona
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
							   ,p.idTipoLicencia
							   ,p.fechaNacimiento
							   ,p.idGenero
							   ,p.vigenciaLicencia
                               ,ctp.tipoPersona
							   ,cl.tipoLicencia
							   ,cg.genero
                               FROM personas p
                               LEFT JOIN catTipoPersona ctp
                               on p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1
							   LEFT JOIN catTipoLicencia cl
							   on p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1
							   LEFT JOIN catGeneros cg
							   on p.idGenero = cg.idGenero
                               WHERE p.estatus = 1
                               AND p.idPersona = @idPersona";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idPersona ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaModel model = new PersonaModel();
                            model.PersonaDireccion = new PersonaDireccionModel();
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();
                            model.nombre = reader["nombre"].ToString();
                            model.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.tipoPersona = reader["tipoPersona"].ToString();
                            model.idGenero = reader["idGenero"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"].ToString());
                            model.genero = reader["genero"].ToString();
                            model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            model.tipoLicencia = reader["tipoLicencia"].ToString();
                            model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
                            model.PersonaDireccion = GetPersonaDireccionByIdPersona((int)model.idPersona);
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
            return modelList.FirstOrDefault();
        }
        public PersonaDireccionModel GetPersonaDireccionByIdPersona(int idPersona)
        {
            List<PersonaDireccionModel> modelList = new List<PersonaDireccionModel>();
            string strQuery = @"SELECT 
                                pd.idPersonasDirecciones
                               ,pd.idEntidad
                               ,pd.idMunicipio
                               ,pd.codigoPostal
                               ,pd.colonia
                               ,pd.calle
                               ,pd.numero
                               ,pd.telefono
                               ,pd.correo
                               ,pd.idPersona
                               ,pd.actualizadoPor
                               ,pd.fechaActualizacion
                               ,pd.estatus
                               ,ce.nombreEntidad
                               ,cm.municipio
                               FROM personasDirecciones pd
                               LEFT JOIN catEntidades ce
                               on pd.idEntidad = ce.idEntidad AND ce.estatus = 1
                               LEFT JOIN catMunicipios cm
                               on pd.idMunicipio = cm.idMunicipio AND cm.estatus = 1
                               WHERE pd.idPersona = @idPersona AND pd.estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idPersona ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaDireccionModel model = new PersonaDireccionModel();
                            model.idPersonasDirecciones = reader["idPersonasDirecciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonasDirecciones"].ToString());
                            model.idEntidad = reader["idEntidad"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idEntidad"].ToString());
                            model.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.entidad = reader["nombreEntidad"].ToString();
                            model.municipio = reader["municipio"].ToString();
                            model.codigoPostal = reader["codigoPostal"].ToString();
                            model.colonia = reader["colonia"].ToString();
                            model.calle = reader["calle"].ToString();
                            model.numero = reader["numero"].ToString();
                            model.telefono = reader["telefono"] == System.DBNull.Value ? default(int) : Convert.ToInt64(reader["telefono"].ToString());
                            model.correo = reader["correo"].ToString();
                            model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
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
            return modelList.Count() == 0 ? new PersonaDireccionModel() : modelList.FirstOrDefault();
        }

        public IEnumerable<PersonaModel> GetAllPersonasMorales(PersonaMoralBusquedaModel modelBusqueda)
        {
            List<PersonaModel> modelList = new List<PersonaModel>();
            string strQuery = @"SELECT
                                p.idPersona,p.numeroLicencia,p.CURP,p.RFC,p.nombre,p.apellidoPaterno,p.apellidoMaterno
                                ,p.fechaActualizacion,p.actualizadoPor,p.estatus,p.idCatTipoPersona,p.idTipoLicencia,p.fechaNacimiento
                                ,p.idGenero,p.vigenciaLicencia,ctp.tipoPersona,cl.tipoLicencia,cg.genero
                                ,pd.idPersonasDirecciones,pd.idEntidad,pd.idMunicipio,pd.codigoPostal
                                ,pd.colonia,pd.calle,pd.numero,pd.telefono,pd.correo,pd.idPersona,pd.actualizadoPor
                                ,pd.fechaActualizacion,pd.estatus,ce.nombreEntidad
                                ,ce.fechaActualizacion,ce.actualizadoPor,ce.estatus
                                ,cm.municipio,cm.fechaActualizacion,cm.actualizadoPor,cm.estatus
                                FROM personas p
                                LEFT JOIN catTipoPersona ctp
                                on p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1
                                LEFT JOIN catTipoLicencia cl
                                on p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1
                                LEFT JOIN catGeneros cg
                                on p.idGenero = cg.idGenero AND cg.estatus = 1
                                LEFT JOIN personasDirecciones pd  on p.idPersona = pd.idPersona AND pd.estatus=1
                                LEFT JOIN catEntidades ce on pd.idEntidad = ce.idEntidad  AND ce.estatus=1
                                LEFT JOIN catMunicipios cm on pd.idMunicipio = cm.idMunicipio AND cm.estatus=1
                                WHERE p.estatus = 1	 AND p.idCatTipoPersona = @idCatTipoPersona AND 
                                (p.RFC LIKE '%' + @RFC + '%' OR p.nombre LIKE '%' + @Nombre + '%')";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idCatTipoPersona", SqlDbType.Int)).Value = (object)modelBusqueda.IdTipoPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@RFC", SqlDbType.NVarChar)).Value = (object)modelBusqueda.RFCBusqueda != null ? modelBusqueda.RFCBusqueda.ToUpper() : DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar)).Value = (object)modelBusqueda.RazonSocial != null ? modelBusqueda.RazonSocial.ToUpper() : DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaModel model = new PersonaModel();
                            model.PersonaDireccion = new PersonaDireccionModel();
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();
                            model.nombre = reader["nombre"].ToString();
                            model.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.tipoPersona = reader["tipoPersona"].ToString();
                            model.idGenero = reader["idGenero"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"].ToString());
                            model.genero = reader["genero"].ToString();
                            model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            model.tipoLicencia = reader["tipoLicencia"].ToString();
                            model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());

                            model.PersonaDireccion.idPersonasDirecciones = reader["idPersonasDirecciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonasDirecciones"].ToString());
                            model.PersonaDireccion.idEntidad = reader["idEntidad"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idEntidad"].ToString());
                            model.PersonaDireccion.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.PersonaDireccion.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.PersonaDireccion.entidad = reader["nombreEntidad"].ToString();
                            model.PersonaDireccion.municipio = reader["municipio"].ToString();
                            model.PersonaDireccion.codigoPostal = reader["codigoPostal"].ToString();
                            model.PersonaDireccion.colonia = reader["colonia"].ToString();
                            model.PersonaDireccion.calle = reader["calle"].ToString();
                            model.PersonaDireccion.numero = reader["numero"].ToString();
                            model.PersonaDireccion.telefono = reader["telefono"] == System.DBNull.Value ? default(int) : Convert.ToInt64(reader["telefono"].ToString());
                            model.PersonaDireccion.correo = reader["correo"].ToString();

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

        public IEnumerable<PersonaModel> GetAllPersonasFisicas()
        {
            List<PersonaModel> modelList = new List<PersonaModel>();
            string strQuery = @"SELECT
                                p.idPersona,p.numeroLicencia,p.CURP,p.RFC,p.nombre,p.apellidoPaterno,p.apellidoMaterno
                                ,p.fechaActualizacion,p.actualizadoPor,p.estatus,p.idCatTipoPersona,p.idTipoLicencia,p.fechaNacimiento
                                ,p.idGenero,p.vigenciaLicencia,ctp.tipoPersona,cl.tipoLicencia,cg.genero
                                ,pd.idPersonasDirecciones,pd.idEntidad,pd.idMunicipio,pd.codigoPostal
                                ,pd.colonia,pd.calle,pd.numero,pd.telefono,pd.correo,pd.idPersona,pd.actualizadoPor
                                ,pd.fechaActualizacion,pd.estatus,ce.nombreEntidad
                                ,ce.fechaActualizacion,ce.actualizadoPor,ce.estatus
                                ,cm.municipio,cm.fechaActualizacion,cm.actualizadoPor,cm.estatus
                                FROM personas p
                                LEFT JOIN catTipoPersona ctp
                                on p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1
                                LEFT JOIN catTipoLicencia cl
                                on p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1
                                LEFT JOIN catGeneros cg
                                on p.idGenero = cg.idGenero AND cg.estatus = 1
                                LEFT JOIN personasDirecciones pd  on p.idPersona = pd.idPersona AND pd.estatus=1
                                LEFT JOIN catEntidades ce on pd.idEntidad = ce.idEntidad  AND ce.estatus=1
                                LEFT JOIN catMunicipios cm on pd.idMunicipio = cm.idMunicipio AND cm.estatus=1
                                WHERE p.estatus = 1";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaModel model = new PersonaModel();
                            model.PersonaDireccion = new PersonaDireccionModel();
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();
                            model.nombre = reader["nombre"].ToString();
                            model.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.tipoPersona = reader["tipoPersona"].ToString();
                            model.idGenero = reader["idGenero"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"].ToString());
                            model.genero = reader["genero"].ToString();
                            model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            model.tipoLicencia = reader["tipoLicencia"].ToString();
                            model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());

                            model.PersonaDireccion.idPersonasDirecciones = reader["idPersonasDirecciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonasDirecciones"].ToString());
                            model.PersonaDireccion.idEntidad = reader["idEntidad"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idEntidad"].ToString());
                            model.PersonaDireccion.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.PersonaDireccion.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.PersonaDireccion.entidad = reader["nombreEntidad"].ToString();
                            model.PersonaDireccion.municipio = reader["municipio"].ToString();
                            model.PersonaDireccion.codigoPostal = reader["codigoPostal"].ToString();
                            model.PersonaDireccion.colonia = reader["colonia"].ToString();
                            model.PersonaDireccion.calle = reader["calle"].ToString();
                            model.PersonaDireccion.numero = reader["numero"].ToString();
                            model.PersonaDireccion.telefono = reader["telefono"] == System.DBNull.Value ? default(int) : Convert.ToInt64(reader["telefono"].ToString());
                            model.PersonaDireccion.correo = reader["correo"].ToString();

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

        public PersonaModel GetPersonaTypeById(int idPersona)
        {
            List<PersonaModel> modelList = new List<PersonaModel>();
            string strQuery = @"SELECT
                                p.idPersona,p.numeroLicencia,p.CURP,p.RFC,p.nombre,p.apellidoPaterno,p.apellidoMaterno
                                ,p.fechaActualizacion,p.actualizadoPor,p.estatus,p.idCatTipoPersona,p.idTipoLicencia,p.fechaNacimiento
                                ,p.idGenero,p.vigenciaLicencia,ctp.tipoPersona,cl.tipoLicencia,cg.genero
                                ,pd.idPersonasDirecciones,pd.idEntidad,pd.idMunicipio,pd.codigoPostal
                                ,pd.colonia,pd.calle,pd.numero,pd.telefono,pd.correo,pd.idPersona,pd.actualizadoPor
                                ,pd.fechaActualizacion,pd.estatus,ce.nombreEntidad
                                ,ce.fechaActualizacion,ce.actualizadoPor,ce.estatus
                                ,cm.municipio,cm.fechaActualizacion,cm.actualizadoPor,cm.estatus
                                FROM personas p
                                LEFT JOIN catTipoPersona ctp
                                on p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1
                                LEFT JOIN catTipoLicencia cl
                                on p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1
                                LEFT JOIN catGeneros cg
                                on p.idGenero = cg.idGenero AND cg.estatus = 1
                                LEFT JOIN personasDirecciones pd  on p.idPersona = pd.idPersona AND pd.estatus=1
                                LEFT JOIN catEntidades ce on pd.idEntidad = ce.idEntidad  AND ce.estatus=1
                                LEFT JOIN catMunicipios cm on pd.idMunicipio = cm.idMunicipio AND cm.estatus=1
                                WHERE p.idPersona = @idPersona";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idPersona ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaModel model = new PersonaModel();
                            model.PersonaDireccion = new PersonaDireccionModel();
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();
                            model.nombre = reader["nombre"].ToString();
                            model.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.tipoPersona = reader["tipoPersona"].ToString();
                            model.idGenero = reader["idGenero"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"].ToString());
                            model.genero = reader["genero"].ToString();
                            model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            model.tipoLicencia = reader["tipoLicencia"].ToString();
                            model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());

                            model.PersonaDireccion.idPersonasDirecciones = reader["idPersonasDirecciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonasDirecciones"].ToString());
                            model.PersonaDireccion.idEntidad = reader["idEntidad"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idEntidad"].ToString());
                            model.PersonaDireccion.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.PersonaDireccion.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.PersonaDireccion.entidad = reader["nombreEntidad"].ToString();
                            model.PersonaDireccion.municipio = reader["municipio"].ToString();
                            model.PersonaDireccion.codigoPostal = reader["codigoPostal"].ToString();
                            model.PersonaDireccion.colonia = reader["colonia"].ToString();
                            model.PersonaDireccion.calle = reader["calle"].ToString();
                            model.PersonaDireccion.numero = reader["numero"].ToString();
                            model.PersonaDireccion.telefono = reader["telefono"] == System.DBNull.Value ? default(int) : Convert.ToInt64(reader["telefono"].ToString());
                            model.PersonaDireccion.correo = reader["correo"].ToString();

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
            return modelList.Count() == 0 ? new PersonaModel() : modelList.FirstOrDefault();
            //return modelList;
        }

        public IEnumerable<PersonaModel> GetAllPersonasMorales()
        {
            List<PersonaModel> modelList = new List<PersonaModel>();
            string strQuery = @"SELECT
                                p.idPersona,p.numeroLicencia,p.CURP,p.RFC,p.nombre,p.apellidoPaterno,p.apellidoMaterno
                                ,p.fechaActualizacion,p.actualizadoPor,p.estatus,p.idCatTipoPersona,p.idTipoLicencia,p.fechaNacimiento
                                ,p.idGenero,p.vigenciaLicencia,ctp.tipoPersona,cl.tipoLicencia,cg.genero
                                ,pd.idPersonasDirecciones,pd.idEntidad,pd.idMunicipio,pd.codigoPostal
                                ,pd.colonia,pd.calle,pd.numero,pd.telefono,pd.correo,pd.idPersona,pd.actualizadoPor
                                ,pd.fechaActualizacion,pd.estatus,ce.nombreEntidad
                                ,ce.fechaActualizacion,ce.actualizadoPor,ce.estatus
                                ,cm.municipio,cm.fechaActualizacion,cm.actualizadoPor,cm.estatus
                                FROM personas p
                                LEFT JOIN catTipoPersona ctp
                                on p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1
                                LEFT JOIN catTipoLicencia cl
                                on p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1
                                LEFT JOIN catGeneros cg
                                on p.idGenero = cg.idGenero AND cg.estatus = 1
                                LEFT JOIN personasDirecciones pd  on p.idPersona = pd.idPersona AND pd.estatus=1
                                LEFT JOIN catEntidades ce on pd.idEntidad = ce.idEntidad  AND ce.estatus=1
                                LEFT JOIN catMunicipios cm on pd.idMunicipio = cm.idMunicipio AND cm.estatus=1
                                WHERE p.estatus = 1";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaModel model = new PersonaModel();
                            model.PersonaDireccion = new PersonaDireccionModel();
                            model.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();
                            model.nombre = reader["nombre"].ToString();
                            model.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.tipoPersona = reader["tipoPersona"].ToString();
                            model.idGenero = reader["idGenero"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"].ToString());
                            model.genero = reader["genero"].ToString();
                            model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            model.tipoLicencia = reader["tipoLicencia"].ToString();
                            model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());

                            model.PersonaDireccion.idPersonasDirecciones = reader["idPersonasDirecciones"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersonasDirecciones"].ToString());
                            model.PersonaDireccion.idEntidad = reader["idEntidad"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idEntidad"].ToString());
                            model.PersonaDireccion.idMunicipio = reader["idMunicipio"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idMunicipio"].ToString());
                            model.PersonaDireccion.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.PersonaDireccion.entidad = reader["nombreEntidad"].ToString();
                            model.PersonaDireccion.municipio = reader["municipio"].ToString();
                            model.PersonaDireccion.codigoPostal = reader["codigoPostal"].ToString();
                            model.PersonaDireccion.colonia = reader["colonia"].ToString();
                            model.PersonaDireccion.calle = reader["calle"].ToString();
                            model.PersonaDireccion.numero = reader["numero"].ToString();
                            model.PersonaDireccion.telefono = reader["telefono"] == System.DBNull.Value ? default(int) : Convert.ToInt64(reader["telefono"].ToString());
                            model.PersonaDireccion.correo = reader["correo"].ToString();

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

        public int CreatePersonaMoral(PersonaModel model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO personas(numeroLicencia,CURP,RFC,nombre,apellidoPaterno
                              ,apellidoMaterno,fechaActualizacion,actualizadoPor,estatus,idCatTipoPersona
                              ,idTipoLicencia
                              ) VALUES (@numeroLicencia
							,@CURP
							,@RFC
							,@nombre
							,@apellidoPaterno
							,@apellidoMaterno
							,@fechaActualizacion
							,@actualizadoPor
							,@estatus
							,@idCatTipoPersona	
							,@idTipoLicencia
							);SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.numeroLicencia ?? DBNull.Value; 
                    command.Parameters.Add(new SqlParameter("@CURP", SqlDbType.NVarChar)).Value = (object)model.CURP ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@RFC", SqlDbType.NVarChar)).Value = (object)model.RFC ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar)).Value = (object)model.nombre ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoPaterno ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoMaterno ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;

                    command.Parameters.Add(new SqlParameter("@idCatTipoPersona", SqlDbType.Int)).Value = (object)model.idCatTipoPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = 1;

                    //command.Parameters.Add(new SqlParameter("@fechaNacimiento", SqlDbType.DateTime)).Value = (object)model.fechaNacimiento ?? DBNull.Value;
                    //command.Parameters.Add(new SqlParameter("@vigenciaLicencia", SqlDbType.DateTime)).Value = (object)model.vigenciaLicencia ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    result = Convert.ToInt32(command.ExecuteScalar());
                    model.PersonaDireccion.idPersona = result;
                    var resultIdDireccion = CreatePersonaDireccion(model.PersonaDireccion);

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

        public int CreatePersona(PersonaModel model)
        {
            int result = 0;

            // Primero, verifica si ya existe un registro con la misma CURP
            string checkQuery = "SELECT top 1 idpersona as test FROM personas WHERE CURP = @CURP";

            using (SqlConnection checkConnection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    checkConnection.Open();

                    SqlCommand checkCommand = new SqlCommand(checkQuery, checkConnection);
                    checkCommand.Parameters.Add(new SqlParameter("@CURP", SqlDbType.NVarChar)).Value = (object)model.CURPFisico ?? DBNull.Value;

                    int existingRecordsCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (existingRecordsCount > 0)
                    {
                        // Ya existe un registro con la misma CURP, muestra un mensaje o lanza una excepción
                        return existingRecordsCount; 
                    }
                }
                catch (SqlException ex)
                {
                    // Manejo de errores
                    return result;
                }
                finally
                {
                    checkConnection.Close();
                }
            }

            DateTime? vigenciaLicenciaValue = (model.vigenciaLicencia > DateTime.MinValue) ? model.vigenciaLicencia : null;

            string strQuery = @"INSERT INTO personas(numeroLicencia,CURP,RFC,nombre,apellidoPaterno
                              ,apellidoMaterno,fechaActualizacion,actualizadoPor,estatus,idCatTipoPersona
                              ,idTipoLicencia
                              ,fechaNacimiento
                              ,vigenciaLicencia
                              ,idGenero
                              ) VALUES (@numeroLicencia
							,@CURP
							,@RFC
							,@nombre
							,@apellidoPaterno
							,@apellidoMaterno
							,@fechaActualizacion
							,@actualizadoPor
							,@estatus
							,@idCatTipoPersona	
							,@idTipoLicencia
                            ,@fechaNacimiento
                            ,@vigenciaLicencia
                            ,@idGenero
							);SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.numeroLicencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@CURP", SqlDbType.NVarChar)).Value = (object)model.CURP ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@RFC", SqlDbType.NVarChar)).Value = (object)model.RFC ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar)).Value = (object)model.nombre ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoPaterno ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoMaterno ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;

                    command.Parameters.Add(new SqlParameter("@idCatTipoPersona", SqlDbType.Int)).Value = (object)model.idCatTipoPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)model.idTipoLicencia ?? DBNull.Value;

                    command.Parameters.Add(new SqlParameter("@fechaNacimiento", SqlDbType.DateTime)).Value = (object)model.fechaNacimiento ?? DBNull.Value;
                    //command.Parameters.Add(new SqlParameter("@vigenciaLicencia", SqlDbType.DateTime)).Value = (object)(model.vigenciaLicencia.HasValue? model.vigenciaLicencia.Value : null);

                    //DateTime vigenciaLicenciaValue = (model.vigenciaLicencia != DateTime.MinValue) ? model.vigenciaLicencia : DateTime.MinValue;
                    command.Parameters.Add(new SqlParameter("@vigenciaLicencia", SqlDbType.DateTime)).Value = (object)vigenciaLicenciaValue ?? DBNull.Value; 


                    command.Parameters.Add(new SqlParameter("@idGenero", SqlDbType.Int)).Value = (object)model.idGenero ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    result = Convert.ToInt32(command.ExecuteScalar());
                    model.PersonaDireccion.idPersona = result;
                    var resultIdDireccion = CreatePersonaFisicaDireccion(model.PersonaDireccion);

                }
                catch (SqlException ex)
                {
                    return 0;
                }
                finally
                {
                    connection.Close();
                }
            }


            return result;
        }

        public int UpdatePersona(PersonaModel model)
        {
            int result = 0;
            string strQuery = @"
                            Update personas
                            set 
                             numeroLicencia	   = @numeroLicencia
                            ,CURP			   = @CURP
                            ,RFC			   = @RFC
                            ,nombre			   = @nombre
                            ,apellidoPaterno   = @apellidoPaterno
                            ,apellidoMaterno   = @apellidoMaterno
                            ,fechaActualizacion= @fechaActualizacion
                            ,actualizadoPor	   = @actualizadoPor
                            ,estatus		   = @estatus
                            ,idCatTipoPersona  = @idCatTipoPersona
                            ,idTipoLicencia	   = @idTipoLicencia
                            ,fechaNacimiento = @fechaNacimiento
                            ,vigenciaLicencia = @vigenciaLicencia
                            ,idGenero = @idGenero
                            where idPersona= @idPersona";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)model.idPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.numeroLicencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@CURP", SqlDbType.NVarChar)).Value = (object)model.CURP ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@RFC", SqlDbType.NVarChar)).Value = (object)model.RFC ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar)).Value = (object)model.nombre ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoPaterno ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoMaterno ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@idCatTipoPersona", SqlDbType.Int)).Value = (object)model.idCatTipoPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)model.idTipoLicencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaNacimiento", SqlDbType.DateTime)).Value = (object)model.fechaNacimiento ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@vigenciaLicencia", SqlDbType.DateTime)).Value = (object)model.vigenciaLicencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idGenero", SqlDbType.Int)).Value = (object)model.idGenero ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    //result = Convert.ToInt32(command.ExecuteScalar());
                    result = command.ExecuteNonQuery();
                    model.PersonaDireccion.idPersona = model.idPersona;
                    var resultIdDireccion = UpdatePersonaDireccion(model.PersonaDireccion);

                }
                catch (SqlException ex)
                {
                    return 0;
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }

        public int CreatePersonaDireccion(PersonaDireccionModel model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO personasDirecciones(
                                idEntidad,idMunicipio,codigoPostal,colonia,calle,numero
                                ,telefono,correo,idPersona,actualizadoPor,fechaActualizacion,estatus
                                 )
                                VALUES (@idEntidad
                                ,@idMunicipio
                                ,@codigoPostal
                                ,@colonia
                                ,@calle
                                ,@numero
                                ,@telefono
                                ,@correo
                                ,@idPersona
                                ,@actualizadoPor
                                ,@fechaActualizacion
                                ,@estatus);SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.idEntidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@codigoPostal", SqlDbType.NVarChar)).Value = (object)model.codigoPostal ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@colonia", SqlDbType.NVarChar)).Value = (object)model.colonia;
                    command.Parameters.Add(new SqlParameter("@calle", SqlDbType.NVarChar)).Value = (object)model.calle;
                    command.Parameters.Add(new SqlParameter("@numero", SqlDbType.NVarChar)).Value = (object)model.numero;
                    command.Parameters.Add(new SqlParameter("@telefono", SqlDbType.BigInt)).Value = (object)model.telefono ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@correo", SqlDbType.NVarChar)).Value = (object)model.correo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)model.idPersona;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;
                    command.CommandType = CommandType.Text;

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
        public int CreatePersonaFisicaDireccion(PersonaDireccionModel model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO personasDirecciones(
                                idEntidad,idMunicipio,codigoPostal,colonia,calle,numero
                                ,telefono,correo,idPersona,actualizadoPor,fechaActualizacion,estatus
                                 )
                                VALUES (@idEntidad
                                ,@idMunicipio
                                ,@codigoPostal
                                ,@colonia
                                ,@calle
                                ,@numero
                                ,@telefono
                                ,@correo
                                ,@idPersona
                                ,@actualizadoPor
                                ,@fechaActualizacion
                                ,@estatus);SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.idEntidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@codigoPostal", SqlDbType.NVarChar)).Value = (object)model.codigoPostal ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@colonia", SqlDbType.NVarChar)).Value = (object)model.colonia;
                    command.Parameters.Add(new SqlParameter("@calle", SqlDbType.NVarChar)).Value = (object)model.calle;
                    command.Parameters.Add(new SqlParameter("@numero", SqlDbType.NVarChar)).Value = (object)model.numero;
                    command.Parameters.Add(new SqlParameter("@telefono", SqlDbType.BigInt)).Value = (object)model.telefono ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@correo", SqlDbType.NVarChar)).Value = (object)model.correo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)model.idPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;
                    command.CommandType = CommandType.Text;

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


        public int UpdatePersonaMoral(PersonaModel model)
        {
            int result = 0;
            string strQuery = @"
                            Update personas
                            set 
                             numeroLicencia	   = @numeroLicencia
                            ,CURP			   = @CURP
                            ,RFC			   = @RFC
                            ,nombre			   = @nombre
                            ,apellidoPaterno   = @apellidoPaterno
                            ,apellidoMaterno   = @apellidoMaterno
                            ,fechaActualizacion= @fechaActualizacion
                            ,actualizadoPor	   = @actualizadoPor
                            ,estatus		   = @estatus
                            ,idCatTipoPersona  = @idCatTipoPersona
                            ,idTipoLicencia	   = @idTipoLicencia
                            where idPersona= @idPersona";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)model.idPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.numeroLicencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@CURP", SqlDbType.NVarChar)).Value = (object)model.CURP ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@RFC", SqlDbType.NVarChar)).Value = (object)model.RFC ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar)).Value = (object)model.nombre ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoPaterno ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoMaterno ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@idCatTipoPersona", SqlDbType.Int)).Value = (object)model.idCatTipoPersona ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)model.idTipoLicencia ?? DBNull.Value;

                    command.CommandType = CommandType.Text;
                    //result = Convert.ToInt32(command.ExecuteScalar());
                    result = command.ExecuteNonQuery();
                    model.PersonaDireccion.idPersona = model.idPersona;
                    var resultIdDireccion = UpdatePersonaDireccion(model.PersonaDireccion);

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

        public int UpdatePersonaDireccion(PersonaDireccionModel model)
        {
            int result = 0;
            string strQuery = @"Update personasDirecciones
                                SET
                                idEntidad			= @idEntidad
                                ,idMunicipio		= @idMunicipio
                                ,codigoPostal		= @codigoPostal
                                ,colonia			= @colonia
                                ,calle				= @calle
                                ,numero				= @numero
                                ,telefono			= @telefono
                                ,correo				= @correo
                                ,idPersona			= @idPersona
                                ,actualizadoPor		= @actualizadoPor
                                ,fechaActualizacion	= @fechaActualizacion
                                ,estatus			= @estatus
                                where idPersonasDirecciones = @idPersonasDirecciones";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.Add(new SqlParameter("@idPersonasDirecciones", SqlDbType.Int)).Value = (object)model.idPersonasDirecciones ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idEntidad", SqlDbType.Int)).Value = (object)model.idEntidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = (object)model.idMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@codigoPostal", SqlDbType.NVarChar)).Value = (object)model.codigoPostal ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@colonia", SqlDbType.NVarChar)).Value = (object)model.colonia;
                    command.Parameters.Add(new SqlParameter("@calle", SqlDbType.NVarChar)).Value = (object)model.calle;
                    command.Parameters.Add(new SqlParameter("@numero", SqlDbType.NVarChar)).Value = (object)model.numero;
                    command.Parameters.Add(new SqlParameter("@telefono", SqlDbType.BigInt)).Value = (object)model.telefono ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@correo", SqlDbType.NVarChar)).Value = (object)model.correo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)model.idPersona;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;
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
        public List<PersonaModel> BusquedaPersona(PersonaModel model)
        {
            //
            List<PersonaModel> ListaPersonas = new List<PersonaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();


                    const string SqlTransact = @"SELECT p.idPersona,
       p.numeroLicencia,
       p.CURP,
       p.RFC,
       p.nombre,
       p.apellidoPaterno,
       p.apellidoMaterno,
       p.fechaActualizacion,
       p.actualizadoPor,
       p.estatus,
       p.idCatTipoPersona,
       p.idTipoLicencia,
       p.fechaNacimiento,
       p.idGenero,
       p.vigenciaLicencia,
       ctp.tipoPersona,
       cl.tipoLicencia,
       cg.genero
FROM personas p
LEFT JOIN catTipoPersona ctp ON p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1
LEFT JOIN catTipoLicencia cl ON p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1
LEFT JOIN catGeneros cg ON p.idGenero = cg.idGenero AND cg.estatus = 1
WHERE
    (p.estatus = 1)
    AND (@numeroLicencia IS NULL OR p.numeroLicencia LIKE '%' + @numeroLicencia + '%')
    AND (@curp IS NULL OR p.CURP LIKE '%' + @curp + '%')
    AND (@rfc IS NULL OR p.RFC LIKE '%' + @rfc + '%')
    AND (@nombre IS NULL OR p.nombre LIKE '%' + @nombre + '%')
    AND (@apellidoPaterno IS NULL OR p.apellidoPaterno LIKE '%' + @apellidoPaterno + '%')
    AND (@apellidoMaterno IS NULL OR p.apellidoMaterno LIKE '%' + @apellidoMaterno + '%');
";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@numeroLicencia", SqlDbType.NVarChar)).Value = (object)model.numeroLicenciaBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@curp", SqlDbType.NVarChar)).Value = (object)model.CURPBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@rfc", SqlDbType.NVarChar)).Value = (object)model.RFCBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar)).Value = (object)model.nombreBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoPaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoPaternoBusqueda ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@apellidoMaterno", SqlDbType.NVarChar)).Value = (object)model.apellidoMaternoBusqueda ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaModel persona = new PersonaModel();

                            persona.PersonaDireccion = new PersonaDireccionModel();
                            persona.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            persona.numeroLicencia = reader["numeroLicencia"].ToString();
                            persona.CURP = reader["CURP"].ToString();
                            persona.RFC = reader["RFC"].ToString();
                            persona.nombre = reader["nombre"].ToString();
                            persona.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            persona.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            persona.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            persona.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            persona.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            persona.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            persona.tipoPersona = reader["tipoPersona"].ToString();
                            persona.idGenero = reader["idGenero"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"].ToString());
                            persona.genero = reader["genero"].ToString();
                            persona.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            persona.tipoLicencia = reader["tipoLicencia"].ToString();
                            persona.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            persona.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
                            //persona.PersonaDireccion = GetPersonaDireccionByIdPersona((int)model.idPersona);

                            ListaPersonas.Add(persona);

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
            return ListaPersonas;


        }

        public bool VerificarLicenciaSitteg(string numeroLicencia)
        {
            bool licenciaNoSITTEG = false;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM personas WHERE numeroLicencia = @NumeroLicencia";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NumeroLicencia", numeroLicencia);

                    int count = (int)command.ExecuteScalar();

                    // Si el resultado es 0, la licencia no existe en la tabla de personas
                    licenciaNoSITTEG = (count == 0);
                }
            }

            return licenciaNoSITTEG;
        }

        public int InsertarDesdeServicio(LicenciaPersonaDatos personaDatos)
        {
            int insertedId = 0; 
            int idPersona = ExistePersona(personaDatos.NUM_LICENCIA, personaDatos.CURP);

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open(); 

                if (idPersona==0) {
                    string query = "INSERT INTO personas (numeroLicencia,CURP,RFC,nombre,apellidoPaterno,apellidoMaterno,fechaActualizacion,actualizadoPor,estatus,idCatTipoPersona,idGenero,fechaNacimiento,idTipoLicencia,vigenciaLicencia) " +
                                         "VALUES (@NumeroLicencia,@curp,@rfc,@nombre,@apellidoPaterno,@apellidoMaterno,@fechaActualizacion,@actualizadoPor,@estatus,@tipoPersona,@genero,@fechaNacimiento,@idTipolicencia,@fechaVigencia)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NumeroLicencia", (object)personaDatos.NUM_LICENCIA ?? DBNull.Value);
                        command.Parameters.AddWithValue("@curp", string.IsNullOrEmpty(personaDatos.CURP) ? "" : personaDatos.CURP);
                        command.Parameters.AddWithValue("@rfc", string.IsNullOrEmpty(personaDatos.RFC) ? "" : personaDatos.RFC);
                        command.Parameters.AddWithValue("@nombre", string.IsNullOrEmpty(personaDatos.NOMBRE) ? "" : personaDatos.NOMBRE);
                        command.Parameters.AddWithValue("@apellidoPaterno", string.IsNullOrEmpty(personaDatos.PRIMER_APELLIDO) ? "" : personaDatos.PRIMER_APELLIDO);
                        command.Parameters.AddWithValue("@apellidoMaterno", string.IsNullOrEmpty(personaDatos.SEGUNDO_APELLIDO) ? "" : personaDatos.SEGUNDO_APELLIDO);

                        command.Parameters.AddWithValue("@tipoPersona", (int)TipoPersona.Fisica);
                        command.Parameters.AddWithValue("@genero", personaDatos.ID_GENERO == null ? "" : personaDatos.ID_GENERO );
                        command.Parameters.AddWithValue("@fechaNacimiento", personaDatos.FECHA_NACIMIENTO == null ? "" : personaDatos.FECHA_NACIMIENTO);
                        command.Parameters.AddWithValue("@idTipolicencia", personaDatos.ID_TIPO_LICENCIA == null ? "" : personaDatos.ID_TIPO_LICENCIA);
                        command.Parameters.AddWithValue("@fechaVigencia", personaDatos.FECHA_TERMINO_VIGENCIA == null ? "" : personaDatos.FECHA_TERMINO_VIGENCIA);

                        command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                        command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                        command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;

                        command.CommandType = CommandType.Text;
                        insertedId = Convert.ToInt32(command.ExecuteScalar());  
                    } 
                } 
            }
            idPersona = ExistePersona(personaDatos.NUM_LICENCIA, personaDatos.CURP);
            insertarDireccion(personaDatos, idPersona);
            return (idPersona);
        }

        public void insertarDireccion(LicenciaPersonaDatos personaDatos, int insertado)
        {
            int insertedDireccion = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                CatEntidadesModel entidad = _catEntidadesService.ObtenerEntidadesByNombre("Guanajuato");
                string qryDomicilio = "INSERT INTO personasDirecciones (idEntidad,idMunicipio,codigoPostal,colonia,calle,numero,telefono,correo,idPersona,actualizadoPor,fechaActualizacion,estatus)" +
                       "VALUES(@idEntidad,@idMunicipio,@codigoPostal,@colonia,@calle,@numero,@telefono,@correo,@idPersona,@actualizadoPor,@fechaActualizacion,@estatus)";
                
                using (SqlCommand command = new SqlCommand(qryDomicilio, connection))
                {

                    command.Parameters.AddWithValue("@idEntidad", entidad.idEntidad);
                    command.Parameters.AddWithValue("@idMunicipio",personaDatos.ID_MUNICIPIO == null ? "" : personaDatos.ID_MUNICIPIO );
                    command.Parameters.AddWithValue("@codigoPostal", string.IsNullOrEmpty(personaDatos.CP) ? "" : personaDatos.CP) ;
                    command.Parameters.AddWithValue("@colonia", string.IsNullOrEmpty(personaDatos.COLONIA) ? "" : personaDatos.COLONIA);
                    command.Parameters.AddWithValue("@calle", string.IsNullOrEmpty(personaDatos.CALLE) ? "" : personaDatos.CALLE);
                    command.Parameters.AddWithValue("@numero", string.IsNullOrEmpty(personaDatos.NUM_EXT) ? "" : personaDatos.NUM_EXT);
                    command.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(personaDatos.TELEFONO1) ? "" : personaDatos.TELEFONO1);
                    command.Parameters.AddWithValue("@correo", string.IsNullOrEmpty(personaDatos.EMAIL) ? "" : personaDatos.EMAIL);
                    command.Parameters.AddWithValue("@idPersona", insertado);
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;
                     
                    command.CommandType = CommandType.Text;
                    insertedDireccion = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int ExistePersona(string licencia, string curp)
        {
            int idPersona = 0;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();

                // Verificar si alguno de los parámetros es null
                if (licencia == null && curp == null)
                {
                    // Manejar la lógica cuando ambos parámetros son null
                    return idPersona;
                }

                // Construir la consulta SQL basada en los parámetros no nulos
                string query = "SELECT idPersona FROM PERSONAS p WHERE ";
                if (licencia != null)
                {
                    query += "numeroLicencia=@licencia ";
                }
                if (curp != null)
                {
                    if (licencia != null)
                    {
                        query += "OR ";
                    }
                    query += "CURP=@curp";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (licencia != null)
                    {
                        command.Parameters.AddWithValue("@licencia", licencia);
                    }
                    if (curp != null)
                    {
                        command.Parameters.AddWithValue("@curp", curp);
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idPersona = reader["idPersona"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"]);
                        }
                    }
                }

            }

            return idPersona;
        }


        public PersonaModel BuscarPersonaSoloLicencia(string numeroLicencia)
        {
            PersonaModel personaEncontrada = null;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();

                string query = "SELECT " +
               "p.idPersona, " +
               "p.numeroLicencia, " +
               "p.CURP, " +
               "p.RFC, " + 
               "p.nombre, " +
               "p.apellidoPaterno, " +
               "p.apellidoMaterno, " +
               "p.fechaActualizacion, " +
               "p.actualizadoPor, " +
               "p.estatus, " +
               "p.idCatTipoPersona, " +
               "p.idTipoLicencia, " +
               "p.fechaNacimiento, " +
               "p.idGenero, " +
               "p.vigenciaLicencia, " +
               "ctp.tipoPersona, " +
               "cl.tipoLicencia, " +
               "cg.genero " +
               "FROM personas p " +
               "LEFT JOIN catTipoPersona ctp ON p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1 " +
               "LEFT JOIN catTipoLicencia cl ON p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1 " +
               "LEFT JOIN catGeneros cg ON p.idGenero = cg.idGenero AND cg.estatus = 1 " +
               "WHERE p.estatus = 1 " +
               "AND p.numeroLicencia = @numeroLicencia;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@numeroLicencia", numeroLicencia);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            personaEncontrada = new PersonaModel();
                            personaEncontrada.PersonaDireccion = new PersonaDireccionModel();
                            personaEncontrada.idPersona = reader["idPersona"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"]);
                            personaEncontrada.numeroLicencia = reader["numeroLicencia"].ToString();
                            personaEncontrada.CURP = reader["CURP"].ToString();
                            personaEncontrada.RFC = reader["RFC"].ToString();
                            personaEncontrada.nombre = reader["nombre"].ToString();
                            personaEncontrada.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            personaEncontrada.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            personaEncontrada.fechaActualizacion = reader["fechaActualizacion"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"]);
                            personaEncontrada.actualizadoPor = reader["actualizadoPor"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"]);
                            personaEncontrada.estatus = reader["estatus"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"]);
                            personaEncontrada.idCatTipoPersona = reader["idCatTipoPersona"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"]);
                            personaEncontrada.tipoPersona = reader["tipoPersona"].ToString();
                            personaEncontrada.idGenero = reader["idGenero"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"]);
                            personaEncontrada.genero = reader["genero"].ToString();
                            personaEncontrada.idTipoLicencia = reader["idTipoLicencia"] == DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"]);
                            personaEncontrada.tipoLicencia = reader["tipoLicencia"].ToString();
                            personaEncontrada.fechaNacimiento = reader["fechaNacimiento"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"]);
                            personaEncontrada.vigenciaLicencia = reader["vigenciaLicencia"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"]);
                            personaEncontrada.PersonaDireccion = GetPersonaDireccionByIdPersona((int)personaEncontrada.idPersona);
                        }
                    }
                }
            }

            return personaEncontrada;
        }
       public List<PersonaModel> ObterPersonaPorIDList(int idPersona)
        {
            //
            List<PersonaModel> ListaPersonas = new List<PersonaModel>();

            string strQuery = @"SELECT
                                p.idPersona
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
							   ,p.idTipoLicencia
							   ,p.fechaNacimiento
							   ,p.idGenero
							   ,p.vigenciaLicencia
                               ,ctp.tipoPersona
							   ,cl.tipoLicencia
							   ,cg.genero
                               FROM personas p
                               LEFT JOIN catTipoPersona ctp
                               on p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1
							   LEFT JOIN catTipoLicencia cl
							   on p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1
							   LEFT JOIN catGeneros cg
							   on p.idGenero = cg.idGenero
                               WHERE p.estatus = 1
                               AND p.idPersona = @idPersona";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idPersona ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaModel persona = new PersonaModel();
                            persona.PersonaDireccion = new PersonaDireccionModel();
                            persona.idPersona = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            persona.numeroLicencia = reader["numeroLicencia"].ToString();
                            persona.CURP = reader["CURP"].ToString();
                            persona.RFC = reader["RFC"].ToString();
                            persona.nombre = reader["nombre"].ToString();
                            persona.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            persona.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            persona.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            persona.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            persona.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            persona.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            persona.tipoPersona = reader["tipoPersona"].ToString();
                            persona.idGenero = reader["idGenero"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"].ToString());
                            persona.genero = reader["genero"].ToString();
                            persona.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            persona.tipoLicencia = reader["tipoLicencia"].ToString();
                            persona.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            persona.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
                            persona.PersonaDireccion = GetPersonaDireccionByIdPersona((int)persona.idPersona);
                            ListaPersonas.Add(persona);
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
            return ListaPersonas;
        }
        public PersonaInfraccionModel GetPersonaInfraccionById(int idPersona)
        {
            List<PersonaInfraccionModel> modelList = new List<PersonaInfraccionModel>();
            string strQuery = @"SELECT
                                p.idPersona
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
							   ,p.idTipoLicencia
							   ,p.fechaNacimiento
							   ,p.idGenero
							   ,p.vigenciaLicencia
                               ,ctp.tipoPersona
							   ,cl.tipoLicencia
							   ,cg.genero
                               FROM personas p
                               LEFT JOIN catTipoPersona ctp
                               on p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1
							   LEFT JOIN catTipoLicencia cl
							   on p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1
							   LEFT JOIN catGeneros cg
							   on p.idGenero = cg.idGenero
                               WHERE p.estatus = 1
                               AND p.idPersona = @idPersona";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPersona", SqlDbType.Int)).Value = (object)idPersona ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PersonaInfraccionModel model = new PersonaInfraccionModel();
                            model.PersonaDireccion = new PersonaDireccionModel();
                            model.idPersonaInfraccion = reader["idPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idPersona"].ToString());
                            model.numeroLicencia = reader["numeroLicencia"].ToString();
                            model.CURP = reader["CURP"].ToString();
                            model.RFC = reader["RFC"].ToString();
                            model.nombre = reader["nombre"].ToString();
                            model.apellidoPaterno = reader["apellidoPaterno"].ToString();
                            model.apellidoMaterno = reader["apellidoMaterno"].ToString();
                            model.fechaActualizacion = reader["fechaActualizacion"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaActualizacion"].ToString());
                            model.actualizadoPor = reader["actualizadoPor"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["actualizadoPor"].ToString());
                            model.estatus = reader["estatus"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["estatus"].ToString());
                            model.idCatTipoPersona = reader["idCatTipoPersona"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idCatTipoPersona"].ToString());
                            model.tipoPersona = reader["tipoPersona"].ToString();
                            model.idGenero = reader["idGenero"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idGenero"].ToString());
                            model.genero = reader["genero"].ToString();
                            model.idTipoLicencia = reader["idTipoLicencia"] == System.DBNull.Value ? default(int) : Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            model.tipoLicencia = reader["tipoLicencia"].ToString();
                            model.fechaNacimiento = reader["fechaNacimiento"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["fechaNacimiento"].ToString());
                            model.vigenciaLicencia = reader["vigenciaLicencia"] == System.DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["vigenciaLicencia"].ToString());
                            model.PersonaDireccion = GetPersonaDireccionByIdPersona((int)model.idPersonaInfraccion);
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
            return modelList.FirstOrDefault();
        }
    }

}



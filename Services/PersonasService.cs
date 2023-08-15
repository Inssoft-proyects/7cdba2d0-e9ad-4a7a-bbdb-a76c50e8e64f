﻿using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class PersonasService : IPersonasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;

        public PersonasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public IEnumerable<PersonaModel> GetAllPersonas()
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
							   on p.idGenero = cg.idGenero AND cg.estatus = 1
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
                    command.Parameters.Add(new SqlParameter("@idTipoLicencia", SqlDbType.Int)).Value = (object)model.idTipoLicencia ?? DBNull.Value;

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
                    command.Parameters.Add(new SqlParameter("@vigenciaLicencia", SqlDbType.DateTime)).Value = (object)model.vigenciaLicencia ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idGenero", SqlDbType.Int)).Value = (object)model.idGenero ?? DBNull.Value;
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
                    return result;
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


                    const string SqlTransact = @"SELECT p.idPersona " +
                               ",p.numeroLicencia " +
                               ",p.CURP " +
                               ",p.RFC " +
                               ",p.nombre " +
                               ",p.apellidoPaterno " +
                               ",p.apellidoMaterno " +
                               ",p.fechaActualizacion " +
                               ",p.actualizadoPor " +
                               ",p.estatus " +
                               ",p.idCatTipoPersona " +
                               ",p.idTipoLicencia " +
							   ",p.fechaNacimiento " +
							   ",p.idGenero " +
							   ",p.vigenciaLicencia " +
                               ",ctp.tipoPersona " +
							   ",cl.tipoLicencia " +
							   ",cg.genero " +
                               "FROM personas p " +
                               "LEFT JOIN catTipoPersona ctp " +
                               "on p.idCatTipoPersona = ctp.idCatTipoPersona AND ctp.estatus = 1 " +
							   "LEFT JOIN catTipoLicencia cl " +
							   "on p.idTipoLicencia = cl.idTipoLicencia AND cl.estatus = 1 " +
							   "LEFT JOIN catGeneros cg " +
							   "on p.idGenero = cg.idGenero AND cg.estatus = 1 " +
                               "WHERE (numeroLicencia LIKE '%' + @numeroLicencia + '%' OR curp LIKE '%' + @curp  + '%' " +
                               "OR rfc LIKE '%' + @rfc + '%' OR nombre LIKE '%' + @nombre OR apellidoPaterno LIKE '%' + @apellidoPaterno + '%' OR apellidoMaterno LIKE '%' + @apellidoMaterno) " +
                               "AND p.estatus = 1";

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

        public void InsertarDesdeServicio(ResultadoLicenciaModel persona)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();

                string query = "INSERT INTO personas (nombre, apellidoPaterno, apellidoMaterno,vigenciaLicencia, numeroLicencia,idTipoLicencia,actualizadoPor, fechaActualizacion,estatus) " +
                               "VALUES (@nombre, @apellidoPaterno, @apellidoMaterno,@fechaVigencia,@NumeroLicencia,@tipoLicenciaVal,@actualizadoPor,@fechaActualizacion,@estatus)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre", persona.Nombre);
                    command.Parameters.AddWithValue("@apellidoPaterno", persona.ApellidoPaterno);
                    command.Parameters.AddWithValue("@apellidoMaterno", persona.ApellidoMaterno);
                    command.Parameters.AddWithValue("@fechaVigencia", persona.FechaVigencia);
                    command.Parameters.AddWithValue("@NumeroLicencia", persona.NumeroLicencia);
                    command.Parameters.AddWithValue("@tipoLicenciaVal", persona.tipoLicenciaVal);
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = (object)DateTime.Now;
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = (object)1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = (object)1;

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}


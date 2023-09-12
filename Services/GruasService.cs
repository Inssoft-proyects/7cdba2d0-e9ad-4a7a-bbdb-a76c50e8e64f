using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class GruasService : IGruasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public GruasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<GruasConcesionariosModel> GetGruasConcesionariosByIdCocesionario(int Id)
        {
            List<GruasConcesionariosModel> GruasConcesionariosList = new List<GruasConcesionariosModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    const string SqlTransact =
                        @"select g.IdGrua,  c.IdConcesionario,g.noEconomico,g.placas,g.modelo,g.capacidad,
                            c.Concesionario,catg.IdTipoGrua, catg.TipoGrua
                            from concesionarios c
                            inner join Gruas g on g.idConcesionario= c.idConcesionario
                            inner join catTipoGrua catg ON catg.IdTipoGrua=g.IdTipoGrua
                            where c.IdConcesionario=@IdConcesionario";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdConcesionario", SqlDbType.Int)).Value = (object)Id ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            GruasConcesionariosModel gruasConcesionario = new GruasConcesionariosModel();
                            gruasConcesionario.IdGrua = Convert.ToInt32(reader["IdGrua"].ToString());
                            gruasConcesionario.IdConcesionario = Convert.ToInt32(reader["IdConcesionario"].ToString());
                            gruasConcesionario.IdTipoGrua = Convert.ToInt32(reader["IdTipoGrua"].ToString());
                            gruasConcesionario.noEconomico = reader["noEconomico"].ToString();
                            gruasConcesionario.placas = reader["placas"].ToString();
                            gruasConcesionario.modelo = reader["modelo"].ToString();
                            gruasConcesionario.capacidad = reader["capacidad"].ToString();
                            //gruasConcesionario.clasificacion = reader["clasificacion"].ToString();
                            gruasConcesionario.Concesionario = reader["Concesionario"].ToString();
                            gruasConcesionario.TipoGrua = reader["TipoGrua"].ToString();
                            GruasConcesionariosList.Add(gruasConcesionario);

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
            return GruasConcesionariosList;

        }

        public List<TipoGruaModel> GetTipoGruas()
        {
            List<TipoGruaModel> ListTipoGruas = new List<TipoGruaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catTipoGrua where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            TipoGruaModel tipoGruaModel = new TipoGruaModel();
                            tipoGruaModel.IdTipoGrua = Convert.ToInt32(reader["IdTipoGrua"].ToString());
                            tipoGruaModel.TipoGrua = reader["TipoGrua"].ToString();
                            ListTipoGruas.Add(tipoGruaModel);

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
            return ListTipoGruas;

        }

        public List<GruasModel> GetGruas()
        {
            List<GruasModel> ListGruas = new List<GruasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from Gruas where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            GruasModel gruaModel = new GruasModel();
                            gruaModel.IdGrua = Convert.ToInt32(reader["IdGrua"].ToString());
                            gruaModel.Placas = reader["Placas"].ToString();
                            ListGruas.Add(gruaModel);

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
            return ListGruas;

        }

        public int CrearGrua(Gruas2Model model)
        {
            int result = 0;
            string strQuery = @"INSERT INTO gruas VALUES(@idConcesionario
                                                        ,@idClasificacion
                                                        ,@idTipoGrua
                                                        ,@idSituacion
                                                        ,@noEconomico
                                                        ,@placas
                                                        ,@modelo
                                                        ,@capacidad
                                                        ,@fechaActualizacion
                                                        ,@actualizadoPor
                                                        ,@estatus)";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    //command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)model.idGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idConcesionario", SqlDbType.Int)).Value = (object)model.idConcesionario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idClasificacion", SqlDbType.Int)).Value = (object)model.idClasificacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoGrua", SqlDbType.Int)).Value = (object)model.idTipoGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSituacion", SqlDbType.Int)).Value = (object)model.idSituacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@noEconomico", SqlDbType.NVarChar)).Value = (object)model.noEconomico ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placas", SqlDbType.NVarChar)).Value = (object)model.placas ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@modelo", SqlDbType.NVarChar)).Value = (object)model.modelo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@capacidad", SqlDbType.NVarChar)).Value = (object)model.capacidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
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

        public int EditarGrua(Gruas2Model model)
        {
            int result = 0;
            string strQuery = @"UPDATE gruas SET idConcesionario       = @idConcesionario    
                                                 ,idClasificacion	   = @idClasificacion	
                                                 ,idTipoGrua			   = @idTipoGrua			
                                                 ,idSituacion		   = @idSituacion		
                                                 ,noEconomico		   = @noEconomico		
                                                 ,placas				   = @placas				
                                                 ,modelo				   = @modelo				
                                                 ,capacidad			   = @capacidad			
                                                 ,fechaActualizacion	   = @fechaActualizacion	
                                                 ,actualizadoPor		   = @actualizadoPor		
                                                 ,estatus			   = @estatus		
                                WHERE idGrua = @idGrua;";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)model.idGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idConcesionario", SqlDbType.Int)).Value = (object)model.idConcesionario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idClasificacion", SqlDbType.Int)).Value = (object)model.idClasificacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idTipoGrua", SqlDbType.Int)).Value = (object)model.idTipoGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@idSituacion", SqlDbType.Int)).Value = (object)model.idSituacion ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@noEconomico", SqlDbType.NVarChar)).Value = (object)model.noEconomico ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@placas", SqlDbType.NVarChar)).Value = (object)model.placas ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@modelo", SqlDbType.NVarChar)).Value = (object)model.modelo ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@capacidad", SqlDbType.NVarChar)).Value = (object)model.capacidad ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
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

        public int EliminarGrua(Gruas2Model model)
        {
            int result = 0;
            string strQuery = @"UPDATE gruas SET fechaActualizacion	   = @fechaActualizacion	
                                                 ,actualizadoPor		   = @actualizadoPor		
                                                 ,estatus			   = @estatus		
                                WHERE idGrua = @idGrua";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)model.idGrua ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    command.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    command.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 0;
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

        public IEnumerable<Gruas2Model> GetAllGruas(int idOficina)
        {
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                 g.idGrua
                                ,g.idConcesionario
                                ,g.idClasificacion
                                ,g.idTipoGrua
                                ,g.idSituacion
                                ,g.noEconomico
                                ,g.placas
                                ,g.modelo
                                ,g.capacidad
                                ,g.fechaActualizacion
                                ,g.actualizadoPor
                                ,g.estatus
								,cm.municipio
								,c.concesionario
                                ,c.idDelegacion
								,ccg.clasificacion
								,ctg.TipoGrua
								,csg.situacion
                                FROM gruas g
								INNER JOIN catClasificacionGruas ccg
								on g.idClasificacion = ccg.idClasificacionGrua
								INNER JOIN catTipoGrua ctg
								on g.idTipoGrua = ctg.IdTipoGrua
								INNER JOIN catSituacionGruas csg
								on g.idSituacion = csg.idSituacion
								INNER JOIN concesionarios c
								on g.idConcesionario = c.idConcesionario AND c.estatus = 1
								INNER JOIN catMunicipios cm
								on c.idMunicipio = cm.idMunicipio AND c.estatus = 1
                                WHERE g.estatus = 1 AND  c.idDelegacion = @idOficina";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            gruasModel.concesionario = reader["concesionario"].ToString();
                            gruasModel.municipio = reader["municipio"].ToString();
                            gruasModel.clasificacion = reader["clasificacion"].ToString();
                            gruasModel.tipoGrua = reader["tipoGrua"].ToString();
                            gruasModel.situacion = reader["situacion"].ToString();
                            ListGruas.Add(gruasModel);
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
            return ListGruas;
        }

        public Gruas2Model GetGruaById(int idGrua)
        {
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                 idGrua
                                ,idConcesionario
                                ,idClasificacion
                                ,idTipoGrua
                                ,idSituacion
                                ,noEconomico
                                ,placas
                                ,modelo
                                ,capacidad
                                ,fechaActualizacion
                                ,actualizadoPor
                                ,estatus
                                FROM gruas
                                WHERE idGrua = @idGrua AND estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)idGrua ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            ListGruas.Add(gruasModel);
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
            return ListGruas.FirstOrDefault();
        }
        public List<Gruas2Model> GetGruaByPension(int iPg)

        {
            //
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                 idGrua
                                ,idConcesionario
                                ,idClasificacion
                                ,idTipoGrua
                                ,idSituacion
                                ,noEconomico
                                ,placas
                                ,modelo
                                ,capacidad
                                ,fechaActualizacion
                                ,actualizadoPor
                                ,estatus
                                FROM gruas
                                WHERE idConcesionario = @idPropietarioGrua AND estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idPropietarioGrua", SqlDbType.Int)).Value = (object)iPg ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            ListGruas.Add(gruasModel);
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
            return ListGruas;
        }
        public IEnumerable<Gruas2Model> GetGruasByIdConcesionario(int idConcesionario)
        {
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                idGrua
                                ,idConcesionario
                                ,idClasificacion
                                ,idTipoGrua
                                ,idSituacion
                                ,noEconomico
                                ,placas
                                ,modelo
                                ,capacidad
                                ,fechaActualizacion
                                ,actualizadoPor
                                ,estatus
                                FROM gruas
                                WHERE idConcesionario = @idConcesionario AND estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idGrua", SqlDbType.Int)).Value = (object)idConcesionario ?? DBNull.Value;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            ListGruas.Add(gruasModel);
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
            return ListGruas;
        }

        public IEnumerable<Gruas2Model> GetGruasToGrid(string placas, string noEconomico, int? idTipoGrua,int idOficina)
        {
            List<Gruas2Model> ListGruas = new List<Gruas2Model>();
            string strQuery = @"SELECT
                                 g.idGrua
                                ,g.idConcesionario
                                ,g.idClasificacion
                                ,g.idTipoGrua
                                ,g.idSituacion
                                ,g.noEconomico
                                ,g.placas
                                ,g.modelo
                                ,g.capacidad
                                ,g.fechaActualizacion
                                ,g.actualizadoPor
                                ,g.estatus
								,cm.municipio
								,c.concesionario
                                ,c.idDelegacion
								,ccg.clasificacion
								,ctg.TipoGrua
								,csg.situacion
                                FROM gruas g
								INNER JOIN catClasificacionGruas ccg
								on g.idClasificacion = ccg.idClasificacionGrua
								INNER JOIN catTipoGrua ctg
								on g.idTipoGrua = ctg.IdTipoGrua
								INNER JOIN catSituacionGruas csg
								on g.idSituacion = csg.idSituacion
								INNER JOIN concesionarios c
								on g.idConcesionario = c.idConcesionario AND c.estatus = 1
								INNER JOIN catMunicipios cm
								on c.idMunicipio = cm.idMunicipio AND c.estatus = 1
                                WHERE g.estatus = 1
                                AND g.placas = {0}
                                AND c.idDelegacion = @idOficina
                                AND g.noEconomico = {1}
                                AND g.idTipoGrua = {2}";

            string strWherePlacas = !string.IsNullOrEmpty(placas) ? string.Format("'{0}'", placas) : "g.placas";
            string strWhereNoEconomico = !string.IsNullOrEmpty(noEconomico) ? string.Format("'{0}'", noEconomico) : "g.noEconomico";
            string strWhereidTipoGrua = idTipoGrua != null ? idTipoGrua.ToString() : "g.idTipoGrua";
            strQuery = string.Format(strQuery, strWherePlacas, strWhereNoEconomico, strWhereidTipoGrua);

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Gruas2Model gruasModel = new Gruas2Model();
                            gruasModel.idGrua = Convert.ToInt32(reader["idGrua"].ToString());
                            gruasModel.idConcesionario = Convert.ToInt32(reader["idConcesionario"].ToString());
                            gruasModel.idClasificacion = Convert.ToInt32(reader["idClasificacion"].ToString());
                            gruasModel.idTipoGrua = Convert.ToInt32(reader["idTipoGrua"].ToString());
                            gruasModel.idSituacion = Convert.ToInt32(reader["idSituacion"].ToString());
                            gruasModel.noEconomico = reader["noEconomico"].ToString();
                            gruasModel.placas = reader["placas"].ToString();
                            gruasModel.modelo = reader["modelo"].ToString();
                            gruasModel.capacidad = reader["capacidad"].ToString();
                            gruasModel.concesionario = reader["concesionario"].ToString();
                            gruasModel.municipio = reader["municipio"].ToString();
                            gruasModel.clasificacion = reader["clasificacion"].ToString();
                            gruasModel.tipoGrua = reader["tipoGrua"].ToString();
                            gruasModel.situacion = reader["situacion"].ToString();
                            ListGruas.Add(gruasModel);
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
            return ListGruas;
        }
    }
}

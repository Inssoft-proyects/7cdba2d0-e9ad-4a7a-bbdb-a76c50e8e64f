using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatClasificacionAccidentesService : ICatClasificacionAccidentes
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatClasificacionAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatClasificacionAccidentesModel> ObtenerClasificacionesActivas()
        {
            //
            List<CatClasificacionAccidentesModel> ListaClasificaciones = new List<CatClasificacionAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT c.*, e.estatus FROM catClasificacionAccidentes AS c INNER JOIN estatus AS e ON c.estatus = e.estatus WHERE c.estatus = 1;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatClasificacionAccidentesModel clasificacion = new CatClasificacionAccidentesModel();
                            clasificacion.IdClasificacionAccidente = Convert.ToInt32(reader["IdClasificacionAccidente"].ToString());
                            clasificacion.NombreClasificacion = reader["NombreClasificacion"].ToString();
                            clasificacion.estatusDesc = reader["estatus"].ToString();
                            clasificacion.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            clasificacion.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            ListaClasificaciones.Add(clasificacion);

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
            return ListaClasificaciones;


        }


    }

}

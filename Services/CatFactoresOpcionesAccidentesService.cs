using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatFactoresOpcionesAccidentesService : ICatFactoresOpcionesAccidentesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatFactoresOpcionesAccidentesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<CatFactoresOpcionesAccidentesModel> ObtenerOpcionesPorFactor(int factorDDValue)
        {
            //
            List<CatFactoresOpcionesAccidentesModel> ListaOpciones = new List<CatFactoresOpcionesAccidentesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT op.*, f.*, e.estatus FROM catFactoresOpcionesAccidentes AS op INNER JOIN catFactoresAccidentes AS f ON op.IdFactorAccidente = f.IdFactorAccidente INNER JOIN estatus AS e ON op.estatus = e.estatus WHERE op.IdFactorAccidente = @IdFactor and op.estatus = 1;", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@IdFactor", SqlDbType.Int)).Value = (object)factorDDValue ?? DBNull.Value;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatFactoresOpcionesAccidentesModel opcion = new CatFactoresOpcionesAccidentesModel();
                            opcion.IdFactorOpcionAccidente = Convert.ToInt32(reader["IdFactorOpcionAccidente"].ToString());
                            opcion.IdFactorAccidente = Convert.ToInt32(reader["IdFactorAccidente"].ToString());
                            opcion.FactorOpcionAccidente = reader["FactorOpcionAccidente"].ToString();
                            opcion.estatusDesc = reader["estatus"].ToString();
                            opcion.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            opcion.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                           // opcion.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaOpciones.Add(opcion);

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
            return ListaOpciones;


        }


    }

}



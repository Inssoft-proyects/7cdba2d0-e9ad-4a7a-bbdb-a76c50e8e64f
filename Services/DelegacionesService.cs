using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class DelegacionesService : IDelegacionesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public DelegacionesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<Delegaciones> GetDelegaciones()
        {
            List<Delegaciones> delegaciones = new List<Delegaciones>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catDelegaciones where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            Delegaciones delegacion = new Delegaciones();
                            delegacion.IdDelegacion = Convert.ToInt32(reader["idDelegacion"].ToString());
                            delegacion.Delegacion = reader["delegacion"].ToString();
                            delegaciones.Add(delegacion);
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
            return delegaciones;
        }
    }
}

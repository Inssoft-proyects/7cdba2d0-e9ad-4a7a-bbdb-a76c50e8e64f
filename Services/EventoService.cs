using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class EventoService : IEventoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public EventoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<EventoModel> GetEventos()
        {
            List<EventoModel> ListEvento = new List<EventoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catEvento where Estatus=1 order by Evento asc", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            EventoModel eventoModel = new EventoModel();
                            eventoModel.IdEvento = Convert.ToInt32(reader["IdEvento"].ToString());
                            eventoModel.Evento = reader["Evento"].ToString();
                            ListEvento.Add(eventoModel);

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
            return ListEvento ;

        }
    }
}

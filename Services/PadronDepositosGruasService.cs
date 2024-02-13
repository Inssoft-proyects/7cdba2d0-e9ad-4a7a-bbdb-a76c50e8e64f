using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static GuanajuatoAdminUsuarios.Models.PadronDepositosGruasModel;

namespace GuanajuatoAdminUsuarios.Services
{
    public class PadronDepositosGruasService : IPadronDepositosGruasService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public PadronDepositosGruasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        private List<PadronDepositosGruasModel> GetJoinsPadronDepositosGruas(int idOficina)
        {
            List<PadronDepositosGruasModel> PadronDepositosGruasList = new List<PadronDepositosGruasModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();

                    #region first query
                    //const string SqlTransact1 =
                    //  @"select tg.TipoGrua, tg.IdTipoGrua, g.IdGrua,g.noEconomico,g.Placas,g.Modelo,
                    //        c.IdConcesionario,c.Concesionario,g.noEconomico,g.placas,g.modelo,g.capacidad,
                    //        g.clasificacion,c.Concesionario,dep.IdDeposito,dep.IdPension,dep.IdConcesionario,p.Pension,
                    //        p.Direccion, p.Telefono,  p.IdMunicipio, m.Municipio
                    //        from municipios m 
                    //        inner join pensiones p on m.idMunicipio = p.idMunicipio
                    //        inner join depositos dep on p.idPension = dep.idPension
                    //        inner join Concesionarios c	on dep.IdConcesionario= c.IdConcesionario
                    //        left join  GruasConcesionario gc on c.IdConcesionario= gc.IdConcesionario
                    //        left join Gruas g on gc.IdGrua= g.IdGrua
                    //        left join catTipoGrua tg ON g.IdTipoGrua = tg.IdTipoGrua";
                    #endregion

                    const string SqlTransact =
                        @"select tg.TipoGrua, tg.IdTipoGrua, g.IdGrua,g.noEconomico,g.Placas,g.Modelo,
                            c.IdConcesionario,c.Concesionario,g.noEconomico,g.placas,g.modelo,g.capacidad,
                            c.Concesionario,dep.IdDeposito,dep.IdPension,dep.IdConcesionario,p.Pension,
                            p.Direccion, p.Telefono,  p.IdMunicipio, m.Municipio
                            from gruas g 
                            inner join concesionarios c on g.IdConcesionario= c.IdConcesionario
                            inner join catTipoGrua tg on g.IdTipoGrua= tg.IdTipoGrua
                            inner join depositos dep on c.IdConcesionario =dep.IdConcesionario
                            inner join pensiones p on dep.idPension= p.idPension
                            inner join catMunicipios m on p.idMunicipio = m.idMunicipio 
                            WHERE c.idDelegacion = @idOficina
                            order by  g.IdGrua,c.IdConcesionario";

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = (object)idOficina ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PadronDepositosGruasModel padronDepositosGruas = new PadronDepositosGruasModel();

                            padronDepositosGruas.IdTipoGrua = Convert.ToInt32(reader["IdTipoGrua"].ToString());
                            padronDepositosGruas.TipoGrua = reader["TipoGrua"].ToString();
                            padronDepositosGruas.IdGrua = Convert.ToInt32(reader["IdGrua"].ToString());
                            padronDepositosGruas.noEconomico = reader["noEconomico"].ToString();
                            padronDepositosGruas.Placas = reader["placas"].ToString();
                            padronDepositosGruas.Modelo = reader["modelo"].ToString();
                            padronDepositosGruas.Concesionario = reader["Concesionario"].ToString();
                            padronDepositosGruas.IdDeposito = Convert.ToInt32(reader["IdDeposito"].ToString());
                            padronDepositosGruas.IdConcesionario = Convert.ToInt32(reader["IdConcesionario"].ToString());
                            padronDepositosGruas.IdPension = Convert.ToInt32(reader["IdPension"].ToString());
                            padronDepositosGruas.Pension = reader["Pension"].ToString();
                            padronDepositosGruas.Direccion = reader["Direccion"].ToString();
                            padronDepositosGruas.Telefono = reader["Telefono"].ToString();
                            padronDepositosGruas.IdMunicipio = Convert.ToInt32(reader["IdMunicipio"].ToString());
                            padronDepositosGruas.Municipio = reader["Municipio"].ToString();
                            PadronDepositosGruasList.Add(padronDepositosGruas);
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
            return PadronDepositosGruasList;
        }

        public List<PadronDepositosGruasModel> GetAllPadronDepositosGruas(int idOficina)
        {

            var modelList = GetJoinsPadronDepositosGruas(idOficina);
            var indices = modelList
                         .Select((s, i) => new { index = i, item = s })
                         .GroupBy(grp => grp.item.IdPension)
                         //.Where(w => !string.IsNullOrEmpty(w.))
                         .SelectMany(sm => sm.Select(s => s.index))
                         .ToList();

            var ListaAgrupada = modelList.Select((s, i) => new { index = i, items = s })
                             .GroupBy(x => indices.FirstOrDefault(r => r > x.index))
                             .Select(s => s.Select(ss => ss.items).ToList())
                             .ToList();

            List<PadronDepositosGruasModel> ListItems = new List<PadronDepositosGruasModel>();
            List<PensionPadronModel> padronPension = new List<PensionPadronModel>();
            foreach (var item in ListaAgrupada)
            {

                if (item.Count() > 1)
                {

                    foreach (var itemInside in item)
                    {
                        PensionPadronModel pension = new PensionPadronModel();
                        pension.IdPension = itemInside.IdPension;
                        pension.Pension = itemInside.Pension;
                        pension.Telefono = itemInside.Telefono;
                        pension.Direccion = itemInside.Direccion;
                        pension.IdMunicipio = itemInside.IdMunicipio;
                        padronPension.Add(pension);
                    }

                    foreach (var itemInside in item)
                    {
                        itemInside.Pensiones = padronPension;
                        ListItems.Add(itemInside);
                    }

                    //var one = item.FirstOrDefault();
                    //one.Pensiones = padronPension;
                    //ListItems.Add(one);
                }
                else
                {
                    ListItems.Add(item.First());
                }
            }
            return ListItems;
        }

        public List<PensionModel> GetPensiones(int idOficina)
        {
            List<PensionModel> ListPensiones = new List<PensionModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from pensiones where estatus=1 AND pensiones.idDelegacion = @idOficina ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PensionModel pension = new PensionModel();
                            pension.IdPension = Convert.ToInt32(reader["IdPension"].ToString());
                            pension.Pension = reader["Pension"].ToString();
                            ListPensiones.Add(pension);
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
            return ListPensiones;
        }

        private List<PadronDepositosGruasModel> GetJoinsPadronDepositosGruas(PadronDepositosGruasBusquedaModel model, int idOficina)
        {
            List<PadronDepositosGruasModel> PadronDepositosGruasList = new List<PadronDepositosGruasModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    string condiciones = "";
                    condiciones += model.IdMunicipio.Equals(null) || model.IdMunicipio == 0 ? "" : " AND c.IdMunicipio = @IdMunicipio ";
                    condiciones += model.IdConcesionario.Equals(null) || model.IdConcesionario == 0 ? "" : " AND c.idConcesionario = @IdConcesionario ";
                    condiciones += model.IdPension.Equals(null) || model.IdPension == 0 ? "" : " AND p.idPension = @IdPension ";
                    condiciones += model.IdTipoGrua.Equals(null) || model.IdTipoGrua == 0 ? "" : " AND g.idTipoGrua = @IdTipoGrua ";
                    if (string.IsNullOrEmpty(condiciones.Trim()))
                    {
                        condiciones = "";
                    }
                    string SqlTransact =
                        @"select m.municipio,c.concesionario,c.idMunicipio,p.idPension,p.pension,p.direccion,p.telefono,g.idGrua,
                                    g.idTipoGrua,g.noEconomico,g.modelo,g.placas,ctg.TipoGrua
                                    FROM concesionarios c
                                    LEFT JOIN gruas g ON g.idConcesionario = c.idConcesionario
                                    LEFT JOIN pensionGruas pg ON pg.idGrua = g.idGrua
                                    LEFT JOIN pensiones p ON p.idPension = pg.idPension
                                    LEFT JOIN catMunicipios m ON m.idMunicipio = c.idMunicipio
                                    LEFT JOIN catTipoGrua ctg ON ctg.IdTipoGrua = g.idTipoGrua
                                    WHERE c.estatus = 1 " + condiciones;

                    SqlCommand command = new SqlCommand(SqlTransact, connection);
                    command.Parameters.Add(new SqlParameter("@IdMunicipio", SqlDbType.Int)).Value = (object)model.IdMunicipio ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdConcesionario", SqlDbType.Int)).Value = (object)model.IdConcesionario ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdPension", SqlDbType.Int)).Value = (object)model.IdPension ?? DBNull.Value;
                    command.Parameters.Add(new SqlParameter("@IdTipoGrua", SqlDbType.Int)).Value = (object)model.IdTipoGrua ?? DBNull.Value;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PadronDepositosGruasModel padronDepositosGruas = new PadronDepositosGruasModel();

                            padronDepositosGruas.IdTipoGrua = reader["IdTipoGrua"] != DBNull.Value ? Convert.ToInt32(reader["IdTipoGrua"]) : 0;
                            padronDepositosGruas.IdGrua = reader["IdGrua"] != DBNull.Value ? Convert.ToInt32(reader["IdGrua"]) : 0;
                            padronDepositosGruas.IdMunicipio = reader["IdMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["IdMunicipio"]) : 0;

                            padronDepositosGruas.TipoGrua = reader["TipoGrua"].ToString();
                            padronDepositosGruas.noEconomico = reader["noEconomico"].ToString();
                            padronDepositosGruas.Placas = reader["placas"].ToString();
                            padronDepositosGruas.Modelo = reader["modelo"].ToString();
                            padronDepositosGruas.Concesionario = reader["Concesionario"].ToString();
                            //padronDepositosGruas.IdDeposito = Convert.ToInt32(reader["IdDeposito"].ToString());
                            // padronDepositosGruas.IdConcesionario = Convert.ToInt32(reader["IdConcesionario"].ToString());
                            padronDepositosGruas.IdPension = reader["IdPension"] != DBNull.Value ? Convert.ToInt32(reader["IdPension"]) : 0;
                            padronDepositosGruas.Pension = reader["pension"].ToString();
                            padronDepositosGruas.Direccion = reader["direccion"].ToString();
                            padronDepositosGruas.Telefono = reader["telefono"].ToString();
                            padronDepositosGruas.Municipio = reader["Municipio"].ToString();
                            PadronDepositosGruasList.Add(padronDepositosGruas);
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
            return PadronDepositosGruasList;
        }


        public List<PadronDepositosGruasModel> GetPadronDepositosGruas(PadronDepositosGruasBusquedaModel model, int idOficina)
        {
            var modelList = GetJoinsPadronDepositosGruas(model, idOficina);

            List<PadronDepositosGruasModel> ListItems = new List<PadronDepositosGruasModel>();

            foreach (var item in modelList)
            {
                List<PensionPadronModel> padronPension = new List<PensionPadronModel>();

                PensionPadronModel pension = new PensionPadronModel();
                pension.IdPension = item.IdPension;
                pension.Pension = item.Pension;
                pension.Telefono = item.Telefono;
                pension.Direccion = item.Direccion;
                pension.IdMunicipio = item.IdMunicipio;
                padronPension.Add(pension);

                item.Pensiones = padronPension;
                ListItems.Add(item);
            }

            return ListItems;
        }


            public List<PensionModel> GetPensionesNoFilter()
        {
            List<PensionModel> ListPensiones = new List<PensionModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from pensiones where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                   // command.Parameters.Add(new SqlParameter("@idOficina", SqlDbType.Int)).Value = idOficina;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            PensionModel pension = new PensionModel();
                            pension.IdPension = Convert.ToInt32(reader["IdPension"].ToString());
                            pension.Pension = reader["Pension"].ToString();
                            ListPensiones.Add(pension);
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
            return ListPensiones;
        }

    }
}

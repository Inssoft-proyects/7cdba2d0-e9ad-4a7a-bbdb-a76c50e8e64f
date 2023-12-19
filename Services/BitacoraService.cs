using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.RESTModels;
using Newtonsoft.Json;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using PdfSharp.Pdf.Content.Objects;

namespace GuanajuatoAdminUsuarios.Services
{
    public class BitacoraService : IBitacoraService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public BitacoraService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public void insertBitacora(decimal id, string ip, string textoCamb, string operacion, string consulta, decimal operador)
        {

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"insert into bitacoradeinfracciones 
                        (BDIIDINFRACCION,
                        BDIFECHA,
                        BDIHORA,
                        BDIIP,
                        BDITEXTOCAMBIO,
                        BDIOPERACION,
                        BDICONSULTA,
                        BDIIDUSUARIO) values
                        ( @id ,FORMAT(getdate(),'yyyy-MM-dd'),FORMAT(getdate(),'HH:mm'),@ip,@textoCamb,@operacion,@consulta,@capturista)"
                    , connection);


                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Decimal)).Value = id;
                    command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                    command.Parameters.Add(new SqlParameter("@textoCamb", SqlDbType.VarChar)).Value = textoCamb;
                    command.Parameters.Add(new SqlParameter("@operacion", SqlDbType.VarChar)).Value = operacion;
                    command.Parameters.Add(new SqlParameter("@consulta", SqlDbType.VarChar)).Value = consulta;
                    command.Parameters.Add(new SqlParameter("@capturista", SqlDbType.Decimal)).Value = operador;
                    command.CommandType = CommandType.Text;
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }




        }



        public List<BitacoraInfraccionesModel> getBitacoraData(string id, string nombre)
        {

            var result = new List<BitacoraInfraccionesModel>();


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select  BDIOPERACION,BDIFECHA,BDIHORA,BDIIP,b.folioInfraccion  from
                                            bitacoradeinfracciones a
											join infracciones b on b.idInfraccion=BDIIDINFRACCION
											where BDIIDINFRACCION=@id
"
					, connection);


                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Decimal)).Value = id;
                    command.CommandType = CommandType.Text;
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                    while (reader.Read())
                    {

                        result.Add(new BitacoraInfraccionesModel
                        {
                            operacion = reader["BDIOPERACION"].ToString(),
                            fecha = reader["BDIFECHA"].ToString(),
                            hora = reader["BDIHORA"].ToString(),
                            ip = reader["BDIIP"].ToString(),
                            nombre= nombre,
                            folio= reader["folioInfraccion"].ToString()
						}) ;

                    }




                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }




            return result;

        }

    }
}

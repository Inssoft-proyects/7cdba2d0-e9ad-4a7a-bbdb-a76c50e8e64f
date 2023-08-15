﻿using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.RESTModels;
using Newtonsoft.Json;
using System.Data;
using System;
using System.Data.SqlClient;
using static GuanajuatoAdminUsuarios.RESTModels.AnulacionDocumentoRequestModel;

namespace GuanajuatoAdminUsuarios.Services
{
    public class AnulacionDocumentoClientService : IAnularDocumentoClientService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public AnulacionDocumentoClientService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public AnulacionDocumentoResponseModel AnularDocumento(AnulacionDocumentoRequestModel requestModel)
        {
            string endPointName = "AnulacionDocumento";
            var json = JsonConvert.SerializeObject(requestModel, Formatting.Indented);
            var bodyRequest = json;
            string result = string.Empty;
            AnulacionDocumentoResponseModel responseModel = new AnulacionDocumentoResponseModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SP_WebClientNoAuthentication", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@EndpointName", SqlDbType.NVarChar)).Value = endPointName;
                    command.Parameters.Add(new SqlParameter("@Body", SqlDbType.NVarChar)).Value = bodyRequest;
                    command.CommandType = CommandType.StoredProcedure;
                    result = Convert.ToString(command.ExecuteScalar());
                    responseModel = JsonConvert.DeserializeObject<AnulacionDocumentoResponseModel>(result);
                }
                catch (SqlException ex)
                {
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }

            return responseModel;
        }
    }
}

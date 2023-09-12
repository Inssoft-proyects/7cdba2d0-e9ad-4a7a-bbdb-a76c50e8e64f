using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.RESTModels;
using Newtonsoft.Json;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Models;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CrearMultasTransitoClientService : ICrearMultasTransitoClientService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CrearMultasTransitoClientService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public CrearMultasTransitoResponseModel CrearMultasTransitoCall(CrearMultasTransitoRequestModel requestModel)
        {
            string endPointName = "CrearMultasTransito";
            var json = JsonConvert.SerializeObject(requestModel, Formatting.Indented);
            var bodyRequest = json;
            string result = string.Empty;
            CrearMultasTransitoResponseModel responseModel = new CrearMultasTransitoResponseModel();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection2()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SP_WebClientConnects", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@EndpointName", SqlDbType.NVarChar)).Value = endPointName;
                    command.Parameters.Add(new SqlParameter("@Body", SqlDbType.NVarChar)).Value = bodyRequest;
                    command.CommandType = CommandType.StoredProcedure;
                    result = Convert.ToString(command.ExecuteScalar());
                    responseModel = JsonConvert.DeserializeObject<CrearMultasTransitoResponseModel>(result);
                }
                catch (SqlException ex)
                {
                    responseModel.MensajeError = "Hubo un problema al intentar crear la multa de tránsito. Por favor, inténtalo nuevamente más tarde.";
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



using iTextSharp.text;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IBitacoraService
    {

        void insertBitacora(decimal id, string ip, string textoCamb, string operacion, string consulta, decimal operador);

        List<BitacoraInfraccionesModel> getBitacoraData(string id,string nombre);


    }
}

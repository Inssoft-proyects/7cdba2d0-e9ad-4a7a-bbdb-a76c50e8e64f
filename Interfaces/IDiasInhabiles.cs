using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IDiasInhabiles
    {
        List<DiasInhabilesModel> GetDiasInhabiles();

        DiasInhabilesModel GetDiasById(int IdDia);

        int CrearDiaInhabil(DiasInhabilesModel Dia);

        int EditDia(DiasInhabilesModel Dia);

        //int DeleteOficial(int oficial);
    }
}

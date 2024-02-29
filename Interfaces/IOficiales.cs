

using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IOficiales

    {
        List<CatOficialesModel> GetOficiales();

        List<CatOficialesModel> GetCatalogoOficialesDependencia(int idDependencia);

        List<CatOficialesModel> GetOficialesActivos();
        List<CatOficialesModel> GetOficialesFiltrados(int idOficina, int idDependencia);
        List<CatOficialesModel> GetOficialesPorDependencia(int idDependencia);
        List<CatOficialesModel> GetOficialesPorDependencia();

        CatOficialesModel GetOficialById(int IdOficial);

        int SaveOficial(CatOficialesModel oficial, int idDependencia);

        int UpdateOficial(CatOficialesModel oficial);

        int DeleteOficial(int oficial);

    }
}
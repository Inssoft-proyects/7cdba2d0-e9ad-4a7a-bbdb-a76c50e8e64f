

using GuanajuatoAdminUsuarios.Entity;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IOficiales

    {
        List<Oficiales> GetOficiales();

        Oficiales GetOficialById(int IdOficial);

        int SaveOficial(Oficiales oficial);

        int UpdateOficial(Oficiales oficial);

        int DeleteOficial(int oficial);

    }
}
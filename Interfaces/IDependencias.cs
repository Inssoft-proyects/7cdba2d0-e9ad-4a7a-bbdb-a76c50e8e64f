using GuanajuatoAdminUsuarios.Entity;
using System.Collections.Generic;


namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IDependencias

    {
        List<Dependencias> GetDependencias();

        Dependencias GetDependenciaById(int IdDependencia);

        int SaveDependencia(Dependencias dependencia);

        int UpdateDependencia(Dependencias dependencia);

        int DeleteDependencia(int dependencia);




    }
}
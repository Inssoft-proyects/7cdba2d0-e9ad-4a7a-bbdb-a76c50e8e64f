using GuanajuatoAdminUsuarios.Entity;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IColores
    {
        List<CatColores> Getcoloress();

        CatColores GetColorById(int IdColor);

        int SaveColor(CatColores color);

        int UpdateColor(CatColores color);

        int DeleteColor(int color);

    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class EditarCausaAccidenteModel
    {
        public int IdAccidente { get; set; }
        public int IdCausaAccidente { get; set; }
        public int IdCausaAccidenteEdit { get; set; }
        public SelectList CausasAccidentes { get; set; }
    }
}

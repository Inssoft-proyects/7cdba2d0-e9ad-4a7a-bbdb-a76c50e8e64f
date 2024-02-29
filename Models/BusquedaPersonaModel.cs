using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class BusquedaPersonaModel
    {
        public bool IsModal { get; set; }

        public string NombreBusqueda { get; set; }
        public string ApellidoPaternoBusqueda { get; set; }
        public string ApellidoMaternoBusqueda { get; set; }
        public string RFCBusqueda { get; set; }
        public string CURPBusqueda { get; set; }

        public string NumeroLicenciaBusqueda { get; set; }
        public PersonaModel PersonaModel { get; set; }


        public List<PersonaModel> ListadoPersonas { get; set; }

        public List<PersonaLicenciaModel> ListadoPersonasLicencia { get; set; }

        public override string ToString()
        {
            return $"[Nombre:{NombreBusqueda},Paterno:{ApellidoPaternoBusqueda},Materno:{ApellidoMaternoBusqueda},RFC:{RFCBusqueda},CURP:{CURPBusqueda},No.Licencia:{NumeroLicenciaBusqueda}]";
        }
    }

}

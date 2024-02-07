using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class BitacoraInfraccionesModel
    {

        public string folio { get; set; }
        public string documento { get; set; } = "-";
        public string monto { get; set; }="-";
         public string operacion { get; set; }
        public string operaciondesc { get
            {

                var indextest = new Dictionary<string, string>();

                indextest["CREAR"] = $"Captura de infracción (Inicial)";
                indextest["CREAR1"] = $"Captura de infracción (Inicial)";
                indextest["Crear2"] = $"Captura de infracción (Inicial)";
                indextest["Editar"] = $"Modificación de infracción (Edición)";
                indextest["Registrar"] = $"Registro de infracción a Finanzas";
                indextest["Registrarcd"] = $"Registro de infracción a Finanzas";
                indextest["Registrarer"] = $"Registro de infracción a Finanzas";
                indextest["ConsultaP"] = $"Modificación de infracción (Edición)";
                indextest["Cancelar"] = $"Modificación de infracción (Edición)";
                indextest["Pagar"] = $"Modificación de infracción (Edición)";
                indextest["Editar2"] = $"Modificación de infracción (Edición)";

                var t = indextest[this.operacion];

                return t;


            } }
         public string fecha { get; set; }
        public string hora { get; set; }
        public string ip { get; set; }
        public string nombre { get; set; }
        public string desc { get { 
            
                var indextest = new Dictionary<string,string>();

                indextest["CREAR"] = $"Se capturó la infracción con folio: {folio}. La primera parte de la captura " ;
                indextest["CREAR1"] = $"Se capturó la infracción con folio: {folio}. La primera parte de la captura " ;
                indextest["Crear2"] = $"Se capturó la infracción con folio: {folio}. La segunda parte de la captura";
                indextest["Editar"]= $"Se modificó la infracción con folio: {folio}. La primera parte de la captura";
                indextest["Editar2"] = $"Se modificó la infracción con folio: {folio}. La segunda parte de la captura";
                indextest["Registrar"] = $"Se llamó proceso de cancelación en finanzas para la infracción folio: {folio} ; a través de WS; Sin confirmación";
                indextest["Registrarcd"] = $"Se registró en finanzas la infracción folio: {folio} se obtuvo No. de documento = {documento}; a través de WS";
                indextest["Registrarer"] = $"No se pudo capturar en finanzas la infracción folio: {folio} Se guardó como pendiente de registro ; a través de WS;";
                indextest["ConsultaP"] = $"Se registró pago de la infracción con folio: {folio}. En registro de recibo de pago";
                indextest["Cancelar"] = $"Se revocó(canceló) la infracción con folio: {folio}. En cancelación de infracción";
                indextest["Pagar"] = $"Se revocó(canceló) la infracción con folio: {folio}. En cancelación de infracción";

                var t = indextest[this.operacion];

                return t;
            
            
            } }

    }
}

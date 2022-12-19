using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTransaccion
{
    public class Request_CU0780
    {
        public string Header { get; set; }
        public string CodigoTransaccion { get; set; }

        public int TipoClaveTributaria { get; set; }
        public int NroClaveTributaria { get; set; }

        public string TipoDocumento { get; set; }

        public int NroDocumento { get; set; }

        public string VersionDocumento { get; set; }
        public string Apellido { get; set; }

        public string Nombre { get; set; }

        public string Sexo { get; set; }

        public string EstadoCivil { get; set; }
        public string Nacionalidad { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string IndicEmancipadoAutorizado { get; set; }
        public int CasaBera { get; set; }
        public string IndicadorEmpleado { get; set; }
        public string PosicionIva { get; set; }
        public string BonificIva { get; set; }

        public string BonificPercepcionnIva { get; set; }
        public DateTime FechaDesdeBonifPercepIVA { get; set; }
        public DateTime FechaHastaBonifPercepIVA { get; set; }
        public int CodigoDeActividad { get; set; }
        public int CodigoDeProfesion {get; set}

        public int OficialDeCuenta { get; set; }
        public string CarpetaCredito_Ramo { get; set; }
        public int CarpetaCredito_Numero { get; set; }
        public string Empleador { get; set; }
        public string IndicadorDeAutonomo { get; set; }
        public DateTime FechaDesdeUltimeActividad { get; set; }
        public DateTime FechaAltaCliente { get; set; }
        public DateTime FechaBajaCliente { get; set; }

        public DateTime FechaDeAcceso { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }

        public string Piso { get; set; }
        public string Departamento { get; set; }

        public string CodigoProvincia { get; set; }
        
        public int CodigoPostal { get; set; }
        public string CodigoManzana { get; set; }
        public string Telefono { get; set; }
        public string Manzana { get; set; }

    }
}

using FixedWidthTextUtils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTransaccion
{
    [StringeableClass(300, ' ')]
    public class Request_CU0506_T107
    {
        [StringField(55)]
        public string Header { get; set; }

        [StringField(4)]
        public string CodigoTransaccion { get; set; }

        [IntegerField(1, true)]
        public int TipoClaveTributaria { get; set; }
        
        [IntegerField(11, true)]
        public long NroClaveTributaria { get; set; }

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
        public int CasaBCRA { get; set; }
        public string IndicadorDeEmpleado { get; set; }
        public string PosicionIva { get; set; }
        public string BonificacionIva { get; set; }

        public string BonificPercepcionIva { get; set; }
        public DateTime FechaDesdeBonifPercepIVA { get; set; }
        public DateTime FechaHastaBonifPercepIVA { get; set; }
        public int CodigoDeActividad { get; set; }
        public int CodigoDeProfesion { get; set; }

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
        public string Fax { get; set; }
        
        public string Localidad { get; set; }
        public string Pais { get; set; }

        public string Email { get; set; }

        public string ProvNoFinancCreditosCOMA5603 { get; set; }

        public string InscrEnRegistroBCRACOMA5603 { get; set; }
        public string NroIdent { get; set; }

        public string PaisDeConstitucion { get; set; }
        
        public string CasillaPostalExterior { get; set; }

        public string CodPostalEnExterior { get; set; }

    }
}

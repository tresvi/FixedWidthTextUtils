using FixedWidthTextUtils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTransaccion
{

    [StringeableClass(600, ' ')]        //Conte 598
    internal class Response_TRX_500ConsultaCliente
    {
        [StringField(9, 9)]
        public string TipoPersona { get; set; }
        public int TipoCuit { get; set; }
        public string NroCuit { get; set; }
        public string TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string PaisOrigen { get; set; }
        public string Version { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string Sexo { get; set; }
        public string EstadoCivil { get; set; }
        public string Nacionalidad { get; set; }
        public string FNAC_CONST { get; set; }    //Substring(189, 8), "yyyyMMdd"
        public string MenEmanc { get; set; }
        public int Casa { get; set; }
        public string Emple { get; set; }  //Empleado? Empleador?? 
        public int LegEmpl { get; set; }
        public string PosIva { get; set; }
        public string BonifIva { get; set; }
        public string BonifPerc { get; set; }
        public int CodActividad { get; set; }
        public int Profes { get; set; }
        public int OfiCred { get; set; }
        public string NombreOfi { get; set; }
        public string Carpeta { get; set; }
        public string LugarTrabajoContacto { get; set; }
        public string Autonomo { get; set; }
        public string FulTact { get; set; }
        //public string FALTA { get; set; }
        public DateTime FechaBaja { get; set; }
        public DateTime FechaVtoEstDec { get; set; }
        public int Sector { get; set; }
        public string TipoCar { get; set; }
        public string SitBCRA { get; set; }
        public string FSitBCRA { get; set; }
        public string SitBNA { get; set; }
        public DateTime FSitBNA { get; set; }
        public string InhabBCRA { get; set; }
        public DateTime FechaAltaInhabBCRA { get; set; }
        public DateTime FechaBajaInahbBCRA { get; set; }
        public string Interdic { get; set; }
        public int Vinculado { get; set; }
        public int ConjEcon { get; set; }
        public string DeudorLiq { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Piso { get; set; }
        public string Depto { get; set; }
        public string CodProvincia { get; set; }
        public int CodPostal { get; set; }
        public string CodPostalManz { get; set; }
        public string Localidad { get; set; }
        public string CodLocalidad { get; set; }        //En el codigo original, decia CodProvincia, seguro estaba mal
        public string DescripcionProv { get; set; }
        public string PaisResidencia { get; set; }
        public string Telefono { get; set; }
        public string Fax { get; set; }
        public int NroIdent { get; set; }
        public int XXXXX { get; set; }                  ////En el codigo original, decia Sector de nuevo...
        public int CodSicenBNA { get; set; }
        public double PorcentajeBonifPerc { get; set; }
        public string RazonSocial { get; set; }
        public string FormaJuridica { get; set; }
        public int CantPers { get; set; }
        public string FCIEBal { get; set; }
        public DateTime FechaIngresoPais { get; set; }


        public int CodSicenBCRA { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaBonPerDesde { get; set; }
        public DateTime FechaBonPerHasta { get; set; }
        public DateTime FechaNacConst { get; set; }
    }
}

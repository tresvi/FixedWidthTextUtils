using FixedWidthTextUtils.Attributes;

namespace PruebaTransaccion
{
    /// <summary>
    /// Alta Clientes Juridicos
    /// </summary>
    //[StringeableClass(713, ' ')]
    internal class Request_CU0504_T109
    {
        [StringField(55)]
        public string Header { get; set; }

        [StringField(4)]
        public string CodigoTransaccion { get; set; }

        [IntegerField(1, true)]
        public int TipoClaveTributaria { get; set; }

        [IntegerField(11, true)]
        public long NroClaveTributaria { get; set; }

        [StringField(60)]
        public string RazonSocial { get; set; }

        [StringField(3)]
        public string FormaJuridica { get; set; }

        [IntegerField(2, true)]
        public int Sector { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaConstitucion { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaVencimientoEstatuto { get; set; }

        [IntegerField(4, true)]
        public int CasaBCRA { get; set; }

        [DateTimeField(4, "ddMM")]
        public DateTime FechaCierreBalances { get; set; }

        [StringField(3)]
        public string PosicionIva { get; set; }

        [BooleanField(1, "S", "N")]
        public bool BonificacionIva { get; set; }

        [BooleanField(1, "S", "N")]
        public bool BonificacionPercepcionIva { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaDesdeBonifPercepIVA { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaHastaBonifPercepIVA { get; set; }

        [IntegerField(3, true)]
        public int CodigoDeActividadOSegmento { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaInicioActividad { get; set; }

        [StringField(25)]
        public string Contacto { get; set; }

        [IntegerField(6, true)]
        public int CantidadDePersonal { get; set; }

        [IntegerField(5, true)]
        public int OficialDeCuenta { get; set; }

        [StringField(2)]
        public string CarpetaCredito_Ramo { get; set; }

        [IntegerField(7, true)]
        public int CarpetaCredito_Numero { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaAltaCliente { get; set; }

        [StringField(30)]
        public string Calle { get; set; }

        [StringField(6)]
        public string Numero { get; set; }

        [StringField(3)]
        public string Piso { get; set; }

        [StringField(4)]
        public string Departamento { get; set; }

        [StringField(1)]
        public string CodigoProvincia { get; set; }

        [IntegerField(5, true)]
        public int CodigoPostal { get; set; }

        [StringField(3)]
        public string CodigoManzana { get; set; }

        [StringField(15)]
        public string Telefono { get; set; }

        [StringField(15)]
        public string Fax { get; set; }

        [StringField(20)]
        public string Localidad { get; set; }

        [StringField(2)]
        public string Pais { get; set; }

        [StringField(70)]
        public string Email { get; set; }

        [BooleanField(1, "S", "N")]
        public bool ProvNoFinancCreditosCOMA5603 { get; set; }

        [BooleanField(1, "S", "N")]
        public bool InscrEnRegistroBCRACOMA5603 { get; set; }

        [StringField(2)]
        public string PaisDeConstitucion { get; set; }

        [StringField(30)]
        public string CasillaPostalExterior { get; set; }

        [StringField(10)]
        public string CodPostalEnExterior { get; set; }

        [StringField(4)]
        public string TipoInscripcion { get; set; }

        [IntegerField(15, true)]
        public long NroInscripcion { get; set; }

        [StringField(2)]
        public string PaisResidFiscalExterior { get; set; }

        [StringField(7)]
        public string ClasificacionFATCA { get; set; }

        [StringField(7)]
        public string ClasificacionOCDE { get; set; }

        [IntegerField(1, true)]
        public int OperadorDeCambio { get; set; }

        [IntegerField(15, true)]
        public long ValorDeclarIngresosAnuales { get; set; }

        [StringField(25)]
        public string TIN { get; set; }

        [IntegerField(4, true)]
        public int MotivoNoInfTIN { get; set; }

        [StringField(2)]
        public string PaisEmisorTIN { get; set; }

        [StringField(5)]
        public string TomoInscripcion { get; set; }

        [StringField(5)]
        public string FolioInscripcion { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaInscripcion { get; set; }

        [StringField(4)]
        public string SujetoObligadoOperar { get; set; }

        [BooleanField(1, "S", "N")]
        public bool AdminFondos3ros { get; set; }

        [IntegerField(15, true)]
        public long ValorMensualEstimaOperar { get; set; }

        [StringField(30)]
        public string Barrio { get; set; }

        [StringField(30)]
        public string DFE_Calle { get; set; }

        [StringField(6)]
        public string DFE_Numero { get; set; }

        [StringField(5)]
        public string DFE_Torre { get; set; }

        [StringField(3)]
        public string DFE_Piso { get; set; }

        [StringField(4)]
        public string DFE_Departamento { get; set; }

        [StringField(20)]
        public string DFE_Ciudad { get; set; }

        [StringField(20)]
        public string DFE_Estado { get; set; }

        [StringField(2)]
        public string DFE_Pais { get; set; }

        [StringField(10)]
        public string DFE_CodPostalExterior { get; set; }

    }
}

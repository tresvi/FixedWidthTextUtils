using FixedWidthTextUtils.Attributes;

namespace PruebaTransaccion
{
    /// <summary>
    /// Alta Clientes Fisicos
    /// </summary>
    //[StringeableClass(753, ' ')]
    internal class Request_CU0503_T109
    {
        [StringField(55)]
        public string Header { get; set; }

        [StringField(4)]
        public string CodigoTransaccion { get; set; }

        [IntegerField(1, true)]
        public int TipoClaveTributaria { get; set; }

        [IntegerField(11, true)]
        public long NroClaveTributaria { get; set; }

        [StringField(2)]
        public string TipoDocumento { get; set; }

        [IntegerField(8, true)]
        public long NroDocumento { get; set; }

        [StringField(3)]
        public string VersionDocumento { get; set; }

        [StringField(40)]
        public string Apellido { get; set; }

        [StringField(40)]
        public string Nombre { get; set; }

        [StringField(1)]
        public string Sexo { get; set; }

        [StringField(1)]
        public string EstadoCivil { get; set; }

        [StringField(2)]
        public string Nacionalidad { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaNacimiento { get; set; }

        [StringField(3)]
        public string IndicEmancipadoOAutorizado { get; set; }

        [IntegerField(4, true)]
        public int CasaBCRA { get; set; }

        [BooleanField(1, "S", "N")]
        public bool IndicadorDeEmpleado { get; set; }

        [StringField(3)]
        public string PosicionIva { get; set; }

        [BooleanField(1, "S", "N")]
        public bool BonificacionIva { get; set; }

        [BooleanField(1, "S", "N")]
        public bool BonificPercepcionIva { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaDesdeBonifPercepIVA { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaHastaBonifPercepIVA { get; set; }

        [IntegerField(3, true)]
        public int CodigoDeActividad { get; set; }

        [IntegerField(3, true)]
        public int CodigoDeProfesion { get; set; }

        [IntegerField(5, true)]
        public int OficialDeCuenta { get; set; }

        [StringField(2)]
        public string CarpetaCredito_Ramo { get; set; }

        [IntegerField(7, true)]
        public int CarpetaCredito_Numero { get; set; }

        [StringField(25)]
        public string Empleador { get; set; }

        [BooleanField(1, "S", "N")]
        public bool IndicadorDeAutonomo { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaDesdeUltimeActividad { get; set; }

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

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaVencimientoDoc { get; set; }

        [StringField(2)]
        public string PaisDeNacimiento { get; set; }

        [StringField(2)]
        public string SegundaNacionalidad { get; set; }

        [StringField(30)]
        public string CasillaPostalExterior { get; set; }

        [StringField(10)]
        public string CodigoPostalExterior { get; set; }

        [StringField(60)]
        public string NombreConyuge { get; set; }

        [BooleanField(2, "SI", "NO")]
        public bool MarcaPEP { get; set; }

        [StringField(2)]
        public string PaisResidFiscalExterior { get; set; }

        [IntegerField(15, true)]
        public long ValorDeclarIngresosAnuales { get; set; }

        [StringField(25)]
        public string TIN { get; set; }

        [IntegerField(4, true)]
        public int MotivoNoInfTIN { get; set; }

        [StringField(4)]
        public string MarcaSujetoObligado { get; set; }

        [StringField(2)]
        public string PaisEmisorTIN { get; set; }

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


        /*
        //Argentina: AR  Cod: 80
        CU_Actualizacion_Clientes  -> https://apiib0001.dcc.dbna.net:7844/WebServiceCU_Actualizacion_Clientes?wsdl
            CU_Actualizacion_Clientes_Manager.cs
                AltaClientePersonaFisica(altaClientePersonaFisicaRequest);
                AltaClientePersonaJuridica(altaClientePersonaJuridicaRequest);
                ModificacionClienteFisicoIntegral(modificacionClienteFisicoIntegralRequest);
                ModificacionClienteJuridico(request);

        CU_Alta_Modificacion_Observaciones  -> https://10.7.232.121:7844/BNA_MSG_0510_2SOAP_HTTP_Service?wsdl    (msg 510?)
            AltModifObs(observaciones)


        CU_Clientes    -> https://apiib0001.dcc.dbna.net:7844/WebServiceCU_Clientes?wsdl
            CU_Clientes_Manager.cs
                ConsultaClientesPorDocumento(consultaClientesPorDocumento);
                ConsultaClientesPorCuit(consultaClientesPorCuit);

        */

    }
}

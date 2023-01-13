using FixedWidthTextUtils.Attributes;

namespace PruebaTransaccion
{
    public class Request_CU1028_T109
    {
        //public string CodigoTransaccion { get; set; }       //Código de transacción(A4) - 1028 - Simple – CU0780NS

        [StringField(55)]
        public string Header { get; set; }

        [StringField(4)]
        public string CodigoTransaccion { get; set; }

        [IntegerField(1, true)]
        public int TipoClaveTributaria { get; set; }

        [IntegerField(11, true)]
        public int NroClaveTributaria { get; set; }

        [StringField(2)]
        public string TipoDocumento { get; set; }

        [IntegerField(8, true)]
        public int NroDocumento { get; set; }

        [StringField(3, StringFieldAttribute.TrimMode.NoTrim)]
        public string VersionDocumento { get; set; }

        [StringField(40, StringFieldAttribute.TrimMode.Trim)]
        public string Apellido { get; set; }

        [StringField(40, StringFieldAttribute.TrimMode.Trim)]
        public string Nombre { get; set; }

        [StringField(1)]
        public string Sexo { get; set; }

        [StringField(1)]
        public string EstadoCivil { get; set; }

        [StringField(2)]
        public string Nacionalidad { get; set; }

        [IntegerField(8, true)]
        public DateTime FechaNacimiento { get; set; }

        [StringField(3)]
        public string IndicEmancipadoAutorizado { get; set; }

        [IntegerField(4, true)]
        public int CasaBera { get; set; }

        [StringField(1)]
        public string IndicadorDeEmpleado { get; set; }

        [StringField(1)]
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
        [StringField(2)]
        public int CarpetaCredito_Numero { get; set; }

        [StringField(25)]
        public string Empleador { get; set; }

        [StringField(1)]
        public string IndicadorDeAutonomo { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaDesdeUltimeActividad { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaAltaCliente { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaBajaCliente { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime FechaDeAcceso { get; set; }

        [StringField(30, StringFieldAttribute.TrimMode.Trim)]
        public string Calle { get; set; }

        [StringField(6, StringFieldAttribute.TrimMode.Trim)]
        public string Numero { get; set; }

        [StringField(3, StringFieldAttribute.TrimMode.Trim)]
        public string Piso { get; set; }

        [StringField(4, StringFieldAttribute.TrimMode.Trim)]
        public string Departamento { get; set; }

        [StringField(1)]
        public string CodigoProvincia { get; set; }

        [StringField(5)]
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

        Nro.Ident(N08)
        Casilla Postal Exterior(A30) (Cudomic)
        Cod.Postal en Exterior (A10) (Cudomic)
        Fecha vencimiento Doc (N08) (O) (Cuclient)
        País de Nacimiento (A02) (O) (Cuclient)
        Segunda Nacionalidad (A02) (Cuclient)
        Marca PEP (A02) (SI/NO) (O) (Cuclient) (*1)
        Valor declarado de Ingr.Anuales(N15) (O) (Cuclient) (*1)(*3)
        Marca Sujeto Obligado(A04) (O) (Cuclient) (*1)
        Admin-Fondos-3ros(A1) (S/N) (O) (Cuclient) (*2)
        Valor-mensual-estima-operar(N15) (Cuclient) (*4)
        Pais de Resid.Fiscal en Exterior(A02) (Cuobserv)
        TIN(A25) (O) (Cuobserv) (*5)
        Barrio(A30) (Cudomic)
        DFE-Calle(A30) (O)
        DFE-Número(A6) (O) (Numérico ó ‘S/N)
        DFE-Torre(A5)
        DFE-Piso(A3)
        DFE- Departamento(A4)
        DFE- Ciudad(A20) (O) --- Es localidad
        DFE- Estado(A20) (O) (se grabará en campo Partido)
        DFE- País(A2) (O) -Según TU302
        DFE-COD-Postal-Exterior(A10) (O)

        /*
        Tipo de clave tributaria(N1) (*) (O)
        Nro.de clave tributaria (N11) (O)
        Tipo de documento (A2) (*) (O)
        Nro. de documento (N8) (O) (O)
        Versión del documento (A3) (*) (O)
        Apellido (A40) (O)
        Nombre (A40) (O)
        Sexo (A1) (*) (O)
        Estado civil (A1) (*) (O)
        Nacionalidad (A2) (*) (O)
        Fecha de nacimiento (N8) (O)
        Indicador de emancipado o autorizado(A3) (Si el cliente es menor de edad)
        Casa Bcra(N4) (O)
        Indicador de empleado(A1) (S/N) (O)
        Posición Iva(A3) (*) (O)
        Bonificación Iva(A1) (S/N) (O)
        Bonificación percepción Iva(A1) (S/N)
        Fecha desde bonif.percep.Iva(N8)
        Fecha hasta bonif.percep.Iva(N8)
        Código de actividad(N3) (*) (O)
        Código de profesión(N3) (*)
        Oficial de cuenta(N5) (Nro.de legajo, sin el dígito verificador)
        Carpeta de crédito
        - Ramo(A2)
        - Número(N7)
        Empleador(A25)
        Indicador de autónomo(A1) (S/N/O)
        Fecha desde de la última actividad(N8)
        Fecha de alta del cliente(N8) (O)
        Fecha de baja del cliente(N8)
        Fecha de deceso(N8)
        Calle(A30) (O)
        Número(A6) (Numérico ó ‘S/N´ para los domicilios sin numeración) (O)
        Piso(A3)
        Departamento(A4)
        Código provincia(A1)
        Código postal(N5) (*) A través de ésta tabla se obtendrá el código de la provincia y la(O)
        localidad respectiva, la que puede ser modificada pro el usuario.

        Código manzana(A3)
        Teléfono(A15) (O)
        Fax(A15)

        Localidad(A20)
        País(A2) (O)
        Email(a70)
        Nro.Ident(N08)
        Casilla Postal Exterior(A30) (Cudomic)
        Cod.Postal en Exterior (A10) (Cudomic)
        Fecha vencimiento Doc (N08) (O) (Cuclient)
        País de Nacimiento (A02) (O) (Cuclient)
        Segunda Nacionalidad (A02) (Cuclient)
        Marca PEP (A02) (SI/NO) (O) (Cuclient) (*1)
        Valor declarado de Ingr.Anuales(N15) (O) (Cuclient) (*1)(*3)
        Marca Sujeto Obligado(A04) (O) (Cuclient) (*1)
        Admin-Fondos-3ros(A1) (S/N) (O) (Cuclient) (*2)
        Valor-mensual-estima-operar(N15) (Cuclient) (*4)
        Pais de Resid.Fiscal en Exterior(A02) (Cuobserv)
        TIN(A25) (O) (Cuobserv) (*5)
        Barrio(A30) (Cudomic)
        DFE-Calle(A30) (O)
        DFE-Número(A6) (O) (Numérico ó ‘S/N)
        DFE-Torre(A5)
        DFE-Piso(A3)
        DFE- Departamento(A4)
        DFE- Ciudad(A20) (O) --- Es localidad
        DFE- Estado(A20) (O) (se grabará en campo Partido)
        DFE- País(A2) (O) -Según TU302
        DFE-COD-Postal-Exterior(A10) (O)
        */
    }
}

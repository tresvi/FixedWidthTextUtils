using FixedWidthTextUtils.Attributes;

namespace PruebaTransaccion
{
    [StringeableClass(95, ' ')]        //Conte 598
    internal class Request_TRX500_ConsultaCliente
    {
        [StringField(0, 54)]
        public string Header { get; set; }     //sbParametros.Append(new String(' ', 51).ToString() + "0043"); //* 43 se hardcodea porque lo necesitan los pgm de natural para trabajar.
        [StringField(55, 58)]
        public string NroTrx { get; set; }     //sbParametros.Append("0500");    //Nro de Trx
        [StringField(59, 59)]
        public string TipoCuit { get; set; }
        [StringField(60, 70)] 
        public string NroCuit { get; set; }
        [StringField(71, 72)]
        public string TipoDoc { get; set; }
        [StringField(73, 92)]
        public string NroDoc { get; set; }
        [StringField(93, 94)]
        public string PaisOrigen { get; set; }      //AR : 80
    }

}

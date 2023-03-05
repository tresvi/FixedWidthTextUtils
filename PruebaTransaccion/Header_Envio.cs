using FixedWidthTextUtils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTransaccion
{

    public class Header_Envio
    {
        [StringField(4)]
        public string TrCIcs { get; set; }      //Se informa solo cuando la transacción a ejecutar debe invocar una transacción CICS
        
        [IntegerField(4, true)]
        public int TrSafe { get; set; }         //Se informa solo cuando la transacción a ejecutar debe invocar una transacción CICS

        [StringField(1)]
        public string Modo { get; set; }        //0:normal – 1:supervisor-- 4:destanqueo 8:reenviada – 9: reenviada supervisor

        [IntegerField(4, true)]
        public int Sucursal { get; set; }

        [IntegerField(6, true)]
        public int JournalLocal { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime Fecha { get; set; }         //N8
        
        [IntegerField(6, true)]
        public int Hora { get; set; }               //N6

        [StringField(8)] 
        public string Usuario { get; set; }         //A8
        
        [StringField(6)] 
        public string JournalReversa { get; set; }  //A6
        
        [StringField(2)] 
        public string Observaciones_OrigenTRX { get; set; }   //A8

        [StringField(2)]
        public string Observaciones_File { get; set; }   //A8

        [StringField(4)]
        public string Observaciones_VersionTransactor { get; set; }   //A8

    }
}

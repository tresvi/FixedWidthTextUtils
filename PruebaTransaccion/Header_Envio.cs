using FixedWidthTextUtils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTransaccion
{
    [StringeableClass(55, ' ')]
    public class Header_Envio
    {
        [StringField(9, 9)]
        public string TrCIcs { get; set; }      //Se informa solo cuando la transacción a ejecutar debe invocar una transacción CICS
        
        [IntegerField(9,9, true)]
        public int TrSafe { get; set; }         //Se informa solo cuando la transacción a ejecutar debe invocar una transacción CICS

        [StringField(9, 9)]
        public string Modo { get; set; }        //0:normal – 1:supervisor-- 4:destanqueo 8:reenviada – 9: reenviada supervisor

        [IntegerField(9, 9, true)]
        public int Sucursal { get; set; }

        [IntegerField(9, 9, true)]
        public int JournalLocal { get; set; }

        [DateTimeField(9, 9, "yyyyMMdd")]
        public DateTime Fecha { get; set; }         //N8
        
        [IntegerField(9, 9, true)]
        public int Hora { get; set; }               //N6

        [StringField(9, 9)] 
        public string Usuario { get; set; }         //A8
        
        [StringField(9, 9)] 
        public string JournalReversa { get; set; }  //A6
        
        [StringField(9, 9)] 
        public string Observaciones { get; set; }   //A8

    }
}

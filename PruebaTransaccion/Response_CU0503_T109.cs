using FixedWidthTextUtils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTransaccion
{
    internal class Response_CU0503_T109
    {
        [IntegerField(1, true)]
        public int TipoTrans { get; set; }

        [IntegerField(4, true)]
        public int CodigoRetorno { get; set; }

        [StringField(60)]
        public string DescripcionRetorno { get; set; }

        [IntegerField(7, true)]
        public int NroLegajo { get; set; }

        [StringField(20)]
        public string NombreReducOficialDeCuenta { get; set; }
    }
}

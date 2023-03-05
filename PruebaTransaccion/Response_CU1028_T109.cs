using FixedWidthTextUtils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTransaccion
{
    internal class Response_CU1028_T109
    {
        [IntegerField(1 , false)]
        int TipoTransaccion { get; set; }

        [IntegerField(1, false)]
        int CodigoRetorno { get; set; }

        [StringField(60)]
        int DescripcionRetorno { get; set; }

        [IntegerField(7, true)]
        int NroLegajo { get; set; }

        [StringField(20)]
        string NombreReducidoOficialCuenta { get; set; }
    }
}

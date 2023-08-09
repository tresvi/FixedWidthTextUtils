using FixedWidthTextUtils.Attributes;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
    internal class FloatingOrdinalNullablesSon: FloatingOrdinalNullables
    {

        /// <summary>
        /// Solo soporta herencia si las properties del hijo, no estan decoradas
        /// o bien, si se usa notacion de posiciones de inicio y fin de campo.
        /// Si se usa ordinal y la clase hija estuviera decorada, al listar las properties, 
        /// antepone las secuencias de corte del hijo a las del padre.
        /// </summary>
        public string? StringPropertySinDecorar { get; set; }
    }
}
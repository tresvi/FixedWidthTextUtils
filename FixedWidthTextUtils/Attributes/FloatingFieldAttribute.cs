using FixedWidthTextUtils.Exceptions;
using System;
using System.Globalization;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FloatingFieldAttribute : FieldAttribute
    {
        internal int DecimalPositions { get; set; }
        internal bool FillLeftWithZero { get; set; }

        internal enum FloatKind : byte { Unbound, Float, Double, Decimal, Unsupported }
        internal FloatKind Kind { get; private set; } = FloatKind.Unbound;

        // Cacheado en ctor para evitar Math.Pow por cada parse/serializacion.
        private readonly long _decimalDivider;

        // Tabla de potencias de 10 hasta 10^18 (cabe en long sin overflow).
        private static readonly long[] PowersOf10 =
        {
            1L, 10L, 100L, 1_000L, 10_000L, 100_000L, 1_000_000L, 10_000_000L,
            100_000_000L, 1_000_000_000L, 10_000_000_000L, 100_000_000_000L,
            1_000_000_000_000L, 10_000_000_000_000L, 100_000_000_000_000L,
            1_000_000_000_000_000L, 10_000_000_000_000_000L, 100_000_000_000_000_000L,
            1_000_000_000_000_000_000L
        };

        public FloatingFieldAttribute(int startPosition, int endPosition, int decimalPositions, bool fillLeftWithZeros = true) 
            : base(startPosition, endPosition)
        {
            if (decimalPositions < 0)
                throw new ArgumentOutOfRangeException($"El valor de {nameof(decimalPositions)} debe ser un valor mayor o igual a 0" );

            DecimalPositions = decimalPositions;
            FillLeftWithZero = fillLeftWithZeros;
            _decimalDivider = decimalPositions < PowersOf10.Length ? PowersOf10[decimalPositions] : (long)Math.Pow(10, decimalPositions);
        }

        public FloatingFieldAttribute(int fieldLength, int decimalPositions, bool fillLeftWithZeros = true) 
            : base(fieldLength)
        {
            if (decimalPositions < 0)
                throw new ArgumentOutOfRangeException($"El valor de {nameof(decimalPositions)} debe ser un valor mayor o igual a 0");

            DecimalPositions = decimalPositions;
            FillLeftWithZero = fillLeftWithZeros;
            _decimalDivider = decimalPositions < PowersOf10.Length ? PowersOf10[decimalPositions] : (long)Math.Pow(10, decimalPositions);
        }


        internal override void Bind(PropertyInfo property)
        {
            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (t == typeof(float)) Kind = FloatKind.Float;
            else if (t == typeof(double)) Kind = FloatKind.Double;
            else if (t == typeof(decimal)) Kind = FloatKind.Decimal;
            else Kind = FloatKind.Unsupported;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            return Parse(property, targetObject, (rawFieldContent ?? string.Empty).AsSpan());
        }


        public override object Parse(PropertyInfo property, object targetObject, ReadOnlySpan<char> rawFieldContent)
        {
            ReadOnlySpan<char> trimmed = rawFieldContent.Trim();

#if NET6_0_OR_GREATER
            if (!long.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out long valorEntero))
#else
            if (!long.TryParse(trimmed.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out long valorEntero))
#endif
                throw new ParseFieldException($"El valor {trimmed.ToString()} no puede ser reconocido como numerico");

            FloatKind kind = Kind;
            if (kind == FloatKind.Unbound)
            {
                Bind(property);
                kind = Kind;
            }

            switch (kind)
            {
                case FloatKind.Float:
                    return (float)valorEntero / _decimalDivider;
                case FloatKind.Double:
                    return (double)valorEntero / _decimalDivider;
                case FloatKind.Decimal:
                    return (decimal)valorEntero / _decimalDivider;
                default:
                    throw new ParseFieldException($"La property {targetObject.GetType().Name}.{property.Name} es del tipo " +
                        $" {property.PropertyType.Name} el cual no es un destino soportado para un número de punto flotante");
            }
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            // Camino legado conservado por compatibilidad. WriteTo escribe directo en el buffer.
            long integerPart, decimalPart;
            long decimalDivider = _decimalDivider;

            FloatKind kind = Kind;
            if (kind == FloatKind.Unbound)
            {
                Bind(property);
                kind = Kind;
            }

            if (kind == FloatKind.Float)
            {
                float valorTemp = (float)property.GetValue(originObject);
                integerPart = (long)Math.Truncate(valorTemp);
                decimalPart = (long)(Math.Round(valorTemp * decimalDivider) - integerPart * decimalDivider);
            }
            else if (kind == FloatKind.Double)
            {
                double valorTemp = (double)property.GetValue(originObject);
                integerPart = (long)Math.Truncate(valorTemp);
                decimalPart = (long)(Math.Round(valorTemp * decimalDivider) - integerPart * decimalDivider);
            }
            else if (kind == FloatKind.Decimal)
            {
                decimal valorTemp = (decimal)property.GetValue(originObject);
                integerPart = (long)Math.Truncate(valorTemp);
                decimalPart = (long)(valorTemp * decimalDivider) - integerPart * decimalDivider;
            }
            else
            {
                throw new SerializeFieldException($"La propiedad \"{originObject.GetType().Name}.{property.Name}\" es" +
                    $" del tipo {property.PropertyType.Name} y no es aceptada para serializar un numero de punto flotante");
            }

            decimalPart = Math.Abs(decimalPart);

            string integerPartText;
            string decimalPartText = Math.Abs(decimalPart).ToString().PadRight((int)this.DecimalPositions, '0');

            string mascara;

            if (this.FillLeftWithZero)
            {
                if (integerPart < 0)
                    mascara = $"D{this.Length - this.DecimalPositions - 1}";
                else
                    mascara = $"D{this.Length - this.DecimalPositions}";

                integerPartText = integerPart.ToString(mascara);
            }
            else
            {
                integerPartText = integerPart.ToString();
            }

            string serializedField = integerPartText + decimalPartText;
            serializedField = serializedField.PadLeft(this.Length, ' ');
            return serializedField;
        }


        public override void WriteTo(PropertyInfo property, object originObject, Span<char> destination)
        {
            object value = property.GetValue(originObject);
            if (value == null)
            {
                destination.Fill(this.FillLeftWithZero ? '0' : ' ');
                return;
            }

            FloatKind kind = Kind;
            if (kind == FloatKind.Unbound) { Bind(property); kind = Kind; }

            long divider = _decimalDivider;
            long scaled;

            switch (kind)
            {
                case FloatKind.Float:
                {
                    float v = (float)value;
                    scaled = (long)Math.Round(v * divider, MidpointRounding.AwayFromZero);
                    break;
                }
                case FloatKind.Double:
                {
                    double v = (double)value;
                    scaled = (long)Math.Round(v * divider, MidpointRounding.AwayFromZero);
                    break;
                }
                case FloatKind.Decimal:
                {
                    decimal v = (decimal)value;
                    scaled = (long)Math.Round(v * divider, MidpointRounding.AwayFromZero);
                    break;
                }
                default:
                    throw new SerializeFieldException($"La propiedad \"{originObject.GetType().Name}.{property.Name}\" es" +
                        $" del tipo {property.PropertyType.Name} y no es aceptada para serializar un numero de punto flotante");
            }

            bool negative = scaled < 0;
            ulong abs = negative ? (ulong)-scaled : (ulong)scaled;

            int totalDigits = CountDigits(abs);
            // Garantizamos al menos DecimalPositions+1 digitos para que la parte entera sea al menos "0".
            int minDigits = DecimalPositions + 1;
            if (totalDigits < minDigits) totalDigits = minDigits;

            int requiredCore = totalDigits + (negative ? 1 : 0);

            if (requiredCore > destination.Length)
                throw new SerializeFieldException($"La serialización del flotante para {originObject.GetType().Name}.{property.Name} produce {requiredCore} caracteres " +
                    $"pero el campo tiene longitud {destination.Length}. Ajuste el valor o el ancho del campo.");

            // Escribimos los digitos de derecha a izquierda en una zona contigua y luego rellenamos.
            int writeIndex = destination.Length - 1;
            ulong remaining = abs;
            for (int d = 0; d < totalDigits; d++)
            {
                destination[writeIndex--] = (char)('0' + (int)(remaining % 10));
                remaining /= 10;
            }

            int padCount = destination.Length - requiredCore;
            if (negative)
            {
                if (this.FillLeftWithZero)
                {
                    destination[0] = '-';
                    if (padCount > 0)
                        destination.Slice(1, padCount).Fill('0');
                }
                else
                {
                    if (padCount > 0)
                        destination.Slice(0, padCount).Fill(' ');
                    destination[padCount] = '-';
                }
            }
            else if (padCount > 0)
            {
                destination.Slice(0, padCount).Fill(this.FillLeftWithZero ? '0' : ' ');
            }
        }

        private static int CountDigits(ulong value)
        {
            int d = 1;
            while (value >= 10) { value /= 10; d++; }
            return d;
        }
    }
}

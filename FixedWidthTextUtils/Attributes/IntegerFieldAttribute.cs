using FixedWidthTextUtils.Exceptions;
using System;
using System.Globalization;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{

    /// <summary>
    /// Atributo de campo para enteros: byte, sbyte, short, ushort, int, uint, long, ulong (y sus tipos anulables con <see cref="NullableIntegerFieldAttribute"/> para null semántico).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IntegerFieldAttribute : FieldAttribute
    {
        internal bool FillLeftWithZero { get; set; }

        /// <summary>
        /// "Discriminador" del tipo entero subyacente. Se calcula una vez en <see cref="Bind"/>
        /// para evitar la cadena de comparaciones de <c>typeof</c> en cada Parse/ToText.
        /// </summary>
        internal enum IntKind : byte { Unbound, Byte, SByte, Short, UShort, Int, UInt, Long, ULong, Unsupported }

        internal IntKind Kind { get; private set; } = IntKind.Unbound;

        public IntegerFieldAttribute(int startPosition, int endPosition, bool fillLeftWithZero = true) 
            : base(startPosition, endPosition)
        {
            FillLeftWithZero = fillLeftWithZero;
        }

        public IntegerFieldAttribute(int fieldLength, bool fillLeftWithZero = true) 
            : base(fieldLength)
        {
            FillLeftWithZero = fillLeftWithZero;
        }


        internal override void Bind(PropertyInfo property)
        {
            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (t == typeof(byte)) Kind = IntKind.Byte;
            else if (t == typeof(sbyte)) Kind = IntKind.SByte;
            else if (t == typeof(short)) Kind = IntKind.Short;
            else if (t == typeof(ushort)) Kind = IntKind.UShort;
            else if (t == typeof(int)) Kind = IntKind.Int;
            else if (t == typeof(uint)) Kind = IntKind.UInt;
            else if (t == typeof(long)) Kind = IntKind.Long;
            else if (t == typeof(ulong)) Kind = IntKind.ULong;
            else Kind = IntKind.Unsupported;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMessage)
        {
            if (GetType() == typeof(IntegerFieldAttribute) && Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                Type underlying = Nullable.GetUnderlyingType(property.PropertyType);
                if (underlying != null && IsCoreIntegerType(underlying))
                {
                    errorMessage = $"La property {property.Name} es de tipo anulable {property.PropertyType.Name}. " +
                        $"Use {nameof(NullableIntegerFieldAttribute)} con un texto para null de longitud {this.Length}.";
                    return false;
                }
            }

            if (!IsSupportedIntegerPropertyType(property.PropertyType))
            {
                errorMessage = $"La property {property.Name} es de tipo {property.PropertyType.Name}, que no es un tipo entero soportado por {nameof(IntegerFieldAttribute)}.";
                return false;
            }

            errorMessage = "";
            return true;
        }

        private static bool IsCoreIntegerType(Type type)
        {
            return type == typeof(byte) || type == typeof(sbyte)
                || type == typeof(short) || type == typeof(ushort)
                || type == typeof(int) || type == typeof(uint)
                || type == typeof(long) || type == typeof(ulong);
        }

        private static bool IsSupportedIntegerPropertyType(Type propertyType)
        {
            Type t = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            return IsCoreIntegerType(t);
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            return Parse(property, targetObject, (rawFieldContent ?? string.Empty).AsSpan());
        }


        public override object Parse(PropertyInfo property, object targetObject, ReadOnlySpan<char> rawFieldContent)
        {
            ReadOnlySpan<char> trimmed = rawFieldContent.Trim();

            // Si Bind aun no fue llamado (uso directo del atributo) calculamos al vuelo.
            IntKind kind = Kind;
            if (kind == IntKind.Unbound)
            {
                Bind(property);
                kind = Kind;
            }

            const NumberStyles styles = NumberStyles.Integer;
            CultureInfo inv = CultureInfo.InvariantCulture;

            switch (kind)
            {
                case IntKind.Byte:
                    if (TryParseByte(trimmed, styles, inv, out byte b)) return b;
                    break;
                case IntKind.SByte:
                    if (TryParseSByte(trimmed, styles, inv, out sbyte sb)) return sb;
                    break;
                case IntKind.Short:
                    if (TryParseShort(trimmed, styles, inv, out short s)) return s;
                    break;
                case IntKind.UShort:
                    if (TryParseUShort(trimmed, styles, inv, out ushort us)) return us;
                    break;
                case IntKind.Int:
                    if (TryParseInt(trimmed, styles, inv, out int i)) return i;
                    break;
                case IntKind.UInt:
                    if (TryParseUInt(trimmed, styles, inv, out uint ui)) return ui;
                    break;
                case IntKind.Long:
                    if (TryParseLong(trimmed, styles, inv, out long l)) return l;
                    break;
                case IntKind.ULong:
                    if (TryParseULong(trimmed, styles, inv, out ulong ul)) return ul;
                    break;
                default:
                    throw new ParseFieldException($"La property {property.Name} es de tipo {property.PropertyType.Name} el cual no es aceptado como tipo numérico entero");
            }

            // Las subclases (NullableIntegerFieldAttribute) podrian construir el mensaje con datos
            // adicionales antes; aqui usamos uno generico para no asignar strings cuando todo OK.
            string failMsg = $"El valor \"{trimmed.ToString()}\" no puede ser reconocido como un entero válido del tipo {property.PropertyType.Name} " +
                $"para la property {targetObject.GetType().Name}.{property.Name}. Verifique que el dato sea numérico y este dentro del rango del tipo correspondiente";
            throw new ParseFieldException(failMsg);
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            // Camino legado conservado por compatibilidad: WriteTo es el camino "rapido" que usa
            // LineParser (sin asignar PadLeft + concat). Aqui se mantiene el comportamiento previo.
            object initialValue = property.GetValue(originObject);
            string outputText = (initialValue ?? "").ToString();

            if (this.FillLeftWithZero)
            {
                if (outputText.StartsWith("-"))
                {
                    outputText = outputText.TrimStart('-');
                    outputText = "-" + outputText.PadLeft(this.Length - 1, '0');
                }
                else
                {
                    outputText = outputText.PadLeft(this.Length, '0');
                }
            }
            else
            {
                outputText = outputText.PadLeft(this.Length, ' ');
            }

            if (outputText.Length != this.Length)
                throw new SerializeFieldException($"La serialización del entero para {originObject.GetType().Name}.{property.Name} produce {outputText.Length} caracteres " +
                    $"pero el campo tiene longitud {this.Length}. Ajuste el valor o el ancho del campo.");

            return outputText;
        }


        public override void WriteTo(PropertyInfo property, object originObject, Span<char> destination)
        {
            object value = property.GetValue(originObject);
            if (value == null)
            {
                destination.Fill(this.FillLeftWithZero ? '0' : ' ');
                return;
            }

            // Convertimos a long/ulong segun el rango (cubre todos los enteros soportados sin
            // multiples ramas TryFormat por tipo concreto).
            long signed = 0;
            ulong unsigned = 0;
            bool isUnsignedKind = Kind == IntKind.Byte || Kind == IntKind.UShort || Kind == IntKind.UInt || Kind == IntKind.ULong;

            if (isUnsignedKind)
            {
                unsigned = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            }
            else
            {
                signed = Convert.ToInt64(value, CultureInfo.InvariantCulture);
            }

            bool negative = !isUnsignedKind && signed < 0;
            ulong absVal = isUnsignedKind ? unsigned : (negative ? (ulong)-signed : (ulong)signed);

            int digits = CountDigits(absVal);
            int requiredCore = digits + (negative ? 1 : 0);

            if (requiredCore > destination.Length)
                throw new SerializeFieldException($"La serialización del entero para {originObject.GetType().Name}.{property.Name} produce {requiredCore} caracteres " +
                    $"pero el campo tiene longitud {destination.Length}. Ajuste el valor o el ancho del campo.");

            char fill = this.FillLeftWithZero ? '0' : ' ';

            // Escritura right-aligned: primero los digitos al final y luego rellenamos a la izquierda.
            int writeIndex = destination.Length - 1;
            ulong remaining = absVal;
            for (int d = 0; d < digits; d++)
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
                destination.Slice(0, padCount).Fill(fill);
            }
        }

        private static int CountDigits(ulong value)
        {
            int d = 1;
            while (value >= 10) { value /= 10; d++; }
            return d;
        }

#if NET6_0_OR_GREATER
        private static bool TryParseByte(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out byte v) => byte.TryParse(s, st, fp, out v);
        private static bool TryParseSByte(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out sbyte v) => sbyte.TryParse(s, st, fp, out v);
        private static bool TryParseShort(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out short v) => short.TryParse(s, st, fp, out v);
        private static bool TryParseUShort(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out ushort v) => ushort.TryParse(s, st, fp, out v);
        private static bool TryParseInt(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out int v) => int.TryParse(s, st, fp, out v);
        private static bool TryParseUInt(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out uint v) => uint.TryParse(s, st, fp, out v);
        private static bool TryParseLong(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out long v) => long.TryParse(s, st, fp, out v);
        private static bool TryParseULong(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out ulong v) => ulong.TryParse(s, st, fp, out v);
#else
        // En netstandard2.0 los TryParse no aceptan ReadOnlySpan<char>; materializamos el span
        // (1 alloc) en lugar de las 2 originales (Substring + Trim).
        private static bool TryParseByte(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out byte v) => byte.TryParse(s.ToString(), st, fp, out v);
        private static bool TryParseSByte(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out sbyte v) => sbyte.TryParse(s.ToString(), st, fp, out v);
        private static bool TryParseShort(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out short v) => short.TryParse(s.ToString(), st, fp, out v);
        private static bool TryParseUShort(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out ushort v) => ushort.TryParse(s.ToString(), st, fp, out v);
        private static bool TryParseInt(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out int v) => int.TryParse(s.ToString(), st, fp, out v);
        private static bool TryParseUInt(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out uint v) => uint.TryParse(s.ToString(), st, fp, out v);
        private static bool TryParseLong(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out long v) => long.TryParse(s.ToString(), st, fp, out v);
        private static bool TryParseULong(ReadOnlySpan<char> s, NumberStyles st, IFormatProvider fp, out ulong v) => ulong.TryParse(s.ToString(), st, fp, out v);
#endif
    }
}

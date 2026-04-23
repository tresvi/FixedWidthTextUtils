using FixedWidthTextUtils.Exceptions;
using System;
using System.Globalization;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DateTimeFieldAttribute : FieldAttribute
    {
        public string Format { get; set; }
        public bool PadToRight { get; set; }
        public bool LeftPadding { get; set; }

        // Cache: el Format.Trim() se usaba en cada Parse, generando una asignacion por linea.
        private readonly string _trimmedFormat;


        public DateTimeFieldAttribute(int startPosition, int endPosition, string format, bool leftPadding = false) : base(startPosition, endPosition)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException(nameof(format), " format no puede ser un valor nulo");      //!! Revisar de como cambiarlo por una excpecion propia

            Format = format;
            LeftPadding = leftPadding;
            _trimmedFormat = format.Trim();
        }

        public DateTimeFieldAttribute(int fieldLength, string format, bool leftPadding = false) : base(fieldLength)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException(nameof(format), " format no puede ser un valor nulo");      //!! Revisar de como cambiarlo por una excpecion propia

            Format = format;
            LeftPadding = leftPadding;
            _trimmedFormat = format.Trim();
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMessage)
        {
            if (this.Length != this.Format.Length)
            {
                errorMessage = $"La longitud definida en el parametro \"{nameof(Format)}\" del attribute ({this.Format.Length} " +
                    $"caracteres) debe coincidir con la longitud definida para este campo ({this.Length} caracteres)";
                return false;
            }

            errorMessage = "";
            return true;
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            return Parse(property, targetObject, (rawFieldContent ?? string.Empty).AsSpan());
        }


        public override object Parse(PropertyInfo property, object targetObject, ReadOnlySpan<char> rawFieldContent)
        {
            bool isValidType = property.PropertyType == typeof(DateTime)
                            || property.PropertyType == typeof(DateTime?);

            if (!isValidType)
                throw new ParseFieldException($"La property {targetObject.GetType().Name}.{property.Name} es de tipo " +
                    $"{property.PropertyType.Name} el cual no es un destino soportado para un {typeof(DateTime?).Name}");

            ReadOnlySpan<char> trimmed = rawFieldContent.Trim();

#if NET6_0_OR_GREATER
            bool parseOK = DateTime.TryParseExact(trimmed, _trimmedFormat.AsSpan(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaTemp);
#else
            bool parseOK = DateTime.TryParseExact(trimmed.ToString(), _trimmedFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaTemp);
#endif

            if (!parseOK)
                throw new ParseFieldException($"El valor \"{trimmed.ToString()}\" no puede ser interpretado como fecha según el formato \"{this.Format}\"  de la propiedad {property.Name}");

            return fechaTemp;
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(DateTime) && property.PropertyType != typeof(DateTime?))
                throw new SerializeFieldException($"La propiedad para la serializacion \"{originObject.GetType().Name}.{property.Name}\" no es del tipo DateTime");

            DateTime DateTemp = (DateTime)property.GetValue(originObject);

            string outputText = DateTemp.ToString(this.Format);
            outputText = this.LeftPadding ? outputText.PadLeft(this.Length) : outputText.PadRight(this.Length);
            return outputText;
        }


        public override void WriteTo(PropertyInfo property, object originObject, Span<char> destination)
        {
            object value = property.GetValue(originObject);
            if (value == null)
            {
                destination.Fill(' ');
                return;
            }

            DateTime dt = (DateTime)value;

            // Importante: usamos el Format ORIGINAL (no _trimmedFormat) para serializar. El Format
            // puede contener espacios literales embebidos que actuan como padding (por ejemplo
            // "    yyyyMMddHHmmss") y ValidateFieldDefinition garantiza Format.Length == Length,
            // por lo que el output cabe exacto en el slice destino.
#if NET6_0_OR_GREATER
            if (!dt.TryFormat(destination, out int written, this.Format.AsSpan(), CultureInfo.InvariantCulture))
                throw new SerializeFieldException($"No se pudo formatear DateTime para {originObject.GetType().Name}.{property.Name} con formato \"{this.Format}\"");

            if (written < destination.Length)
            {
                if (this.LeftPadding)
                {
                    // En el camino legado, si el output todavia es mas corto que el campo (caso
                    // raro porque Format.Length == Length), se aplica PadLeft con espacios.
                    int pad = destination.Length - written;
                    Span<char> tmp = stackalloc char[written];
                    destination.Slice(0, written).CopyTo(tmp);
                    destination.Slice(0, pad).Fill(' ');
                    tmp.CopyTo(destination.Slice(pad));
                }
                else
                {
                    destination.Slice(written).Fill(' ');
                }
            }
#else
            string formatted = dt.ToString(this.Format, CultureInfo.InvariantCulture);
            if (formatted.Length > destination.Length)
                throw new SerializeFieldException($"La serialización de DateTime para {originObject.GetType().Name}.{property.Name} produce {formatted.Length} caracteres pero el campo tiene longitud {destination.Length}.");

            if (this.LeftPadding && formatted.Length < destination.Length)
            {
                int pad = destination.Length - formatted.Length;
                destination.Slice(0, pad).Fill(' ');
                formatted.AsSpan().CopyTo(destination.Slice(pad));
            }
            else
            {
                formatted.AsSpan().CopyTo(destination);
                if (formatted.Length < destination.Length)
                    destination.Slice(formatted.Length).Fill(' ');
            }
#endif
        }
    }
}

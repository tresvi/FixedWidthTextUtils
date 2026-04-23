using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class StringFieldAttribute : FieldAttribute
    {
        public enum TrimMode { NoTrim, Trim, TrimStart, TrimEnd };
        public TrimMode TrimInputMode { get; set; }
        public bool LeftPadding { get; set; }


        public StringFieldAttribute(int startPosition, int endPosition, TrimMode trimInputMode = TrimMode.TrimEnd, bool leftPadding = false) : base(startPosition, endPosition)
        {
            TrimInputMode = trimInputMode;
            LeftPadding = leftPadding;
        }

        public StringFieldAttribute(int fieldLength, TrimMode trimInputMode = TrimMode.TrimEnd, bool leftPadding = false) : base(fieldLength)
        {
            if (fieldLength < 1)
                throw new ArgumentException(nameof(fieldLength), $"{nameof(fieldLength)} debe ser mayor o igual a 1");

            TrimInputMode = trimInputMode;
            LeftPadding = leftPadding;
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
            if (property.PropertyType != typeof(string))
                throw new ParseFieldException($"La propiedad de asignacion {property.Name} no es del tipo string");

            ReadOnlySpan<char> trimmed;
            switch (this.TrimInputMode)
            {
                case TrimMode.Trim: trimmed = rawFieldContent.Trim(); break;
                case TrimMode.TrimStart: trimmed = rawFieldContent.TrimStart(); break;
                case TrimMode.TrimEnd: trimmed = rawFieldContent.TrimEnd(); break;
                default: trimmed = rawFieldContent; break;
            }

            // Una sola asignacion: la string final que se asigna a la property.
            return trimmed.Length == 0 ? string.Empty : trimmed.ToString();
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(string))
                throw new SerializeFieldException($"La propiedad para la serializacion {property.Name} no es del tipo string");

            string outputText = (property.GetValue(originObject) ?? "").ToString();
            outputText = this.LeftPadding ? outputText.PadLeft(this.Length) : outputText.PadRight(this.Length);
            return outputText;
        }


        public override void WriteTo(PropertyInfo property, object originObject, Span<char> destination)
        {
            if (property.PropertyType != typeof(string))
                throw new SerializeFieldException($"La propiedad para la serializacion {property.Name} no es del tipo string");

            string s = (string)property.GetValue(originObject);
            if (s == null) s = string.Empty;

            int n = s.Length;
            if (n > destination.Length)
            {
                // Se trunca al ancho del campo (mismo comportamiento que el ToText original con
                // Pad*: Pad* nunca trunca; pero asignar mas que el campo siempre dio sobreescritura
                // del campo siguiente en el flujo viejo via ReplaceAt). Aqui simplemente truncamos
                // para no salir del slice destinado.
                n = destination.Length;
            }

            if (this.LeftPadding)
            {
                int pad = destination.Length - n;
                if (pad > 0) destination.Slice(0, pad).Fill(' ');
                s.AsSpan(0, n).CopyTo(destination.Slice(pad));
            }
            else
            {
                s.AsSpan(0, n).CopyTo(destination);
                if (n < destination.Length)
                    destination.Slice(n).Fill(' ');
            }
        }
    }
}

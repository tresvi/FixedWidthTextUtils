using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    public abstract class FieldAttribute : Attribute
    {
        internal int StartPosition { get; set; }
        internal int EndPosition { get; set; }
        //internal int FieldLength { get; set; }
        internal bool IsOrdinalMode { get; set; }

        internal int Length { get; set; }

        public FieldAttribute(int startPosition, int endPosition)
        {
            if (startPosition < 0)
                throw new ArgumentException(nameof(startPosition), "StartPosition debe ser >= 0");

            if (endPosition < startPosition)
                throw new ArgumentException(nameof(EndPosition), "EndPosition debe ser >= a StartPosition");

            StartPosition = startPosition;
            EndPosition = endPosition;
            Length = EndPosition - StartPosition + 1;
            IsOrdinalMode = false;
        }

        public FieldAttribute(int fieldLength)
        {
            Length = fieldLength;
            //FieldLength = fieldLength;
            IsOrdinalMode = true;
        }

        /// <summary>
        /// Hook llamado una unica vez por <see cref="LineModelPlanCache"/> al construir el plan,
        /// permite a las subclases cachear datos derivados del <see cref="PropertyInfo"/> (por
        /// ejemplo el "kind" del entero a parsear) sin pagar ese costo en cada linea.
        /// </summary>
        internal virtual void Bind(PropertyInfo property) { }

        public abstract object Parse(PropertyInfo property, object targetObject, string rawFieldContent);
        public abstract string ToText(PropertyInfo property, object originObject);
        public abstract bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMessage);

        /// <summary>
        /// Camino "rapido" basado en <see cref="ReadOnlySpan{T}"/>: por defecto materializa la
        /// porcion de linea en un <see cref="string"/> y delega en <see cref="Parse(PropertyInfo, object, string)"/>.
        /// Las subclases incluidas en la libreria sobreescriben este metodo para evitar la asignacion.
        /// </summary>
        public virtual object Parse(PropertyInfo property, object targetObject, ReadOnlySpan<char> rawFieldContent)
        {
            return Parse(property, targetObject, rawFieldContent.ToString());
        }

        /// <summary>
        /// Escribe la representacion textual del campo directamente en <paramref name="destination"/>.
        /// Por defecto delega en <see cref="ToText(PropertyInfo, object)"/> y copia el resultado.
        /// Las subclases internas escriben sin asignar strings intermedios.
        /// </summary>
        public virtual void WriteTo(PropertyInfo property, object originObject, Span<char> destination)
        {
            string text = ToText(property, originObject);
            if (text == null) text = string.Empty;
            int len = Math.Min(text.Length, destination.Length);
            text.AsSpan(0, len).CopyTo(destination);
            if (len < destination.Length)
                destination.Slice(len).Fill(' ');
        }
    }
}

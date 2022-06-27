using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class BooleanFieldAttribute : FieldAttribute
    {
        public string TextForTrue { get; set; }
        public string TextForFalse { get; set; }

        ///// <summary>
        ///// Constructor. Asigna True a aquellas palabras que coincidan con textForTrue y False a 
        ///// todas aquellas que no coincidan
        ///// </summary>
        ///// <param name="startPosition">Posición inicial del texto a evaluar para el campo</param>
        ///// <param name="endPosition">Posicion final del texto a evaluar para el campo</param>
        ///// <param name="textForTrue">Valor de texto por el cual se identifiacará al campo como True</param>
        //public BooleanFieldAttribute(int startPosition, int endPosition, string textForTrue)
        //    : base(startPosition, endPosition)
        //{
        //    this.TextForTrue = textForTrue;
        //}

        /// <summary>
        /// Constructor. Asigna True a aquellas palabras que coincidan con textForTrue, False a todas
        /// aquellas que coincidan con textForFalse, y un lanzará una excepcion en caso de no reocnocer ningun valor.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="textForFalse"></param>
        /// <param name="textForTrue"></param>
        /// <param name="textFroFalse"></param>
        public BooleanFieldAttribute(int startPosition, int endPosition, string textForTrue, string textForFalse) 
            : base(startPosition, endPosition)
        {
            this.TextForTrue = textForTrue;
            this.TextForFalse = textForFalse;
        }


        internal override void Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            if (property.PropertyType != typeof(bool))
                throw new ParseFieldException($"La propiedad de asignacion \"{property.Name}\" no es del tipo bool");

            bool value = false;
            if (this.TextForFalse == "")
            {
                value = rawFieldContent == this.TextForTrue;
            }
            else
            {
                if (rawFieldContent == this.TextForTrue)
                    value = true;
                else if (rawFieldContent == this.TextForFalse)
                    value = false;
                else
                    throw new ParseFieldException($"El valor \"{rawFieldContent}\" no puede ser reconocido como un booleano válido para " +
                        $"ser asignado a la property \"{property.Name}\". Verifique que el dato coincida con los valores definidos para la property");
            }
 
            property.SetValue(targetObject, value);
        }


        internal override string ToPlainText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(bool))
                throw new SerializeFieldException($"La propiedad para la serializacion \"{property.Name}\" no es del tipo bool");

            bool value = (bool)property.GetValue(originObject);

            if (value)
                return this.TextForTrue;
            else 
                return this.TextForFalse;
        }
    }
}

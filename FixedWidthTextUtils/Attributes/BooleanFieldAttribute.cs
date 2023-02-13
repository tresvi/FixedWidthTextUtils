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
        /// aquellas que coincidan con textForFalse, y un lanzará una excepcion en caso de no reconocer ningun valor.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="textForFalse"></param>
        /// <param name="textForTrue"></param>
        public BooleanFieldAttribute(int startPosition, int endPosition, string textForTrue, string textForFalse)
            : base(startPosition, endPosition)
        {
            this.TextForTrue = textForTrue;
            this.TextForFalse = textForFalse;
        }

        /// <summary>
        /// Constructor. Asigna True a aquellas palabras que coincidan con textForTrue, False a todas
        /// aquellas que coincidan con textForFalse, y un lanzará una excepcion en caso de no reconocer ningun valor.
        /// </summary>
        /// <param name="fieldLength">Longitud del campo</param>
        /// <param name="textForFalse"></param>
        /// <param name="textForTrue"></param>
        public BooleanFieldAttribute(int fieldLength, string textForTrue, string textForFalse)
            : base(fieldLength)
        {
            this.TextForTrue = textForTrue;
            this.TextForFalse = textForFalse;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMesage)
        {
            if (this.Length != this.TextForTrue.Length)
            {
                errorMesage = $"La longitud definida en el parametro \"{nameof(TextForTrue)}\" del attribute ({this.TextForTrue.Length} " +
                    $"caracteres) debe coincidir con la longitud definida para este campo ({this.Length} caracteres)";
                return false;
            }

            if (this.TextForTrue == this.TextForFalse)
            {
                errorMesage = $"El valor del parametro \"{nameof(TextForTrue)}\" no puede coincidir con el valor del parametro \"{nameof(TextForFalse)}\"";
                return false;
            }

            //Se permite que el false sea el caracter empty ". esto es para dar flexibilidad en la definicionde los false
            if (this.Length != this.TextForFalse.Length && this.TextForFalse != "")
            {
                errorMesage = $"La longitud definida en el parametro \"{nameof(TextForFalse)}\" del attribute ({this.TextForFalse.Length} " +
                    $"caracteres) debe coincidir con la longitud del campo definido ({this.Length} caracteres)";
                return false;
            }

            errorMesage = "";
            return true;
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            if (property.PropertyType != typeof(bool))
                throw new ParseFieldException($"La propiedad de asignacion \"{targetObject.GetType().Name}.{property.Name}\" no es del tipo bool");

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
                    throw new ParseFieldException($"El valor \"{rawFieldContent}\" no puede ser reconocido como un booleano válido para ser asignado a " +
                        $"la property \"{targetObject.GetType().Name}.{property.Name}\". Verifique que el dato coincida con los valores definidos para la property");
            }

            return value;
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(bool) && property.PropertyType != typeof(bool?))
                throw new SerializeFieldException($"La propiedad para la serializacion \"{originObject.GetType().Name}.{property.Name}\" no es del tipo bool");

            bool value = (bool)property.GetValue(originObject);

            if (value)
                return this.TextForTrue;
            else 
                return this.TextForFalse;
        }
    }
}

﻿using FixedWidthTextUtils.Exceptions;
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
                throw new ArgumentException(nameof(fieldLength), $"{nameof(fieldLength)} debe ser mayor a 1");

            TrimInputMode = trimInputMode;
            LeftPadding = leftPadding;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMesage)
        {
            errorMesage = "";
            return true;
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            if (property.PropertyType != typeof(String) || property.PropertyType != typeof(string))
                throw new ParseFieldException($"La propiedad de asignacion {property.Name} no es del tipo string");

            switch (this.TrimInputMode)
            {
                case TrimMode.Trim:
                    rawFieldContent = rawFieldContent.Trim();
                    break;
                case TrimMode.TrimStart:
                    rawFieldContent = rawFieldContent.TrimStart();
                    break;
                case TrimMode.TrimEnd:
                    rawFieldContent = rawFieldContent.TrimEnd();
                    break;
            }

            return rawFieldContent;
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(String) && property.PropertyType != typeof(string))
                throw new SerializeFieldException($"La propiedad para la serializacion {property.Name} no es del tipo string");

            string outputText = (property.GetValue(originObject) ?? "").ToString();
            outputText = this.LeftPadding ? outputText.PadLeft(this.Length) : outputText.PadRight(this.Length);
            return outputText;
        }
    }
}

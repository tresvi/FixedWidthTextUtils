using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FixedWidthTextUtils
{
    internal static class Utils
    {
        internal static string GetInitializedLine(object value)
        {
            try
            {
                List<Attribute> stringeableAttribs = value.GetType().GetCustomAttributes(typeof(StringeableClassAttribute)).ToList();
                if (stringeableAttribs.Count == 0)
                    throw new Exception($"La clase {value.GetType().Name} no fue decorada como {typeof(StringeableClassAttribute).Name}");

                StringeableClassAttribute stringeable = (StringeableClassAttribute)stringeableAttribs.First();
                return new string(stringeable.FillerChar, stringeable.RegisterLineLength);
            }
            catch (Exception ex)
            {
                throw new NonStringeableClassException($"Error al determinar la longitud de linea y el caracter de relleno" +
                    $" de la clase  {value.GetType().Name}. {ex.Message}", ex);
            }
        }


        internal static int GetLineLength(object value)
        {
            try
            {
                List<Attribute> stringeableAttribs = value.GetType().GetCustomAttributes(typeof(StringeableClassAttribute)).ToList();
                if (stringeableAttribs.Count == 0)
                    throw new Exception($"La clase {value.GetType().Name} no fue decorada como {typeof(StringeableClassAttribute).Name}");

                StringeableClassAttribute stringeable = (StringeableClassAttribute)stringeableAttribs.First();
                return stringeable.RegisterLineLength;
            }
            catch (Exception ex)
            {
                throw new NonStringeableClassException($"Error al determinar la longitud de linea y el caracter de relleno de la clase  {value.GetType().Name}. {ex.Message}", ex);
            }
        }


        internal static StringBuilder ReplaceAt(StringBuilder sb, int replaceIndex, string textToInsert)
        {
            sb.Remove(replaceIndex, textToInsert.Length).Insert(replaceIndex, textToInsert);
            return sb;
        }


        internal static string ReplaceAt(string str, int replaceIndex, string textToInsert)
        {
            return str.Remove(replaceIndex, textToInsert.Length).Insert(replaceIndex, textToInsert);
        }
    }
}

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
                int lineLength = Utils.GetLineLength(value);

                return new string(' ', lineLength);
            }
            catch (Exception ex)
            {
                throw new NonStringeableClassException($"Error al determinar la longitud de linea de la clase serializada" +
                    $" de la clase  {value.GetType().Name}. {ex.Message}", ex);
            }
        }


        internal static int GetLineLength(object value)
        {
            try
            {
                PropertyInfo[] properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                bool existsOrdinalsFields = false;
                //bool existsPositionalFields = false;

                foreach (PropertyInfo property in properties)
                {
                    foreach (FieldAttribute fieldAttrib in property.GetCustomAttributes(typeof(FieldAttribute), true))
                    {
                        if (fieldAttrib.IsOrdinalMode) existsOrdinalsFields = true;
                        //if (fieldAttrib.IsOrdinalMode == false) existsPositionalFields = true;
                    }
                }

                //!!! Ver que se hace con esto
                //if (existsOrdinalsFields && existsPositionalFields)
                //{
                //    throw new NonStringeableClassException($"La clase {value.GetType().Name} no fue decorada con constructores de Campo para lectura posicional y ordinal. Ambos no pueden mezclarse dentro de la misma clase, debe usar solo los de un tipo u otro ");
                //}

                int maxEndPosition = 0;


                if (existsOrdinalsFields)
                {
                    foreach (PropertyInfo property in properties)
                    {
                        foreach (FieldAttribute fieldAttrib in property.GetCustomAttributes(typeof(FieldAttribute), true))
                        {
                            maxEndPosition += fieldAttrib.Length;
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo property in properties)
                    {
                        foreach (FieldAttribute fieldAttrib in property.GetCustomAttributes(typeof(FieldAttribute), true))
                        {
                            if (fieldAttrib.EndPosition > maxEndPosition) maxEndPosition = fieldAttrib.EndPosition;
                        }
                    }
                    maxEndPosition += 1;
                }

                return maxEndPosition;
                /* Original
                List<Attribute> stringeableAttribs = value.GetType().GetCustomAttributes(typeof(StringeableClassAttribute)).ToList();
                if (stringeableAttribs.Count == 0)
                    throw new Exception($"La clase {value.GetType().Name} no fue decorada como {typeof(StringeableClassAttribute).Name}");

                StringeableClassAttribute stringeable = (StringeableClassAttribute)stringeableAttribs.First();
                return stringeable.RegisterLineLength;
                */
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

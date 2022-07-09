using FixedWidthTextUtils.Exceptions;
using Models.FixedWidthTextUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FixedWidthTextUtils
{
    public class FileConvert
    {
        public string Path { get; set; }
        public Encoding Encoding { get; set; }
        public List<InvalidLine> InvalidLines { get; set; }

        public FileConvert(string path)
        {
            Path = path;
            Encoding = Encoding.UTF8;
            InvalidLines = new List<InvalidLine>();
        }

        public FileConvert(string path, Encoding encoding)
        {
            Path = path;
            Encoding = encoding;
            InvalidLines = new List<InvalidLine>();
        }


        public List<T> ParseFile<T>(bool ignoreWrongLines) where T : new()
        {
            List<T> parsedLines = new List<T>();
            long lineNumber = 0;
            string inputLine = "";
            T value = new T();
            int lineLengthOK = Utils.GetLineLength(value);

            try
            {
                using (StreamReader sr = new StreamReader(this.Path, this.Encoding))
                {
                    while ((inputLine = sr.ReadLine()) != null)
                    {
                        try
                        {
                            lineNumber++;

                            if (inputLine.Length != lineLengthOK)
                            {
                                string errorLineLengthMsge = $"Longitud de linea esperada es {lineLengthOK} pero se leyo una linea de longitud {inputLine.Length}";

                                if (ignoreWrongLines)
                                    InvalidLines.Add(new InvalidLine(lineNumber, inputLine, errorLineLengthMsge));
                                else
                                    throw new ParseFieldException(errorLineLengthMsge);
                            }

                            parsedLines.Add(RegisterConvert.Parse<T>(inputLine));
                        }
                        catch (Exception ex)
                        {
                            if (ignoreWrongLines)
                                InvalidLines.Add(new InvalidLine(lineNumber, inputLine, ex.Message));
                            else
                                throw new ParseFieldException(ex.Message, ex);
                        }
                    }
                }
            }
            catch (ParseFieldException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al acceder al archivo de entrada. Detalles: {ex.Message}", ex);
            }
            return parsedLines;
        }


        public void SerializeFile<T>(List<T> entities,  string outputPath, Encoding encoding = Encoding.UTF8)
        {
            foreach (T entity in entities)
            { 
                entity.Serialize(entity, outputPath, encoding);
            }
        
        }
    }
}

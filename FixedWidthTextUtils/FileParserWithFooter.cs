using FixedWidthTextUtils.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FixedWidthTextUtils
{
    public class FileParserWithFooter<F> : FileParser where F : new()
    {
        public F Footer { get; set; }

        public FileParserWithFooter(string path) : base(path) { }
        public FileParserWithFooter(string path, Encoding encoding): base(path, encoding) { }

        public List<T> Parse<T>(bool ignoreWrongLines, out F footer) where T : new()
        {
            int lastLineNumber;
            try
            {
                string lastLine = Utils.GetLastLine(Path, Encoding, out lastLineNumber);
                footer = LineParser.Parse<F>(lastLine);
            }
            catch (IOException) { throw; }
            catch (ParseFieldException ex) { 
                throw new ParseFieldException($"Error al parsear el Footer. Detalles: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al acceder al archivo de entrada. Detalles: {ex.Message}", ex);
            }

            return base.Parse<T>(ignoreWrongLines, lastLineNumber);
        }


        public void ToFlatFile<T>(List<T> entities, F footer, string outputPath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    foreach (object entity in entities)
                        writer.WriteLine(LineParser.ToTextLine(entity));
                    writer.WriteLine(LineParser.ToTextLine(footer));
                }
            }
            catch (IOException) { throw; }
            catch (SerializeFieldException) { throw; }
            catch (Exception ex)
            {
                throw new SerializeFieldException($"Error generar archivo de texto en {outputPath}. Detalles: {ex.Message}", ex);
            }
        }
    }
}

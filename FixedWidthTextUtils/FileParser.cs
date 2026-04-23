using FixedWidthTextUtils.Exceptions;
using Models.FixedWidthTextUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FixedWidthTextUtils
{
    //TODO: Agregar Test de Archivo vacio
    public class FileParser
    {
        // Buffer interno del StreamReader; el default es 1KB y para archivos grandes implica
        // muchas mas llamadas al disco. 64KB reduce notablemente la presion de I/O.
        private const int StreamReaderBufferSize = 64 * 1024;

        public string Path { get; set; }
        public Encoding Encoding { get; set; }
        public List<InvalidLine> InvalidLines { get; set; }

        public FileParser(string path)
        {
            Path = path;
            Encoding = Encoding.UTF8;
            InvalidLines = new List<InvalidLine>();
        }

        public FileParser(string path, Encoding encoding)
        {
            Path = path;
            Encoding = encoding;
            InvalidLines = new List<InvalidLine>();
        }


        public List<T> Parse<T>(bool ignoreWrongLines) where T : new()
        {
            return this.Parse<T>(ignoreWrongLines, 0);
        }


        internal List<T> Parse<T>(bool ignoreWrongLines, int skipLine) where T : new()
        {
            // Estimacion grosera de capacidad para evitar resizes del List<T> en archivos grandes.
            // Suponemos linea promedio de 80 bytes; sobrestima si las lineas son largas, lo cual
            // sigue siendo mejor que multiples Array.Resize.
            int estimatedCapacity = 16;
            try
            {
                FileInfo fi = new FileInfo(this.Path);
                if (fi.Exists && fi.Length > 0)
                {
                    long estimate = fi.Length / 80;
                    if (estimate > int.MaxValue) estimate = int.MaxValue;
                    estimatedCapacity = (int)Math.Max(16, estimate);
                }
            }
            catch
            {
                // Si no podemos consultar el FileInfo (permisos, etc.) seguimos con default.
            }

            List<T> parsedLines = new List<T>(estimatedCapacity);
            long lineNumber = 0;
            string inputLine = "";

            try
            {
                using (FileStream fs = new FileStream(this.Path, FileMode.Open, FileAccess.Read, FileShare.Read,
                                                      StreamReaderBufferSize, FileOptions.SequentialScan))
                using (StreamReader sr = new StreamReader(fs, this.Encoding, true, StreamReaderBufferSize))
                {
                    while ((inputLine = sr.ReadLine()) != null)
                    {
                        try
                        {
                            lineNumber++;
                            if (lineNumber == skipLine) continue;
                            parsedLines.Add(LineParser.Parse<T>(inputLine));
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
                return parsedLines;
            }
            catch (ParseFieldException){ throw; }
            catch (IOException) { throw; }
            catch (Exception ex)
            {
                throw new Exception($"Error al acceder al archivo de entrada. Detalles: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Variante streaming de <see cref="Parse{T}(bool)"/>: lee y parsea linea a linea de forma perezosa,
        /// sin materializar toda la coleccion en memoria. Ideal para archivos muy grandes o pipelines.
        /// </summary>
        public IEnumerable<T> ParseStream<T>(bool ignoreWrongLines) where T : new()
        {
            return ParseStream<T>(ignoreWrongLines, 0);
        }


        internal IEnumerable<T> ParseStream<T>(bool ignoreWrongLines, int skipLine) where T : new()
        {
            using (FileStream fs = new FileStream(this.Path, FileMode.Open, FileAccess.Read, FileShare.Read,
                                                  StreamReaderBufferSize, FileOptions.SequentialScan))
            using (StreamReader sr = new StreamReader(fs, this.Encoding, true, StreamReaderBufferSize))
            {
                long lineNumber = 0;
                string inputLine;
                while ((inputLine = sr.ReadLine()) != null)
                {
                    lineNumber++;
                    if (lineNumber == skipLine) continue;

                    T parsed = default(T);
                    bool ok = true;
                    try
                    {
                        parsed = LineParser.Parse<T>(inputLine);
                    }
                    catch (Exception ex)
                    {
                        if (ignoreWrongLines)
                        {
                            InvalidLines.Add(new InvalidLine(lineNumber, inputLine, ex.Message));
                            ok = false;
                        }
                        else
                        {
                            throw new ParseFieldException(ex.Message, ex);
                        }
                    }

                    if (ok) yield return parsed;
                }
            }
        }


        public virtual void ToFlatFile<T>(List<T> entities,  string outputPath)
        {
            try
            {
                using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None,
                                                      StreamReaderBufferSize, FileOptions.SequentialScan))
                using (StreamWriter writer = new StreamWriter(fs, this.Encoding, StreamReaderBufferSize))
                {
                    foreach (object entity in entities)
                        writer.WriteLine(LineParser.ToTextLine(entity));
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

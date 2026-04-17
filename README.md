# FixedWidthTextUtils

Librería .NET para parsear y serializar archivos de texto plano de **ancho fijo** (fixed-width) mediante atributos sobre las propiedades de tus modelos. Pensada para integraciones con mainframes, layouts bancarios, archivos COBOL/AS400, *flat files* y similares.

- Target: **.NET Standard 2.0** (consumible desde .NET Framework 4.8, .NET 6/7/8 y Mono).
- Declaración **por atributos** sobre las propiedades: cero configuración externa.
- Dos modos de mapeo: **posicional** (`start`/`end`) u **ordinal** (solo longitud).
- Soporte para tipos nativos y *nullable* (`int?`, `DateTime?`, etc.) con marcador de `null`.
- Parseo/serialización de **líneas sueltas** o **archivos completos** (con y sin footer).
- Mensajes de error orientados a producción (clase, propiedad, valor en crudo, motivo).
- **Plan de modelo cacheado por tipo**: se calcula una sola vez y se reutiliza, eliminando la reflexión por línea en el hot path.

---

## Índice

1. [Instalación](#instalación)
2. [Conceptos: modo posicional vs ordinal](#conceptos-modo-posicional-vs-ordinal)
3. [Quick Start](#quick-start)
4. [Atributos de campo](#atributos-de-campo)
   - [`StringField`](#stringfield--nullablestringfield)
   - [`IntegerField`](#integerfield--nullableintegerfield)
   - [`FloatingField`](#floatingfield--nullablefloatingfield)
   - [`BooleanField`](#booleanfield--nullablebooleanfield)
   - [`DateTimeField`](#datetimefield--nullabledatetimefield)
5. [API de parseo](#api-de-parseo)
   - [`LineParser`](#lineparser-lineas-sueltas)
   - [`FileParser`](#fileparser-archivo-completo)
   - [`FileParserWithFooter<F>`](#fileparserwithfooterf-archivo-con-footer)
6. [Reglas de validación de modelos](#reglas-de-validación-de-modelos)
7. [Excepciones](#excepciones)
8. [Performance: caché de plan por tipo](#performance-caché-de-plan-por-tipo)
9. [Ejemplos incluidos en el repositorio](#ejemplos-incluidos-en-el-repositorio)
10. [Tests](#tests)

---

## Instalación

Clonar el repositorio y referenciar el proyecto `FixedWidthTextUtils/FixedWidthTextUtils.csproj` desde la solución, o compilarlo como DLL y referenciarla directamente.

```bash
dotnet build FixedWidthTextUtils/FixedWidthTextUtils.csproj -c Release
```

Compila a **netstandard2.0**, por lo que puede consumirse tanto desde proyectos clásicos (.NET Framework 4.8) como modernos (.NET 6+).

---

## Conceptos: modo posicional vs ordinal

Cada propiedad de tu modelo representa un **campo** dentro de la línea. Hay dos formas mutuamente excluyentes de declarar dónde está cada campo:

- **Posicional**: indicas la **posición inicial y final** (inclusive, base 0) dentro de la línea.
  ```csharp
  [IntegerField(0, 8, fillLeftWithZero: true)]   // caracteres 0..8 (9 caracteres)
  public long Id { get; set; }
  ```
- **Ordinal**: indicas solo el **largo** del campo; el parser calcula la posición por acumulación según el orden en que aparecen las propiedades.
  ```csharp
  [IntegerField(9, fillLeftWithZero: true)]      // 9 caracteres (consume las posiciones 0..8)
  public long Id { get; set; }
  ```

> **Importante**: una misma clase **no puede mezclar** ambos modos. Si lo intentas, recibirás una excepción en el primer `Parse` o `ToTextLine` (ver [Reglas de validación](#reglas-de-validación-de-modelos)).

---

## Quick Start

### 1) Define tu modelo

Ejemplo **posicional** (103 caracteres):

```csharp
using FixedWidthTextUtils.Attributes;
using System;

public class Cliente
{
    [IntegerField(0, 8, true)]
    public long Id { get; set; }

    [StringField(9, 9)]
    public string Code { get; set; }

    [StringField(10, 29, StringFieldAttribute.TrimMode.Trim)]
    public string Name { get; set; }

    [IntegerField(50, 54, true)]
    public int HouseNumber { get; set; }

    [DateTimeField(89, 96, "yyyyMMdd")]
    public DateTime BirthDate { get; set; }

    [FloatingField(100, 102, 1, true)]
    public float WeightFloat { get; set; }
}
```

### 2) Parsea una línea

```csharp
using FixedWidthTextUtils;

string line = "123456789ANed Flanders        ...19811229180919";
Cliente cliente = LineParser.Parse<Cliente>(line);
```

### 3) Vuelve a texto

```csharp
string textLine = LineParser.ToTextLine(cliente);
```

### 4) Archivo completo

```csharp
var fileParser = new FileParser(@"C:\data\clientes.txt");
List<Cliente> lista = fileParser.Parse<Cliente>(ignoreWrongLines: true);
fileParser.ToFlatFile(lista, @"C:\data\clientes_out.txt");
```

---

## Atributos de campo

Todos los atributos se aplican a **propiedades públicas con `get`/`set`** y ofrecen dos constructores: uno **posicional** (`start`, `end`) y uno **ordinal** (`length`).

### `StringField` / `NullableStringField`

Para `string` (y `string` nullable en la versión *Nullable*).

```csharp
// Posicional:
[StringField(10, 29, StringFieldAttribute.TrimMode.Trim)]
public string Name { get; set; }

// Ordinal + relleno a la izquierda:
[StringField(5, StringFieldAttribute.TrimMode.TrimStart, leftPadding: true)]
public string PostCode { get; set; }

// Nullable con marcador de null:
[NullableStringField(10, 19, textForNull: "NULLVALUE ")]
public string? Description { get; set; }
```

Modos de `Trim` disponibles al **parsear**:

| Modo              | Comportamiento                     |
|-------------------|------------------------------------|
| `NoTrim`          | Deja el valor tal cual             |
| `Trim`            | `Trim()` (ambos extremos)          |
| `TrimStart`       | `TrimStart()`                      |
| `TrimEnd`         | `TrimEnd()` (**default**)          |

Al **serializar**, el campo se completa a la longitud exacta; `LeftPadding = true` usa `PadLeft`, y `false` usa `PadRight`.

### `IntegerField` / `NullableIntegerField`

Soporta `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong` (y sus variantes nullable con `NullableIntegerField`).

```csharp
[IntegerField(0, 8, fillLeftWithZero: true)]
public long Id { get; set; }               // "000012345"

[IntegerField(97, 99, fillLeftWithZero: false)]
public int Height { get; set; }            // "  42" (padding con espacios)

[NullableIntegerField(50, 54, textForNull: "00000")]
public int? HouseNumber { get; set; }      // "00000" se interpreta como null
```

- `fillLeftWithZero = true` ⇒ se completa con ceros a la izquierda (respetando el signo `-`).
- `fillLeftWithZero = false` ⇒ se completa con espacios a la izquierda.
- Si el valor serializado **excede el ancho del campo**, se lanza `SerializeFieldException`.

### `FloatingField` / `NullableFloatingField`

Soporta `float` (`Single`), `double`, `decimal` y sus variantes nullable.

El valor se representa como un **entero plano** en la línea de texto (sin punto decimal): `decimalPositions` indica cuántos dígitos finales son los decimales.

```csharp
[FloatingField(100, 102, decimalPositions: 1, fillLeftWithZeros: true)]
public float WeightFloat { get; set; }    // 18.5 kg -> "185"

[FloatingField(5, decimalPositions: 2, fillLeftWithZeros: true)]
public decimal Amount { get; set; }       // 12.34 -> "01234"

[NullableFloatingField(5, decimalPositions: 2, textForNull: "XXXXX")]
public decimal? Optional { get; set; }
```

### `BooleanField` / `NullableBooleanField`

Define explícitamente los literales para `true` y `false`; opcionalmente un literal para `null`.

```csharp
[BooleanField(21, 23, textForTrue: "YES", textForFalse: "NO ")]
public bool Enable { get; set; }

[BooleanField(1, textForTrue: "S", textForFalse: "N")]
public bool Activo { get; set; }

[NullableBooleanField(1, textForTrue: "Y", textForFalse: "N", textForNull: "?")]
public bool? Flag { get; set; }
```

Restricciones:
- `textForTrue.Length` debe **igualar** la longitud del campo.
- `textForFalse.Length` debe igualar la longitud del campo, o ser `""` (string vacío). Si es `""`, **cualquier valor que no coincida con `textForTrue` se interpreta como `false`**.
- `textForTrue != textForFalse`.

### `DateTimeField` / `NullableDateTimeField`

Parsea con `DateTime.TryParseExact` usando el `format` indicado y la cultura **invariante**.

```csharp
[DateTimeField(89, 96, format: "yyyyMMdd")]
public DateTime BirthDate { get; set; }

[DateTimeField(8, "yyyyMMdd")]                         // ordinal, 8 chars
public DateTime OrderDate { get; set; }

[NullableDateTimeField(8, "yyyyMMdd", textForNull: "        ")]
public DateTime? CancelDate { get; set; }
```

El `format.Length` debe coincidir con la longitud del campo. Se puede usar `leftPadding: true` para pegar la salida a la derecha durante la serialización.

---

## API de parseo

### `LineParser` — líneas sueltas

Clase estática con tres puntos de entrada:

```csharp
T        LineParser.Parse<T>(string input)       where T : new();
bool     LineParser.TryParse<T>(string input, out T result) where T : new();
string   LineParser.ToTextLine(object value);
```

- `Parse<T>` lanza excepción si algo falla (ver [Excepciones](#excepciones)).
- `TryParse<T>` **nunca** lanza por errores esperables (definición de campo inválida, parseo fallido, input vacío): devuelve `false` y `result = default`. Para errores inesperados (IO, etc.) sí propaga.
- `ToTextLine` serializa cualquier instancia cuyo tipo esté bien decorado. El tamaño final de la línea se deduce del propio modelo.

### `FileParser` — archivo completo

```csharp
var fp = new FileParser(path);                       // UTF-8
var fp = new FileParser(path, Encoding.Latin1);

List<Cliente> ok = fp.Parse<Cliente>(ignoreWrongLines: true);

foreach (var bad in fp.InvalidLines)
    Console.WriteLine($"Línea {bad.Number}: {bad.ErrorDescription}");

fp.ToFlatFile(ok, outputPath);
```

- **`ignoreWrongLines = true`**: las líneas mal formadas no abortan el proceso; se acumulan en `fp.InvalidLines` (con número de línea, contenido crudo y mensaje de error).
- **`ignoreWrongLines = false`**: ante la primera línea inválida se lanza `ParseFieldException`.
- El `Encoding` configurado se usa tanto para **lectura** como para **escritura** (incluyendo BOM en UTF-8 si lo configuras así).

### `FileParserWithFooter<F>` — archivo con footer

Para layouts con una **última línea** que describe totales / cantidad de registros con una estructura distinta.

```csharp
public class Footer
{
    [StringField(0, 1)]   public string RegisterType { get; set; }
    [IntegerField(2, 6)]  public int RegisterCount { get; set; }
    [IntegerField(7, 19)] public long TotalAmount { get; set; }
}

var fp = new FileParserWithFooter<Footer>(path);
List<Cliente> clientes = fp.Parse<Cliente>(ignoreWrongLines: false, out Footer footer);

fp.ToFlatFile(clientes, footer, outputPath);
```

El footer se parsea con `LineParser.Parse<F>(ultimaLinea)` y se expone como `out`. El resto de líneas se parsea como en `FileParser`.

---

## Reglas de validación de modelos

La librería valida la coherencia de tu modelo al construir su **plan de parseo** (una sola vez por tipo). Las validaciones incluidas:

| Regla                                                                               | Excepción                                                     |
|-------------------------------------------------------------------------------------|---------------------------------------------------------------|
| Propiedad con tipo no soportado por su atributo (p. ej. `IntegerField` sobre `decimal`) | `ArgumentException` en `Parse`                                |
| Atributo no-nullable sobre propiedad nullable (p. ej. `IntegerField` en `int?`)      | `ArgumentException` en `Parse`                                |
| `TextForTrue`, `TextForFalse`, `TextForNull` o `Format` con longitud ≠ ancho del campo | `ArgumentException` en `Parse`                                |
| `TextForTrue == TextForFalse` en `BooleanField`                                      | `ArgumentException` en `Parse`                                |
| `StringField` ordinal con `fieldLength < 1`                                          | `ArgumentException` al construir el atributo                  |
| **Mezcla de modo posicional y ordinal en la misma clase**                           | `ArgumentException` en `Parse` / `SerializeFieldException` en `ToTextLine` |
| Valor serializado más ancho que el campo                                            | `SerializeFieldException` en `ToTextLine`                     |
| Valor crudo no convertible al tipo destino                                          | `ParseFieldException` en `Parse`                              |

`TryParse<T>` captura `ParseFieldException` y `ArgumentException`, y devuelve `false` sin lanzar.

---

## Excepciones

Todas heredan de `FixedWidthTextException` (que a su vez deriva de `ApplicationException`), lo que permite capturar todas las excepciones de la librería con un solo `catch`.

| Excepción                  | Cuándo se lanza                                                                  |
|----------------------------|----------------------------------------------------------------------------------|
| `ParseFieldException`      | Error al convertir un campo al tipo destino o input vacío en `Parse`.            |
| `SerializeFieldException`  | Error al serializar (campo demasiado ancho, tipo incompatible, mezcla de modos). |
| `DateFormatException`      | Reservada para errores específicos de fechas (usada en capas superiores).        |
| `FixedWidthTextException`  | Base abstracta para capturar todas las anteriores.                               |

Para errores de **definición del modelo**, `Parse` lanza `ArgumentException` (consistente con el resto del .NET BCL y capturable por `TryParse`).

---

## Performance: caché de plan por tipo

Al llamar por primera vez a `Parse<T>` o `ToTextLine` para un tipo `T`, la librería construye un **plan de modelo** con:

- La lista de propiedades decoradas y sus `FieldAttribute`.
- Las posiciones de inicio/fin calculadas (para ordinal, ya acumuladas).
- El largo total de la línea.
- Un flag de mezcla ordinal/posicional.
- El resultado de validar todas las definiciones.

Este plan se cachea en un `ConcurrentDictionary<Type, LineModelPlan>`, por lo que las llamadas siguientes **no repiten** `GetProperties`, `GetCustomAttributes` ni la validación de definición. Es thread-safe y pensado para uso masivo.

> Medí tu caso: ver el proyecto `Examples/ParseBenchmark_Example_NF_4_8` incluido en el repo, que parsea la misma línea 1.000.000 de veces en modo posicional y ordinal.

---

## Ejemplos incluidos en el repositorio

Bajo la carpeta `Examples/`:

| Proyecto                          | Descripción                                                                  |
|-----------------------------------|------------------------------------------------------------------------------|
| `Positional_Example_NF_4_8`       | Modelo `Cliente` con declaración **posicional** (103 caracteres) y round-trip Parse/ToTextLine. |
| `Ordinal_Example_NF_4_8`          | Modelo `ClienteOrdinal` con declaración **ordinal** (29 caracteres).         |
| `ParseBenchmark_Example_NF_4_8`   | Mide el tiempo de parsear 1.000.000 de veces la misma línea en ambos modos (útil para comparar antes/después del caché). |

Todos los proyectos de ejemplo compilan para **.NET Framework 4.8** y referencian la librería por `ProjectReference` a `FixedWidthTextUtils.csproj`.

---

## Tests

Proyecto de pruebas: `FixedWidthTextUtils_NUnit_Test/` (NUnit, **.NET 6**).

```bash
dotnet test FixedWidthTextUtils_NUnit_Test/FixedWidthTextUtils_NUnit_Test.csproj
```

Cubre:

- Parseo y serialización con todos los atributos (nativos y nullable).
- Round-trip (`Parse` → `ToTextLine` → mismo texto).
- Validación de definición de modelos (todos los guards).
- `FileParser` y `FileParserWithFooter` con fixtures reales bajo `TestFiles/` y `TestFilesWithFooter/`.
- `Encoding` configurable (UTF-8 con y sin BOM).
- Casos de mezcla ordinal/posicional (debe rechazarse tanto en `Parse` como en `ToTextLine`).

---

## Licencia / Autor

Ver el repositorio para información de licencia y contribuciones.

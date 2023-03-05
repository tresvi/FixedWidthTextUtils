using FixedWidthTextUtils.Attributes;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
    internal class IntegerOrdinalNullables
    {
        [NullableIntegerField(2, "  ")]
        public byte? ByteNull { get; set; }
        [NullableIntegerField(2, "  ")]
        public byte? ByteNotNull { get; set; }

        [NullableIntegerField(2, "  ")]
        public sbyte? SByteNull { get; set; }
        [NullableIntegerField(2, "  ")]
        public sbyte? SByteNotNull { get; set; }

        [NullableIntegerField(2, "  ")]
        public short? ShortNull { get; set; }
        [NullableIntegerField(2, "  ")]
        public short? ShortNotNull { get; set; }

        [NullableIntegerField(2, "  ")]
        public ushort? UShortNull { get; set; }
        [NullableIntegerField(2, "  ")]
        public ushort? UShortNotNull { get; set; }

        [NullableIntegerField(2, "  ")]
        public int? IntNull { get; set; }
        [NullableIntegerField(2, "  ")]
        public int? IntNotNull { get; set; }

        [NullableIntegerField(2, "  ")]
        public uint? UIntNull { get; set; }
        [NullableIntegerField(2, "  ")]
        public uint? UIntNotNull { get; set; }

        [NullableIntegerField(2, "  ")]
        public long? LongNull { get; set; }
        [NullableIntegerField(2, "  ")]
        public long? LongNotNull { get; set; }

        [NullableIntegerField(2, "  ")]
        public ulong? ULongNull { get; set; }
        [NullableIntegerField(2, "  ")]
        public ulong? ULongNotNull { get; set; }        
    }
}

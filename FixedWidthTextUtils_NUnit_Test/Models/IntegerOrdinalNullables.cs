using FixedWidthTextUtils.Attributes;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
    internal class IntegerOrdinalNullables
    {
        [NullableIntegerField(2, true, "  ")]
        public byte? ByteNull { get; set; }
        [NullableIntegerField(2, true, "  ")]
        public byte? ByteNotNull { get; set; }

        [NullableIntegerField(2, true, "  ")]
        public sbyte? SByteNull { get; set; }
        [NullableIntegerField(2, true, "  ")]
        public sbyte? SByteNotNull { get; set; }

        [NullableIntegerField(2, true, "  ")]
        public short? ShortNull { get; set; }
        [NullableIntegerField(2, true, "  ")]
        public short? ShortNotNull { get; set; }

        [NullableIntegerField(2, true, "  ")]
        public ushort? UShortNull { get; set; }
        [NullableIntegerField(2, true, "  ")]
        public ushort? UShortNotNull { get; set; }

        [NullableIntegerField(2, true, "  ")]
        public int? IntNull { get; set; }
        [NullableIntegerField(2, true, "  ")]
        public int? IntNotNull { get; set; }

        [NullableIntegerField(2, true, "  ")]
        public uint? UIntNull { get; set; }
        [NullableIntegerField(2, true, "  ")]
        public uint? UIntNotNull { get; set; }

        [NullableIntegerField(2, true, "  ")]
        public long? LongNull { get; set; }
        [NullableIntegerField(2, true, "  ")]
        public long? LongNotNull { get; set; }

        [NullableIntegerField(2, true, "  ")]
        public ulong? ULongNull { get; set; }
        [NullableIntegerField(2, true, "  ")]
        public ulong? ULongNotNull { get; set; }        
    }
}

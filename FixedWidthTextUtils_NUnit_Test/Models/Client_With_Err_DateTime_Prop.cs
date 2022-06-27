using FixedFlatFileUtils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
    internal class Client_With_Err_DateTime_Prop
    {
        [IntegerField(0, 3, true)]
        public long Id { get; set; }


        [DateTimeField(4, 11, "yyyyMMdd")]
        public int Date { get; set; }
    }
}

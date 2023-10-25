using System;

namespace Zippy.ZipAnalysis.Extensions
{
    public static class NumericExtensions
    {

        public static DateTime ToDate(this ushort dosDate)
        {
            var day = (dosDate & 0b0000000000011111);
            var month = (dosDate & 0b0000000111100000) >> 5;
            var year = ((dosDate & 0b1111111000000000) >> 9) + 1980;

            return new DateTime(year, month, day);
        }



        public static TimeSpan ToTime(this ushort dosTime)
        {
            var second = (dosTime & 0b0000000000011111) * 2;
            var minute = (dosTime & 0b0000011111100000) >> 5;
            var hour = (dosTime & 0b1111100000000000) >> 11;

            return new TimeSpan(hour, minute, second);
        }

    }
}
